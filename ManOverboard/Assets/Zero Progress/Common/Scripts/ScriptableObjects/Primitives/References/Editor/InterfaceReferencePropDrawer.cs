using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;

using ZeroProgress.Common.Reflection;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Base class for all Interface Reference property drawers to inherit from
    /// </summary>
    /// <typeparam name="WrapperType">The type that this property drawer will be affecting</typeparam>
    /// <typeparam name="InterfaceType">The type of interface the wrapper looks at</typeparam>
    public abstract class InterfaceReferencePropDrawer<WrapperType, InterfaceType> : PropertyDrawer
        where WrapperType : InterfaceReference<InterfaceType>
        where InterfaceType : class
    {
        /// <summary>
        /// Determines the display mode based on the currently set value of the wrapper
        /// </summary>
        protected enum DisplayMode
        {
            /// <summary>
            /// Display Mode not determined
            /// </summary>
            NotSet,
            /// <summary>
            /// Display the menu for setting a scriptable object that implements
            /// the required interface
            /// </summary>
            ScriptableObjectDisplay,
            /// <summary>
            /// Display the menu for setting a monobehaviour that implements
            /// the required interface
            /// </summary>
            MonobehaviourDisplay,
            /// <summary>
            /// Display the menu for modifying a POCO that implements
            /// the required interface
            /// </summary>
            SystemObjectDisplay
        }

        /// <summary>
        /// Details regarding the actual InterfaceReference instance and the
        /// serialized structures pertaining to its stored information
        /// </summary>
        protected class ReferenceDetails
        {
            /// <summary>
            /// The object that owns the SerializedProperties
            /// </summary>
            public SerializedObject Owner;
            /// <summary>
            /// The SerializedProperty pointing to the Unity Object reference
            /// </summary>
            public SerializedProperty UnityObjectRefProp;
            /// <summary>
            /// Reference to the actual object. Changes should not be made
            /// directly to this item unless you know what you're doing
            /// </summary>
            public InterfaceReference<InterfaceType> Instance;
            /// <summary>
            /// Reference to the poco object
            /// </summary>
            public SerializedPoco PocoInstance;

            /// <summary>
            /// Retrieves the type that is currently being referenced
            /// by the InterfaceReference wrapper
            /// </summary>
            /// <returns>The type of the item that the InterfaceReference
            /// points to, or null if there is no item</returns>
            public Type GetInstanceContainedType()
            {
                if (Instance != null && Instance.Value != null)
                    return Instance.Value.GetType();

                return null;
            }

            /// <summary>
            /// Retrieves the string of the type that is currently being referenced
            /// by the InterfaceReference wrapper
            /// </summary>
            /// <returns>The string representation of the type of the item that the 
            /// InterfaceReference points to, or null if there is no item</returns>
            public String GetInstanceContainedTypeString()
            {
                Type type = GetInstanceContainedType();

                if (type == null)
                    return string.Empty;

                return type.AssemblyQualifiedName;
            }

            /// <summary>
            /// Resets the references to null and updates the owner accordingly
            /// </summary>
            public void ResetInstanceData()
            {
                UnityObjectRefProp.objectReferenceValue = null;

                PocoInstance.SetValue(null);
                
                Owner.ApplyModifiedProperties();
                Owner.Update();
            }
        }

        /// <summary>
        /// Since PropertyDrawers are re-used for multiple properties, we can't
        /// simply create a variable. Therefore this class stores the data that
        /// should be persisted (or cached) for an individual instance. This data
        /// will then be uniquely mapped to each property
        /// </summary>
        protected class CacheItem
        {
            /// <summary>
            /// Details regarding the item itself
            /// </summary>
            public ReferenceDetails ReferenceInformation;

            /// <summary>
            /// The type dynamically created that is used to
            /// serialize the POCO instance for display in the
            /// inspector
            /// </summary>
            public Type DynamicType { get; set; }

            /// <summary>
            /// Sets the wrapper used to serialize the poco class
            /// </summary>
            public ScriptableObject PocoWrapper { get; set; }

            /// <summary>
            /// The Value property in the poco wrapper to be displayed if a
            /// POCO item is used
            /// </summary>
            public SerializedProperty PocoWrapperValueProp { get; set; }

            /// <summary>
            /// The string representation of the currently selected
            /// type (from the type drop-down)
            /// </summary>
            public string SelectedTypeString { get; private set; }

            /// <summary>
            /// The currently selected type (from the type drop-down)
            /// </summary>
            public Type SelectedType { get; private set; }

            /// <summary>
            /// True if this cache stores a valid type, false if not
            /// (a valid type being not null and exists in the assembly)
            /// </summary>
            public bool HasValidType { get; private set; }

            /// <summary>
            /// The currently active display mode
            /// </summary>
            public DisplayMode Display { get; private set; }

            /// <summary>
            /// Default Constructor
            /// </summary>
            public CacheItem()
            {
                SelectedTypeString = string.Empty;
            }

            /// <summary>
            /// Sets the type stored by this cache item to the specified
            /// string representation of a type
            /// 
            /// If the provided string does not resolve to a proper type,
            /// the type will then be set to null
            /// </summary>
            /// <param name="selectedType">The string representation of the type
            /// to set the stored type to</param>
            public void SetType(string selectedType)
            {
                if (selectedType == null)
                    selectedType = string.Empty;

                SelectedTypeString = selectedType;

                SelectedType = Type.GetType(selectedType, throwOnError: false, ignoreCase: true);

                Display = DetermineDisplayMode(SelectedType);

                HasValidType = SelectedType != null;
            }

            /// <summary>
            /// Sets the type stored by this cache item to the specified type
            /// </summary>
            /// <param name="selectedType">The type to consider selected</param>
            public void SetType(Type selectedType)
            {
                if (selectedType == null)
                    SelectedTypeString = string.Empty;
                else
                    SelectedTypeString = selectedType.AssemblyQualifiedName;

                SelectedType = selectedType;

                Display = DetermineDisplayMode(SelectedType);

                HasValidType = SelectedType != null;
            }

            /// <summary>
            /// Clears the generated items used to display the POCO
            /// </summary>
            public void ClearPocoValue()
            {
                PocoWrapperValueProp = null;
                DynamicType = null;

                if (PocoWrapper != null)
                {
                    ScriptableObject.DestroyImmediate(PocoWrapper);
                    PocoWrapper = null;
                }
            }

            /// <summary>
            /// Helper to determine the display mode based on the stored type
            /// </summary>
            /// <param name="type">The type to evaluate</param>
            /// <returns></returns>
            private DisplayMode DetermineDisplayMode(Type type)
            {
                if (type == null)
                    return DisplayMode.NotSet;

                if (type.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    if (type.IsSubclassOf(typeof(ScriptableObject)))
                        return DisplayMode.ScriptableObjectDisplay;
                    else
                        return DisplayMode.MonobehaviourDisplay;
                }
                else
                {
                    return DisplayMode.SystemObjectDisplay;
                }
            }
        }

        /// <summary>
        /// PropertyDrawers get cached to re-use for multiple values. This will store
        /// per-instance cache data for us, specifically regarding the currently selected type
        /// 
        /// Key: SerializedProperty.propertyPath  Value: CacheItem containing details of an individual instance
        /// </summary>
        protected Dictionary<string, CacheItem> drawerCache = new Dictionary<string, CacheItem>();

        /// <summary>
        /// The field name of the poco instance for accessing through reflection
        /// </summary>
        protected const string POCO_INSTANCE_FIELD_NAME = "interfaceAsPoco";

        protected bool isInitialized = false;

        /// <summary>
        /// This will force the drawer to update immediately when changes are made.
        /// 
        /// Before implementing this, Undo's were not recognized until the window lost
        /// focus and then regained (the field was undone, but not the drawer ui)
        /// 
        /// The dynamic height based on the item selection also responds immediately now
        /// </summary>
        /// <param name="property">The property</param>
        /// <returns>False to indicate caching should not occur (due to dynamic appearance)</returns>
        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        /// <summary>
        /// Determines how much vertical space should be allocated to the drawer.
        /// We return a different value based on the currently selected type
        /// </summary>
        /// <param name="property">The property to determine the size of</param>
        /// <param name="label">The label to be displayed with the property</param>
        /// <returns>Float representing the desired height</returns>
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            CacheItem propDetails = Initialize(property);

            switch (propDetails.Display)
            {
                case DisplayMode.NotSet:
                    return base.GetPropertyHeight(property, label);
                case DisplayMode.ScriptableObjectDisplay:
                    return GetScriptableObjectDisplayHeight(propDetails.ReferenceInformation);
                case DisplayMode.MonobehaviourDisplay:
                    return GetMonobehaviourDisplayHeight(propDetails.ReferenceInformation);
                case DisplayMode.SystemObjectDisplay:
                    return GetSystemObjectDisplayHeight(propDetails);
                default:
                    return base.GetPropertyHeight(property, label);
            }
        }

        /// <summary>
        /// Determines the height for when a scriptable object reference
        /// needs to be displayed
        /// </summary>
        /// <param name="referenceDetails">The details regarding the InterfaceReference
        /// to be displayed</param>
        /// <returns>The vertical height required to display the InterfaceReference</returns>
        protected virtual float GetScriptableObjectDisplayHeight(ReferenceDetails referenceDetails)
        {
            return EditorGUIUtility.singleLineHeight * 2f;
        }

        /// <summary>
        /// Determines the height for when a monobehaviour reference
        /// needs to be displayed
        /// </summary>
        /// <param name="referenceDetails">The details regarding the InterfaceReference
        /// to be displayed</param>
        /// <returns>The vertical height required to display the InterfaceReference</returns>
        protected virtual float GetMonobehaviourDisplayHeight(ReferenceDetails referenceDetails)
        {
            float addButtonHeight = EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing * 2f);

            if (referenceDetails.Instance.Value != null)
                addButtonHeight = 0f;

            return EditorGUIUtility.singleLineHeight * 2f + addButtonHeight;
        }

        /// <summary>
        /// Determines the height for when a POCO reference
        /// needs to be displayed
        /// </summary>
        /// <param name="cachedItem">The details regarding the InterfaceReference
        /// to be displayed</param>
        /// <returns>The vertical height required to display the InterfaceReference</returns>
        protected virtual float GetSystemObjectDisplayHeight(CacheItem cachedItem)
        {
            if(cachedItem.PocoWrapperValueProp != null)
                return EditorGUI.GetPropertyHeight(cachedItem.PocoWrapperValueProp) + EditorGUIUtility.singleLineHeight;

            return EditorGUIUtility.singleLineHeight;
        }

        /// <summary>
        /// Unity callback to render the property drawer
        /// </summary>
        /// <param name="position">Where to render it</param>
        /// <param name="property">The property to render</param>
        /// <param name="label">The label to attach</param>
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if(!isInitialized)
            {
                Undo.undoRedoPerformed += (() =>
               {
                   // Clear the cache so we 're-initialize' everything to ensure that if
                   // it was our components that were undone that they get refreshed properly
                   drawerCache.Clear();
               });

                isInitialized = true;
            }

            CacheItem propDetails = Initialize(property);

            string newSelection = RenderTypeSelection(ref position, label, propDetails.SelectedTypeString);
            
            if (propDetails.SelectedTypeString != newSelection)
                HandleTypeChange(propDetails, newSelection);

            EditorGUI.indentLevel++;
            position = EditorGUI.IndentedRect(position);
            Display(position, propDetails);
            EditorGUI.indentLevel--;

            property.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Handles changing between types
        /// </summary>
        /// <param name="propDetails">The cached details for the current property</param>
        /// <param name="newSelection">The new selection</param>
        protected void HandleTypeChange(CacheItem propDetails, string newSelection)
        {
            Type selectedType = Type.GetType(newSelection);

            if (selectedType != null && selectedType == propDetails.SelectedType)
                return;

            Undo.RegisterCompleteObjectUndo(
                propDetails.ReferenceInformation.Owner.targetObject, "Poco Info");

            SerializedPoco poco = propDetails.ReferenceInformation.PocoInstance;
            System.Object oldItem = poco.Value;

            MethodInfo method = typeof(SerializedPoco).GetMethod("SerializePoco", BindingFlags.NonPublic | BindingFlags.Instance);
            string serialized = method.Invoke(poco, new object[] { false }) as string;
            
            propDetails.ClearPocoValue();
            propDetails.SetType(newSelection);
            propDetails.ReferenceInformation.ResetInstanceData();

            // If the new type is a POCO
            if (propDetails.Display == DisplayMode.SystemObjectDisplay)
            {
                InitializePoco(propDetails);
                
                if (oldItem != null && propDetails.SelectedType != null)
                {
                    // Determine if the new type is a parent or descendant of the old
                    // type so that we can transfer some of the properties
                    if (propDetails.SelectedType.IsSubclassOf(oldItem.GetType()) ||
                        oldItem.GetType().IsSubclassOf(propDetails.SelectedType))
                    {
                        MethodInfo deserialize = typeof(SerializedPoco).GetMethod("DeserializePoco", BindingFlags.NonPublic | BindingFlags.Instance);

                        deserialize.Invoke(poco, new object[] { poco.Value, serialized });

                        propDetails.ReferenceInformation.Owner.ApplyModifiedProperties();
                        propDetails.ReferenceInformation.Owner.Update();

                        // Clear the cache to force a re-draw since there is something
                        // that I am caching that prevents the update
                        drawerCache.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// Initializes the cache data, or retrieves it if it has been
        /// previously initialized
        /// </summary>
        /// <param name="property">The property to extract information from</param>
        /// <returns>The cache item pertaining to this property</returns>
        protected CacheItem Initialize(SerializedProperty property)
        {
            property.serializedObject.ApplyModifiedProperties();
            property.serializedObject.Update();

            CacheItem cachedInstance = drawerCache.GetOrCreate(property.propertyPath);

            if (cachedInstance.ReferenceInformation == null)
            {
                cachedInstance.ReferenceInformation = new ReferenceDetails()
                {
                    Owner = property.serializedObject,
                    UnityObjectRefProp = property.FindPropertyRelative("interfaceAsUnityObject"),
                    Instance = property.GetPropertyInstanceObject<WrapperType>(),
                    PocoInstance = property.FindPropertyRelative("interfaceAsPoco").GetPropertyInstanceObject<SerializedPoco>()
                };
            }
            
            if (!cachedInstance.HasValidType)
            {
                cachedInstance.SetType(cachedInstance.ReferenceInformation.GetInstanceContainedType());

                // If we extracted a type and it isn't a unity object (i.e. it IS a POCO)
                // then extract the poco details
                if (cachedInstance.HasValidType &&
                    !cachedInstance.SelectedType.IsSubclassOf(typeof(UnityEngine.Object)))
                {
                    InitializePoco(cachedInstance);
                }
            }

            return cachedInstance;
        }

        /// <summary>
        /// Displays the drop down box for the type to be displayed
        /// </summary>
        /// <param name="position">The position to render this at. It is ref so
        /// that it can be adjusted to exclude the used area</param>
        /// <param name="label">The label to be displayed</param>
        /// <param name="currentSelection">The current selection</param>
        /// <returns>The selected item</returns>
        protected string RenderTypeSelection(ref Rect position, GUIContent label, string currentSelection)
        {
            Type interfaceType = typeof(InterfaceType);
            string interfaceTypeString = interfaceType.FullName;

            Rect drawRect = new Rect(position)
            {
                height = EditorGUIUtility.singleLineHeight
            };

            position.yMin += drawRect.height;

            string selection = TypeReferencePropertyDrawer.DrawTypeSelectionControl(drawRect, label,
                currentSelection, new ClassImplementsAttribute(interfaceType));

            return selection;
        }

        /// <summary>
        /// Display the item within the provided position
        /// </summary>
        /// <param name="position">The position to render the item</param>
        /// <param name="propDetails">The details to use to determine what to display</param>
        protected void Display(Rect position, CacheItem propDetails)
        {
            switch (propDetails.Display)
            {
                case DisplayMode.ScriptableObjectDisplay:

                    DisplayScriptableObject(position,
                        propDetails.ReferenceInformation.UnityObjectRefProp,
                        propDetails.SelectedType, propDetails.ReferenceInformation.Owner);

                    break;

                case DisplayMode.MonobehaviourDisplay:

                    DisplayMonobehaviour(position,
                        propDetails.ReferenceInformation.UnityObjectRefProp,
                        propDetails.SelectedType, propDetails.ReferenceInformation.Owner);

                    break;

                case DisplayMode.SystemObjectDisplay:

                    DisplayPocoObject(position, propDetails);

                    break;

                case DisplayMode.NotSet:
                default:
                    break;
            }
        }

        /// <summary>
        /// Displays the monobehaviour details in the drawer
        /// </summary>
        /// <param name="position">The position to render at</param>
        /// <param name="unityObjectProperty">The property to be set with the newly
        /// created behaviour</param>
        /// <param name="selectedType">The type of monobehaviour</param>
        /// <param name="wrapperOwner">The object to push the changes to if added</param>
        protected void DisplayMonobehaviour(Rect position, SerializedProperty unityObjectProperty,
            Type selectedType, SerializedObject wrapperOwner)
        {
            EditorGUI.BeginChangeCheck();

            if (unityObjectProperty.objectReferenceValue == null)
            {
                Rect addButtonRect = new Rect(position)
                {
                    xMin = position.xMax - 50f,
                    height = EditorGUIUtility.singleLineHeight
                };

                addButtonRect.yMin += 1f;

                DisplayAddButton(addButtonRect, new GUIContent("Add"), unityObjectProperty, selectedType, wrapperOwner);

                position.yMin += EditorGUIUtility.singleLineHeight + (EditorGUIUtility.standardVerticalSpacing);
            }

            unityObjectProperty.objectReferenceValue = EditorGUI.ObjectField(position,
                "Implementation:", unityObjectProperty.objectReferenceValue,
                selectedType, allowSceneObjects: true);

            if (EditorGUI.EndChangeCheck())
                unityObjectProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Displays and handles the add button for Monobehaviour types
        /// </summary>
        /// <param name="position">The position to render the button at</param>
        /// <param name="label">The label to be displayed</param>
        /// <param name="unityObjectProperty">The property to be set with the newly
        /// created behaviour</param>
        /// <param name="typeToCreate">The type of monobehaviour to be created if
        /// the add button is clicked</param>
        /// <param name="wrapperOwner">The object to push the changes to if added</param>
        protected void DisplayAddButton(Rect position, GUIContent label,
            SerializedProperty unityObjectProperty, Type typeToCreate, SerializedObject wrapperOwner)
        {
            if (GUI.Button(position, label))
            {
                if (typeToCreate.IsSubclassOf(typeof(ScriptableObject)))
                {
                    ScriptableObject item = ScriptableObject.CreateInstance(typeToCreate);
                    unityObjectProperty.objectReferenceValue = item;
                    Undo.RegisterCreatedObjectUndo(item, "Undo Create ScriptableObject Instance");
                }
                else if (typeToCreate.IsSubclassOf(typeof(Component)))
                {
                    GameObject go = GetGameObject(wrapperOwner);

                    if (go == null)
                    {
                        Debug.LogError("Failed to find a game object to attach the component to");
                        return;
                    }

                    unityObjectProperty.objectReferenceValue = Undo.AddComponent(go, typeToCreate);
                }
                else
                {
                    Debug.LogError("Add button shouldn't be shown for POCO classes");
                }
            }
        }

        protected void DisplayScriptableObject(Rect position, SerializedProperty unityObjectProperty,
            Type selectedType, SerializedObject wrapperOwner)
        {
            EditorGUI.BeginChangeCheck();

            unityObjectProperty.objectReferenceValue = EditorGUI.ObjectField(position,
                "Implementation:", unityObjectProperty.objectReferenceValue, selectedType, allowSceneObjects: false);

            if (EditorGUI.EndChangeCheck())
                unityObjectProperty.serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Handles rendering the poco object
        /// </summary>
        /// <param name="position">The position to render the poco object at</param>
        /// <param name="cacheItem">The cache item containing the details for display</param>
        protected void DisplayPocoObject(Rect position, CacheItem cacheItem)
        {
            if (cacheItem.PocoWrapperValueProp == null)
                return;

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(position, cacheItem.PocoWrapperValueProp, includeChildren: true);

            if (EditorGUI.EndChangeCheck())
            {
                cacheItem.PocoWrapperValueProp.serializedObject.ApplyModifiedProperties();

                System.Object currentValue = cacheItem.PocoWrapper.
                    Reflection_GetFieldValue("Value", BindingFlags.Instance | BindingFlags.Public);

                cacheItem.ReferenceInformation.PocoInstance.SetValue(currentValue);

                cacheItem.ReferenceInformation.Owner.ApplyModifiedProperties();
            }
        }
        
        /// <summary>
        /// Initializes the POCO instance within the provided cache item
        /// (provided it needs the initialization
        /// </summary>
        /// <param name="cacheItem">The cache item to be populated with the
        /// details of the initialized poco</param>
        protected void InitializePoco(CacheItem cacheItem)
        {
            if (!cacheItem.HasValidType || cacheItem.ReferenceInformation.Instance == null)
                return;

            System.Object pocoInstance = cacheItem.ReferenceInformation.PocoInstance.Value;
            
            if (pocoInstance == null)
            {
                pocoInstance = Activator.CreateInstance(cacheItem.SelectedType);

                cacheItem.ReferenceInformation.PocoInstance.SetValue(pocoInstance);
            }

            if(cacheItem.PocoWrapper == null)
            {
                cacheItem.DynamicType = GetPocoWrapperType(cacheItem.SelectedType);
                
                cacheItem.PocoWrapper = ScriptableObject.CreateInstance(cacheItem.DynamicType);
                
                cacheItem.PocoWrapper.Reflection_SetFieldValue("Value",
                    pocoInstance, BindingFlags.Instance | BindingFlags.Public);
                
                SerializedObject pocoWrapperSerialized = new SerializedObject(cacheItem.PocoWrapper);

                cacheItem.PocoWrapperValueProp = pocoWrapperSerialized.FindProperty("Value");
            }
        }

        /// <summary>
        /// Retrieves the dynamically created type used to
        /// serialize the POCO
        /// </summary>
        /// <param name="PocoType">The type of POCO that needs to be serialized</param>
        /// <returns>The dynamic type</returns>
        protected Type GetPocoWrapperType(Type PocoType)
        {
            string dynamicTypeName = "_Internal" + PocoType.Name + "Surrogate";

            Type createdType = DynamicAssembly.GetCachedType(dynamicTypeName);

            if (createdType != null)
                return createdType;

            TypeBuilder builder = DynamicAssembly.GetTypeBuilder(dynamicTypeName);
            
            builder.DefineSimpleDefaultConstructor();

            builder.SetParent(typeof(_InternalSurrogate<>).MakeGenericType(PocoType));

            return builder.CreateType();
        }

        /// <summary>
        /// Retrieves the game object that the behaviour is a part of
        /// </summary>
        /// <param name="wrapperOwner">The SerializedObject that represents the
        /// instance that we are retrieving the game object from</param>
        /// <returns>The game object if available, or null if not found</returns>
        private GameObject GetGameObject(SerializedObject wrapperOwner)
        {
            Type parentType = wrapperOwner.targetObject.GetType();

            if (parentType.IsSubclassOf(typeof(GameObject)))
                return wrapperOwner.targetObject as GameObject;

            if(parentType.IsSubclassOf(typeof(Component)))
            {
                Component component = wrapperOwner.targetObject as Component;
                return component.gameObject;
            }

            return null;
        }
    }

    /// <summary>
    /// An internal class for POCO serialization
    /// 
    /// It is only public for dynamic assembly access. Do not
    /// use unless you know what you're doing
    /// </summary>
    /// <typeparam name="T">The type of the contained value</typeparam>
    public class _InternalSurrogate<T> : ScriptableObject
    {
        public T Value;
    }
}
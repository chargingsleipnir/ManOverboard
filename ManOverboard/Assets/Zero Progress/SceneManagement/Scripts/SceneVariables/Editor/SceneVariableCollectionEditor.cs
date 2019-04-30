using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEditorInternal;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;
using ZeroProgress.SceneManagementUtility.Editors;

namespace ZeroProgress.SceneManagementUtility
{
    /// <summary>
    /// Editor for the Scene Variables collection of the Scene Manager
    /// </summary>
    public class SceneVariableCollectionEditor : Editor
    {
        private static readonly GUIContent refreshTransitionContent = 
            new GUIContent(string.Empty, "If enabled, this variable will reset its" +
                " value after each scene transition. If not, it will keep its current value");
        
        public delegate void AddVariableDelegate(string name, Type variableType);
        public delegate void RemoveVariableDelegate(SerializedProperty listProp, int index);
                
        public bool ShowSearchBar = true;

        public bool ShowAddButton = true;

        public bool DisplayAddMenu = false;

        public AddVariableDelegate AddAction;

        public RemoveVariableDelegate RemoveAction;

        #region New Variable Section

        private string newVariableName = "";

        private string newVariableType = "";

        private ClassImplementsAttribute newVariableTypeFilter;

        private GUIContent typeWindowContent = new GUIContent("Variable Type");

        #endregion

        #region Variable Listing Variables

        public string HeaderText = "Scene Variables";

        public event EventHandler<ValueChangedEventArgs<string>> OnVariableNameChanged;

        public event EventHandler OnSomethingChanged;
        
        private SearchField searchBar;
        private string currentSearchString = "";
        
        private ReorderableList variableListDisplay;

        private SerializedSceneVariableCollection serializedVariables;

        private float elementMargin = 2f;

        /// <summary>
        /// Mapping of original index to filtered index for when a search string is present
        /// </summary>
        private Dictionary<int, int> filterIndices = new Dictionary<int, int>();

        /// <summary>
        /// Mapping of property path to inline editor
        /// </summary>
        private Dictionary<string, IInlineEditor> inlineEditors =
            new Dictionary<string, IInlineEditor>();
        
        #endregion

        /// <summary>
        /// Indicates the serialized object and property that will be
        /// displayed through this reorderable list
        /// </summary>
        /// <param name="sceneManagerObject">The owner of the collection property</param>
        /// <param name="variableCollectionProperty">The serialized representation of the
        /// variables collection</param>
        public void SetVariableCollectionProperty(SerializedSceneVariableCollection variableCollectionContainer)
        {
            searchBar = new SearchField();

            serializedVariables = variableCollectionContainer;
            
            variableListDisplay = new ReorderableList(variableCollectionContainer.SerializedContainer, 
                variableCollectionContainer.VariableListProp, true, true, false, true);
            
            variableListDisplay.drawHeaderCallback = OnDrawHeader;
            variableListDisplay.elementHeightCallback = GetElementHeight;
            variableListDisplay.drawElementBackgroundCallback = DrawBackground;
            variableListDisplay.drawElementCallback = DrawElement;
            variableListDisplay.onRemoveCallback = OnRemoveClicked;

            newVariableTypeFilter = new ClassImplementsAttribute(typeof(IVariable))
            {
                AllowGenerics = false,
                RemoveNamespaceFromSelected = true,
                ShowNoneOption = false,
                RemoveIfHasCustomDisplay = true,
                CustomGroupingLogic = GroupVariableType,
                Grouping = ClassGrouping.Custom,
                AdditionalFiltering = IsValidVariableType
            };

            newVariableTypeFilter.CustomDisplays.Add(typeof(ScriptableInt), "Int");
            newVariableTypeFilter.CustomDisplays.Add(typeof(ScriptableFloat), "Float");
            newVariableTypeFilter.CustomDisplays.Add(typeof(ScriptableString), "String");
            newVariableTypeFilter.CustomDisplays.Add(typeof(ScriptableBool), "Bool");

            ResetNewVariableSection();
        }
        
        public override void OnInspectorGUI()
        {
            if (serializedVariables == null)
                return;
            
            variableListDisplay.draggable = !HasFilterString();

            RenderVariablesToolbar();

            Rect rect = GUILayoutUtility.GetLastRect();
            rect.yMin = rect.yMax;
            rect.height = variableListDisplay.GetHeight();

            variableListDisplay.DoList(rect);
        }
        
        private void RenderVariablesToolbar()
        {
            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.BeginHorizontal();

            string previousSearch = currentSearchString;

            if(ShowSearchBar)
                currentSearchString = searchBar.OnGUI(currentSearchString);

            if (previousSearch != currentSearchString)
                RefreshFilteredIndex();

            if (!DisplayAddMenu && ShowAddButton)
                if (GUILayout.Button("+"))
                    DisplayAddMenu = true;

            EditorGUILayout.EndHorizontal();

            if (DisplayAddMenu)
                RenderNewVariableSection();

            EditorGUILayout.EndVertical();
        }

        private void RenderNewVariableSection()
        {
            EditorGUILayout.LabelField("New Variable");

            EditorGUI.indentLevel += 2;

            newVariableName = EditorGUILayout.TextField("Name: ", newVariableName);

            Rect controlRect = EditorGUILayout.GetControlRect();

            newVariableType = TypeReferencePropertyDrawer.DrawTypeSelectionControl(controlRect,
                typeWindowContent, newVariableType, newVariableTypeFilter);

            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Cancel", GUILayout.MinWidth(100f)))
                ResetNewVariableSection();

            if (GUILayout.Button("Add", GUILayout.MinWidth(100f)) && AddAction != null)
            {
                Type selectionAsType = Type.GetType(newVariableType);
                AddAction(newVariableName, selectionAsType);
                serializedVariables.SerializedContainer.Update();
                ResetNewVariableSection();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUI.indentLevel -= 2;
        }

        private void ResetNewVariableSection()
        {
            DisplayAddMenu = false;
            newVariableType = typeof(ScriptableInt).AssemblyQualifiedName;
            newVariableName = string.Empty;
        }

        private bool IsValidVariableType(System.Type variableType)
        {
            if (variableType.Namespace != null &&
                    variableType.Namespace.Contains("ZeroProgress"))
                return false;

            return true;
        }

        private string GroupVariableType(System.Type variableType)
        {
            return "Custom/" +
                TypeReferencePropertyDrawer.FormatByNamespace(variableType);
        }

        #region Reorderable List Callbacks

        /// <summary>
        /// Used by the ReorderableList to determine what to display as the header
        /// </summary>
        /// <param name="rect">The rect that the header is restricted to</param>
        private void OnDrawHeader(Rect rect)
        {
            EditorGUI.LabelField(rect, HeaderText);
        }

        /// <summary>
        /// Calculates how tall the element is
        /// </summary>
        /// <param name="index">The index of the element being rendered</param>
        /// <returns>The height that the element should have</returns>
        private float GetElementHeight(int index)
        {
            if (IsFilteredOut(index))
                return 0f;

            SerializedSceneVariable serializedVariable = 
                serializedVariables.GetSerializedVariableAt(index);
            
            if (IsBuiltInVariable(serializedVariable.ValueProp))
                return EditorGUIUtility.singleLineHeight + elementMargin;
            else
            {
                IInlineEditor inlineEditor = GetCustomInlineEditor(serializedVariable);

                if (inlineEditor != null)
                    return inlineEditor.GetInlineEditorHeight() + EditorGUIUtility.singleLineHeight + elementMargin;
                else
                    // 2f because without a custom inline editor we render the name + the object reference
                    // 1.5f because we use half element margin between var name and value and around the whole element
                    return EditorGUIUtility.singleLineHeight * 2f + (elementMargin * 1.5f);
            }
        }
        
        /// <summary>
        /// Handles drawing the background before applying the element on top
        /// </summary>
        /// <param name="rect">The rectangle to render the variable in</param>
        /// <param name="index">The index of the property</param>
        /// <param name="isActive">True if the element is active, false if not</param>
        /// <param name="isSelected">True if the element is selected, false if not</param>
        private void DrawBackground(Rect rect, int index, bool isActive, bool isSelected)
        {
            int finalIndex = index;

            if (HasFilterString())
                filterIndices.TryGetValue(index, out finalIndex);

            Color color = ZPCommonResources.GetElementBgColor(finalIndex, EditorGUIUtility.isProSkin);

            if (isSelected)
                color = GUI.skin.settings.selectionColor;
            
            GUIExtensions.ColouredBox(rect, color, ZPCommonResources.SimpleSolidBackground);            
        }

        /// <summary>
        /// Draws a scene variable element of the lsit
        /// </summary>
        /// <param name="rect">The rectangle to render the variable in</param>
        /// <param name="index">The index of the property</param>
        /// <param name="isActive">True if the element is active, false if not</param>
        /// <param name="isSelected">True if the element is selected, false if not</param>
        private void DrawElement(Rect rect, int index, bool isActive, bool isSelected)
        {
            if (IsFilteredOut(index))
                return;
            
            SerializedSceneVariable serializedVar = serializedVariables.GetSerializedVariableAt(index);
            
            rect.yMin += (elementMargin * 0.5f);
            rect.height -= (elementMargin * 0.5f);

            EditorGUI.BeginChangeCheck();

            if (IsBuiltInVariable(serializedVar.ValueProp))
                RenderBuiltInVariable(serializedVar, rect, index, isSelected);
            else
                RenderCustomVariable(serializedVar, rect, index, isActive, isSelected);

            if (EditorGUI.EndChangeCheck())
            {
                serializedVariables.SerializedContainer.ApplyModifiedProperties();

                if (!EditorApplication.isPlaying)
                    serializedVariables.SceneVariablesContainer.ResetVariables();

                OnSomethingChanged.SafeInvoke(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Handler for when the delete button of the reorderable list is clicked
        /// </summary>
        /// <param name="list">The list that had the remove button clicked</param>
        private void OnRemoveClicked(ReorderableList list)
        {
            if (RemoveAction != null)
                RemoveAction(serializedVariables.VariableListProp, list.index);
        }
        
        /// <summary>
        /// Determines if the value of the item is considered a 'built-in'
        /// type or if it's a complex/custom type
        /// </summary>
        /// <param name="variableValueProp">The property representing the
        /// element value</param>
        /// <returns>True if it's a built in variable type, false if not</returns>
        private bool IsBuiltInVariable(SerializedProperty variableValueProp)
        {
            ScriptableObject objRef = variableValueProp.objectReferenceValue
                       as ScriptableObject;

            Type valueType = objRef.GetType();

            return valueType.IsSubclassOfRawGeneric(typeof(ScriptablePrimitive<>));
        }

        /// <summary>
        /// Handles rendering a variable that is considered a 'built-in' supported
        /// type
        /// </summary>
        /// <param name="serializedVar">The serialized representation of the variable</param>
        /// <param name="rect">The rect to render in</param>
        /// <param name="index">The index of the item in the list</param>
        /// <param name="isActive">True if the element is active, false if not</param>
        /// <param name="isSelected">True if the element is selected, false if not</param>
        private void RenderBuiltInVariable(SerializedSceneVariable serializedVar,
            Rect rect, int index, bool isSelected)
        {
            SerializedObject serializedValue = serializedVar.GetSerializedValue();

            SerializedProperty activeValueProp = serializedValue.FindProperty("DefaultValue");

            if (EditorApplication.isPlaying)
                activeValueProp = serializedValue.FindProperty("currentValue");
            
            Rect nameRect = new Rect(rect);
            nameRect.width = EditorGUIUtility.labelWidth;

            RenderVariableName(nameRect, serializedVar.NameProp, index, isSelected);

            Rect refreshToggleRect = new Rect(rect);
            refreshToggleRect.xMin = nameRect.xMax + 2f;
            refreshToggleRect.width = 20f;

            RenderRefreshToggle(refreshToggleRect, serializedVar);

            Rect valueRect = new Rect(rect);

            valueRect.xMin = refreshToggleRect.xMax + 2f;

            EditorGUI.BeginChangeCheck();

            EditorGUI.PropertyField(valueRect, activeValueProp, GUIContent.none);

            if (EditorGUI.EndChangeCheck())
                serializedValue.ApplyModifiedProperties();
        }

        /// <summary>
        /// Renders a SceneVariable defined as a 'custom' type (one that isn't considered
        /// 'built-in')
        /// </summary>
        /// <param name="serializedVar">The serialized representation of the variable</param>
        /// <param name="rect">The rect to render in</param>
        /// <param name="index">The index of the item in the list</param>
        /// <param name="isActive">True if the element is active, false if not</param>
        /// <param name="isSelected">True if the element is selected, false if not</param>
        private void RenderCustomVariable(SerializedSceneVariable serializedVar,
            Rect rect, int index, bool isActive, bool isSelected)
        {
            float originalHeight = rect.height;
            
            rect.height = EditorGUIUtility.singleLineHeight;
            
            Rect nameRect = new Rect(rect);
            nameRect.width = EditorGUIUtility.labelWidth;

            RenderVariableName(nameRect, serializedVar.NameProp, index, isSelected);

            Rect refreshToggleRect = new Rect(rect);
            refreshToggleRect.xMin = nameRect.xMax + 2f;
            refreshToggleRect.width = 20f;

            RenderRefreshToggle(refreshToggleRect, serializedVar);
            
            IInlineEditor inlineEditor = GetCustomInlineEditor(serializedVar);

            Rect valueRect = new Rect(rect);
            valueRect.yMin = nameRect.yMax + (elementMargin * 0.5f);
            valueRect.height = originalHeight - rect.height - (elementMargin * 0.5f);

            if (inlineEditor == null)
            {
                EditorGUI.BeginDisabledGroup(true);

                serializedVar.ValueProp.isExpanded =
                    EditorGUI.PropertyField(valueRect, serializedVar.ValueProp, GUIContent.none);

                EditorGUI.EndDisabledGroup();
            }
            else
                inlineEditor.OnInlineEditorGUI(valueRect);
        }

        /// <summary>
        /// Helper to get the inline editor for the provided property
        /// </summary>
        /// <param name="elementProperty">The variable property</param>
        /// <param name="valueProperty">The variable value property</param>
        /// <returns>The inline editor, or null if none exist</returns>
        private IInlineEditor GetCustomInlineEditor(SerializedSceneVariable serializedVariable)
        {
            IInlineEditor inlineEditor = null;

            string dictKey = serializedVariable.VariableElementProp.propertyPath;

            if (!inlineEditors.TryGetValue(dictKey, out inlineEditor))
            {
                Editor editor = CreateEditor(serializedVariable.ValueProp.objectReferenceValue);

                if (editor is IInlineEditor)
                {
                    inlineEditor = editor as IInlineEditor;
                    inlineEditors.Add(dictKey, inlineEditor);
                }
                else
                {
                    inlineEditors.Add(dictKey, null);
                    DestroyImmediate(editor);
                }
                
            }

            return inlineEditor;
        }

        /// <summary>
        /// Renders the variable name for the element, which is
        /// a label that can be transformed into a text box for editing
        /// </summary>
        /// <param name="nameProperty">The property representing the variable name</param>
        /// <param name="index">The index of the item to render</param>
        /// <param name="isSelected">True if the item is currently selected, false if not</param>
        private void RenderVariableName(Rect rect, SerializedProperty nameProperty, int index, bool isSelected)
        {
            EditorGUI.BeginChangeCheck();

            string previousName = nameProperty.stringValue;
            bool previousExpansion = nameProperty.isExpanded;

            nameProperty.isExpanded = EditorGUIExtensions.EditableLabel(rect,
                nameProperty, nameProperty.isExpanded);

            if (EditorGUI.EndChangeCheck())
            {
                OnVariableNameChanged.SafeInvoke(this,
                    new ValueChangedEventArgs<string>(previousName, nameProperty.stringValue));
            }
        }

        /// <summary>
        /// Handles display of the refresh-on-transition button
        /// </summary>
        /// <param name="elementProp">The owner of the refresh property</param>
        private void RenderRefreshToggle(Rect rect, SerializedSceneVariable serializedVar)
        {
            serializedVar.ResetOnTransProp.boolValue = GUI.Toggle(rect, 
                serializedVar.ResetOnTransProp.boolValue,
                refreshTransitionContent, SceneManagerResources.RefreshOnTransitionStyle);
        }

        #endregion

        #region Search Logic

        /// <summary>
        /// Determines if the variable at the specified index
        /// should be filtered out from being rendered
        /// </summary>
        /// <param name="index">The index of the list item</param>
        /// <returns>True to filter out, false to include</returns>
        private bool IsFilteredOut(int index)
        {
            if (!HasFilterString())
                return false;

            SerializedSceneVariable serializedVar = serializedVariables.GetSerializedVariableAt(index);
            
            return !serializedVar.NameProp.stringValue.
                Contains(currentSearchString, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Determines if there is a valid filter
        /// </summary>
        /// <returns>True if there is a filter, false if not</returns>
        private bool HasFilterString()
        {
            if (currentSearchString == null)
                return false;

            if (string.IsNullOrEmpty(currentSearchString.Trim()))
                return false;

            return true;
        }

        /// <summary>
        /// Determines how many items are filtered out by the current filter string
        /// </summary>
        private void RefreshFilteredIndex()
        {
            int entries = serializedVariables.VariableListProp.arraySize;

            filterIndices.Clear();

            int filterIndex = -1;

            for (int i = 0; i < entries; i++)
            {
                if (!IsFilteredOut(i))
                {
                    filterIndex++;
                    filterIndices.Add(i, filterIndex);
                }
            }
        }

        #endregion
    }
}

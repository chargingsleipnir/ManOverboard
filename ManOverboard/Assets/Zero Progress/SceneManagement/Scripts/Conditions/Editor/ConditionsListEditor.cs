using System;
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditorInternal;
using UnityEngine;
using ZeroProgress.Common;
using ZeroProgress.Common.Editors;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    public class ConditionsListEditor : Editor
    {
        private readonly static Type[] builtInConditionTypes = new Type[]
        {
            typeof(BoolSceneCondition),
            typeof(IntSceneCondition),
            typeof(FloatSceneCondition),
            typeof(StringSceneCondition)
        };

        private static Type[] customConditionTypes = new Type[0];

        public string ListHeader = "Conditions";

        private bool showElements = true;

        public bool ShowElements
        {
            get { return showElements; }
            set { showElements = value;

                if (conditionList == null)
                    return;

                conditionList.displayAdd = showElements;
                conditionList.displayRemove = showElements;
                conditionList.draggable = showElements;
            }
        }


        private SerializedProperty conditionsListProperty;

        private SerializedSceneManager serializedManager;

        private ReorderableList conditionList;
        
        private Dictionary<string, Editor> conditionEditors = 
            new Dictionary<string, Editor>();

        public void Initialize(SerializedSceneManager serializedManager, 
            SerializedProperty conditionsListProperty)
        {
            this.conditionsListProperty = conditionsListProperty;
            this.serializedManager = serializedManager;

            conditionList = new ReorderableList(conditionsListProperty.serializedObject, 
                conditionsListProperty, showElements, true, showElements, showElements);

            conditionList.drawHeaderCallback = OnDrawHeader;
            conditionList.elementHeightCallback = OnElementHeight;
            conditionList.drawElementCallback = OnDrawElement;
            conditionList.onAddDropdownCallback = OnAddDropDown;
            conditionList.onRemoveCallback = OnDeleteCondition;

            ClearEditors();
        }

        public override void OnInspectorGUI()
        {
            conditionsListProperty.serializedObject.Update();

            // For some reason re-ordering the conditions won't work without
            // this line. I'm not sure if there is some weird caching that's occuring
            // somewhere or what. Feels hackish, but leaving for now
            if (Event.current.type == EventType.Layout)
                ClearEditors();

            EditorGUILayout.BeginVertical();

            EditorGUI.BeginChangeCheck();
            
            conditionList.DoLayoutList();

            RenderSelectedEditor();

            if (EditorGUI.EndChangeCheck())
            {
                SceneManagerEditorWindow activeWindow = SceneManagerEditorWindow.TryGetExistingWindow();
                activeWindow.RefreshStatuses();

                conditionsListProperty.serializedObject.ApplyModifiedProperties();
            }

            EditorGUILayout.EndVertical();

        }

        private void RenderSelectedEditor()
        {
            int selectedIndex = conditionList.index;

            if (selectedIndex < 0)
                return;

            SerializedProperty conditionProp = conditionsListProperty.GetArrayElementAtIndex(selectedIndex);

            Editor editor = GetOrCreateEditor(conditionProp);

            if (editor is IInlineConditionEditor)
                return;

            EditorGUILayout.BeginVertical(GUI.skin.box);

            EditorGUILayout.LabelField(conditionProp.objectReferenceValue.GetType().Name);

            EditorGUILayout.Space();

            EditorGUI.indentLevel++;

            editor.OnInspectorGUI();

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        [DidReloadScripts]
        private static void OnScriptsCompiled()
        {
            customConditionTypes =
                AppDomain.CurrentDomain.GetAllTypesThatExtend<SceneCondition>(false)
                .Where((x) => !x.IsAbstract && !builtInConditionTypes.Contains(x)).ToArray();
        }

        #region Reorderable List Callbacks

        private void OnDrawHeader(Rect rect)
        {
            if (ListHeader == null)
                ListHeader = string.Empty;

            GUI.Label(rect, ListHeader);
        }

        private float OnElementHeight(int index)
        {
            if (!ShowElements)
                return 0f;

            SerializedProperty conditionProp = conditionsListProperty.GetArrayElementAtIndex(index);

            Editor editor = GetOrCreateEditor(conditionProp);

            IInlineConditionEditor inlineEditor = editor as IInlineConditionEditor;

            if (inlineEditor != null)
                return inlineEditor.GetInlineEditorHeight();
            else
                return EditorGUIUtility.singleLineHeight;
        }

        private void OnDrawElement(Rect rect, int index, bool isActive, bool isSelected)
        {
            if (!ShowElements)
                return;

            SerializedProperty conditionProp = conditionsListProperty.GetArrayElementAtIndex(index);
            
            Editor editor = GetOrCreateEditor(conditionProp);

            IInlineConditionEditor inlineEditor = editor as IInlineConditionEditor;

            if (inlineEditor != null)
            {
                inlineEditor.InitializeVariablesList(
                    serializedManager.TargetManager.SceneVariables);
                inlineEditor.OnInlineEditorGUI(rect);
            }
            else
                GUI.Label(rect, conditionProp.objectReferenceValue.GetType().Name);
        }
        
        private void OnAddDropDown(Rect buttonRect, ReorderableList list)
        {
            GenericMenu availableConditionsMenu = new GenericMenu();
            //TypeReferencePropertyDrawer
            foreach (Type builtIn in builtInConditionTypes)
            {
                availableConditionsMenu.AddItem(new GUIContent(builtIn.Name),
                    false, OnNewConditionTypeSelected, builtIn);
            }

            foreach(Type customType in customConditionTypes)
            {
                string typeName = TypeReferencePropertyDrawer.FormatByNamespaceFlat(customType);

                availableConditionsMenu.AddItem(
                    new GUIContent("Custom/" + typeName), false,
                    OnNewConditionTypeSelected, customType);
            }

            availableConditionsMenu.ShowAsContext();
        }

        private void OnNewConditionTypeSelected(System.Object selectedType)
        {
            Type type = selectedType as Type;

            if (type == null)
                return;
            
            string assetName = AssetDatabaseExtensions.GetUniqueSubAssetName(
                serializedManager.TargetManager, type.Name, StringComparison.OrdinalIgnoreCase);

            SceneCondition created = 
                serializedManager.CreateNewCondition(assetName, type);

            conditionsListProperty.AddArrayObjectValue(created);
        }

        private void OnDeleteCondition(ReorderableList list)
        {
            if (list.index < 0)
                return;

            SerializedProperty element = conditionsListProperty.GetArrayElementAtIndex(list.index);

            DeleteEditorAt(element);
            serializedManager.DeleteCondition(conditionsListProperty, list.index);

            list.index = -1;
        }

        #endregion

        private Editor GetOrCreateEditor(SerializedProperty elementProperty)
        {
            Editor existing;

            if (!conditionEditors.TryGetValue(elementProperty.propertyPath, out existing))
            {
                existing = CreateEditor(elementProperty.objectReferenceValue);
                conditionEditors.Add(elementProperty.propertyPath, existing);
            }

            return existing;
        }

        private void DeleteEditorAt(SerializedProperty elementProperty)
        {
            Editor existing;

            if (!conditionEditors.TryGetValue(elementProperty.propertyPath, out existing))
                return;

            DestroyImmediate(existing);
            conditionEditors.Remove(elementProperty.propertyPath);
        }

        private void ClearEditors()
        {
            foreach (KeyValuePair<string, Editor> editorPairing in conditionEditors)
            {
                DestroyImmediate(editorPairing.Value);
            }

            conditionEditors.Clear();
        }
    }
}
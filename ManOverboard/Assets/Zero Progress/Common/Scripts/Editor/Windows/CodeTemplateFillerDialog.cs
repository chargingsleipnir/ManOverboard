using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common.Reflection;

namespace ZeroProgress.Common.Editors
{
    /// <summary>
    /// Dialog window for displaying the fields to configure how the class
    /// file should be populated
    /// </summary>
    public class CodeTemplateFillerDialog : EditorWindow
    {
        private ScriptableCodeTemplate activeTemplate;
        
        private string outputPath;
        private DragReceiver folderDrag;

        private Dictionary<string, System.Object> parameterValues = new Dictionary<string, System.Object>();
        private IEnumerable<CodeGenerationParameter> codeParamList;

        private int selectedTab = 0;

        private string replacedString;
        private bool allParametersValid = false;

        private Vector2 codePreviewScrollPos;
        private Vector2 parametersScrollPos;

        private ClassExtendsAttribute typeDropdownFilter = new ClassExtendsAttribute(typeof(System.Object))
            { Grouping = ClassGrouping.ByNamespace, AllowStructs = true };

        [MenuItem("Assets/Populate Code Template")]
        private static void ShowWindow()
        {
            CodeTemplateFillerDialog window = 
                CreateInstance(typeof(CodeTemplateFillerDialog)) as CodeTemplateFillerDialog;

            window.Show();

            if(Selection.activeObject != null)
            {
                if (Selection.activeObject.GetType().IsSubclassOf(typeof(ScriptableCodeTemplate)) ||
                    Selection.activeObject.GetType() == typeof(ScriptableCodeTemplate))
                {
                    window.ApplyTemplate(Selection.activeObject as ScriptableCodeTemplate);
                }
                else
                    window.outputPath = AssetDatabase.GetAssetPath(Selection.activeObject) + "/";
            }
        }

        public void ApplyTemplate(ScriptableCodeTemplate template)
        {
            if (activeTemplate == template)
                return;

            activeTemplate = template;
            parameterValues.Clear();

            replacedString = string.Empty;
            allParametersValid = false;

            codeParamList = new List<CodeGenerationParameter>();

            if (template == null)
                return;
            
            codeParamList = ReflectionUtilities.GetFieldByName
                <IEnumerable<CodeGenerationParameter>>(activeTemplate,
                "parameters", BindingFlags.Instance | BindingFlags.NonPublic);
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();

            RenderActiveTemplateSection();

            if (activeTemplate == null)
                return;

            outputPath = EditorGUILayoutExtensions.FolderTextbox(outputPath, new GUIContent("Output Path:"), position.position);
            
            EditorGUILayout.BeginHorizontal();

            RenderCodeParameters();

            RenderCodePreviewTab();

            EditorGUILayout.EndHorizontal();

            RenderGenerateButton();
        }

        private void RenderActiveTemplateSection()
        {
            ScriptableCodeTemplate current = activeTemplate;

            UnityEngine.Object selectedObject = EditorGUILayout.ObjectField("Active Template:", activeTemplate, typeof(ScriptableCodeTemplate), false);

            ScriptableCodeTemplate selected = selectedObject as ScriptableCodeTemplate;

            if (selected != current)
                ApplyTemplate(selected);
        }

        private void RenderCodeParameters()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(position.width * 0.5f));

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Parameters: ");

            EditorGUI.indentLevel++;

            parametersScrollPos = EditorGUILayout.BeginScrollView(parametersScrollPos);

            foreach (CodeGenerationParameter codeParam in codeParamList)
            {
                RenderCodeParameter(codeParam);
            }

            EditorGUILayout.EndScrollView();

            EditorGUI.indentLevel--;

            EditorGUILayout.EndVertical();
        }

        private void RenderCodeParameter(CodeGenerationParameter parameter)
        {
            System.Object existingValue;

            parameterValues.TryGetValue(parameter.Identifier, out existingValue);

            string displayName = ObjectNames.NicifyVariableName(parameter.Identifier);

            System.Object newValue = null;

            switch (parameter.ParameterType)
            {
                case "int":
                    int intValue = existingValue == null ? 0 : (int)existingValue;

                    intValue = EditorGUILayout.IntField(displayName, intValue);
                    
                    parameterValues[parameter.Identifier] = intValue;
                    newValue = intValue;
                    break;

                case "string":
                    string stringValue = existingValue == null ? string.Empty : (string)existingValue;

                    stringValue = EditorGUILayout.TextField(displayName, stringValue);

                    parameterValues[parameter.Identifier] = stringValue;
                    newValue = stringValue;
                    break;

                case "float":
                    float floatValue = existingValue == null ? 0f : (float)existingValue;

                    floatValue = EditorGUILayout.FloatField(displayName, floatValue);

                    parameterValues[parameter.Identifier] = floatValue;
                    newValue = floatValue;
                    break;

                case "double":
                    double doubleValue = existingValue == null ? 0 : (double)existingValue;

                    doubleValue = EditorGUILayout.DoubleField(displayName, doubleValue);

                    parameterValues[parameter.Identifier] = doubleValue;
                    newValue = doubleValue;
                    break;

                case "type":
                    Type typeValue = existingValue as Type;

                    string currentSelection = typeValue == null ? "" : typeValue.AssemblyQualifiedName;

                    Rect typeRect = EditorGUILayout.GetControlRect();

                    string selection = TypeReferencePropertyDrawer.DrawTypeSelectionControl(typeRect,
                        new GUIContent(displayName), currentSelection, typeDropdownFilter);

                    typeValue = Type.GetType(selection);

                    parameterValues[parameter.Identifier] = typeValue;
                    newValue = typeValue;
                    break;

                default:
                    Debug.LogError(parameter.ParameterType + " is not a supported input type");
                    break;
            }

            if (newValue != null && newValue.Equals(existingValue))
                RefreshReplacementString();
        }

        private void RenderCodePreviewTab()
        {
            EditorGUILayout.BeginVertical();

            selectedTab = GUILayout.Toolbar(selectedTab, new string[] { "Preview", "Original" });

            switch (selectedTab)
            {
                case 0:
                    RenderPreviewWindow(replacedString);                    
                    break;

                case 1:
                    RenderPreviewWindow(activeTemplate.CurrentValue);
                    break;

                default:
                    return;
            }

            EditorGUILayout.EndVertical();
        }

        private void RenderPreviewWindow(string text)
        {
            codePreviewScrollPos = EditorGUILayout.BeginScrollView(codePreviewScrollPos);

            EditorGUI.BeginDisabledGroup(true);
                        
            EditorGUILayout.TextArea(text, GUILayout.ExpandHeight(true), GUILayout.Width(position.width * 0.5f));

            EditorGUI.EndDisabledGroup();

            EditorGUILayout.EndScrollView();
        }

        private void RenderGenerateButton()
        {
            EditorGUILayout.BeginHorizontal();

            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Generate"))
                GenerateCode();

            EditorGUILayout.EndHorizontal();
        }

        private void RefreshReplacementString()
        {
            replacedString = activeTemplate.CurrentValue;

            allParametersValid = true;

            foreach (CodeGenerationParameter parameter in codeParamList)
            {
                System.Object paramValue;

                if (!parameterValues.TryGetValue(parameter.Identifier, out paramValue) ||
                    paramValue == null)
                {
                    allParametersValid = false;
                    continue;
                }

                replacedString = replacedString.Replace(parameter.ReplacementString, paramValue.ToString());
            }
        }

        private void GenerateCode()
        {
            if(!allParametersValid)
            {
                Debug.LogError("Not all parameters valid, cannot create");
                return;
            }

            string validPath = AssetDatabaseExtensions.GetValidAssetPath(outputPath, ".cs");

            if (string.IsNullOrEmpty(validPath))
                return;

            Debug.Log(validPath);

            // Create any required folders
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(validPath));
            
            if(System.IO.File.Exists(validPath))
            {
                if (!EditorUtility.DisplayDialog("File Exists!", 
                    "File Exists. Do you want to overwrite it?", "Yes", "No"))
                    return;
            }

            // Create the file
            System.IO.File.WriteAllText(validPath, replacedString);

            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }
    }
}
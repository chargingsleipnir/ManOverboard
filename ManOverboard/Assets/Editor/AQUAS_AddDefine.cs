using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

namespace AQUAS
{
	[InitializeOnLoad]
	public class AQUAS_AddDefine : Editor {
        
		static AQUAS_AddDefine()
		{

            var symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

			if (!symbols.Contains("AQUAS_PRESENT"))
			{
				symbols += ";" + "AQUAS_PRESENT";
				PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
			}

            if (!symbols.Contains("UNITY_POST_PROCESSING_STACK_V1") && File.Exists (Application.dataPath + "/PostProcessing/Runtime/PostProcessingBehaviour.cs"))
            {
                symbols += ";" + "UNITY_POST_PROCESSING_STACK_V1";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }

            if (!symbols.Contains("UNITY_POST_PROCESSING_STACK_V2") && File.Exists(Application.dataPath + "/PostProcessing/Runtime/PostProcessLayer.cs"))
            {
                symbols += ";" + "UNITY_POST_PROCESSING_STACK_V2";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }

            if (!symbols.Contains("UNITY_POST_PROCESSING_STACK_V2") && Directory.Exists(Application.dataPath + "/PostProcessing-2"))
            {
                symbols += ";" + "UNITY_POST_PROCESSING_STACK_V2";
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }

            // the following only removes the scripting define symbol if the pp stack v2 is not in the project!
            if (symbols.Contains("UNITY_POST_PROCESSING_STACK_V2") && !Directory.Exists(Application.dataPath + "/PostProcessing-2"))
            {
                symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

                symbols = symbols.Replace("UNITY_POST_PROCESSING_STACK_V2;", "");
                symbols = symbols.Replace("UNITY_POST_PROCESSING_STACK_V2", "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }

            if (symbols.Contains("UNITY_POST_PROCESSING_STACK_V2") && !File.Exists(Application.dataPath + "/PostProcessing/Runtime/PostProcessLayer.cs"))
            {
                symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup);

                symbols = symbols.Replace("UNITY_POST_PROCESSING_STACK_V2;", "");
                symbols = symbols.Replace("UNITY_POST_PROCESSING_STACK_V2", "");
                PlayerSettings.SetScriptingDefineSymbolsForGroup(EditorUserBuildSettings.selectedBuildTargetGroup, symbols);
            }
        }
	}
}

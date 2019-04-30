using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Model representing a parameter found in a code template
    /// </summary>
    [Serializable]
    public class CodeGenerationParameter
    {
        /// <summary>
        /// The name of the parameter for display
        /// </summary>
        public string Identifier;

        /// <summary>
        /// The type of the item to be added
        /// </summary>
        public string ParameterType;

        /// <summary>
        /// The text to be replaced with the provided value
        /// </summary>
        public string ReplacementString;

        public override string ToString()
        {
            return "Identifier: " + Identifier +
                "\nParameter Type: " + ParameterType +
                "\nReplacement String: " + ReplacementString;
        }
    }

    /// <summary>
    /// An asset representing a code template to be used for generating code
    /// </summary>
    [CreateAssetMenu(fileName = "New Code Template", menuName = ScriptableObjectPaths.ZERO_PROGRESS_CODE_GENERATION_PATH + "Code Template")]
    public class ScriptableCodeTemplate : ScriptableString
    {
        private const string PARAMETER_IDENTIFIER = "###";

        /// <summary>
        /// The collection of parsed out parameters
        /// </summary>
        [SerializeField]
        private List<CodeGenerationParameter> parameters;
        
        private void ParseParameters(string template)
        {
            parameters.Clear();

            Dictionary<string, CodeGenerationParameter> extractedParameters = new Dictionary<string, CodeGenerationParameter>();

            int paramStartIndex = template.IndexOf(PARAMETER_IDENTIFIER);

            if (paramStartIndex < 0)
                return;

            do
            {
                string parameter = ExtractParameter(template, ref paramStartIndex);

                CodeGenerationParameter generationParameter = CreateGenerationParameter(parameter);

                if (generationParameter != null && !extractedParameters.ContainsKey(generationParameter.Identifier))
                    extractedParameters.Add(generationParameter.Identifier, generationParameter);

            } while (paramStartIndex >= 0);

            parameters.AddRange(extractedParameters.Values);
        }

        private string ExtractParameter(string template, ref int startIndex)
        {
            int paramEndIndex = template.IndexOf(PARAMETER_IDENTIFIER, startIndex + 1);

            if (paramEndIndex < 0)
                throw new ArgumentException("Closing " + PARAMETER_IDENTIFIER + " not found");

            string parameter = template.Substring(startIndex,
                paramEndIndex - startIndex + PARAMETER_IDENTIFIER.Length);
            
            startIndex = template.IndexOf(PARAMETER_IDENTIFIER, paramEndIndex + 1);

            return parameter;
        }

        private CodeGenerationParameter CreateGenerationParameter(string parameter)
        {
            string paramWithoutIdentifiers = parameter.Replace("###", "");

            string[] split = paramWithoutIdentifiers.Split(new char[] { ':' }, 2, StringSplitOptions.RemoveEmptyEntries);

            if(split.Length != 2)
            {
                Debug.LogError("Invalid structure for parameter. Should be ###IDENTIFIER:TYPE###. Instead found " + parameter);
                return null;
            }

            string identifier = split[0].Trim();
            string typeStr = split[1].Trim();

            Type type = MatchType(typeStr);

            if(type == null)
            {
                Debug.LogError("Could not match type for " + parameter + "\nType string recognized as: " + typeStr);
                return null;
            }

            CodeGenerationParameter generationParameter = new CodeGenerationParameter();
            generationParameter.Identifier = identifier;
            generationParameter.ParameterType = typeStr;
            generationParameter.ReplacementString = parameter;

            return generationParameter;
        }

        private Type MatchType(string typeString)
        {
            switch (typeString.ToLower())
            {
                case "int":
                    return typeof(int);
                case "string":
                    return typeof(string);
                case "float":
                    return typeof(float);
                case "double":
                    return typeof(double);
                case "type":
                    return typeof(Type);
                default:
                    return null;
            }
        }
    }
}
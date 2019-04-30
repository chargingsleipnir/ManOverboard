using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using ZeroProgress.Common;

namespace ZeroProgress.SceneManagementUtility.Editors
{
    public class VariableDetails : IEquatable<VariableDetails>
    {
        public static readonly VariableDetails NoneVariable =
            new VariableDetails("(None)", null);

        public readonly string Name;
        public readonly Type VariableType;

        public VariableDetails(string name, Type type)
        {
            Name = name;
            VariableType = type;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode() | VariableType.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            VariableDetails varDetails = obj as VariableDetails;

            if (varDetails == null)
                return false;

            return Equals(varDetails);
        }

        public bool Equals(VariableDetails other)
        {
            return other.Name == Name && other.VariableType == VariableType;
        }

        public static bool operator == (VariableDetails v1, VariableDetails v2)
        {
            if (System.Object.ReferenceEquals(v1, null))
                return System.Object.ReferenceEquals(v2, null);
            
            return v1.Equals(v2);
        }

        public static bool operator != (VariableDetails v1, VariableDetails v2)
        {
            return !(v1 == v2);
        }
    }

    public interface IVariableListProvider
    {
        VariableDetails[] VariablesList
        {
            get;
            set;
        }

    }

    public interface  IInlineConditionEditor : IInlineEditor
    {
        void InitializeVariablesList(SceneVariableContainer variablesContainer);
    }

    /// <summary>
    /// Base editor for conditions that can be rendered inline of the
    /// conditions list
    /// </summary>
    public abstract class InlineSceneConditionEditor : Editor, IVariableListProvider, IInlineConditionEditor
    {
        public delegate bool IsValidVariableDelegate(Type variableType);

        private VariableDetails[] variablesList;

        public VariableDetails[] VariablesList
        {
            get
            {
                return variablesList;
            }

            set
            {
                variablesList = AddNoneOption(value);
            }
        }

        public abstract float GetInlineEditorHeight();

        public abstract void OnInlineEditorGUI(Rect rect);

        public void InitializeVariablesList(SceneVariableContainer variablesContainer)
        {
            IEnumerable<KeyValuePair<string, Type>> varDetails = 
                variablesContainer.GetVariableDetails();

            List<VariableDetails> details = new List<VariableDetails>();

            foreach (var varDetail in varDetails)
            {
                VariableDetails detail = new VariableDetails(varDetail.Key, varDetail.Value);
                details.Add(detail);
            }

            VariablesList = details.ToArray();
        }

        /// <summary>
        /// Render the Variable Name dropdown
        /// </summary>
        /// <param name="label">The label to display</param>
        /// <param name="currentValue">The currently selected variable value</param>
        /// <param name="options">The available options</param>
        /// <param name="variableTypeFilter">The type of variables to use</param>
        /// <returns>The name of the selected variable, or string.empty
        /// if none</returns>
        protected static string RenderVariableSelector(Rect rect, string label,
            string currentValue, VariableDetails[] options, IsValidVariableDelegate variableTypeFilter)
        {
            VariableDetails[] filtered = options.Where(
                (x) => variableTypeFilter(x.VariableType)).ToArray();

            filtered = AddNoneOption(filtered);

            int selectedIndex = Array.FindIndex(filtered,
                (x) => x.Name.Equals(currentValue, StringComparison.OrdinalIgnoreCase));
            
            if (selectedIndex < 0)
                selectedIndex = 0;

            selectedIndex = EditorGUI.Popup(rect, label,
                selectedIndex, filtered.Select((x) => x.Name).ToArray());
            
            if (selectedIndex < 0 || selectedIndex >= filtered.Length)
                return "";

            VariableDetails selected = filtered[selectedIndex];

            if (selected == VariableDetails.NoneVariable)
                return string.Empty;
            
            return selected.Name;
        }
        
        /// <summary>
        /// Helper to make sure that the variable details array contains
        /// a None option
        /// </summary>
        /// <param name="variableDetails">The array to ensure contains a None option</param>
        /// <returns>The array with a none option</returns>
        private static VariableDetails[] AddNoneOption(VariableDetails[] variableDetails)
        {
            if (variableDetails == null)
            {
                variableDetails = new VariableDetails[] { VariableDetails.NoneVariable };
                return variableDetails;
            }

            int index = Array.FindIndex(variableDetails, (x) => x == VariableDetails.NoneVariable);

            if (index < 0)
            {
                VariableDetails[] newList = new VariableDetails[variableDetails.Length + 1];
                newList[0] = VariableDetails.NoneVariable;
                variableDetails.CopyTo(newList, 1);
                return newList;
            }
            else if(index != 0)
            {
                variableDetails.SwapValues(0, index);
            }

            return variableDetails;
        }

    }
}
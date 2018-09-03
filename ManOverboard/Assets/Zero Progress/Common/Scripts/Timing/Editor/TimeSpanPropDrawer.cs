using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ZeroProgress.Common
{
    [CustomPropertyDrawer(typeof(DisplayTimeSpanAttribute))]
    public class TimeSpanPropDrawer : PropertyDrawer
    {
        /// <summary>
        /// Caches the selected interval unit in SingleModeSelectable display mode
        /// </summary>
        private Dictionary<string, TimeIntervalType> intervalSelectionCache = new Dictionary<string, TimeIntervalType>();

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            DisplayTimeSpanAttribute displayAttribute = attribute as DisplayTimeSpanAttribute;

            TimeSpanDisplayMode displayMode = displayAttribute.DisplayMode;

            switch (displayMode)
            {
                case TimeSpanDisplayMode.SingleMode:
                case TimeSpanDisplayMode.SingleModeSelectable:
                    return EditorGUIUtility.singleLineHeight * 2f;
                case TimeSpanDisplayMode.ColumnMode:
                default:
                    return EditorGUIUtility.singleLineHeight * 3f;
            }
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            DisplayTimeSpanAttribute timeSpanAttribute = attribute as DisplayTimeSpanAttribute;

            Rect valuesRect = DisplayLabel(position, label);

            TimeSpanDisplayMode displayMode = timeSpanAttribute.DisplayMode;

            EditorGUI.indentLevel++;
            Rect indentedValuesRect = EditorGUI.IndentedRect(valuesRect);
            EditorGUI.indentLevel--;

            switch (displayMode)
            {
                case TimeSpanDisplayMode.SingleMode:
                    DisplayTimeSpanWithSingleUnit(indentedValuesRect, property, timeSpanAttribute);
                    break;
                case TimeSpanDisplayMode.SingleModeSelectable:
                    DisplayTimeSpanWithSelectableUnit(indentedValuesRect, property, timeSpanAttribute);
                    break;
                case TimeSpanDisplayMode.ColumnMode:
                default:
                    DisplayTimeSpanAsColumns(indentedValuesRect, property, timeSpanAttribute);
                    break;
            }
        }

        /// <summary>
        /// Handles displaying the identifier of the property
        /// </summary>
        /// <param name="position">The rect defining the allowable area for the entire property drawer</param>
        /// <param name="label">The label to be displayed</param>
        /// <returns>The position rect adjusted to be under the label</returns>
        protected virtual Rect DisplayLabel(Rect position, GUIContent label)
        {
            Rect labelRect = new Rect(position);
            labelRect.height = EditorGUIUtility.singleLineHeight;

            GUI.Label(labelRect, label);

            Rect remainingSpace = new Rect(position);
            remainingSpace.y += EditorGUIUtility.singleLineHeight;
            remainingSpace.height -= EditorGUIUtility.singleLineHeight;

            return remainingSpace;
        }

        /// <summary>
        /// Renders the time span data as a float field with a drop-down to select the time unit (milliseconds, seconds, minutes, hours)
        /// </summary>
        /// <param name="position">The rectangle depicting the available render area</param>
        /// <param name="property">The property containing the float value to be displayed as a time span</param>
        /// <param name="displayTimeSpanAttribute">The attribute that identifies the float as a time span value</param>
        /// <returns>A rectangle depicting the remaining available area</returns>
        protected virtual Rect DisplayTimeSpanWithSingleUnit(Rect position, SerializedProperty property, DisplayTimeSpanAttribute displayTimeSpanAttribute)
        {
            TimeSpan propertyTimeSpanValue = displayTimeSpanAttribute.UnderlyingValueType.GetTimeSpanValue(property.floatValue);

            float columnSpacing = 1f;

            float columnWidth = (position.width * 0.5f) - columnSpacing;

            Rect valueRect = new Rect(position);
            valueRect.width = columnWidth;
            valueRect.height = EditorGUIUtility.singleLineHeight;

            Rect unitRect = new Rect(valueRect);
            unitRect.x = valueRect.xMax + columnSpacing;

            TimeIntervalType timeSpanUnit = displayTimeSpanAttribute.UnderlyingValueType;
            
            EditorGUI.LabelField(unitRect, timeSpanUnit.ToString());

            float currentValue = timeSpanUnit.GetFloatValue(propertyTimeSpanValue);

            currentValue = Mathf.Abs(EditorGUI.FloatField(valueRect, currentValue));

            TimeSpan currentTimespanValue = timeSpanUnit.GetTimeSpanValue(currentValue);

            property.floatValue = displayTimeSpanAttribute.UnderlyingValueType.GetFloatValue(currentTimespanValue);

            Rect returnRect = new Rect(position);
            returnRect.y += EditorGUIUtility.singleLineHeight;
            returnRect.height -= EditorGUIUtility.singleLineHeight;

            return returnRect;
        }

        /// <summary>
        /// Renders the time span data as a float field with a drop-down to select the time unit (milliseconds, seconds, minutes, hours)
        /// </summary>
        /// <param name="position">The rectangle depicting the available render area</param>
        /// <param name="property">The property containing the float value to be displayed as a time span</param>
        /// <param name="displayTimeSpanAttribute">The attribute that identifies the float as a time span value</param>
        /// <returns>A rectangle depicting the remaining available area</returns>
        protected virtual Rect DisplayTimeSpanWithSelectableUnit(Rect position, SerializedProperty property, DisplayTimeSpanAttribute displayTimeSpanAttribute)
        {
            TimeSpan propertyTimeSpanValue = displayTimeSpanAttribute.UnderlyingValueType.GetTimeSpanValue(property.floatValue);

            float columnSpacing = 1f;

            float columnWidth = (position.width * 0.5f) - columnSpacing;

            Rect valueRect = new Rect(position);
            valueRect.width = columnWidth;
            valueRect.height = EditorGUIUtility.singleLineHeight;

            Rect unitRect = new Rect(valueRect);
            unitRect.x = valueRect.xMax + columnSpacing;

            TimeIntervalType timeSpanUnit;

            if (!intervalSelectionCache.TryGetValue(property.propertyPath, out timeSpanUnit))
            {
                timeSpanUnit = displayTimeSpanAttribute.UnderlyingValueType;
                intervalSelectionCache.Add(property.propertyPath, timeSpanUnit);
            }

            timeSpanUnit = (TimeIntervalType)EditorGUI.EnumPopup(unitRect, timeSpanUnit);

            intervalSelectionCache[property.propertyPath] = timeSpanUnit;

            float currentValue = timeSpanUnit.GetFloatValue(propertyTimeSpanValue);

            currentValue = Mathf.Abs(EditorGUI.FloatField(valueRect, currentValue));

            TimeSpan currentTimespanValue = timeSpanUnit.GetTimeSpanValue(currentValue);

            property.floatValue = displayTimeSpanAttribute.UnderlyingValueType.GetFloatValue(currentTimespanValue);

            Rect returnRect = new Rect(position);
            returnRect.y += EditorGUIUtility.singleLineHeight;
            returnRect.height -= EditorGUIUtility.singleLineHeight;

            return returnRect;
        }

        /// <summary>
        /// Renders the time span data as columns pertaining to each unit (milliseconds, seconds, minutes, hours)
        /// </summary>
        /// <param name="position">The rectangle depicting the available render area</param>
        /// <param name="property">The property containing the float value to be displayed as a time span</param>
        /// <param name="displayTimeSpanAttribute">The attribute that identifies the float as a time span value</param>
        /// <returns>A rectangle depicting the remaining available area</returns>
        protected virtual Rect DisplayTimeSpanAsColumns(Rect position, SerializedProperty property, DisplayTimeSpanAttribute displayTimeSpanAttribute)
        {
            float columnSpacing = 1f;

            // separate into 4 columns (Hours, Minutes, Seconds, Milliseconds)
            // we're not going to support days because it's going to be rare we need a timer that long
            float columnWidth = (position.width / 4f) - columnSpacing;

            TimeSpan propertyTimeSpanValue = displayTimeSpanAttribute.UnderlyingValueType.GetTimeSpanValue(property.floatValue);

            string[] timeTypeNames = Enum.GetNames(typeof(TimeIntervalType));
            float[] timeTypeValues = GetTimeIntervalValues(propertyTimeSpanValue);

            float xPosition = position.x;

            // Skip the Day value by starting at the second last element (timeTypeNames.Length - 2)
            for (int i = timeTypeNames.Length - 2; i >= 0; i--)
            {
                string currentIntervalName = timeTypeNames[i];
                float currentIntervalValue = timeTypeValues[i];

                Rect displayRect = new Rect(position);
                displayRect.x = xPosition;
                displayRect.width = columnWidth;
                displayRect.height = EditorGUIUtility.singleLineHeight;

                GUI.Label(displayRect, currentIntervalName);

                displayRect.y += EditorGUIUtility.singleLineHeight;

                timeTypeValues[i] = Mathf.Abs(EditorGUI.FloatField(displayRect, currentIntervalValue));

                xPosition = displayRect.xMax + columnSpacing;
            }

            propertyTimeSpanValue = SetTimeIntervalValues(timeTypeValues);

            property.floatValue = displayTimeSpanAttribute.UnderlyingValueType.GetFloatValue(propertyTimeSpanValue);

            Rect returnRect = new Rect(position);
            returnRect.y += EditorGUIUtility.singleLineHeight * 2f;
            returnRect.height -= EditorGUIUtility.singleLineHeight * 2f;

            return returnRect;
        }

        /// <summary>
        /// Helper to retrieve the individual unit values from a time span
        /// </summary>
        /// <param name="Time">The timespan to extract the data from</param>
        /// <returns>>An array of floats containing the value for each unit (Milliseconds, Seconds, Minutes, Hours, Days)</returns>
        protected float[] GetTimeIntervalValues(TimeSpan Time)
        {
            float[] values = new float[Enum.GetNames(typeof(TimeIntervalType)).Length];

            values[(int)TimeIntervalType.Days] = Time.Days;
            values[(int)TimeIntervalType.Hours] = Time.Hours;
            values[(int)TimeIntervalType.Minutes] = Time.Minutes;
            values[(int)TimeIntervalType.Seconds] = Time.Seconds;
            values[(int)TimeIntervalType.Milliseconds] = Time.Milliseconds;

            return values;
        }

        /// <summary>
        /// Sets a timespan from an array of floats depicting the individual units
        /// </summary>
        /// <param name="Values">An array of floats containing the value for each unit (Milliseconds, Seconds, Minutes, Hours, Days)</param>
        /// <returns>A time span object composed of all of the individual values</returns>
        protected TimeSpan SetTimeIntervalValues(float[] Values)
        {
            TimeSpan days = TimeSpan.FromDays(Values[(int)TimeIntervalType.Days]);
            TimeSpan hours = TimeSpan.FromHours(Values[(int)TimeIntervalType.Hours]);
            TimeSpan minutes = TimeSpan.FromMinutes(Values[(int)TimeIntervalType.Minutes]);
            TimeSpan seconds = TimeSpan.FromSeconds(Values[(int)TimeIntervalType.Seconds]);
            TimeSpan milliseconds = TimeSpan.FromMilliseconds(Values[(int)TimeIntervalType.Milliseconds]);

            return days + hours + minutes + seconds + milliseconds;
        }
    }
}
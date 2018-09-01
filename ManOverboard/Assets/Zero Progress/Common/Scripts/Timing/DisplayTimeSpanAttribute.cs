using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Types of time intervals
    /// </summary>
    public enum TimeIntervalType
    {
        Milliseconds,
        Seconds,
        Minutes,
        Hours,
        Days
    }

    /// <summary>
    /// The different built-in styles to display a Time Span
    /// </summary>
    public enum TimeSpanDisplayMode
    {
        /// <summary>
        /// Uses the underlying unit (or the preferences override) (Milliseconds, Seconds, etc) and has a field to type in a value
        /// </summary>
        SingleMode,
        /// <summary>
        /// Displays a single field but with a selectable unit
        /// </summary>
        SingleModeSelectable,
        /// <summary>
        /// Enter a value in each of the unit fields (i.e. 30 seconds 500 milliseconds)
        /// </summary>
        ColumnMode
    }

    /// <summary>
    /// Extensions attached to the TimeIntervalType enumeration
    /// </summary>
    public static class TimeIntervalTypeExtensions
    {
        /// <summary>
        /// Converts the provided float value to its TimeSpan representation based on the
        /// currently selected IntervalType
        /// </summary>
        /// <param name="ThisIntervalType">The interval type that defines the conversion</param>
        /// <param name="FloatValue">The value to be converted</param>
        /// <returns>The TimeSpan representing the float value</returns>
        public static TimeSpan GetTimeSpanValue(this TimeIntervalType ThisIntervalType, float FloatValue)
        {
            return ThisIntervalType.GetTimeSpanValue((Double)FloatValue);
        }

        /// <summary>
        /// Converts the provided double value to its TimeSpan representation based on the
        /// currently selected IntervalType
        /// </summary>
        /// <param name="ThisIntervalType">The interval type that defines the conversion</param>
        /// <param name="DoubleValue">The value to be converted</param>
        /// <returns>The TimeSpan representing the float value</returns>
        public static TimeSpan GetTimeSpanValue(this TimeIntervalType ThisIntervalType, double DoubleValue)
        {
            switch (ThisIntervalType)
            {
                case TimeIntervalType.Milliseconds:
                    return TimeSpan.FromMilliseconds(DoubleValue);
                case TimeIntervalType.Seconds:
                    return TimeSpan.FromSeconds(DoubleValue);
                case TimeIntervalType.Minutes:
                    return TimeSpan.FromMinutes(DoubleValue);
                case TimeIntervalType.Hours:
                    return TimeSpan.FromHours(DoubleValue);
                case TimeIntervalType.Days:
                    return TimeSpan.FromDays(DoubleValue);
                default:
                    throw new ArgumentOutOfRangeException("Invalid Enum Value");
            }
        }

        /// <summary>
        /// Converts the provided TimeSpan to its float representation based on the 
        /// currently selected IntervalType
        /// </summary>
        /// <param name="ThisIntervalType">The interval type that defines the conversion</param>
        /// <param name="TimeSpanValue">The value to be converted</param>
        /// <returns>The float value representing the TimeSpan in the specified unit</returns>
        public static float GetFloatValue(this TimeIntervalType ThisIntervalType, TimeSpan TimeSpanValue)
        {
            return (float)ThisIntervalType.GetDoubleValue(TimeSpanValue);
        }

        /// <summary>
        /// Converts the provided TimeSpan to its double representation based on the 
        /// currently selected IntervalType
        /// </summary>
        /// <param name="ThisIntervalType">The interval type that defines the conversion</param>
        /// <param name="TimeSpanValue">The value to be converted</param>
        /// <returns>The float value representing the TimeSpan in the specified unit</returns>
        public static double GetDoubleValue(this TimeIntervalType ThisIntervalType, TimeSpan TimeSpanValue)
        {
            switch (ThisIntervalType)
            {
                case TimeIntervalType.Milliseconds:
                    return TimeSpanValue.TotalMilliseconds;
                case TimeIntervalType.Seconds:
                    return TimeSpanValue.TotalSeconds;
                case TimeIntervalType.Minutes:
                    return TimeSpanValue.TotalMinutes;
                case TimeIntervalType.Hours:
                    return TimeSpanValue.TotalHours;
                case TimeIntervalType.Days:
                    return TimeSpanValue.TotalDays;
                default:
                    throw new ArgumentOutOfRangeException("Invalid Enum Value");
            }
        }
    }

    /// <summary>
    /// An attribute that indicates that a float should be treated as 
    /// a TimeSpan
    /// </summary>
    public class DisplayTimeSpanAttribute : PropertyAttribute
    {
        /// <summary>
        /// The value that the underlying float represents
        /// 
        /// Defaults to Seconds
        /// </summary>
        public TimeIntervalType UnderlyingValueType = TimeIntervalType.Seconds;

        /// <summary>
        /// The mode that drives how the time span information is displayed
        /// </summary>
        public TimeSpanDisplayMode DisplayMode = TimeSpanDisplayMode.SingleMode;
    }
}
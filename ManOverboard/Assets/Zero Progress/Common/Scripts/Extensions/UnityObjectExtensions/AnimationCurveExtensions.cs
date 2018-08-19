using System;
using UnityEngine;

namespace ZeroProgress.Common
{
    /// <summary>
    /// Extensions for the AnimationCurve type
    /// </summary>
    public static class AnimationCurveExtensions
    {
        /// <summary>
        /// Retrieves the start time for this animation curve
        /// </summary>
        /// <param name="ThisCurve">The animation curve to retrieve the start time for</param>
        /// <returns>The value of the start time, or -1.0f if there are no keys</returns>
        public static float GetStartTime(this AnimationCurve ThisCurve)
        {
            if (ThisCurve.length == 0)
                return -1.0f;

            return ThisCurve.keys[0].time;
        }

        /// <summary>
        /// Retrieves the end time for this animation curve
        /// </summary>
        /// <param name="ThisCurve">The animation curve to retrieve the end time for</param>
        /// <returns>The value of the end time, or -1.0f if there are no keys</returns>
        public static float GetEndTime(this AnimationCurve ThisCurve)
        {
            if (ThisCurve.length == 0)
                return -1.0f;

            return ThisCurve.keys[ThisCurve.length - 1].time;
        }

        /// <summary>
        /// Creates a new animation curve where the key times of this curve
        /// are scaled by the corresponding amount
        /// </summary>
        /// <param name="ThisCurve">The curve to copy keys from</param>
        /// <param name="ScaleValue">The value to adjust by</param>
        /// <returns>A new curve with all key times adjusted by the corresponding scale</returns>
        public static AnimationCurve ScaleCurveTime(this AnimationCurve ThisCurve, float ScaleValue)
        {
            if (ScaleValue < 0.0f)
                throw new ArgumentException("Scale value cannot be less than 0");

            if (ThisCurve.length == 0)
                return ThisCurve;

            AnimationCurve returnCurve = new AnimationCurve();

            for (int i = 0; i < ThisCurve.length; i++)
            {
                Keyframe thisKeyframe = ThisCurve.keys[i];

                Keyframe newKeyframe = new Keyframe(thisKeyframe.time * ScaleValue,
                    thisKeyframe.value, thisKeyframe.inTangent, thisKeyframe.outTangent);

                returnCurve.AddKey(newKeyframe);
            }

            return returnCurve;
        }
    }
}
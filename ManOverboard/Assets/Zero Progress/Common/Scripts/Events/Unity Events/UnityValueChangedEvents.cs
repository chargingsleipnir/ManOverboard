using System;
using UnityEngine.Events;

namespace ZeroProgress.Common
{
    [Serializable]
    public class FloatValueChangedEvent : UnityEvent<FloatChangedEventArgs>
    {        
    }

    [Serializable]
    public class StringValueChangedEvent : UnityEvent<StringChangedEventArgs>
    {
    }

    [Serializable]
    public class IntValueChangedEvent : UnityEvent<IntChangedEventArgs>
    {
    }
}
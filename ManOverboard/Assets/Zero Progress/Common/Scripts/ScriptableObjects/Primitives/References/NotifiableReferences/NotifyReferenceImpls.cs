using System;

namespace ZeroProgress.Common
{
    [Serializable]
    public class NotifyFloatReference : NotifyReference<float, ScriptableFloat, FloatValueChangedEvent, FloatChangedEventArgs>
    {
    }

    [Serializable]
    public class NotifyStringReference : NotifyReference<string, ScriptableString, StringValueChangedEvent, StringChangedEventArgs>
    {
    }

    [Serializable]
    public class NotifyIntReference : NotifyReference<int, ScriptableInt, IntValueChangedEvent, IntChangedEventArgs>
    {
    }
}

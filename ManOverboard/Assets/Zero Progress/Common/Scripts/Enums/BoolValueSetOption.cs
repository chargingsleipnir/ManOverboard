namespace ZeroProgress.Common
{
    public enum BoolValueSetOption
    {
        NOT_SET,
        TRUE,
        FALSE,
        UNCHANGED,
        INVERSE
    }

    public static class BoolValueSetOptionExtensions
    {
        public static bool GetBoolValue(this BoolValueSetOption SetOption, bool CurrentValue)
        {
            switch (SetOption)
            {
                case BoolValueSetOption.TRUE:
                    return true;
                case BoolValueSetOption.FALSE:
                    return false;
                case BoolValueSetOption.UNCHANGED:
                    return CurrentValue;
                case BoolValueSetOption.INVERSE:
                    return !CurrentValue;
                case BoolValueSetOption.NOT_SET:
                default:
                    throw new System.ArgumentException("Set Option out of bounds, cannot get bool value");
            }
        }
    }
}

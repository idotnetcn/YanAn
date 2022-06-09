
namespace DotNet.Tools.Extensions
{
    public static class ObjectExtensions
    {
        public static bool IsNullOrEmpty(this object? value)
        {
            if (value == null)
                return true;
            if (value is string)
            {
                return string.IsNullOrWhiteSpace(value.ToString());
            }
            return false;
        }
        public static bool IsNotNullOrEmpty(this object? value)
        {
            return !value.IsNullOrEmpty();
        }

        #region ConvertTo

        public static string? ToStr(this object? value)
        {
            if (value == null)
                return null;
            return (string)value;
        }


        public static int ToInt(this object value, int defaultValue = 0)
        {
            if (value == null) return defaultValue;
            if (!int.TryParse(value.ToString(), out var result))
                return defaultValue;
            return result;
        }

        public static long ToLong(this object value, long defaultValue = 0)
        {
            if (value == null) return defaultValue;
            if (!long.TryParse(value.ToString(), out var result))
                return defaultValue;
            return result;
        }

        public static decimal ToDecimal(this object value, decimal defaultValue = 0)
        {
            if (value == null) return defaultValue;
            if (!decimal.TryParse(value.ToString(), out var result))
                return defaultValue;
            return result;
        }

        #endregion ConvertTo
    }
}

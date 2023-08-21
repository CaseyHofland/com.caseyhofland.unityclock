using System;

namespace UnityClock
{
    internal static class UITimeFieldsUtils
    {
        public static readonly string k_AllowedCharactersForTime = InternalEngineBridge.k_AllowedCharactersForInt + ".:";

        public static readonly string k_TimeFieldFormatString = "g";

        //private static bool TryConvertTimeSpanStringToLongString(string str, out string longString)
        //{
        //    longString = default;
        //    var timeSpanChars = new char[12] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.', ':' };

        //    int i;
        //    int start;
        //    int end;
        //    for (i = start = end = str.Length - 1; i >= 0; i--)
        //    {
        //        if (Array.IndexOf(timeSpanChars, str[i]) == -1)
        //        {
        //            start = i;

        //            var timeSpanString = str.Remove(start, end - start);
        //            if (!TimeSpan.TryParse(timeSpanString, out var timeSpan))
        //            {
        //                return false;
        //            }
        //            str = str.Insert(start, timeSpan.Ticks.ToString());
        //        }
        //        else if (end < start)
        //        {
        //            end = i;
        //        }
        //    }
        //    longString = str;

        //    return true;
        //}

        //public static bool TryConvertStringToTimeSpan(string str, out TimeSpan value)
        //{
        //    if (TryConvertTimeSpanStringToLongString(str, out var longString)
        //        && InternalEngineBridge.TryConvertStringToLong(longString, out var longValue))
        //    {
        //        value = new(longValue);
        //        return true;
        //    }
        //    else
        //    {
        //        value = default;
        //        return false;
        //    }
        //}

        public static bool TryConvertStringToTimeSpan(string str, out TimeSpan value) => TimeSpan.TryParse(str, out value);
        public static bool TryConvertStringToTimeSpan(string str, string initialValueAsString, out TimeSpan value) => TryConvertStringToTimeSpan(str, out value) || TryConvertStringToTimeSpan(initialValueAsString, out value);

        public static bool TryConvertStringToTimeOnly(string str, out TimeOnly value)
        {
            str = !str.Contains(':') ? $"0.{str}:00" : $"0.{str}";
            var parsed = TimeSpan.TryParse(str, out var result);
            value = parsed ? TimeOnly.FromTimeSpan(result) : default;
            return parsed;
        }
        public static bool TryConvertStringToTimeOnly(string str, string initialValueAsString, out TimeOnly value) => TryConvertStringToTimeOnly(str, out value) || TryConvertStringToTimeOnly(initialValueAsString, out value);
    }
}

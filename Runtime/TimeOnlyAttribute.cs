using UnityEngine;

namespace UnityClock
{
    public class TimeOnlyAttribute : PropertyAttribute
    {
        public bool showHour = true;
        public bool showMinute = true;
        public bool showSecond = false;
        public bool showMillisecond = false;
        public bool showInterpolant = true;
        public string timeFormat = "g";

        public TimeOnlyAttribute() { }
        public TimeOnlyAttribute(bool enableAll) : this(enableAll, enableAll, enableAll, enableAll, enableAll) { }
        public TimeOnlyAttribute(string timeFormat) 
        {
            this.timeFormat = timeFormat;
        }
        public TimeOnlyAttribute(bool enableAll, string timeFormat) : this(enableAll, enableAll, enableAll, enableAll, enableAll, timeFormat) { }
        public TimeOnlyAttribute(bool showHour = true, bool showMinute = true, bool showSecond = false, bool showMillisecond = false, bool showInterpolant = true, string timeFormat = "g")
        {
            this.showHour = showHour;
            this.showMinute = showMinute;
            this.showSecond = showSecond;
            this.showMillisecond = showMillisecond;
            this.showInterpolant = showInterpolant;
            this.timeFormat = timeFormat;
        }
    }
}

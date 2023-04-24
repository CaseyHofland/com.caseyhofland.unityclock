using UnityEngine;

namespace UnityClock
{
    public class TimeSpanAttribute : PropertyAttribute
    {
        public bool showMinus = false;
        public bool showDays = false;
        public bool showHours = false;
        public bool showMinutes = true;
        public bool showSeconds = true;
        public bool showMilliseconds = false;
        public string timeFormat = "g";

        public TimeSpanAttribute() { }
        public TimeSpanAttribute(bool enableAll) : this(enableAll, enableAll, enableAll, enableAll, enableAll, enableAll) { }
        public TimeSpanAttribute(string timeFormat)
        {
            this.timeFormat = timeFormat;
        }
        public TimeSpanAttribute(bool enableAll, string timeFormat) : this(enableAll, enableAll, enableAll, enableAll, enableAll, enableAll, timeFormat) { }
        public TimeSpanAttribute(bool showMinus = false, bool showDays = false, bool showHours = false, bool showMinutes = true, bool showSeconds = true, bool showMilliseconds = false, string timeFormat = "g")
        {
            this.showMinus = showMinus;
            this.showDays = showDays;
            this.showHours = showHours;
            this.showMinutes = showMinutes;
            this.showSeconds = showSeconds;
            this.showMilliseconds = showMilliseconds;
            this.timeFormat = timeFormat;
        }
    }
}

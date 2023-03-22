using UnityEngine;

namespace UnityClock
{
    public class TimeSpanAttribute : PropertyAttribute
    {
        public bool showDays = false;
        public bool showHours = false;
        public bool showMinutes = true;
        public bool showSeconds = false;
        public bool showMilliseconds = false;

        public TimeSpanAttribute() { }
        public TimeSpanAttribute(bool showDays = false, bool showHours = false, bool showMinutes = true, bool showSeconds = true, bool showMilliseconds = false)
        {
            this.showDays = showDays;
            this.showHours = showHours;
            this.showMinutes = showMinutes;
            this.showSeconds = showSeconds;
            this.showMilliseconds = showMilliseconds;
        }
    }
}

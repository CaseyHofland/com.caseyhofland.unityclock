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

        public TimeOnlyAttribute() { }
        public TimeOnlyAttribute(bool showHour = true, bool showMinute = true, bool showSecond = false, bool showMillisecond = false, bool showInterpolant = true)
        {
            this.showHour = showHour;
            this.showMinute = showMinute;
            this.showSecond = showSecond;
            this.showMillisecond = showMillisecond;
            this.showInterpolant = showInterpolant;
        }
    }
}

using System;
using UnityEngine;

namespace UnityClock
{
    [Serializable]
    public struct SerializableTimeSpan
    {
        [SerializeField] private int _days;
        [SerializeField] [Range(-23, 23)] private int _hours;
        [SerializeField] [Range(-59, 59)] private int _minutes;
        [SerializeField] [Range(-59, 59)] private int _seconds;
        [SerializeField] [Range(-999, 999)] private int _milliseconds;

        public SerializableTimeSpan(TimeSpan timeSpan)
        {
            this._days = timeSpan.Days;
            this._hours = timeSpan.Hours;
            this._minutes = timeSpan.Minutes;
            this._seconds = timeSpan.Seconds;
            this._milliseconds = timeSpan.Milliseconds;
        }

        public override string ToString() => ((TimeSpan)this).ToString();
        public string ToString(string format) => ((TimeSpan)this).ToString(format);
        public string ToString(string format, IFormatProvider formatProvider) => ((TimeSpan)this).ToString(format, formatProvider);

        public static implicit operator TimeSpan(SerializableTimeSpan serialized) => new TimeSpan(serialized._days, serialized._hours, serialized._minutes, serialized._seconds, serialized._milliseconds);
        public static implicit operator SerializableTimeSpan(TimeSpan timeSpan) => new SerializableTimeSpan(timeSpan);
    }
}

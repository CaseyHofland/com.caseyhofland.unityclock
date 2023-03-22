using System;
using UnityEngine;

namespace UnityClock
{
    [Serializable]
    public struct SerializableTimeOnly
    {
        [SerializeField] [Range(0, 23)] private int _hour;
        [SerializeField] [Range(0, 59)] private int _minute;
        [SerializeField] [Range(0, 59)] private int _second;
        [SerializeField] [Range(0, 999)] private int _millisecond;

        public SerializableTimeOnly(TimeOnly timeOnly)
        {
            this._hour = timeOnly.Hour;
            this._minute = timeOnly.Minute;
            this._second = timeOnly.Second;
            this._millisecond = timeOnly.Millisecond;
        }

        public override string ToString() => ((TimeOnly)this).ToString();
        public string ToString(string format) => ((TimeOnly)this).ToString(format);
        public string ToString(string format, IFormatProvider formatProvider) => ((TimeOnly)this).ToString(format, formatProvider);

        public static implicit operator TimeOnly(SerializableTimeOnly serializableTimeOnly) => new(serializableTimeOnly._hour, serializableTimeOnly._minute, serializableTimeOnly._second, serializableTimeOnly._millisecond);
        public static implicit operator SerializableTimeOnly(TimeOnly timeOnly) => new(timeOnly);
    }
}
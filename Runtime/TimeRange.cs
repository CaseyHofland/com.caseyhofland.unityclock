using System;
using UnityEngine;

namespace UnityClock
{
    [Serializable]
    public struct TimeRange : ISerializationCallbackReceiver
    {
        [SerializeField, TimeOnly(true, true, true, true, true)] private long _start;
        [SerializeField, TimeOnly(true, true, true, true, true)] private long _end;

        public TimeOnly start;
        public TimeOnly end;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _start = start.Ticks;
            _end = end.Ticks;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            start = new(_start);
            end = new(_end);
        }

        public TimeRange(TimeOnly start, TimeOnly end)
        {
            _start = (this.start = start).Ticks;
            _end = (this.end = end).Ticks;
        }

        public float Interpolant() => Clock.InverseLerp(start, end);
        public float Interpolant(TimeOnly time) => Clock.InverseLerp(start, end, time);

        public float InverseInterpolant() => Clock.InverseLerp(end, start);
        public float InverseInterpolant(TimeOnly time) => Clock.InverseLerp(end, start, time);

        public bool IsBetween() => Clock.time.IsBetween(start, end);
        public bool IsBetween(TimeOnly time) => time.IsBetween(start, end);
    }
}
using System;
using UnityEngine;

namespace UnityClock
{
    [Serializable]
    public struct TimeRange : ISerializationCallbackReceiver
    {
        [SerializeField] private SerializableTimeOnly _start;
        [SerializeField] private SerializableTimeOnly _end;

        public TimeOnly start;
        public TimeOnly end;

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _start = start;
            _end = end;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            start = _start;
            end = _end;
        }

        public TimeRange(TimeOnly from, TimeOnly till)
        {
            this.start = this._start = from;
            this.end = this._end = till;
        }

        public float Interpolant() => Clock.InverseLerp(start, end);
        public float Interpolant(TimeOnly time) => Clock.InverseLerp(start, end, time);

        public float InverseInterpolant() => Clock.InverseLerp(end, start);
        public float InverseInterpolant(TimeOnly time) => Clock.InverseLerp(end, start, time);

        public bool IsBetween() => Clock.time.IsBetween(start, end);
        public bool IsBetween(TimeOnly time) => time.IsBetween(start, end);
    }
}
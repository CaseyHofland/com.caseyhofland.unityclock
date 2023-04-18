#nullable enable
using System;
using UnityEngine;

namespace UnityClock
{
    public interface ITemporal
    {
        void Evaluate(TimeOnly time);
    }

    [Obsolete] public interface ITemporalOld<T>
    {
        T lastValue { get; set; }

        T Evaluate(TimeOnly time);
        void ForceChange(T value);

        public void TryChange(TimeOnly time)
        {
            var value = Evaluate(time);
            if (lastValue?.Equals(value) ?? false)
            {
                return;
            }

            ForceChange(lastValue = value);
        }
    }

    [Obsolete] public interface ITemporal<out T> : ITemporal
    {
        void ITemporal.Evaluate(TimeOnly time) => Evaluate(time);
        new T Evaluate(TimeOnly time);
    }
}

#nullable enable

using System;

namespace UnityClock
{
    public interface ITemporal<T>
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

    public interface ITemporal2
    {
        void Evaluate(TimeOnly time);
    }

    public interface ITemporal2<out T> : ITemporal2
    {
        void ITemporal2.Evaluate(TimeOnly time) => Evaluate(time);
        new T Evaluate(TimeOnly time);
    }
}

#nullable enable
using System;

namespace UnityClock
{
    public interface ITemporal
    {
        void Evaluate(TimeOnly time);
    }
}

#nullable enable
using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityClock
{
    [ExecuteAlways]
    [Obsolete]
    public class TemporalActivity : MonoBehaviour, ITemporalOld<bool>
    {
        [field: SerializeField] public TimeRange activeHours { get; set; }
        [field: SerializeField] public UnityEvent<bool> activityChanged { get; set; } = new();

        [field: SerializeField, HideInInspector] bool ITemporalOld<bool>.lastValue { get; set; }

        private void OnEnable()
        {
            Clock.timeChanged += ((ITemporalOld<bool>)this).TryChange;
            ForceChange(Evaluate(Clock.time));
        }

        private void OnDisable()
        {
            Clock.timeChanged -= ((ITemporalOld<bool>)this).TryChange;
        }

        public bool Evaluate(TimeOnly time) => activeHours.IsBetween(time);
        public void ForceChange(bool value) => activityChanged.Invoke(value);
    }
}

#nullable enable
using System;
using UnityEngine;
using UnityEngine.Events;

namespace UnityClock
{
    [ExecuteAlways]
    public class TemporalActivity : MonoBehaviour, ITemporal<bool>
    {
        [field: SerializeField] public TimeRange activeHours { get; set; }
        [field: SerializeField] public UnityEvent<bool> activityChanged { get; set; } = new();

        [field: SerializeField, HideInInspector] bool ITemporal<bool>.lastValue { get; set; }

        private void OnEnable()
        {
            Clock.timeChanged += ((ITemporal<bool>)this).TryChange;
            ForceChange(Evaluate(Clock.time));
        }

        private void OnDisable()
        {
            Clock.timeChanged -= ((ITemporal<bool>)this).TryChange;
        }

        public bool Evaluate(TimeOnly time) => activeHours.IsBetween(time);
        public void ForceChange(bool value) => activityChanged.Invoke(value);
    }
}

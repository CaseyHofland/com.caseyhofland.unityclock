#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClock
{
    public abstract class Instance<T> : MonoBehaviour where T : Instance<T>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SubsystemRegistration()
        {
            instances.Clear();
        }

        private static List<T> instances = new();

        /// <summary>
        /// Return the current instance.
        /// </summary>
        public static T? current => instances.Count > 0 ? instances[0] : null;

        protected virtual void OnEnable()
        {
            instances.Insert(0, (T)this);
            SetState();
        }

        private void OnDisable()
        {
            var wasCurrent = current == this;
            instances.Remove((T)this);
            if (wasCurrent && current != null)
            {
                current.SetState();
            }
        }

        /// <summary>
        /// Sets the instances' state.
        /// </summary>
        public abstract void SetState();
    }

    [AddComponentMenu(nameof(UnityClock) + "/" + nameof(ClockInstance))]
    [DisallowMultipleComponent]
    public class ClockInstance : Instance<ClockInstance>, ISerializationCallbackReceiver
    {
        [SerializeField, TimeOnly] private long _time;
        [SerializeField, TimeSpan] private long _daySpan;

        public TimeOnly time { get; set; }
        public TimeSpan daySpan { get; set; }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            _time = time.Ticks;
            _daySpan = daySpan.Ticks;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            time = new(_time);
            daySpan = new(_daySpan);
        }

        [ContextMenu(nameof(SetState))]
        public override void SetState()
        {
            Clock.time = time;
            Clock.daySpan = daySpan;
        }
    }
}

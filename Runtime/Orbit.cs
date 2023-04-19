using System;
using UnityEngine;

namespace UnityClock
{
    [ExecuteAlways]
    [AddComponentMenu(nameof(Clock) + "/" + nameof(Orbit))]
    [Obsolete]
    public class Orbit : MonoBehaviour
    {
        [field: SerializeField]
        public TimeRange timeRange { get; set; } = new TimeRange(TimeOnly.FromTimeSpan(TimeSpan.FromHours(6)), TimeOnly.FromTimeSpan(TimeSpan.FromHours(18) - TimeSpan.FromTicks(1)));

        [field: SerializeField]
        [field: Range(0f, 360f)]
        private float yaw { get; set; }

        [field: SerializeField]
        [field: Range(0f, 360f)]
        private float roll { get; set; }

        private void Update()
        {
            if (timeRange.IsBetween())
            {
                var t = timeRange.Interpolant();

                transform.eulerAngles = new Vector3(0f, yaw, roll);
                transform.RotateAround(transform.position, transform.right, t * 180f);
            }
            else
            {
                var t = timeRange.InverseInterpolant();

                transform.eulerAngles = new Vector3(0f, yaw, roll);
                transform.RotateAround(transform.position, transform.right, 180f + t * 180f);
            }
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            var wirePos = transform.position;

            UnityEditor.Handles.color = Color.blue;
            UnityEditor.Handles.DrawWireDisc(wirePos, transform.right, 1f);

            var spherePos = wirePos + Quaternion.Euler(transform.right) * -transform.forward;

            UnityEditor.Handles.color = Color.green;
            UnityEditor.Handles.SphereHandleCap(default, spherePos, Quaternion.identity, 0.25f, EventType.Repaint);
#endif
        }
    }
}
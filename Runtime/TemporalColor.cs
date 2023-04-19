#nullable enable
using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;

namespace UnityClock
{
    [ExecuteAlways]
    [Obsolete]
    public class TemporalColor : MonoBehaviour, ITemporalOld<Color>
    {
//#if UNITY_EDITOR
//        private static readonly FieldInfo _focusChangedField = typeof(UnityEditor.EditorApplication).GetField("focusChanged", BindingFlags.Static | BindingFlags.NonPublic);

//        private static Action<bool>? unityEditorFocusChanged
//        {
//            get => (Action<bool>)_focusChangedField.GetValue(null);
//            set => _focusChangedField.SetValue(null, value);
//        }

//        [UnityEditor.InitializeOnLoadMethod]
//        private static void ResetColor()
//        {
//            unityEditorFocusChanged -= Stuff;
//            unityEditorFocusChanged += Stuff;
//        }

//        private static void Stuff(bool focus)
//        {
//            if (focus)
//            {
//                foreach (ITemporal<Color> temporalColor in FindObjectsOfType<TemporalColor>())
//                {
//                    temporalColor.TryChange(Clock.time);
//                }
//            }
//            else
//            {
//                foreach (var temporalColor in FindObjectsOfType<TemporalColor>())
//                {
//                    temporalColor.colorChanged.Invoke(default);
//                    temporalColor.materialChanged.Invoke(Shader.PropertyToID(temporalColor.colorName), default);
//                }
//            }
//        }
//#endif

        [field: SerializeField, GradientUsage(true)] public Gradient gradient { get; set; } = new();
        [field: SerializeField] public bool pingPong { get; set; } = true;
        [field: SerializeField] public UnityEvent<Color> colorChanged { get; set; } = new();
        [field: SerializeField] public string colorName { get; set; } = string.Empty;
        [field: SerializeField] public UnityEvent<int, Color> materialChanged { get; set; } = new();

        [field: SerializeField, HideInInspector] Color ITemporalOld<Color>.lastValue { get; set; }

        private void OnEnable()
        {
            Clock.timeChanged += ((ITemporalOld<Color>)this).TryChange;
        }

        private void OnDisable()
        {
            Clock.timeChanged -= ((ITemporalOld<Color>)this).TryChange;
        }

        public Color Evaluate(TimeOnly time)
        {
            return !pingPong
                ? gradient.Evaluate(time.Ticks * Clock.dayMultiplier)
                : gradient.Evaluate(Mathf.PingPong(time.Ticks * Clock.pingPongMultiplier, 1f));
        }

        public void ForceChange(Color color)
        {
            colorChanged.Invoke(color);
            materialChanged.Invoke(Shader.PropertyToID(colorName), color);
        }
    }
}

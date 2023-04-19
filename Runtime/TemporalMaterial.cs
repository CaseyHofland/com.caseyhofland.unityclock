#nullable enable
using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityClock
{
    [CreateAssetMenu(menuName = "Temporal Material", order = 302)]
    [Obsolete]
    public class TemporalMaterial : ScriptableObject, ITemporal
    {
        [field: SerializeField] public Material? targetMaterial { get; set; }

        [Serializable]
        public record PropertyGradient
        {
            public ShaderId id;
            public ClockGradient gradient = new();
        }

        [Serializable]
        public record PropertyCurve
        {
            public ShaderId id;
            public AnimationCurve curve = AnimationCurve.Constant(0f, 1f, 1f);
        }

        [Serializable]
        public record KeywordTimeRange
        {
            public string keyword;
            public TimeRange timeRange = new(new(6, 0), new(18, 0));
        }

        [field: SerializeField] public List<PropertyGradient> propertyGradients { get; set; } = new();
        [field: SerializeField] public List<PropertyCurve> propertyCurves { get; set; } = new();
        [field: SerializeField] public List<KeywordTimeRange> keywordTimeRanges { get; set; } = new();

        private void Reset()
        {
            hideFlags |= HideFlags.DontUnloadUnusedAsset;
        }

        private void OnEnable()
        {
            Clock.timeChanged += Evaluate;
            Evaluate(Clock.time);
        }

        private void OnDisable()
        {
            Clock.timeChanged -= Evaluate;
        }

        public void Evaluate(TimeOnly time)
        {
            if (targetMaterial == null)
            {
                return;
            }

            foreach (var propertyGradient in propertyGradients)
            {
                var color = propertyGradient.gradient.Evaluate(time);
                targetMaterial.SetColor(propertyGradient.id, color);
            }

            foreach (var propertyCurve in propertyCurves)
            {
                var value = propertyCurve.curve.Evaluate(Mathf.PingPong(time.Ticks * Clock.pingPongMultiplier, 1f));
                targetMaterial.SetFloat(propertyCurve.id, value);
            }

            foreach (var keywordTimeRange in keywordTimeRanges)
            {
                var isBetween = keywordTimeRange.timeRange.IsBetween();
                if (isBetween)
                {
                    targetMaterial.EnableKeyword(keywordTimeRange.keyword);
                }
                else
                {
                    targetMaterial.DisableKeyword(keywordTimeRange.keyword);
                }
            }
        }

        /*
        [SerializeField, ShaderSelectionDropdown] private Shader shader;

        public Material? sharedMaterial { get; set; }

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (shader != (sharedMaterial == null ? null : sharedMaterial.shader))
            {
                DestroyImmediate(sharedMaterial, true);

                if (shader != null)
                {
                    sharedMaterial = new Material(shader);
                    Evaluate(Clock.time);
                    UnityEditor.AssetDatabase.AddObjectToAsset(sharedMaterial, this);
                }
                //UnityEditor.AssetDatabase.SaveAssetIfDirty(this); // TODO: Add Auto Save when changing shader or name
            }

            if (sharedMaterial != null)
            {
                sharedMaterial.name = name;
            }
        }
#endif
        */
    }
}

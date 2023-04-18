#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UnityClock
{
    [Obsolete("Use Temporal Material Instead until a better workflow has been created for this.", false)]
    [CreateAssetMenu(menuName = "Temporal Material Lerper", order = 302)]
    public class TemporalMaterialLerper : ScriptableObject, ISerializationCallbackReceiver
    {
        [SerializeField] private Shader shader;
        [SerializeField, TimeOnly(true, true, true, true, true)] private List<long> _keys = new();
        [SerializeField] private List<Material> _values = new();

        public SortedList<TimeOnly, Material> materialsByTime = new();

        public Material? sharedMaterial { get; set; }

        public void Evaluate(TimeOnly time)
        {
            if (sharedMaterial == null
                || materialsByTime.Count == 0)
            {
                return;
            }

            int min = materialsByTime.Count - 1, max = 0;
            TimeOnly check = materialsByTime.Keys[max];
            if (time > check)
            {
                check = materialsByTime.Keys[min];
                for (; time <= check && min >= 0;)
                {
                    max = min;
                    min--;
                    check = materialsByTime.Keys[min];
                }
            }

            var start = materialsByTime.Values[min];
            var end = materialsByTime.Values[max];
            var t = Clock.InverseLerp(materialsByTime.Keys[min], materialsByTime.Keys[max], time);

            Debug.Log(t);

            sharedMaterial.Lerp(start, end, t);
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

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (shader != (sharedMaterial == null ? null : sharedMaterial.shader))
            {
                DestroyImmediate(sharedMaterial, true);

                if (shader != null)
                {
                    sharedMaterial = new Material(shader);
                    //sharedMaterial.hideFlags = HideFlags.NotEditable;
                    Evaluate(Clock.time);
                    UnityEditor.AssetDatabase.AddObjectToAsset(sharedMaterial, this);
                }
            }

            if (sharedMaterial != null)
            {
                sharedMaterial.name = name;
            }
        }
#endif

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            //_keys = materialsByTime.Keys.ToList().ConvertAll(time => time.Ticks);
            //_values = materialsByTime.Values.ToList();
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            materialsByTime = new();

            for (int i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            {
                materialsByTime.Add(new(_keys[i]), _values[i]);
            }
        }
    }
}

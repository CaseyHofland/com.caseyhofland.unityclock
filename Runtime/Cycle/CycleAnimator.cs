#nullable enable
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

namespace UnityClock
{
    [AddComponentMenu(nameof(UnityClock) + "/" + nameof(CycleAnimator))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Animator))]
    public class CycleAnimator : MonoBehaviour, IAnimationClipSource, IList<AnimationClipCycle>, ICycleSource
    {
        [field: SerializeField, HideInInspector] public Animator animator { get; private set; }

        [SerializeField] private List<AnimationClipCycle> cycles = new();
        private AnimationPlayableOutput output;
        private AnimationMixerPlayable mixer;
        private List<Playable> playables = new();

        private void InitializeComponents()
        {
            animator = GetComponent<Animator>();
        }

        #region IAnimationClipSource Methods
        void IAnimationClipSource.GetAnimationClips(List<AnimationClip> results)
        {
            results.AddRange(cycles.ConvertAll(cycle => cycle.clip));
        }
        #endregion

        #region IList Methods
        public AnimationClipCycle this[int index]
        {
            get => cycles[index];
            set
            {
                cycles[index] = value;

                if (mixer.IsValid())
                {
                    playables[index].DestroyCycle();
                    var playable = playables[index] = AnimationClipCycle.Create(mixer.GetGraph(), value);
                    mixer.ConnectInput(index, playable, 0, 1f);
                }
            }
        }

        public void Add(AnimationClipCycle item)
        {
            cycles.Add(item);

            if (mixer.IsValid())
            {
                var playable = AnimationClipCycle.Create(mixer.GetGraph(), item);
                playables.Add(playable);
                mixer.AddInput(playable, 0, 1f);
            }
        }

        public bool Remove(AnimationClipCycle item)
        {
            var index = IndexOf(item);
            if (index == -1)
            {
                return false;
            }

            RemoveAt(index);
            return true;
        }

        public void Insert(int index, AnimationClipCycle item)
        {
            cycles.Insert(index, item);

            if (mixer.IsValid())
            {
                var playable = AnimationClipCycle.Create(mixer.GetGraph(), item);
                playables.Insert(index, playable);

                // Move mixer ports
                for (; index < playables.Count - 1; index++)
                {
                    mixer.ConnectInput(index, playables[index], 0, 1f);
                }
                mixer.AddInput(playables[index], 0, 1f);
            }
        }

        public void RemoveAt(int index)
        {
            cycles.RemoveAt(index);

            if (mixer.IsValid())
            {
                playables[index].DestroyCycle();
                playables.RemoveAt(index);

                // Move mixer ports
                for (; index < playables.Count; index++)
                {
                    mixer.ConnectInput(index, playables[index], 0, 1f);
                }
                mixer.SetInputCount(index);
            }
        }

        public void Clear()
        {
            cycles.Clear();

            if (mixer.IsValid())
            {
                playables.ForEach(playable => playable.DestroyCycle());
                playables.Clear();
                mixer.SetInputCount(0);
            }
        }

        public int IndexOf(AnimationClipCycle item) => cycles.IndexOf(item);
        public bool Contains(AnimationClipCycle item) => cycles.Contains(item);
        public void CopyTo(AnimationClipCycle[] array, int arrayIndex) => cycles.CopyTo(array, arrayIndex);
        public int Count => cycles.Count;
        bool ICollection<AnimationClipCycle>.IsReadOnly => ((ICollection<AnimationClipCycle>)cycles).IsReadOnly;
        public IEnumerator<AnimationClipCycle> GetEnumerator() => cycles.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => cycles.GetEnumerator();
        #endregion

        #region ICycleSource Methods
        void ICycleSource.Create(PlayableGraph graph)
        {
            mixer = AnimationMixerPlayable.Create(graph, cycles.Count);
            playables.AddRange(cycles.ConvertAll(cycle => (Playable)AnimationClipCycle.Create(graph, cycle)));
            for (int port = 0; port < playables.Count; port++)
            {
                mixer.ConnectInput(port, playables[port], 0, 1f);
            }
        }

        void ICycleSource.Destroy()
        {
            playables.ForEach(playable => playable.DestroyCycle());
            playables.Clear();
            mixer.Destroy();
        }

        void ICycleSource.OnPlay()
        {
            output = AnimationPlayableOutput.Create(mixer.GetGraph(), nameof(cycles), animator);
            output.SetSourcePlayable(mixer);
        }

        void ICycleSource.OnStop()
        {
            if (output.IsOutputValid())
            {
                mixer.GetGraph().DestroyOutput(output);
            }
        }
        #endregion

        #region Unity Methods
        private void Reset()
        {
            InitializeComponents();
        }

        private void OnValidate()
        {
            if (mixer.IsValid())
            {
                var wasActiveAndEnabled = isActiveAndEnabled;
                if (wasActiveAndEnabled)
                {
                    OnDisable();
                }

                OnDestroy();
                Awake();

                if (wasActiveAndEnabled)
                {
                    OnEnable();
                }
            }
        }

        private void Awake()
        {
            InitializeComponents();

            ((ICycleSource)this).Create(Clock.playableGraph);
        }

        private void OnDestroy()
        {
            ((ICycleSource)this).Destroy();
        }

        private void OnEnable()
        {
            ((ICycleSource)this).OnPlay();

            foreach (var playable in playables)
            {
                if (playable.IsValid() && playable.GetPlayState() == PlayState.Playing)
                {
                    playable.SyncCycle();
                }
            }
        }

        private void OnDisable()
        {
            ((ICycleSource)this).OnStop();
        }
        #endregion
    }
}
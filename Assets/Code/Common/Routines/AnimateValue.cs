using Sirenix.OdinInspector;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Echo.Common
{
    public abstract class AnimateValue<T> : RoutineBehaviour
    {
        [SerializeField, LabelText("Delta Time")]
        private DeltaTimeKind _deltaTimeKind;

        [SerializeField]
        private T _start;

        [SerializeField]
        private T _end;

        [SerializeField]
        private AnimationCurve _curve;

        [SerializeField]
        private UnityEvent<T> _apply;

        public override IEnumerator RunAsync()
        {
            var progress = 0f;
            var duration = _curve.GetDuration();

            void Update(float time)
            {
                var ratio = time / duration;
                var value = Interpolate(_start, _end, ratio);
                _apply?.Invoke(value);
            }

            while (progress < duration)
            {
                Update(progress);
                yield return null;

                progress += _deltaTimeKind.GetValue();
            }
            Update(1f);
        }

        protected abstract T Interpolate(T start, T end, float ratio);
    }
}
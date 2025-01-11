using DG.Tweening;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using UnityEngine;

namespace Echo.Common
{
    public abstract class TweenBehaviour : RoutineBehaviour
    {
        #region Nested types

        [Flags]
        public enum Features
        {
            None = 0,

            Ease = 1 << 0,
            Delay = 1 << 1
        }

        #endregion

        [SerializeField, MinValue(0f)]
        private float _duration;

        [SerializeField]
        private Features _features;

        [SerializeField, ShowIf(nameof(HasEase))]
        private Ease _ease;

        [SerializeField, MinValue(0f), ShowIf(nameof(HasDelay))]
        private float _delay;

        private bool HasEase => _features.HasFlag(Features.Ease);
        private bool HasDelay => _features.HasFlag(Features.Delay);

        public override IEnumerator RunAsync()
        {
            var tween = StartTween(_duration);

            if (HasEase)
                tween.SetEase(_ease);

            if (HasDelay)
            {
                tween.SetDelay(_delay);
                yield return new WaitForSeconds(_delay);
            }

            yield return new WaitForSeconds(_duration);
        }

        protected abstract Tween StartTween(float duration);
    }
}
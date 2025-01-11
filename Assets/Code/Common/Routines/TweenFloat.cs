using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace Echo.Common
{
    public class TweenFloat : TweenBehaviour
    {
        [SerializeField]
        private float _from;

        [SerializeField]
        private float _to;

        [SerializeField]
        private UnityEvent<float> _apply;

        private float _value;

        protected override Tween StartTween(float duration)
        {
            _value = _from;
            var tween = DOTween.To(
                getter: () => _value,
                setter: (value) => _value = value,
                endValue: _to,
                duration: duration);
            
            tween.OnUpdate(() => _apply?.Invoke(_value)); 

            return tween;
        }
    }
}
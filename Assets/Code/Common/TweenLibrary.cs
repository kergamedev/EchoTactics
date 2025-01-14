using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UIElements;

namespace Echo.Common
{
    [CreateAssetMenu(fileName = "TweenLibrary", menuName = "Echo Tactics/Utilities/Tween Library")]
    public class TweenLibrary : ScriptableObject
    {
        [SerializeField, LabelText("Duration"), MinValue(0.025f), FoldoutGroup("Button Click")]
        private float _buttonClickTweenDuration = 0.1f;

        [SerializeField, LabelText("Scale Punch Intensity"), MinValue(0.05f), FoldoutGroup("Button Click")]
        private float _buttonClickScalePunchIntensity = 0.15f;

        [SerializeField, LabelText("Rotate Punch Intensity"), MinValue(1.5f), FoldoutGroup("Button Click")]
        private float _buttonClickRotatePunchIntensity = 10f;

        [SerializeField, LabelText("Duration"), MinValue(0.025f), FoldoutGroup("Appear")]
        private float _appearDuration = 0.1f;

        [SerializeField, LabelText("Easing"), FoldoutGroup("Appear")]
        private Ease _appearEasing = Ease.OutBounce;

        [SerializeField, LabelText("Bounce Duration"), MinValue(0.025f), FoldoutGroup("Appear")]
        private float _appearBounceDuration = 0.1f;

        [SerializeField, LabelText("Bounce Intensity"), MinValue(0.05f), FoldoutGroup("Appear")]
        private float _appearBounceIntensity = 0.15f;

        public Sequence DoButtonClick(VisualElement button)
        {
            var scale = DOTween.Punch(
                () => button.style.scale.value.value,
                (scale) => button.style.scale = new StyleScale(scale),
                Vector3.one * _buttonClickScalePunchIntensity,
                _buttonClickTweenDuration)
                .OnComplete(() => button.style.scale = new StyleScale(StyleKeyword.Null));

            var rotate = DOTween.Punch(
                () => Vector3.one * button.style.rotate.value.angle.value,
                (rotate) => button.style.rotate = new Rotate(new Angle(rotate.x)),
                Vector3.one * _buttonClickRotatePunchIntensity,
                _buttonClickTweenDuration);

            return DOTween.Sequence()
                .Append(scale)
                .Join(rotate);
        }

        public Tween DoAppear(VisualElement element)
        {
            element.style.scale = new StyleScale(Vector3.zero);

            var scaleUp = DOTween.To(
                () => element.style.scale.value.value,
                (scale) => element.style.scale = new StyleScale(scale),
                Vector3.one,
                _appearDuration)
                .SetEase(_appearEasing);

            var bounce = DOTween.Punch(
                () => element.style.scale.value.value,
                (scale) => element.style.scale = new StyleScale(scale),
                Vector3.one * _appearBounceIntensity,
                _appearBounceDuration)
                .OnComplete(() => element.style.scale = new StyleScale(StyleKeyword.Null));
          
            return DOTween.Sequence()
                .Append(scaleUp)
                .Append(bounce);
        }
    }
}
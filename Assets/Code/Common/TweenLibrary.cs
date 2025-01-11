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

        public void DoButtonClick(Button button)
        {
            DOTween.Sequence()
                .Append(DOTween.Punch(
                    () => button.style.scale.value.value,
                    (scale) => button.style.scale = new StyleScale(scale),
                    Vector3.one * _buttonClickScalePunchIntensity,
                    _buttonClickTweenDuration))
                .Join(DOTween.Punch(
                    () => Vector3.one * button.style.rotate.value.angle.value,
                    (rotate) => button.style.rotate = new Rotate(new Angle(rotate.x)),
                    Vector3.one * _buttonClickRotatePunchIntensity,
                    _buttonClickTweenDuration));
        }
    }
}
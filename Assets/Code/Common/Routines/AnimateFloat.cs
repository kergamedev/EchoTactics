using UnityEngine;

namespace Echo.Common
{
    public class AnimateFloat : AnimateValue<float> 
    {
        protected override float Interpolate(float start, float end, float ratio)
        {
            return Mathf.Lerp(start, end, ratio);
        }
    }
}
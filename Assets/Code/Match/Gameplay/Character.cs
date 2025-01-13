using UnityEngine;

namespace Echo.Match
{
    public class Character : MonoBehaviour
    {
        [SerializeField]
        private Renderer _renderer;

        public void Initialize(Match.CharacterInfo info)
        {
            _renderer.material = info.Material;

            transform.SetParent(info.Spot);
            transform.localPosition = Vector3.zero;
            transform.localRotation = Quaternion.identity;
        }
    }
}
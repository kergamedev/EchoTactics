using System.Collections;
using UnityEngine;
using UnityEngine.Events;

namespace Echo.Common
{
    public class DoInstantly : RoutineBehaviour
    {
        [SerializeField]
        private UnityEvent _execute;

        public override IEnumerator RunAsync()
        {
            _execute?.Invoke();
            yield break;
        }
    }
}
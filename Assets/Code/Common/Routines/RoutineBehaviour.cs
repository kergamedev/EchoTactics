using System.Collections;
using UnityEngine;

namespace Echo.Common
{
    public abstract class RoutineBehaviour : MonoBehaviour
    {
        public bool IsRunning => _handle != null;

        private Coroutine _handle;

        public void Run()
        {
            _handle = StartCoroutine(WRAPPER_RunAsync());
        }
        private IEnumerator WRAPPER_RunAsync()
        {       
            yield return RunAsync();
            _handle = null;
        }

        public abstract IEnumerator RunAsync();
    }
}
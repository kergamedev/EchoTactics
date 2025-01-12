using System;
using System.Collections;
using UnityEngine;

namespace Echo.Common
{
    public class LoadingTransition : MonoBehaviour
    {
        #region Nested types

        public enum State
        {
            None,
            Begin,
            Loading,
            End
        }

        #endregion

        [SerializeField]
        private RoutineBehaviour _begin;

        [SerializeField]
        private RoutineBehaviour _end;

        private Coroutine _currentHandle;

        public State CurrentState { get; private set; }

        private void OnEnable()
        {
            CurrentState = State.None;
        }

        public void Begin()
        {
            _currentHandle = StartCoroutine(BeginAsync());
        }
        public IEnumerator BeginAsync()
        {
            if (CurrentState != State.None)
                throw new Exception($"'{nameof(State)}={State.Begin}' can only be executed when the current state is '{State.None}'");

            CurrentState = State.Begin;
            yield return _begin.RunAsync();

            CurrentState = State.Loading;
            _currentHandle = null;
        }

        public void End()
        {
            _currentHandle = StartCoroutine(EndAsync());
        }
        public IEnumerator EndAsync()
        {
            if (CurrentState != State.Loading)
                throw new Exception($"'{nameof(State)}={State.End}' can only be executed when the current state is '{State.Loading}'");

            CurrentState = State.End;
            yield return _end.RunAsync();

            CurrentState = State.None;
            _currentHandle = null;
        }

        private void OnDisable()
        {
            if (_currentHandle != null)
                StopCoroutine(_currentHandle);

            _currentHandle = null;
        }
    }
}
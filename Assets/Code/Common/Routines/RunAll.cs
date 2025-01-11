using DG.Tweening.Core;
using System;
using System.Collections;
using System.Linq;
using UnityEngine;

namespace Echo.Common
{
    public class RunAll : RoutineBehaviour
    {
        #region Nested types

        [Serializable]
        private class SimultaneousGroup
        {
            [SerializeField]
            private RoutineBehaviour[] _routines;

            public IEnumerator RunAsync()
            {
                foreach (var routine in _routines)
                    routine.Run();

                yield return new WaitUntil(() => !_routines.Any(routine => routine.IsRunning));
            }
        }

        #endregion

        [SerializeField]
        private SimultaneousGroup[] _groups;

        public override IEnumerator RunAsync()
        {
            foreach (var group in _groups)
                yield return group.RunAsync();
        }
    }
}
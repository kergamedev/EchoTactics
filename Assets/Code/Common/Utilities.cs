using System;
using System.Collections;
using System.Threading.Tasks;

namespace Echo.Common
{
    public static class Utilities
    {
        #region Async

        public static IEnumerator WaitForCompletion(Func<Task> method)
        {
            var task = method();
            yield return task.WaitForCompletion();
        }

        #endregion
    }
}
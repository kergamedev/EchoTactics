using System;
using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using Unity.Multiplayer;
using Unity.Multiplayer.Playmode;

namespace Echo.Common
{
    public static class Utilities
    {
        #region Async

        public static IEnumerator WaitForCompletion(Func<Task> method)
        {
            var task = method();
            yield return task.WaitForCompletionAsync();
        }

        #endregion

        #region Network

        public static bool IsServer()
        {
            return MultiplayerRolesManager.ActiveMultiplayerRoleMask == MultiplayerRoleFlags.Server;
        }

        #endregion
    }
}
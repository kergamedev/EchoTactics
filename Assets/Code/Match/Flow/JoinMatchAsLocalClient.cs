using System.Threading.Tasks;
using UnityEngine;

namespace Echo.Match
{
    public class JoinMatchAsLocalClient : IStartMachHandler
    {
        public async Task ExecuteAsync(Match match)
        {
            match.NetworkManager.StartClient();

            while (match.GameState == null)
                await Awaitable.WaitForSecondsAsync(1f);
        }
    }
}
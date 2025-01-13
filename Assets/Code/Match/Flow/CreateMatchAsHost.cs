using Echo.Common;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Echo.Match
{
    public class CreateMatchAsHost : IStartMachHandler
    {
        public async Task ExecuteAsync(Match match)
        {
            await match.SetupServerAsync(new Server(match));
            match.NetworkManager.StartHost();
            match.NetworkManager.Spawn(match.Prefabs.Bot);
            match.NetworkManager.Spawn(match.Prefabs.GameState);
        }
    }
}
using Echo.Common;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Echo.Match
{
    public class CreateMatchAsServer : IStartMachHandler
    {
        public async Task ExecuteAsync(Match match)
        {
            await match.SetupServerAsync(new Server(match));
        
            #if !DEDICATED_SERVER

            match.NetworkManager.StartServer();

            #endif
        }
    } 
}
using System.Threading.Tasks;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Matchmaker.Models;
using UnityEngine;

namespace Echo.Match
{
    public class JoinMatchAsClient : IStartMachHandler
    {
        public readonly MultiplayAssignment Assignment;

        public JoinMatchAsClient(MultiplayAssignment assignment)
        {
            Assignment = assignment;
        }

        public async Task ExecuteAsync(Match match)
        {
            var transport = match.NetworkManager.GetComponent<UnityTransport>();
            transport.SetConnectionData(Assignment.Ip, (ushort)Assignment.Port);

            match.NetworkManager.StartClient();

            while (match.GameState == null)
                await Awaitable.WaitForSecondsAsync(1f);
        }
    }
}
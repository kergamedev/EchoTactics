using Echo.Common;
using System.Threading.Tasks;
using Unity.Netcode;

namespace Echo.Match
{
    public interface IStartMachHandler
    {
        Task ExecuteAsync(Match match);
    }
}
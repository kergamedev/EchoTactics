using System.Threading;
using System.Threading.Tasks;
using UnityEngine.Localization;

namespace Echo.Common
{
    public interface IGame
    {
        TweenLibrary TweenLibrary { get; }
        IPlayerAccount PlayerAccount { get; }
        ISaveSystem SaveSystem { get; }

        Task GoToHomeAsync(bool withTransition = true);

        Task<IMatchStartArgs> TrySetupOnlineMatchAsync(CancellationToken cancellationToken = default(CancellationToken));
        Task StartSoloMatchAsync(bool withTransitions = true);
        Task StartMatchAsync(IMatchStartArgs args, bool withTransitions = true);

        Task ChangeLocaleAsync(Locale locale);

        void Quit();
    }
}
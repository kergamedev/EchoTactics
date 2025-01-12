using Echo.Common;
using System.Threading.Tasks;
using UnityEngine;

namespace Echo.Home
{
    public class Home : MonoBehaviour, IHome
    {
        private void Awake()
        {
            Global.Home = this;
        }

        public async Task GoToMatchAsync()
        {
            await Global.Game.GoToMatchAsync();
        }

        private void OnDestroy()
        {
            Global.Home = null;
        }
    }
}
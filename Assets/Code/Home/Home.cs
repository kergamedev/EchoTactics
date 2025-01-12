using Echo.Common;
using System.Threading.Tasks;
using UnityEngine;

namespace Echo.Home
{
    public class Home : MonoBehaviour, IHome
    {
        private void OnEnable()
        {
            Global.Home = this;
        }

        public async Task GoToMatchAsync()
        {
            await Global.Game.GoToMatchAsync();
        }

        private void OnDisable()
        {
            Global.Home = null;
        }
    }
}
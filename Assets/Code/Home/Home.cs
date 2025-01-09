using Echo.Common;
using UnityEngine;

namespace Echo.Home
{
    public class Home : MonoBehaviour
    {
        public void GoToMatch()
        {
            Global.Game.GoToMatch();
        }
    }
}
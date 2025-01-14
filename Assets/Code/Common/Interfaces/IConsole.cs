using System;
using UnityEngine.SceneManagement;

namespace Echo.Common
{
    public interface IConsole
    {
        void AddCommand(Scene boundTo, string name, Action<string[]> method);
    }
}
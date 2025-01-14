using System;
using UnityEngine.SceneManagement;

namespace Echo.Game
{
    public class ConsoleCommand
    {
        public readonly string Name;
        public readonly Scene BoundTo;

        private readonly Action<string[]> _method;

        public ConsoleCommand(Scene boundTo, string name, Action<string[]> method)
        {
            BoundTo = boundTo;
            Name = name;

            _method = method;
        }

        public void Execute(string[] parameters)
        {
            _method?.Invoke(parameters);
        }
    }
}
using Echo.Common;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace Echo.Game
{
    public class Console : MonoBehaviour, IConsole
    {
        #region Nested types

        private enum State
        {
            RequestedToShow,
            Shown,
            RequestedToHide,
            Hidden
        }

        #endregion

        private const float BACKGROUND_HEIGHT = 95f;
        private const float PADDING = 10f;
        private const string INPUT_CONTROL_NAME = "Commands.Input";

        [SerializeField]
        private GUISkin _skin;

        [SerializeField]
        private InputActionReference _showInput;

        [SerializeField]
        private InputActionReference _confirmInput;

        [SerializeField]
        private InputActionReference _abortInput;

        private State _state;
        private string _input;
        private Dictionary<string, ConsoleCommand> _commands;

        public bool IsShown => _state == State.Shown;

        private void OnEnable()
        {
            Global.Console = this;

            _state = State.Hidden;
            _input = string.Empty;
            _commands = new Dictionary<string, ConsoleCommand>();
            
            _showInput.action.performed += Show;
            _confirmInput.action.performed += ExecuteCommand;
            _abortInput.action.performed += Abort;

            SceneManager.sceneUnloaded += OnSceneUnloaded;
        }

        private void Show(InputAction.CallbackContext _)
        {
            Show();
        }
        public void Show()
        {
            _state = State.RequestedToShow;
        }

        private void OnGUI()
        {
            switch (_state)
            {
                case State.RequestedToShow:
                case State.Shown: 
                    break;

                case State.RequestedToHide:
                    GUI.FocusControl(string.Empty);
                    _state = State.Hidden;
                    _input = string.Empty;
                    return;

                case State.Hidden: 
                    return;
            }

            GUI.skin = _skin;

            var backgroundRect = new Rect(0, Screen.height - BACKGROUND_HEIGHT, Screen.width, BACKGROUND_HEIGHT);
            GUI.Box(backgroundRect, GUIContent.none);

            var inputFieldRect = backgroundRect.Pad(PADDING);
            GUI.SetNextControlName(INPUT_CONTROL_NAME);
            _input = GUI.TextField(inputFieldRect, _input);

            if (_state == State.RequestedToShow)
            {
                GUI.FocusControl(INPUT_CONTROL_NAME);
                _state = State.Shown;
            }
        }

        public void AddCommand(Scene boundTo, string name, Action<string[]> method)
        {
            var command = new ConsoleCommand(boundTo, name.ToLower(), method);
            AddCommand(command);
        }
        public void AddCommand(ConsoleCommand command)
        {
            _commands.Add(command.Name, command);
        }

        private void ExecuteCommand(InputAction.CallbackContext _)
        {
            if (!IsShown || _input.IsNullOrEmpty())
                return;

            var splitInput = _input.Split(' ');
            var commandName = splitInput[0].ToLower();

            if (!_commands.TryGetValue(commandName, out var command))
            {
                Debug.LogWarning($"[CONSOLE] No command with 'Name={commandName}' could be found");
                Hide();

                return;
            }

            var parameters = default(string[]);
            if (splitInput.Length > 1)
            {
                parameters = new string[splitInput.Length - 1];
                for (var i = 1; i < splitInput.Length; i++)
                    parameters[i] = splitInput[i];
            }
            else parameters = new string[0];

            command.Execute(parameters);
            Hide();
        }

        private void Abort(InputAction.CallbackContext _)
        {
            if (IsShown)
                Hide();
        }

        public void Hide()
        {
            _state = State.RequestedToHide;
        }

        private void OnSceneUnloaded(Scene scene)
        {
            var toRemove = new List<string>();
            foreach (var kvp in _commands)
            {
                var command = kvp.Value;
                if (command.BoundTo == scene)
                    toRemove.Add(kvp.Key);
            }

            foreach (var key in toRemove)
                _commands.Remove(key);
        }

        private void OnDisable()
        {
            SceneManager.sceneUnloaded -= OnSceneUnloaded;

            _showInput.action.performed -= Show;
            _confirmInput.action.performed -= ExecuteCommand;
            _abortInput.action.performed -= Abort;

            _commands.Clear();
            _commands = null;

            Global.Console = null;
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PrototUnity.DebugConsole {
	public class DebugConsole : MonoBehaviour {
		private bool isShowing;
		private bool showHelp;
		private List<InputAction> previouslyActiveMaps;
		private readonly List<DebugCommandBase> commands = new();
		
		private string input;
		private Vector2 helpScroll;
		private const float lineHeight = 45f;
		private const int commandsOnOneScroll = 4;
		
		private readonly List<string> commandHistory = new();
		private const int maxCommandHistory = 3;
		private int commandHistoryIndex;

		public void AddCommand(DebugCommandBase newCommand) {
			if (commands.Any(it => it.CommandName == newCommand.CommandName)) {
				Debug.LogError($"Command with name {newCommand.CommandName} already exists");
				return;
			}
			
			commands.Add(newCommand);
		}

		private void OnEnable() {
			commands.Add(new DebugCommand("help", "Show all available commands", () => {
				showHelp = true;
			}));
		}

		private void Update() {
			if (!Application.isEditor) return;
			if (Keyboard.current.backquoteKey.wasPressedThisFrame) {
				OnDebugConsoleToggleEvent();
			} else if (Keyboard.current.enterKey.wasPressedThisFrame) {
				OnDebugConsoleEnterEvent();
			} else if (Keyboard.current.upArrowKey.wasPressedThisFrame) {
				OnDebugShowPreviousCommandEvent();
			} else if (Keyboard.current.downArrowKey.wasPressedThisFrame) {
				OnDebugShowNextCommandEvent();
			}
		}

		private void OnDebugConsoleToggleEvent() {
			isShowing = !isShowing;
			if (isShowing) {
				previouslyActiveMaps = InputSystem.ListEnabledActions();
				previouslyActiveMaps.ForEach(it => it.Disable());
			} else {
				previouslyActiveMaps.ForEach(it => it.Enable());
				previouslyActiveMaps.Clear();
				input = "";
			}
		}

		private void OnDebugConsoleEnterEvent() {
			if (!isShowing) return;
			
			AddCurrentInputToHistory();
				
			var parsedInput = ParseInput(input);
			input = "";
			
			var commandName = parsedInput[0];
			var command = commands.FirstOrDefault(it => it.CommandName == commandName);
			switch (command) {
				case DebugCommand debugCommand:
					debugCommand.Invoke();
					break;
				case DebugCommandWithOneArg when parsedInput.Count != 2:
					Debug.LogError($"Command {commandName} requires one argument");
					return;
				case DebugCommandWithOneArg debugCommandWithOneArg:
					debugCommandWithOneArg.Invoke(parsedInput[1]);
					break;
				case DebugCommandWithTwoArgs when parsedInput.Count != 3:
					Debug.LogError($"Command {commandName} requires two arguments");
					return;
				case DebugCommandWithTwoArgs debugCommandWithTwoArgs:
					debugCommandWithTwoArgs.Invoke(parsedInput[1], parsedInput[2]);
					break;
			}
		}

		private static List<string> ParseInput(string consoleInput) {
			var tokens = consoleInput.Split(' ');
			var result = new List<string>();

			var combinedWord = "";
			foreach (var untrimmedToken in tokens) {
				var token = untrimmedToken.Trim();
				if (token.StartsWith("\"") && token.EndsWith("\"")) {
					result.Add(token[1..^1]);
				} else if (token.StartsWith("\"")) {
					combinedWord += token[1..];
				} else if (token.EndsWith("\"")) {
					result.Add(combinedWord + " " + token[..^1]);
					combinedWord = "";
				} else if (combinedWord != "") {
					combinedWord += " " + token;
				} else {
					result.Add(token);
				}
			}

			if (combinedWord != "") {
				result.Add(combinedWord);
				Debug.LogError("Invalid input: no closing quote for opened one");
			}
			
			return result;
		}

		private void AddCurrentInputToHistory() {
			commandHistory.Insert(0, input);
			if (commandHistory.Count > maxCommandHistory) commandHistory.RemoveAt(commandHistory.Count - 1);
			commandHistoryIndex = 0;
		}
		
		private void OnDebugShowPreviousCommandEvent() {
			if (commandHistoryIndex >= commandHistory.Count) {
				return;
			}
			
			input = commandHistory[commandHistoryIndex];
			commandHistoryIndex++;
		}

		private void OnDebugShowNextCommandEvent() {
			if (commandHistoryIndex <= 0) {
				input = "";
				return;
			}
			
			commandHistoryIndex--;
			input = commandHistory[commandHistoryIndex];
		}

		private void OnGUI() {
			if (!isShowing) return;
			
			GUIStyle textStyle = new(GUI.skin.textField) {
				fontSize = 30,
			};

			var startY = DrawHelp(textStyle);
			DrawDebugConsole(startY, textStyle);
		}

		private void DrawDebugConsole(float startY, GUIStyle textStyle) {
			GUI.Box(new Rect(0, startY, Screen.width, 60f), "");

			GUI.SetNextControlName("DebugConsoleTextField");
			input = GUI.TextField(new Rect(10, startY + 5f, Screen.width - 20, lineHeight), input, textStyle);
			GUI.FocusControl("DebugConsoleTextField");
		}

		private float DrawHelp(GUIStyle textStyle) {
			if (!showHelp) return 0;
			GUI.Box(new Rect(0, 0, Screen.width, lineHeight * commandsOnOneScroll), "");
			var viewport = new Rect(0, 0, Screen.width - 20, lineHeight * commands.Count);
			var scrollPosition = new Rect(10, 5f, Screen.width - 20, lineHeight * commandsOnOneScroll);
			helpScroll = GUI.BeginScrollView(scrollPosition, helpScroll, viewport);
			for (var i = 0; i < commands.Count; i++) {
				var command = commands[i];
				var label = $"{command.CommandName} - {command.CommandDescription}";
				var labelRect = new Rect(5, lineHeight * i, viewport.width - 20, lineHeight);
				GUI.Label(labelRect, label, textStyle);
			}
			GUI.EndScrollView();
			return lineHeight * commandsOnOneScroll;
		}
	}
}
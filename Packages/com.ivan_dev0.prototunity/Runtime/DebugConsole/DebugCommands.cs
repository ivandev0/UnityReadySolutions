using UnityEngine;

namespace PrototUnity.DebugConsole {
	public class DebugCommands : MonoBehaviour {
		[SerializeField] private DebugConsole console;
		
		private void OnEnable() {
			// Example command:
			var echoCommand = new DebugCommandWithOneArg("echo", "Prints given argument into console", Debug.Log);
			console.AddCommand(echoCommand);
		}
	}
}
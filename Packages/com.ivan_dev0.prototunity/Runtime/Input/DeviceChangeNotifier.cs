using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace PrototUnity.Input {
	[RequireComponent(typeof(PlayerInput))]
	public class DeviceChangeNotifier : MonoBehaviour {
		[SerializeField] PlayerInput playerInput;
	
		public ActionScheme Current { get; private set; } = ActionScheme.KeyboardAndMouse;
		public enum ActionScheme {
			KeyboardAndMouse,
			Gamepad,
		}
	
		public event UnityAction<ActionScheme> DeviceChangedEvent = delegate { };
	
		private void OnEnable() {
			playerInput.onControlsChanged += OnPlayerInputChanged;
		}

		private void OnDisable() {
			playerInput.onControlsChanged -= OnPlayerInputChanged;
		}

		private void OnPlayerInputChanged(PlayerInput input) {
			ActionScheme scheme;
			switch (input.currentControlScheme) {
				case "Keyboard&Mouse":
					scheme = ActionScheme.KeyboardAndMouse;
					break;
				case "Gamepad":
					scheme = ActionScheme.Gamepad;
					break;
				default:
					Debug.LogError($"Unknown control scheme: {input.currentControlScheme}");
					return;
			}

			if (scheme != Current) {
				Current = scheme;
				DeviceChangedEvent.Invoke(Current);
			}
		}
	}
}
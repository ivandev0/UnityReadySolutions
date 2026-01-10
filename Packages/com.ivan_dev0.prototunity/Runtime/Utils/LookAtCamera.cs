using UnityEngine;

namespace PrototUnity.Utils {
    public class LookAtCamera : MonoBehaviour {
        [SerializeField] private UnityEngine.Camera activeCamera;

        private enum Mode {
            LookAt,
            LookAtInverted,
            CameraForward,
            CameraForwardInverted,
        }

        [SerializeField] private Mode mode;

        private void Start() {
            if (activeCamera == null) {
                activeCamera = UnityEngine.Camera.main;
            }
        }

        private void LateUpdate() {
            switch (mode) {
                case Mode.LookAt:
                    transform.LookAt(activeCamera.transform);
                    break;
                case Mode.LookAtInverted:
                    Vector3 dirFromCamera = transform.position - activeCamera.transform.position;
                    transform.LookAt(transform.position + dirFromCamera);
                    break;
                case Mode.CameraForward:
                    transform.forward = activeCamera.transform.forward;
                    break;
                case Mode.CameraForwardInverted:
                    transform.forward = -activeCamera.transform.forward;
                    break;
            }
        }

    }
}
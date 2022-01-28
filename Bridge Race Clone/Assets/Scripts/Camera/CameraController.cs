using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CameraControllerSettings _cameraControllerSettings;

        public Transform Target;

        private void Start()
        {
            transform.SetParent(null);
        }

        private void LateUpdate()
        {
            Vector3 targetPosition = new Vector3(0, 0, Target.position.z) + _cameraControllerSettings.Offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _cameraControllerSettings.LerpValue);
        }
    }    
}

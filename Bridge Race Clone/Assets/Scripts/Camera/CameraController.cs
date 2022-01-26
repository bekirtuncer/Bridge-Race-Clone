using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private CameraControllerSettings _cameraControllerSettings;

        public Transform Target;

        private void LateUpdate()
        {
            Vector3 targetPosition = Target.position + _cameraControllerSettings.Offset;
            transform.position = Vector3.Lerp(transform.position, targetPosition, _cameraControllerSettings.LerpValue);
        }
    }    
}

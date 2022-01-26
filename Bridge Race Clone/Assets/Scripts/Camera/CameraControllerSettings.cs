using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace
{
    [CreateAssetMenu(menuName = "BridgeRace/Camera/CameraSettings")]
    public class CameraControllerSettings : ScriptableObject
    {
        public Vector3 Offset;
        public float LerpValue;
    }
}

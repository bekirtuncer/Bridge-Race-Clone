using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.PlayerMovement
{
    [CreateAssetMenu(menuName ="BridgeRace/Movement/PlayerMovement")]
    public class PlayerMovementSettings : ScriptableObject
    {
        public float TurnSpeed;
        public float Speed;
        public float LerpValue;
    }    
}

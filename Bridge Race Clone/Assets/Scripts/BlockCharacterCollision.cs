using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace
{
    public class BlockCharacterCollision : MonoBehaviour
    {
        [SerializeField] private BoxCollider _characterCollider;
        [SerializeField] private CapsuleCollider _characterBlockerCollider;
        // Start is called before the first frame update
        void Start()
        {
            Physics.IgnoreCollision(_characterCollider, _characterBlockerCollider, true);
        }        
    }
}

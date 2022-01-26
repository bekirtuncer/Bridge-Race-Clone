using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.PlayerMovement
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings _playerMovementSettings;
        [SerializeField] private Camera _camera;
        [SerializeField] private Animator _animator;

        public LayerMask Layer;

        private void FixedUpdate()
        {
            if (Input.GetMouseButton(0))
            {
                MovePlayer();
            }
            else
            {
                if (_animator.GetBool("Running"))
                {
                    _animator.SetBool("Running", false);
                }
            }
        }

        private void MovePlayer()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = _camera.transform.localPosition.z;

            Ray ray = _camera.ScreenPointToRay(mousePos);

            RaycastHit hit;
            if(Physics.Raycast(ray, out hit, Mathf.Infinity, Layer))
            {
                Vector3 hitVec = hit.point;
                hitVec.y = transform.position.y;

                transform.position = Vector3.MoveTowards(transform.position, Vector3.Lerp(transform.position, hitVec,
                    _playerMovementSettings.LerpValue), Time.deltaTime * _playerMovementSettings.Speed);
                Vector3 newMovePoint = new Vector3(hit.point.x, transform.position.y, hit.point.z);
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(newMovePoint -
                    transform.position), _playerMovementSettings.TurnSpeed * Time.deltaTime);
                if (!_animator.GetBool("Running"))
                {
                    _animator.SetBool("Running", true);
                }
            }
        }
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using BridgeRace.ObstaclesController;

namespace BridgeRace.PlayerMovement
{
    public class PlayerMovementController : MonoBehaviour
    {
        [SerializeField] private PlayerMovementSettings _playerMovementSettings;
        [SerializeField] private Camera _camera;
        [SerializeField] private Animator _animator;

        [SerializeField] private Transform _gathersTopObject;
        [SerializeField] private GameObject _prevObject;
        [SerializeField] private List<GameObject> _cubes = new List<GameObject>();

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

        private void OnTriggerEnter(Collider target)
        {
            if (target.gameObject.tag.StartsWith(transform.GetChild(1).
                GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1)))
            {
                target.transform.SetParent(_gathersTopObject);
                Vector3 position = _prevObject.transform.localPosition;

                position.y += 0.22f;
                position.z = 0;
                position.x = 0;

                target.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);

                target.transform.DOLocalMove(position, 0.2f);
                _prevObject = target.gameObject;
                _cubes.Add(target.gameObject);

                target.tag = "Untagged";

                ObstacleSpawner.Instance.SpawnObstacle(0);
            }
        }
    }    
}

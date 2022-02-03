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
        private Vector3 _lastPos;
        private int currentStage;

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

            CheckWinner();
        }

        private void CheckWinner()
        {
            if (ObstacleSpawner.Instance.isWinnerPresent)
            {
                if (!ObstacleSpawner.Instance.isSecondTaken)
                {
                    ObstacleSpawner.Instance.isSecondTaken = true;
                    transform.DOMove(GameObject.Find("SecondPosition").transform.position, 1.0f);

                    _animator.SetBool("Running", false);
                    _animator.SetBool("Crying", true);
                }
                else if (!ObstacleSpawner.Instance.isThirdTaken)
                {
                    ObstacleSpawner.Instance.isThirdTaken = true;
                    transform.DOMove(GameObject.Find("ThirdPosition").transform.position, 1.0f);

                    _animator.SetBool("Running", false);
                    _animator.SetBool("Crying", true);
                }

                Destroy(this);
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
            if (target.gameObject.tag.StartsWith(transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().
                    material.name.Substring(0, 1)))
            {
                target.transform.SetParent(_gathersTopObject);

                _lastPos.y += 0.22f;
                _lastPos.x = 0f;
                _lastPos.z = 0f;

                target.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);

                target.transform.DOLocalMove(_lastPos, 0.2f);
                _cubes.Add(target.gameObject);                
                target.tag = "Untagged";                

                ObstacleSpawner.Instance.SpawnObstacle(0, null, currentStage);
            }
            else if (target.gameObject.tag == "Dumpster" && _cubes.Count > 0 && !target.GetComponent<DumpsterController>().isComplete)
            {
                Collider masterTarget = target;
                target = masterTarget.transform.GetChild(0).GetComponent<Collider>();

                if (target.gameObject.tag.StartsWith("Align"))
                {
                    if (_cubes.Count > 0)
                    {
                        int i = 0;
                        int foundIndex = 0;
                        for (i = 0; i < masterTarget.transform.childCount; i++)
                        {
                            if (masterTarget.transform.GetChild(i).GetComponent<MeshRenderer>() != null && !masterTarget.transform.GetChild(i).GetComponent<MeshRenderer>().enabled)
                            {
                                foundIndex = i;
                                break;
                            }
                        }

                        int cubesCount = _cubes.Count;
                        int usedCubes = 0;

                        for (i = 0; i < cubesCount; i++)
                        {
                            GameObject obj = _cubes[i];

                            if (foundIndex + i <= masterTarget.transform.childCount - 4)
                            {
                                target = masterTarget.transform.GetChild(foundIndex + i).GetComponent<Collider>();

                                target.GetComponent<MeshRenderer>().material = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
                                target.GetComponent<MeshRenderer>().enabled = true;
                                target.tag = "Align" + transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1);

                                Destroy(obj);

                                usedCubes++;
                            }
                        }

                        for (int z = 0; z < _cubes.Count; z++)
                        {
                            if (z >= usedCubes)
                            {
                                _cubes[z].transform.SetParent(null);
                                _cubes[z].AddComponent<Rigidbody>();
                                _cubes[z].AddComponent<MeshCollider>().convex = true;
                                _cubes[z].AddComponent<KillZController>();
                                _cubes[z].GetComponent<Rigidbody>().AddForce((Random.insideUnitSphere * Random.Range(5, 20)) + (Vector3.up * Random.Range(10, 20)), ForceMode.Impulse);
                                _cubes[z].tag = "Untagged";
                            }
                        }

                        _cubes.Clear();

                        _lastPos = Vector3.zero;

                        string obstacleColorName = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1);

                        switch (obstacleColorName)
                        {
                            case "O":
                                obstacleColorName = "Orange";
                                break;
                            case "R":
                                obstacleColorName = "Red";
                                break;
                            case "G":
                                obstacleColorName = "Green";
                                break;
                        }


                        if (masterTarget.transform.GetChild(masterTarget.transform.childCount - 4).GetComponent<MeshRenderer>().enabled && masterTarget.transform.GetChild(masterTarget.transform.childCount - 4).GetComponent<MeshRenderer>().material.name.Contains(obstacleColorName))
                        {
                            masterTarget.GetComponent<DumpsterController>().hasClaimed = true;
                            masterTarget.GetComponent<DumpsterController>().isComplete = true;
                            masterTarget.GetComponent<SphereCollider>().isTrigger = false;
                            Destroy(masterTarget.GetComponent<BoxCollider>());

                            if (obstacleColorName == "Orange")
                            {
                                masterTarget.gameObject.layer = LayerMask.NameToLayer("OrangeOnly");
                            }
                            else if (obstacleColorName == "Red")
                            {
                                masterTarget.gameObject.layer = LayerMask.NameToLayer("RedOnly");
                            }
                            else if (obstacleColorName == "Green")
                            {
                                masterTarget.gameObject.layer = LayerMask.NameToLayer("GreenOnly");
                            }

                            currentStage += 1;
                            
                            foreach (GameObject _target in GameObject.FindGameObjectsWithTag(obstacleColorName))
                            {
                                Destroy(_target);
                            }                            

                            return;
                        }
                        else
                        {
                            masterTarget.GetComponent<DumpsterController>().hasClaimed = false;
                        }
                    }
                    else
                    {
                        Debug.LogError("No cubes but tries to dump!");
                    }
                }
            }
            else if (target.tag == "StageInitiate")
            {
                Destroy(target.gameObject.GetComponent<Collider>());

                if(currentStage == 2)
                {                    
                    if(!ObstacleSpawner.Instance.isWinnerPresent)
                    {
                        ObstacleSpawner.Instance.isWinnerPresent = true;
                        transform.DOMove(GameObject.Find("WinPosition").transform.position, 1.0f);

                        _animator.SetBool("Running", false);
                        _animator.SetBool("Dancing", true);
                    }                    
                    else if(!ObstacleSpawner.Instance.isSecondTaken)
                    {
                        ObstacleSpawner.Instance.isSecondTaken = true;
                        transform.DOMove(GameObject.Find("SecondPosition").transform.position, 1.0f);

                        _animator.SetBool("Running", false);
                        _animator.SetBool("Crying", true);
                    }
                    else if (!ObstacleSpawner.Instance.isThirdTaken)
                    {
                        ObstacleSpawner.Instance.isThirdTaken = true;
                        transform.DOMove(GameObject.Find("ThirdPosition").transform.position, 1.0f);

                        _animator.SetBool("Running", false);
                        _animator.SetBool("Crying", true);
                    }

                    Destroy(this);
                }                
                else
                {
                    ObstacleSpawner.Instance.SpawnObstacle(0, null, currentStage);
                }                
            }
        }
    }    
}

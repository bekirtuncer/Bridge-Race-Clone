using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;
using BridgeRace.ObstaclesController;

namespace BridgeRace.AI
{   
    public enum Character { one=1, two=2 } 
    public class CharacterAIController : MonoBehaviour
    {
        public List<GameObject> _targets = new List<GameObject>();

        [SerializeField] private GameObject _targetsParent;
        [SerializeField] private Transform _gathersMainObject;        
        [SerializeField] private List<GameObject> _cubes = new List<GameObject>();
        [SerializeField] private Transform[] _stairs_S1;
        [SerializeField] private Transform[] _stairs_S2;
        [SerializeField] private Character _characterEnum;

        [SerializeField] private CharacterAISettings _characterAISettings;
        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _agent;
        [SerializeField] private Vector3 _lastPos; //last stack pos

        private bool _lockTarget = false;
        private Vector3 _targetTransform;

        private bool canReachForFinal = false;
        private bool hasFinished = false;
        private int currentStage = 0;
        private GameObject lastDumpDestination;

        private void Start()
        {
            for (int i = 0; i < _targetsParent.transform.childCount; i++)
            {
                _targets.Add(_targetsParent.transform.GetChild(i).gameObject);
            }
        }

        private void Update()
        {
            CheckFinish(false);

            if (!hasFinished && !canReachForFinal && !_lockTarget && _targets.Count>0 && _cubes.Count <= 5)
            {
                FindTarget();
            }
        }

        private void FindTarget()
        {            
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _characterAISettings.Radius);
            List<Vector3> ourColors = new List<Vector3>();
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].tag.StartsWith(transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().
                    material.name.Substring(0, 1)))
                {
                    ourColors.Add(hitColliders[i].transform.position);
                }
            }
            
            if (ourColors.Count > 0)
            {
                _targetTransform = ourColors[0];
            }
            else
            {
                int random = Random.Range(0, _targets.Count);
                _targetTransform = _targets[random].transform.position;
            }

            _agent.SetDestination(_targetTransform);
            if (!_animator.GetBool("Running"))
            {
                _animator.SetBool("Running", true);
            }
            
            _lockTarget = true;
        }

        private bool DumpTarget()
        {
            int randomNumber = Random.Range(0, 3);
            Transform[] _stairs;

            if (currentStage == 0)
            {
                _stairs = _stairs_S1;
            }            
            else
            {
                _stairs = _stairs_S2;
            }


            if (_cubes.Count == 5)
            {
                int randomStair = -1;

                bool foundUnclaimedStairs = false;

                int whileIterator = 0;

                while(!foundUnclaimedStairs)
                {
                    randomStair = Random.Range(0, _stairs.Length);

                    if (!_stairs[randomStair].GetComponent<DumpsterController>().hasClaimed && !_stairs[randomStair].GetComponent<DumpsterController>().isComplete)
                    {
                        foundUnclaimedStairs = true;
                        _stairs[randomStair].GetComponent<DumpsterController>().hasClaimed = true;
                    }

                    whileIterator++;

                    if (whileIterator > 100)
                    {
                        Debug.LogError("Can't find random stair? Breaking while loop.");
                        break;
                    }
                }                
                List<Transform> stairsNotActiveChild = new List<Transform>();

                //int required
                foreach (Transform item in _stairs[randomStair])
                {
                    if (item.gameObject.tag == "Align")
                    {
                        stairsNotActiveChild.Add(item);
                    }
                }

                if(stairsNotActiveChild.Count != 0)
                {
                    _targetTransform = _cubes.Count <= stairsNotActiveChild.Count ? stairsNotActiveChild[stairsNotActiveChild.Count - 1].
                        position : stairsNotActiveChild[stairsNotActiveChild.Count - 1].position;

                    _agent.SetDestination(_stairs[randomStair].GetChild(0).position);
                    lastDumpDestination = _stairs[randomStair].gameObject;

                    _lockTarget = true;                    
                    return true;
                }               
                else
                {
                    return false;
                }
            }

            return false;
        }

        private void OnTriggerEnter(Collider target)
        {
            if (target.gameObject.tag.StartsWith(transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().
                    material.name.Substring(0, 1)))
            {
                target.transform.SetParent(_gathersMainObject);               

                _lastPos.y += 0.22f;
                _lastPos.x = 0f;
                _lastPos.z = 0f;

                target.transform.localRotation = new Quaternion(0, 0.7071068f, 0, 0.7071068f);

                target.transform.DOLocalMove(_lastPos, 0.2f);                
                _cubes.Add(target.gameObject);

                _targets.Remove(target.gameObject);
                target.tag = "Untagged";
                _lockTarget = false;

                ObstacleSpawner.Instance.SpawnObstacle((int)_characterEnum, this, currentStage);

                if(_cubes.Count == 5)
                {
                    DumpTarget();
                    _lockTarget = true;
                }
            }
            else if(target.gameObject == lastDumpDestination && target.gameObject.tag == "Dumpster" && _cubes.Count > 0)
            {
                Collider masterTarget = target;
                target = masterTarget.transform.GetChild(0).GetComponent<Collider>();                

                if (target.gameObject.tag.StartsWith("Align") )
                {
                    if (_cubes.Count > 0)
                    {
                        int i = 0;
                        int foundIndex = 0;
                        for (i = 0; i < masterTarget.transform.childCount; i++)
                        {
                            if(masterTarget.transform.GetChild(i).GetComponent<MeshRenderer>() != null && !masterTarget.transform.GetChild(i).GetComponent<MeshRenderer>().enabled)
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

                            if(foundIndex + i <= masterTarget.transform.childCount - 4)
                            {
                                target = masterTarget.transform.GetChild(foundIndex + i).GetComponent<Collider>();

                                target.GetComponent<MeshRenderer>().material = transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material;
                                target.GetComponent<MeshRenderer>().enabled = true;
                                target.tag = "Align" + transform.GetChild(1).GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1);

                                Destroy(obj);

                                usedCubes++;
                            }                                                          
                        }                        

                        for(int z = 0; z < _cubes.Count; z++)
                        {
                            if(z >= usedCubes)
                            {
                                _cubes[z].transform.SetParent(null);
                                _cubes[z].AddComponent<Rigidbody>();
                                _cubes[z].AddComponent<MeshCollider>().convex = true;
                                _cubes[z].AddComponent<KillZController>();
                                _cubes[z].GetComponent<Rigidbody>().AddForce((Random.insideUnitSphere * Random.Range(5,20)) + (Vector3.up * Random.Range(10, 20)), ForceMode.Impulse);
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

                        masterTarget.GetComponent<DumpsterController>().hasClaimed = false;

                        if (masterTarget.transform.GetChild(masterTarget.transform.childCount - 4).GetComponent<MeshRenderer>().enabled && masterTarget.transform.GetChild(masterTarget.transform.childCount - 4).GetComponent<MeshRenderer>().material.name.Contains(obstacleColorName))
                        {                            
                            masterTarget.GetComponent<DumpsterController>().isComplete = true;
                            masterTarget.GetComponent<SphereCollider>().isTrigger = false;
                            Destroy(masterTarget.GetComponent<BoxCollider>());                            

                            if(obstacleColorName == "Orange")
                            {
                                masterTarget.gameObject.layer = LayerMask.NameToLayer("OrangeOnly");
                            }
                            else if(obstacleColorName == "Red")
                            {
                                masterTarget.gameObject.layer = LayerMask.NameToLayer("RedOnly");
                            }
                            else if(obstacleColorName == "Green")
                            {
                                masterTarget.gameObject.layer = LayerMask.NameToLayer("GreenOnly");
                            }                            

                            canReachForFinal = true;
                            _agent.SetDestination(masterTarget.transform.GetChild(masterTarget.transform.childCount - 1).position);

                            currentStage += 1;

                            foreach (GameObject _target in GameObject.FindGameObjectsWithTag(obstacleColorName))
                            {
                                Destroy(_target);
                            }

                            _targets.Clear();
                            
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

                _lockTarget = false;
            }
            else if(target.tag == "StageInitiate")
            {
                Destroy(target.gameObject.GetComponent<Collider>());
                
                _lockTarget = false;
                canReachForFinal = false;                

                if(currentStage == 2)
                {
                    hasFinished = true;                    
                    CheckFinish(true);
                }                
                else
                {
                    ObstacleSpawner.Instance.SpawnObstacle((int)_characterEnum, this, currentStage);
                }                
            }
        }

        private void CheckFinish(bool ledByTrigger)
        {            
            if(ledByTrigger)
            {
                if (!ObstacleSpawner.Instance.isWinnerPresent)
                {
                    ObstacleSpawner.Instance.isWinnerPresent = true;
                    transform.DOMove(GameObject.Find("WinPosition").transform.position, 1.0f);

                    _animator.SetBool("Running", false);
                    _animator.SetBool("Dancing", true);
                }
                else if (!ObstacleSpawner.Instance.isSecondTaken)
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

                Destroy(GetComponent<NavMeshAgent>());
                Destroy(this);
            }            
            else if(ObstacleSpawner.Instance.isWinnerPresent)
            {
                hasFinished = true;

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

                Destroy(GetComponent<NavMeshAgent>());
                Destroy(this);
            }
        }
    }    
}

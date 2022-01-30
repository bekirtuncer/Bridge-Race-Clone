using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using DG.Tweening;

namespace BridgeRace.AI
{
    public class CharacterAIController : MonoBehaviour
    {
        [SerializeField] private GameObject _targetsParent;
        [SerializeField] private List<GameObject> _targets = new List<GameObject>();
        [SerializeField] private Transform _gathersTopObject;
        [SerializeField] private GameObject _prevObject;
        [SerializeField] private List<GameObject> _cubes = new List<GameObject>();

        [SerializeField] private CharacterAISettings _characterAISettings;
        [SerializeField] private Animator _animator;
        [SerializeField] private NavMeshAgent _agent;

        private bool _haveTarget = false;
        private Vector3 _targetTransform;

        private void Start()
        {
            for (int i = 0; i < _targetsParent.transform.childCount; i++)
            {
                _targets.Add(_targetsParent.transform.GetChild(i).gameObject);
            }
        }

        private void Update()
        {
            if (!_haveTarget && _targets.Count>0)
            {
                ChooseTarget();
            }
        }

        private void ChooseTarget()
        {
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, _characterAISettings.Radius);
            List<Vector3> ourColors = new List<Vector3>();
            for (int i = 0; i < hitColliders.Length; i++)
            {
                if (hitColliders[i].tag.StartsWith(transform.GetChild(1).
                    GetComponent<SkinnedMeshRenderer>().material.name.Substring(0, 1))) 
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
            _haveTarget = true;
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

                _targets.Remove(target.gameObject);
                target.tag = "Untagged";
                _haveTarget = false;
            }
        }
    }
}
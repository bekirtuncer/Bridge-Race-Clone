using BridgeRace.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.ObstaclesController
{
    public class ObstacleSpawner : MonoBehaviour
    {
        public static ObstacleSpawner Instance;

        [SerializeField] private ObstacleSpawnerSettings _obstacleSpawnerSettings;

        [SerializeField] private GameObject _redObstacles, _greenObstacles, _orangeObstacles;
        [SerializeField] private Transform _redObsParent, _greenObsParent, _orangeObsParent;

        public LayerMask LayerMask;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
        }

        public void SpawnObstacle(int number, CharacterAIController characterAI=null)
        {
            if (number==0)
            {
                Spawn(_redObstacles, _redObsParent, characterAI);
            }
            if (number==1)
            {
                Spawn(_greenObstacles, _greenObsParent, characterAI);
            }
            if (number == 2)
            {
                Spawn(_orangeObstacles, _orangeObsParent, characterAI);
            }
        }
        private void Spawn(GameObject gameObject, Transform parent, CharacterAIController characterAI=null)
        {
            GameObject obs = Instantiate(gameObject);

            Vector3 targetPosition = RandomizePosition();

            obs.SetActive(false);

            Collider[] colliders = Physics.OverlapSphere(targetPosition, 1, LayerMask);

            while (colliders.Length != 0)
            {
                targetPosition = RandomizePosition();
                colliders = Physics.OverlapSphere(targetPosition, 1, LayerMask);
            }
            obs.SetActive(true);
            obs.transform.position = targetPosition;
            if(characterAI != null)
            {
                characterAI._targets.Add(obs);
            }
        }
        private Vector3 RandomizePosition()
        {
            return new Vector3(Random.Range(_obstacleSpawnerSettings.MinX, _obstacleSpawnerSettings.MaxX),
                _redObstacles.transform.position.y, Random.Range(_obstacleSpawnerSettings.MinZ, _obstacleSpawnerSettings.MaxZ));
        }
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.ObstaclesController
{
    [CreateAssetMenu(menuName = "BridgeRace/Obstacles/ObstaclesControllerSettings")]
    public class ObstacleSpawnerSettings : ScriptableObject
    {
        public List<int> MinX, MaxX, MinZ, MaxZ;
    }
}

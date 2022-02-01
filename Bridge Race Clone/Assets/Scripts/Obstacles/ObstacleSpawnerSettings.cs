using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.ObstaclesController
{
    [CreateAssetMenu(menuName = "BridgeRace/Obstacles/ObstaclesControllerSettings")]
    public class ObstacleSpawnerSettings : ScriptableObject
    {
        public int MinX, MaxX, MinZ, MaxZ;
    }
}

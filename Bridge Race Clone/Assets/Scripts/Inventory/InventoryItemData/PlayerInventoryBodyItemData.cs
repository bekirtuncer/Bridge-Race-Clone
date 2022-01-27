using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.Inventory
{
    [CreateAssetMenu(menuName = "BridgeRace/Inventory/PlayerInventoryBodyItemData")]
    public class PlayerInventoryBodyItemData : AbstractPlayerInventoryItemData<PlayerInventoryBodyItemMono>
    {
        public override void CreateIntoInventory(PlayerInventoryController targetPlayerInventoryController)
        {
            var instantiated = InstantiateAndInitializePrefab(targetPlayerInventoryController.Parent);
        }
    }
}

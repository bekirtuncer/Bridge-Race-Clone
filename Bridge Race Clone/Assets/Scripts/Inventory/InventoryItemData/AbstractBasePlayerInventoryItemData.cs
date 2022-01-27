using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.Inventory
{
    public abstract class AbstractBasePlayerInventoryItemData : ScriptableObject
    {
        public abstract void CreateIntoInventory(PlayerInventoryController targetPlayerInventoryController);

        public virtual void Destroy()
        {
            Destroy(this);
        }
    }    
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BridgeRace.Inventory
{
    public class PlayerInventoryController : MonoBehaviour
    {
        [SerializeField] private AbstractBasePlayerInventoryItemData[] _inventoryItemDataArray;
        public Transform Parent;
        private void Start()
        {
            InitializeInventory(_inventoryItemDataArray);
        }

        private void InitializeInventory(AbstractBasePlayerInventoryItemData[] inventoryItemDataArray)
        {
            for(int i = 0; i < inventoryItemDataArray.Length; i++)
            {
                inventoryItemDataArray[i].CreateIntoInventory(this);
            }
        }
    }    
}

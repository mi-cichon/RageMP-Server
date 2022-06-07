using System;
using System.Collections.Generic;
using System.Text;
using GTANetworkAPI;

namespace ServerSide
{
    public static class DroppedItemsManager
    {
        static List<DroppedItem> droppedItems = new List<DroppedItem>();
        public static void AddItem(Vector3 position, int itemID, string name)
        {
            droppedItems.Add(new DroppedItem(position, itemID, name));
        }

        public static void PickItemUp(Player player, UInt64 itemId)
        {
            foreach(DroppedItem item in droppedItems)
            {
                if(item.id == itemId)
                {
                    player.TriggerEvent("checkIfItemFits", item.itemID, item.id.ToString());
                    break;
                }
            }
        }
        public static void ConfirmPickingUp(Player player, UInt64 itemId)
        {
            foreach (DroppedItem item in droppedItems)
            {
                if (item.id == itemId)
                {
                    player.TriggerEvent("addItemToEquipment", item.itemID);
                    PlayerDataManager.NotifyPlayer(player, "Podniesiono " + item.name);
                    item.RemoveItem();
                    droppedItems.Remove(item);
                    break;
                }
            }
        }
        
    }

    public class DroppedItem
    {
        GTANetworkAPI.Object obj;
        TextLabel label;
        ColShape col;
        public Vector3 position;
        public int itemID;
        public string name;
        public UInt64 id;
        public DroppedItem(Vector3 position, int itemID, string name)
        {
            this.position = position;
            this.itemID = itemID;
            this.name = name;
            DateTime time = DateTime.Now;
            id = Convert.ToUInt64(NAPI.Util.GetHashKey(time.ToString() + time.Millisecond.ToString()));
            createItem();
        }

        void createItem()
        {
            obj = NAPI.Object.CreateObject(690307545, position, new Vector3());
            obj.SetSharedData("type", "dropped");
            label = NAPI.TextLabel.CreateTextLabel(name, position + new Vector3(0, 0, 0.3), 10.0f, 0.5f, 4, new Color(255, 255, 255), entitySeethrough: false);
            col = NAPI.ColShape.CreateCylinderColShape(position, 1.3f, 1.5f);
            col.SetSharedData("type", "droppeditem");
            col.SetSharedData("id", id);
        }

        public void RemoveItem()
        {
            obj.Delete();
            label.Delete();
            col.Delete();
        }
    }
}

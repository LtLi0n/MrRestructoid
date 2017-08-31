using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using System;
using System.Collections.Generic;
using System.Text;
using MrRestructoid.Bot.WarsOfTanoth.Items.Gear;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace MrRestructoid.Bot.WarsOfTanoth.Items
{
    public class Inventory
    {
        public List<InventoryItem> Items { get; private set; }
        [JsonIgnore] public Player Player { get; private set; }

        public Inventory(Player player)
        {
            Items = new List<InventoryItem>();
            Player = player;
        }

        public void AddItem(int ID, int amount = 1)
        {
            InventoryItem invItem = Items.Find(x => x.ID == ID);

            if (invItem == null)
            {
                Items.Add(new InventoryItem(ID, amount));
            }
            else
            {
                invItem.amount += amount;
            }
        }

        public void AddItem(params InventoryItem[] items)
        {
            foreach (InventoryItem item in items)
            {
                InventoryItem invItem = Items.Find(x => x.ID == item.ID);

                if (invItem == null)
                {
                    Items.Add(new InventoryItem(item.ID, 1));
                }
                else
                {
                    invItem.amount++;
                }
            }
        }

        public void RemoveItem(int ID, int amount = 1)
        {
            InventoryItem invItem = Items.Find(x => x.ID == ID);

            if (invItem == null)
            {
                return;
            }
            else
            {
                if (amount >= invItem.amount)
                {
                    Items.Remove(invItem);
                }
                else
                {
                    invItem.amount -= amount;
                }
            }
        }

        public int UpgradeGear(string gearName)
        {
            gearName = gearName.ToLower();

            bool throwItemNotFound = false;
            int levelRequired = 0;

            if (gearName == "weapon" || gearName == "sword")
            {
                if (Player.Character.Level < (Player.Character.Gear.Sword.ID + 1) * 5)
                {
                    levelRequired = (Player.Character.Gear.Sword.ID + 1) * 5;
                }
                else
                {
                    InventoryItem ruby = Items.Find(x => x.ID == Player.Character.Gear.Sword.ID);

                    if (ruby != null)
                    {
                        RemoveItem(ruby.ID);
                        Player.Character.Gear.Sword = GearSword.Swords[ruby.ID + 1];

                        return ruby.ID + 2;
                    }
                    else
                    {
                        throwItemNotFound = true;
                    }
                }
            }
            else if (gearName == "helmet")
            {
                if (Player.Character.Level < (Player.Character.Gear.Helmet.ID + 1) * 5)
                {
                    levelRequired = (Player.Character.Gear.Helmet.ID + 1) * 5;
                }
                else
                {
                    InventoryItem ruby = Items.Find(x => x.ID == Player.Character.Gear.Helmet.ID);

                    if (ruby != null)
                    {
                        RemoveItem(ruby.ID);
                        Player.Character.Gear.Helmet = GearHelmet.Helmets[ruby.ID + 1];

                        return ruby.ID + 2;
                    }
                    else
                    {
                        throwItemNotFound = true;
                    }
                }
                
            }
            else if (gearName == "body" || gearName == "chestplate" || gearName == "platebody")
            {
                if (Player.Character.Level < (Player.Character.Gear.Chestplate.ID + 1) * 5)
                {
                    levelRequired = (Player.Character.Gear.Chestplate.ID + 1) * 5;
                }
                else
                {
                    InventoryItem ruby = Items.Find(x => x.ID == Player.Character.Gear.Chestplate.ID);

                    if (ruby != null)
                    {
                        RemoveItem(ruby.ID);
                        Player.Character.Gear.Chestplate = GearChestplate.Chestplates[ruby.ID + 1];

                        return ruby.ID + 2;
                    }
                    else
                    {
                        throwItemNotFound = true;
                    }
                }
            }
            else if (gearName == "leggings" || gearName == "platelegs" || gearName == "legs")
            {
                if (Player.Character.Level < (Player.Character.Gear.Leggings.ID + 1) * 5)
                {
                    levelRequired = (Player.Character.Gear.Leggings.ID + 1) * 5;
                }
                else
                {
                    InventoryItem ruby = Items.Find(x => x.ID == Player.Character.Gear.Leggings.ID);

                    if (ruby != null)
                    {
                        RemoveItem(ruby.ID);
                        Player.Character.Gear.Leggings = GearLeggings.Leggings[ruby.ID + 1];

                        return ruby.ID + 2;
                    }
                    else
                    {
                        throwItemNotFound = true;
                    }
                }
            }
            else if (gearName == "boots")
            {
                if (Player.Character.Level < (Player.Character.Gear.Boots.ID + 1) * 5)
                {
                    levelRequired = (Player.Character.Gear.Boots.ID + 1) * 5;
                }
                else
                {
                    InventoryItem ruby = Items.Find(x => x.ID == Player.Character.Gear.Boots.ID);

                    if (ruby != null)
                    {
                        RemoveItem(ruby.ID);
                        Player.Character.Gear.Boots = GearBoots.Boots[ruby.ID + 1];

                        return ruby.ID + 2;
                    }
                    else
                    {
                        throwItemNotFound = true;
                    }
                }
            }
            else if (gearName == "gloves")
            {
                if (Player.Character.Level < (Player.Character.Gear.Gloves.ID + 1) * 5)
                {
                    levelRequired = (Player.Character.Gear.Gloves.ID + 1) * 5;
                }
                else
                {
                    InventoryItem ruby = Items.Find(x => x.ID == Player.Character.Gear.Gloves.ID);

                    if (ruby != null)
                    {
                        RemoveItem(ruby.ID);
                        Player.Character.Gear.Gloves = GearGloves.Gloves[ruby.ID + 1];

                        return ruby.ID + 2;
                    }
                    else
                    {
                        throwItemNotFound = true;
                    }
                }
            }
            if(levelRequired != 0)
            {
                throw new Exception($"You need to achieve atleast a level of **{levelRequired}** to upgrade your **{gearName.Substring(0, 1).ToUpper() + gearName.Remove(0, 1)}**.");
            }
            else if(throwItemNotFound)
            {
                throw new Exception("Required level ruby was not found.");
            }
            else
            {
                throw new Exception($"Gear was not found. Try using names from `{Program.PREFIX}gear`.");
            }
        }
    }
}

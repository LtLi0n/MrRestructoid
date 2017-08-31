using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Items
{
    public class InventoryItem
    {
        public int ID;
        public int amount;

        [JsonIgnore] public Item Item => Item.GetItem(ID);

        [JsonConstructor]
        public InventoryItem(int ID, int amount = 1)
        {
            this.ID = ID;
            this.amount = amount;
        }

        public InventoryItem(Item item, int amount = 1)
        {
            ID = item.ID;
            this.amount = amount;
        }
    }

    public class Item
    {
        public string Name { get; private set; }
        public int ID { get; private set; }
        public bool Usable { get; private set; }
        public int Price { get; private set; }
        public int MinLevel { get; private set; }
        public bool Purchasable { get; private set; }

        public Item(string name, int ID, bool usable, int price, int minLevel = 1, bool purchasable = true)
        {
            Name = name;
            this.ID = ID;
            Usable = usable;
            Price = price;
            MinLevel = minLevel;
            Purchasable = purchasable;
        }

        public static class WoodCutting
        {
            public static readonly Item Oak_Logs = new Item(name: "Oak Logs", ID: 103, usable: true, price: 500);
            public static readonly Item Willow_Logs = new Item(name: "Willow Logs", ID: 104, usable: true, price: 1000);
        }

        public static class Fishing
        {
            public static readonly Item Raw_Shrimp = new Item(name: "Raw Shrimp", ID: 105, usable: true, price: 250);
            public static readonly Item Raw_Trout = new Item(name: "Raw Trout", ID: 106, usable: true, price: 2000);
        }

        public static class Ruby
        {
            public static readonly Item Ruby_Level_1 = new Item(name: "Ruby Level 1", ID: 0, usable: true, price: 50);
            public static readonly Item Ruby_Level_2 = new Item(name: "Ruby Level 2", ID: 1, usable: true, price: 200);
            public static readonly Item Ruby_Level_3 = new Item(name: "Ruby Level 3", ID: 2, usable: true, price: 500);
            public static readonly Item Ruby_Level_4 = new Item(name: "Ruby Level 4", ID: 3, usable: true, price: 1000);
            public static readonly Item Ruby_Level_5 = new Item(name: "Ruby Level 5", ID: 4, usable: true, price: 2000);
            public static readonly Item Ruby_Level_6 = new Item(name: "Ruby Level 6", ID: 5, usable: true, price: 3000);
            public static readonly Item Ruby_Level_7 = new Item(name: "Ruby Level 7", ID: 6, usable: true, price: 5000);
            public static readonly Item Ruby_Level_8 = new Item(name: "Ruby Level 8", ID: 7, usable: true, price: 7500);
            public static readonly Item Ruby_Level_9 = new Item(name: "Ruby Level 9", ID: 8, usable: true, price: 10000);
            public static readonly Item Ruby_Level_10 = new Item(name: "Ruby Level 10", ID: 9, usable: true, price: 15000);
            public static readonly Item Ruby_Level_11 = new Item(name: "Ruby Level 11", ID: 10, usable: true, price: 20000);
            public static readonly Item Ruby_Level_12 = new Item(name: "Ruby Level 12", ID: 11, usable: true, price: 25000);

            public static readonly Item Ruby_Level_13 = new Item(name: "Ruby Level 13", ID: 12, usable: true, price: 30000);
            public static readonly Item Ruby_Level_14 = new Item(name: "Ruby Level 14", ID: 13, usable: true, price: 40000);
            public static readonly Item Ruby_Level_15 = new Item(name: "Ruby Level 15", ID: 14, usable: true, price: 50000);
            public static readonly Item Ruby_Level_16 = new Item(name: "Ruby Level 16", ID: 15, usable: true, price: 60000);
        }

        public static class Emerald
        {
            public static readonly Item Emerald_Level_1 = new Item(name: "Emerald Level 1", ID: 50, usable: true, price: 10000);
            public static readonly Item Emerald_Level_2 = new Item(name: "Emerald Level 2", ID: 51, usable: true, price: 15000);
            public static readonly Item Emerald_Level_3 = new Item(name: "Emerald Level 3", ID: 52, usable: true, price: 30000);
            public static readonly Item Emerald_Level_4 = new Item(name: "Emerald Level 4", ID: 53, usable: true, price: 50000);
        }

        public static class Discontinued
        {
            public static readonly Item Pet_Egg = new Item(name: "Pet Egg", ID: 100, usable: false, price: 10000, purchasable: false);
            public static readonly Item Pet_Food = new Item(name: "Pet Food", ID: 101, usable: false, price: 100, purchasable: false);
            public static readonly Item Rename_Stone = new Item(name: "Rename Stone", ID: 102, usable: false, price: 2000, purchasable: false);
        }


        public static Item[] Items { get; private set; }

        public static Item GetItem(int ID)
        {
            for (int i = 0; i < Items.Length; i++)
            {
                if (Items[i].ID == ID)
                {
                    return Items[i];
                }
            }

            return null;
        }

        public static Task LoadItems()
        {
            Items = new Item[]
            {
                WoodCutting.Oak_Logs, WoodCutting.Willow_Logs,
                Fishing.Raw_Shrimp, Fishing.Raw_Trout,

                Ruby.Ruby_Level_1, Ruby.Ruby_Level_2, Ruby.Ruby_Level_3, Ruby.Ruby_Level_4, Ruby.Ruby_Level_5, Ruby.Ruby_Level_6,
                Ruby.Ruby_Level_7, Ruby.Ruby_Level_8, Ruby.Ruby_Level_9, Ruby.Ruby_Level_10, Ruby.Ruby_Level_11, Ruby.Ruby_Level_12,
                Ruby.Ruby_Level_13, Ruby.Ruby_Level_14, Ruby.Ruby_Level_15, Ruby.Ruby_Level_16,

                Emerald.Emerald_Level_1, Emerald.Emerald_Level_2, Emerald.Emerald_Level_3, Emerald.Emerald_Level_4,

                Discontinued.Pet_Egg, Discontinued.Pet_Food, Discontinued.Rename_Stone
            };

            return Task.CompletedTask;
        }
    }
}

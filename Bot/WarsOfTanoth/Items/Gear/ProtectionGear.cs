using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Items.Gear
{
    public abstract class ProtectionGear : IGear
    {
        protected enum ProtGearType { Helmet, Chestplate, Leggings, Gloves, Boots };

        [JsonIgnore] public int Protection { get; set; }

        public ProtectionGear(int protection, int level, int minLevel, string name, int ID)
        {
            Name = name;
            this.ID = ID;
            Protection = protection;
            Level = level;
            MinLevel = minLevel;
        }

        protected ProtectionGear(int ID, ulong TOTAL_XP, ProtGearType type)
        {
            dynamic gear = null;

            switch(type)
            {
                case ProtGearType.Helmet:
                    gear = GearHelmet.Helmets[ID];
                    break;

                case ProtGearType.Chestplate:
                    gear = GearChestplate.Chestplates[ID];
                    break;

                case ProtGearType.Leggings:
                    gear = GearLeggings.Leggings[ID];
                    break;

                case ProtGearType.Gloves:
                    gear = GearGloves.Gloves[ID];
                    break;

                case ProtGearType.Boots:
                    gear = GearBoots.Boots[ID];
                    break;

                default:
                    throw new Exception("Gear Type was not found.");
            }

            Protection = gear.Protection;
            Name = gear.Name;
            this.ID = ID;
            this.TOTAL_XP = TOTAL_XP;
            MinLevel = gear.MinLevel;
            Level = gear.Level;
        }
    }

    public class GearHelmet : ProtectionGear
    {
        [JsonConstructor]
        public GearHelmet(int ID, ulong TOTAL_XP) : base(ID, TOTAL_XP, ProtGearType.Helmet) { }

        public static readonly GearHelmet Leather_Cap = new GearHelmet(protection: 3, level: 1, minLevel: 1, name: "Leather Cap", ID: 0);
        public static readonly GearHelmet Hard_Leather_Cap = new GearHelmet(protection: 6, level: 5, minLevel: 5, name: "Hard Leather Cap", ID: 1);
        public static readonly GearHelmet Studded_Leather_Cap = new GearHelmet(protection: 9, level: 10, minLevel: 10, name: "Studded Leather Cap", ID: 2);
        public static readonly GearHelmet Spiked_Leather_Cap = new GearHelmet(protection: 12, level: 15, minLevel: 15, name: "Spiked Leather Cap", ID: 3);
        public static readonly GearHelmet Iron_Helmet = new GearHelmet(protection: 15, level: 20, minLevel: 20, name: "Iron Helmet", ID: 4);
        public static readonly GearHelmet Chain_Helmet = new GearHelmet(protection: 18, level: 25, minLevel: 25, name: "Chain Helmet", ID: 5);
        public static readonly GearHelmet Steel_Helmet = new GearHelmet(protection: 21, level: 30, minLevel: 30, name: "Steel Helmet", ID: 6);
        public static readonly GearHelmet Silver_Helmet = new GearHelmet(protection: 24, level: 35, minLevel: 35, name: "Silver Helmet", ID: 7);
        public static readonly GearHelmet Great_Battle_Helmet = new GearHelmet(protection: 27, level: 40, minLevel: 40, name: "Great Battle Helmet", ID: 8);
        public static readonly GearHelmet Dragons_Leather_Cap = new GearHelmet(protection: 30, level: 45, minLevel: 45, name: "Dragons Leather Cap", ID: 9);
        public static readonly GearHelmet Gods_Army_Helmet = new GearHelmet(protection: 33, level: 50, minLevel: 50, name: "Gods Army Helmet", ID: 10);
        public static readonly GearHelmet Sunshine_Helmet = new GearHelmet(protection: 36, level: 55, minLevel: 55, name: "Sunshine Helmet", ID: 11);
        public static readonly GearHelmet Burning_Helmet = new GearHelmet(protection: 39, level: 60, minLevel: 60, name: "Burning Helmet", ID: 12);
        //New Gear
        public static readonly GearHelmet Helmet_Of_Fallen_Lands = new GearHelmet(protection: 45, level: 65, minLevel: 65, name: "Helmet Of Fallen Lands", ID: 13);
        public static readonly GearHelmet Helmet_Of_Binding_Kings = new GearHelmet(protection: 51, level: 70, minLevel: 70, name: "Helmet Of Binding Kings", ID: 14);
        public static readonly GearHelmet Obsidian_Helmet_Of_Faded_Visions = new GearHelmet(protection: 57, level: 75, minLevel: 75, name: "Obsidian Helmet Of Faded Visions", ID: 15);
        public static readonly GearHelmet Helmet_Of_Ancient_Honor = new GearHelmet(protection: 63, level: 80, minLevel: 80, name: "Helmet Of Ancient Honor", ID: 16);

        public static GearHelmet[] Helmets { get; private set; }

        public static Task LoadHelmets()
        {
            Helmets = new GearHelmet[]
            {
                Leather_Cap, //0
                Hard_Leather_Cap, //1
                Studded_Leather_Cap, //2
                Spiked_Leather_Cap, //3
                Iron_Helmet, //4
                Chain_Helmet, //5
                Steel_Helmet, //6
                Silver_Helmet, //7
                Great_Battle_Helmet, //8
                Dragons_Leather_Cap, //9
                Gods_Army_Helmet, //10
                Sunshine_Helmet, //11
                Burning_Helmet, //12
                Helmet_Of_Fallen_Lands, //13
                Helmet_Of_Binding_Kings, //14
                Obsidian_Helmet_Of_Faded_Visions, //15
                Helmet_Of_Ancient_Honor //16
            };

            return Task.CompletedTask;
        }

        private GearHelmet(int protection, int level, int minLevel, string name, int ID) : base(protection, level, minLevel, name, ID) { }
        public GearHelmet(GearHelmet helmet) : base(helmet.Protection, helmet.Level, helmet.MinLevel, helmet.Name, helmet.ID) { }
    }
    public class GearChestplate : ProtectionGear
    {
        [JsonConstructor]
        public GearChestplate(int ID, ulong TOTAL_XP) : base(ID, TOTAL_XP, ProtGearType.Chestplate) { }

        public static readonly GearChestplate Leather_Armor = new GearChestplate(protection: 6, level: 1, minLevel: 1, name: "Leather Armor", ID: 0);
        public static readonly GearChestplate Hard_Leather_Armor = new GearChestplate(protection: 12, level: 5, minLevel: 5, name: "Hard Leather Armor", ID: 1);
        public static readonly GearChestplate Studded_Leather_Armor = new GearChestplate(protection: 18, level: 10, minLevel: 10, name: "Studded Leather Armor", ID: 2);
        public static readonly GearChestplate Spiked_Leather_Armor = new GearChestplate(protection: 24, level: 15, minLevel: 15, name: "Spiked Leather Armor", ID: 3);
        public static readonly GearChestplate Chest_Plate = new GearChestplate(protection: 30, level: 20, minLevel: 20, name: "Iron Plate", ID: 4);
        public static readonly GearChestplate Chain_Mail = new GearChestplate(protection: 36, level: 25, minLevel: 25, name: "Chain Mail", ID: 5);
        public static readonly GearChestplate Plate_Mail = new GearChestplate(protection: 42, level: 30, minLevel: 30, name: "Plate Mail", ID: 6);
        public static readonly GearChestplate Silver_Plate = new GearChestplate(protection: 48, level: 35, minLevel: 35, name: "Silver Plate", ID: 7);
        public static readonly GearChestplate Great_Battle_Plate = new GearChestplate(protection: 54, level: 40, minLevel: 40, name: "Great Battle Plate", ID: 8);
        public static readonly GearChestplate Dragons_Leather_Armor = new GearChestplate(protection: 60, level: 45, minLevel: 45, name: "Dragons Leather Armor", ID: 9);
        public static readonly GearChestplate Gods_Army_Loricae = new GearChestplate(protection: 66, level: 50, minLevel: 50, name: "Gods Army Loricae", ID: 10);
        public static readonly GearChestplate Sunshine_Loricae = new GearChestplate(protection: 72, level: 55, minLevel: 55, name: "Sunshine Loricae", ID: 11);
        public static readonly GearChestplate Burning_Cuirass = new GearChestplate(protection: 78, level: 60, minLevel: 60, name: "Burning Cuirass", ID: 12);
        //New Gear
        public static readonly GearChestplate Chestplate_Of_Fallen_Lands = new GearChestplate(protection: 90, level: 65, minLevel: 65, name: "Chestplate Of Fallen Lands", ID: 13);
        public static readonly GearChestplate Chestplate_Of_Binding_Kings = new GearChestplate(protection: 102, level: 70, minLevel: 70, name: "Chestplate Of Binding Kings", ID: 14);
        public static readonly GearChestplate Obsidian_Cuirass_Of_Faded_Visions = new GearChestplate(protection: 114, level: 75, minLevel: 75, name: "Obsidian Cuirass Of Faded Visions", ID: 15);
        public static readonly GearChestplate Chestplace_Of_Ancient_Honor = new GearChestplate(protection: 126, level: 80, minLevel: 80, name: "Chestplate Of Ancient Honor", ID: 16);

        public static GearChestplate[] Chestplates { get; private set; }

        public static Task LoadChestplates()
        {
            Chestplates = new GearChestplate[]
            {
                Leather_Armor, //0
                Hard_Leather_Armor, //1
                Studded_Leather_Armor, //2
                Spiked_Leather_Armor, //3
                Chest_Plate, //4
                Chain_Mail, //5
                Plate_Mail, //6
                Silver_Plate, //7
                Great_Battle_Plate, //8
                Dragons_Leather_Armor, //9
                Gods_Army_Loricae, //10
                Sunshine_Loricae, //11
                Burning_Cuirass, //12
                Chestplate_Of_Fallen_Lands, //13
                Chestplate_Of_Binding_Kings, //14
                Obsidian_Cuirass_Of_Faded_Visions, //15
                Chestplace_Of_Ancient_Honor //16
            };

            return Task.CompletedTask;
        }

        private GearChestplate(int protection, int level, int minLevel, string name, int ID) : base(protection, level, minLevel, name, ID) { }
        public GearChestplate(GearChestplate chestplate) : base(chestplate.Protection, chestplate.Level, chestplate.MinLevel, chestplate.Name, chestplate.ID) { }
    }
    public class GearLeggings : ProtectionGear
    {
        [JsonConstructor]
        public GearLeggings(int ID, ulong TOTAL_XP) : base(ID, TOTAL_XP, ProtGearType.Leggings) { }

        public static readonly GearLeggings Leather_Leggings = new GearLeggings(protection: 3, level: 1, minLevel: 1, name: "Leather Leggings", ID: 0);
        public static readonly GearLeggings Hard_Leather_Leggings = new GearLeggings(protection: 6, level: 5, minLevel: 5, name: "Hard Leather Leggings", ID: 1);
        public static readonly GearLeggings Studded_Leather_Leggings = new GearLeggings(protection: 9, level: 10, minLevel: 10, name: "Studded Leather Leggings", ID: 2);
        public static readonly GearLeggings Spiked_Leather_Leggings = new GearLeggings(protection: 12, level: 15, minLevel: 15, name: "Spiked Leather Leggings", ID: 3);
        public static readonly GearLeggings Iron_Leggingst = new GearLeggings(protection: 15, level: 20, minLevel: 20, name: "Iron Leggings", ID: 4);
        public static readonly GearLeggings Chain_Leggings = new GearLeggings(protection: 18, level: 25, minLevel: 25, name: "Chain Leggings", ID: 5);
        public static readonly GearLeggings Plate_Leggings = new GearLeggings(protection: 21, level: 30, minLevel: 30, name: "Plate Leggings", ID: 6);
        public static readonly GearLeggings Silver_Leggings = new GearLeggings(protection: 24, level: 35, minLevel: 35, name: "Silver Leggings", ID: 7);
        public static readonly GearLeggings Great_Battle_Leggings = new GearLeggings(protection: 27, level: 40, minLevel: 40, name: "Great Battle Leggings", ID: 8);
        public static readonly GearLeggings Dragons_Leather_Leggings = new GearLeggings(protection: 30, level: 45, minLevel: 45, name: "Dragons Leather Leggings", ID: 9);
        public static readonly GearLeggings Gods_Army_Leggings = new GearLeggings(protection: 33, level: 50, minLevel: 50, name: "Gods Army Leggings", ID: 10);
        public static readonly GearLeggings Sunshine_Leggings = new GearLeggings(protection: 36, level: 55, minLevel: 55, name: "Sunshine Leggings", ID: 11);
        public static readonly GearLeggings Burning_Leggings = new GearLeggings(protection: 39, level: 60, minLevel: 60, name: "Burning Leggings", ID: 12);
        //New Gear
        public static readonly GearLeggings Leggings_Of_Fallen_Lands = new GearLeggings(protection: 45, level: 65, minLevel: 65, name: "Leggings Of Fallen Lands", ID: 13);
        public static readonly GearLeggings Leggings_Of_Binding_Kings = new GearLeggings(protection: 51, level: 70, minLevel: 70, name: "Leggings Of Binding Kings", ID: 14);
        public static readonly GearLeggings Obsidian_Leggings_Of_Faded_Visions = new GearLeggings(protection: 57, level: 75, minLevel: 75, name: "Obsidian Leggings Of Faded Visions", ID: 15);
        public static readonly GearLeggings Leggings_Of_Ancient_Honor = new GearLeggings(protection: 63, level: 80, minLevel: 80, name: "Leggings Of Ancient Honor", ID: 16);

        public static GearLeggings[] Leggings { get; private set; }

        public static Task LoadLeggings()
        {
            Leggings = new GearLeggings[]
            {
                Leather_Leggings, //0
                Hard_Leather_Leggings, //1
                Studded_Leather_Leggings, //2
                Spiked_Leather_Leggings, //3
                Iron_Leggingst, //4
                Chain_Leggings, //5
                Plate_Leggings, //6
                Silver_Leggings, //7
                Great_Battle_Leggings, //8
                Dragons_Leather_Leggings, //9
                Gods_Army_Leggings, //10
                Sunshine_Leggings, //11
                Burning_Leggings, //12
                Leggings_Of_Fallen_Lands, //13
                Leggings_Of_Binding_Kings, //14
                Obsidian_Leggings_Of_Faded_Visions, //15
                Leggings_Of_Ancient_Honor //16
            };

            return Task.CompletedTask;
        }

        private GearLeggings(int protection, int level, int minLevel, string name, int ID) : base(protection, level, minLevel, name, ID) { }
        public GearLeggings(GearLeggings leggings) : base(leggings.Protection, leggings.Level, leggings.MinLevel, leggings.Name, leggings.ID) { }
    }
    public class GearGloves : ProtectionGear
    {
        [JsonConstructor]
        public GearGloves(int ID, ulong TOTAL_XP) : base(ID, TOTAL_XP, ProtGearType.Gloves) { }

        public static readonly GearGloves Leather_Gloves = new GearGloves(protection: 3, level: 1, minLevel: 1, name: "Leather Gloves", ID: 0);
        public static readonly GearGloves Hard_Leather_Gloves = new GearGloves(protection: 6, level: 5, minLevel: 5, name: "Hard Leather Gloves", ID: 1);
        public static readonly GearGloves Studded_Leather_Gloves = new GearGloves(protection: 9, level: 10, minLevel: 10, name: "Studded Leather Gloves", ID: 2);
        public static readonly GearGloves Spiked_Leather_Gloves = new GearGloves(protection: 12, level: 15, minLevel: 15, name: "Spiked Leather Gloves", ID: 3);
        public static readonly GearGloves Iron_Gloves = new GearGloves(protection: 15, level: 20, minLevel: 20, name: "Iron Gloves", ID: 4);
        public static readonly GearGloves Chain_Gloves = new GearGloves(protection: 18, level: 25, minLevel: 25, name: "Chain Gloves", ID: 5);
        public static readonly GearGloves Steel_Gloves = new GearGloves(protection: 21, level: 30, minLevel: 30, name: "Steel Gloves", ID: 6);
        public static readonly GearGloves Silver_Gloves = new GearGloves(protection: 24, level: 35, minLevel: 35, name: "Silver Gloves", ID: 7);
        public static readonly GearGloves Great_Battle_Gloves = new GearGloves(protection: 27, level: 40, minLevel: 40, name: "Great Battle Gloves", ID: 8);
        public static readonly GearGloves Dragons_Leather_Gloves = new GearGloves(protection: 30, level: 45, minLevel: 45, name: "Dragons Leather Gloves", ID: 9);
        public static readonly GearGloves Gods_Army_Gloves = new GearGloves(protection: 33, level: 50, minLevel: 50, name: "Gods Army Gloves", ID: 10);
        public static readonly GearGloves Sunshine_Gloves = new GearGloves(protection: 36, level: 55, minLevel: 55, name: "Sunshine Gloves", ID: 11);
        public static readonly GearGloves Burning_Gloves = new GearGloves(protection: 39, level: 60, minLevel: 60, name: "Burning Gloves", ID: 12);
        //New Gear
        public static readonly GearGloves Gloves_Of_Fallen_Lands = new GearGloves(protection: 45, level: 65, minLevel: 65, name: "Gloves Of Fallen Lands", ID: 13);
        public static readonly GearGloves Gloves_Of_Binding_Kings = new GearGloves(protection: 51, level: 70, minLevel: 70, name: "Gloves Of Binding Kings", ID: 14);
        public static readonly GearGloves Obsidian_Gloves_Of_Faded_Visions = new GearGloves(protection: 57, level: 75, minLevel: 75, name: "Obsidian Gloves Of Faded Visions", ID: 15);
        public static readonly GearGloves Gloves_Of_Ancient_Honor = new GearGloves(protection: 63, level: 80, minLevel: 80, name: "Gloves Of Ancient Honor", ID: 16);

        public static GearGloves[] Gloves { get; private set; }

        public static Task LoadGloves()
        {
            Gloves = new GearGloves[]
            {
                Leather_Gloves, //0
                Hard_Leather_Gloves, //1
                Studded_Leather_Gloves, //2
                Spiked_Leather_Gloves, //3
                Iron_Gloves, //4
                Chain_Gloves, //5
                Steel_Gloves, //6
                Silver_Gloves, //7
                Great_Battle_Gloves, //8
                Dragons_Leather_Gloves, //9
                Gods_Army_Gloves, //10
                Sunshine_Gloves, //11
                Burning_Gloves, //12
                Gloves_Of_Fallen_Lands, //13
                Gloves_Of_Binding_Kings, //14
                Obsidian_Gloves_Of_Faded_Visions, //15
                Gloves_Of_Ancient_Honor //16
            };

            return Task.CompletedTask;
        }

        private GearGloves(int protection, int level, int minLevel, string name, int ID) : base(protection, level, minLevel, name, ID) { }
        public GearGloves(GearGloves gloves) : base(gloves.Protection, gloves.Level, gloves.MinLevel, gloves.Name, gloves.ID) { }
    }
    public class GearBoots : ProtectionGear
    {
        [JsonConstructor]
        public GearBoots(int ID, ulong TOTAL_XP) : base(ID, TOTAL_XP, ProtGearType.Boots) { }

        public static readonly GearBoots Leather_Boots = new GearBoots(protection: 3, level: 1, minLevel: 1, name: "Leather Boots", ID: 0);
        public static readonly GearBoots Hard_Leather_Boots = new GearBoots(protection: 6, level: 5, minLevel: 5, name: "Hard Leather Boots", ID: 1);
        public static readonly GearBoots Studded_Leather_Boots = new GearBoots(protection: 9, level: 10, minLevel: 10, name: "Studded Leather Boots", ID: 2);
        public static readonly GearBoots Spiked_Leather_Boots = new GearBoots(protection: 12, level: 15, minLevel: 15, name: "Spiked Leather Boots", ID: 3);
        public static readonly GearBoots Iron_Boots = new GearBoots(protection: 15, level: 20, minLevel: 20, name: "Iron Boots", ID: 4);
        public static readonly GearBoots Chain_Boots = new GearBoots(protection: 18, level: 25, minLevel: 25, name: "Chain Boots", ID: 5);
        public static readonly GearBoots Steel_Boots = new GearBoots(protection: 21, level: 30, minLevel: 30, name: "Steel Boots", ID: 6);
        public static readonly GearBoots Silver_Boots = new GearBoots(protection: 24, level: 35, minLevel: 35, name: "Silver Boots", ID: 7);
        public static readonly GearBoots Great_Battle_Boots = new GearBoots(protection: 27, level: 40, minLevel: 40, name: "Great Battle Boots", ID: 8);
        public static readonly GearBoots Dragons_Leather_Boots = new GearBoots(protection: 30, level: 45, minLevel: 45, name: "Dragons Leather Boots", ID: 9);
        public static readonly GearBoots Gods_Army_Boots = new GearBoots(protection: 33, level: 50, minLevel: 50, name: "Gods Army Boots", ID: 10);
        public static readonly GearBoots Sunshine_Boots = new GearBoots(protection: 36, level: 55, minLevel: 55, name: "Sunshine Boots", ID: 11);
        public static readonly GearBoots Burning_Boots = new GearBoots(protection: 39, level: 60, minLevel: 60, name: "Burning Boots", ID: 12);
        //New Gear
        public static readonly GearBoots Boots_Of_Fallen_Lands = new GearBoots(protection: 45, level: 65, minLevel: 65, name: "Boots Of Fallen Lands", ID: 13);
        public static readonly GearBoots Boots_Of_Binding_Kings = new GearBoots(protection: 51, level: 70, minLevel: 70, name: "Boots Of Binding Kings", ID: 14);
        public static readonly GearBoots Obsidian_Boots_Of_Faded_Visions = new GearBoots(protection: 57, level: 75, minLevel: 75, name: "Obsidian Boots Of Faded Visions", ID: 15);
        public static readonly GearBoots Boots_Of_Ancient_Honor = new GearBoots(protection: 63, level: 80, minLevel: 80, name: "Boots Of Ancient Honor", ID: 16);

        public static GearBoots[] Boots { get; private set; }

        public static Task LoadBoots()
        {
            Boots = new GearBoots[]
            {
                Leather_Boots, //0
                Hard_Leather_Boots, //1
                Studded_Leather_Boots, //2
                Spiked_Leather_Boots, //3
                Iron_Boots, //4
                Chain_Boots, //5
                Steel_Boots, //6
                Silver_Boots, //7
                Great_Battle_Boots, //8
                Dragons_Leather_Boots, //9
                Gods_Army_Boots, //10
                Sunshine_Boots, //11
                Burning_Boots, //12
                Boots_Of_Fallen_Lands, //13
                Boots_Of_Binding_Kings, //14
                Obsidian_Boots_Of_Faded_Visions, //15
                Boots_Of_Ancient_Honor //16
            };

            return Task.CompletedTask;
        }

        private GearBoots(int protection, int level, int minLevel, string name, int ID) : base(protection, level, minLevel, name, ID) { }
        public GearBoots(GearBoots boots) : base(boots.Protection, boots.Level, boots.MinLevel, boots.Name, boots.ID) { }
    }
}

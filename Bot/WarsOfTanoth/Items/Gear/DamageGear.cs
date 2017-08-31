using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Items.Gear
{
    public abstract class DamageGear : IGear
    {
        [JsonIgnore] public int Damage { get; set; }

        public DamageGear(int damage, int level, int minLevel, string name, int ID)
        {
            Damage = damage;
            Level = level;
            Name = name;
            this.ID = ID;
            MinLevel = minLevel;
            TOTAL_XP = 0;
        }

        public DamageGear() { }
    }

    public class GearSword : DamageGear
    {
        public static readonly GearSword Short_Sword = new GearSword(damage: 2, level: 1, minLevel: 1, name: "Short Sword", ID: 0);
        public static readonly GearSword Scimitar = new GearSword(damage: 5, level: 5, minLevel: 5, name: "Scimitar", ID: 1);
        public static readonly GearSword Bronze_Sword = new GearSword(damage: 10, level: 10, minLevel: 10, name: "Bronze Sword", ID: 2);
        public static readonly GearSword Silver_Sword = new GearSword(damage: 20, level: 15, minLevel: 15, name: "Silver Sword", ID: 3);
        public static readonly GearSword Glaive = new GearSword(damage: 30, level: 20, minLevel: 20, name: "Glaive", ID: 4);
        public static readonly GearSword Sacrificial_Short_Sword = new GearSword(damage: 40, level: 25, minLevel: 25, name: "Sacrificial Short Sword", ID: 5);
        public static readonly GearSword Crystal_Sword = new GearSword(damage: 50, level: 30, minLevel: 30, name: "Crystal Sword", ID: 6);
        public static readonly GearSword Kryss = new GearSword(damage: 60, level: 20, minLevel: 35, name: "Kryss", ID: 7);
        public static readonly GearSword Cursed_Sword = new GearSword(damage: 70, level: 40, minLevel: 40, name: "Cursed Sword", ID: 8);
        public static readonly GearSword Sword_Of_Ice = new GearSword(damage: 80, level: 45, minLevel: 45, name: "Sword of Ice", ID: 9);
        public static readonly GearSword Mantis_Sword = new GearSword(damage: 90, level: 50, minLevel: 50, name: "Mantis Sword", ID: 10);
        public static readonly GearSword Lost_Sword = new GearSword(damage: 100, level: 55, minLevel: 55, name: "Lost Sword", ID: 11);
        public static readonly GearSword Burning_Sword = new GearSword(damage: 110, level: 60, minLevel: 60, name: "Burning Sword", ID: 12);
        //New Gear
        public static readonly GearSword Deathraze = new GearSword(damage: 130, level: 65, minLevel: 65, name: "Deathraze", ID: 13);
        public static readonly GearSword Nightbane = new GearSword(damage: 150, level: 70, minLevel: 70, name: "Nightbane", ID: 14);
        public static readonly GearSword Hollow_Greatsword = new GearSword(damage: 170, level: 75, minLevel: 75, name: "Hollow Greatsword", ID: 15);
        public static readonly GearSword Hellreaver = new GearSword(damage: 200, level: 80, minLevel: 80, name: "Hellreaver", ID: 16);

        public static GearSword[] Swords { get; set; }

        public static Task LoadSwords()
        {
            Swords = new GearSword[] 
            {
                Short_Sword, //0
                Scimitar, //1
                Bronze_Sword, //2
                Silver_Sword, //3
                Glaive, //4
                Sacrificial_Short_Sword, //5
                Crystal_Sword, //6
                Kryss, //7
                Cursed_Sword, //8
                Sword_Of_Ice, //9
                Mantis_Sword, //10
                Lost_Sword, //11
                Burning_Sword, //12
                Deathraze, //13
                Nightbane, //14
                Hollow_Greatsword, //15
                Hellreaver //16
            };

            return Task.CompletedTask;
        }

        [JsonConstructor]
        public GearSword(int ID, ulong TOTAL_XP)
        {
            GearSword sword = Swords[ID];

            sword.TOTAL_XP = TOTAL_XP;

            Damage = sword.Damage;
            Level = sword.Level;
            MinLevel = sword.MinLevel;
            Name = sword.Name;
            this.ID = ID;
        }

        private GearSword(int damage, int level, int minLevel, string name, int ID) : base(damage, level, minLevel, name, ID) { }
        public GearSword(GearSword sword) : base(sword.Damage, sword.Level, sword.MinLevel, sword.Name, sword.ID) { }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Entity.Creatures
{
    public class Monster
    {
        public int Health { get; set; }
        public int Damage { get; set; }
        public int Level { get; set; }
        public string Name { get; set; }
        public int ID { get; set; }

        public static Monster[] Monsters { get; private set; }

        public static readonly Monster Rat = new Monster(health: 5, damage: 2, level: 1, name: "Rat", ID: 0);
        public static readonly Monster Goblin = new Monster(health: 10, damage: 5, level: 3, name: "Goblin", ID: 1);
        public static readonly Monster Wolf = new Monster(health: 24, damage: 8, level: 5, name: "Wolf", ID: 2);
        public static readonly Monster Wolf_Leader = new Monster(health: 50, damage: 14, level: 9, name: "Wolf Leader", ID: 3);
        public static readonly Monster Minotaur_Warrior = new Monster(health: 50, damage: 24, level: 17, name: "Minotaur Warrior", ID: 4);
        public static readonly Monster Elf_Warrior = new Monster(health: 80, damage: 30, level: 18, name: "Elf Warrior", ID: 5);
        public static readonly Monster Elf_Elite = new Monster(health: 125, damage: 38, level: 25, name: "Elf Elite", ID: 6);
        public static readonly Monster Assassin = new Monster(health: 40, damage: 19, level: 10, name: "Assassin", ID: 7);
        public static readonly Monster Minotaur_Worker = new Monster(health: 50, damage: 15, level: 13, name: "Minotaur Worker", ID: 8);
        public static readonly Monster Goblin_Warrior = new Monster(health: 15, damage: 8, level: 3, name: "Goblin Warrior", ID: 9);
        public static readonly Monster Rock_Troll = new Monster(health: 200, damage: 35, level: 30, name: "Rock Troll", ID: 10);
        public static readonly Monster Scorpion_King = new Monster(health: 150, damage: 50, level: 30, name: "Scorpion King", ID: 11);
        public static readonly Monster Scorpion = new Monster(health: 130, damage: 40, level: 27, name: "Scorpion", ID: 12);
        public static readonly Monster Spider = new Monster(health: 164, damage: 45, level: 30, name: "Spider", ID: 13);
        public static readonly Monster Spider_Queen = new Monster(health: 290, damage: 55, level: 40, name: "Spider Queen", ID: 14);
        public static readonly Monster Skeletal_Ranger = new Monster(health: 300, damage: 35, level: 40, name: "Skeletal Ranger", ID: 15);
        public static readonly Monster Skeletal_Warrior = new Monster(health: 330, damage: 55, level: 42, name: "Skeletal Warrior", ID: 16);
        public static readonly Monster Undead_Warrior = new Monster(health: 504, damage: 33, level: 42, name: "Undead Warrior", ID: 17);
        public static readonly Monster Dwarf = new Monster(health: 200, damage: 30, level: 25, name: "Dwarf", ID: 18);
        public static readonly Monster Felguard = new Monster(health: 403, damage: 53, level: 47, name: "Felguard", ID: 19);

        public static Task LoadMonsters()
        {
            Monsters = new Monster[]
            {
                Rat, //0
                Goblin, //1
                Wolf, //2
                Wolf_Leader, //3
                Minotaur_Warrior, //4
                Elf_Warrior, //5
                Elf_Elite, //6
                Assassin, //7
                Minotaur_Warrior, //8
                Goblin_Warrior, //9
                Rock_Troll, //10
                Scorpion_King, //11
                Scorpion, //12
                Spider, //13
                Spider_Queen, //14
                Skeletal_Ranger, //15
                Skeletal_Warrior, //16
                Undead_Warrior, //17
                Dwarf, //18
                Felguard //19
            };

            return Task.CompletedTask;
        }

        private Monster(int health, int damage, int level, string name, int ID)
        {
            Health = health;
            Damage = damage;
            Level = level;
            Name = name;
            this.ID = ID;
        }

        private Monster() { }

        public static Monster GetMonster(int ID)
        {
            if(ID < Monsters.Length)
            {
                return Monsters[ID];
            }
            else
            {
                Console.WriteLine($"monster [{ID}] was not found.");
                return null;
            }
        }
    }
}

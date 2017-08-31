using MrRestructoid.Bot.WarsOfTanoth.Entity.Creatures;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Adventure
{
    public class QuestReport
    {
        public Player Player { get; private set; }
        public string QuestName { get; private set; }
        public bool SendQuestCompleted { get; private set; }
        public bool QuestFailed { get; private set; }
        public int TotalProtection { get; private set; }
        public int TotalProtectionMax { get; private set; }
        public int ObtainedXp { get; private set; }
        public int ObtainedGold { get; private set; }
        public int LostHealthOnTrip { get; private set; }

        public QuestReport (Player player, string questName, bool sendQuestCompleted, bool questFailed, int totalProtection, int totalProtectionMax, int obtainedXp, int obtainedGold, int lostHealthOnTrip)
        {
            Player = player;
            QuestName = questName;
            SendQuestCompleted = sendQuestCompleted;
            QuestFailed = questFailed;
            TotalProtection = totalProtection;
            TotalProtectionMax = totalProtectionMax;
            ObtainedXp = obtainedXp;
            ObtainedGold = obtainedGold;
            LostHealthOnTrip = lostHealthOnTrip;
        }
    }

    public class Quest
    {
        public string Name { get; set; }
        public int ID { get; set; }
        public int Duration { get; set; }
        public Monster[] monsters { get; set; }
        public int QuestDifficulty { get; private set; }

        public static Quest[] Quests { get; private set; }

        public static readonly Quest Lonely_In_The_Sewers = new Quest("Lonely In The Sewers", 10, new Monster[] { Monster.Rat, Monster.Rat, Monster.Rat }, 0);
        public static readonly Quest Rescue_Of_A_Victim = new Quest("Rescue Of A Victim", 10, new Monster[] { Monster.Rat, Monster.Goblin, Monster.Goblin }, 1);
        public static readonly Quest Attacking_The_Village_Of_Goblins = new Quest("Attacking The Village Of Goblins", 10, new Monster[] { Monster.Goblin, Monster.Goblin, Monster.Goblin_Warrior }, 2);
        public static readonly Quest Travelling_Through_The_Forest = new Quest("Travelling Through The Forest", 15, new Monster[] { Monster.Wolf, Monster.Wolf }, 3);
        public static readonly Quest Hunt_Of_The_Leader = new Quest("Hunt Of The Leader", 15, new Monster[] { Monster.Wolf, Monster.Wolf, Monster.Wolf_Leader }, 4);
        public static readonly Quest Dealing_With_Assassins = new Quest("Dealing With Assassins", 15, new Monster[] { Monster.Assassin, Monster.Assassin, Monster.Assassin }, 5);
        public static readonly Quest Defending_A_Small_Village = new Quest("Defending A Small Village", 15, new Monster[] { Monster.Wolf, Monster.Wolf, Monster.Minotaur_Warrior, Monster.Minotaur_Warrior }, 6);
        public static readonly Quest Stolen_Mines = new Quest("Stolen Mines", 15, new Monster[] { Monster.Minotaur_Worker, Monster.Minotaur_Worker, Monster.Minotaur_Warrior, Monster.Minotaur_Warrior }, 7);
        public static readonly Quest Looking_For_Trouble = new Quest("Looking For Trouble", 15, new Monster[] { Monster.Elf_Warrior, Monster.Elf_Warrior, Monster.Elf_Warrior }, 8);
        public static readonly Quest Damn_You_Elves = new Quest("Damn You Elves", 15, new Monster[] { Monster.Elf_Warrior, Monster.Elf_Warrior, Monster.Elf_Warrior, Monster.Elf_Elite }, 9);
        public static readonly Quest Through_The_Mountains = new Quest("Through The Mountains", 20, new Monster[] { Monster.Rock_Troll, Monster.Rock_Troll }, 10);
        public static readonly Quest The_Desert = new Quest("The Desert", 20, new Monster[] { Monster.Scorpion, Monster.Scorpion, Monster.Scorpion_King }, 11);
        public static readonly Quest The_Nest_Of_Spiders = new Quest("The Nest Of Spiders", 20, new Monster[] { Monster.Spider, Monster.Spider, Monster.Spider, Monster.Spider }, 12);
        public static readonly Quest Putting_The_Queen_To_Rest = new Quest("Putting The Queen To Rest", 20, new Monster[] { Monster.Spider, Monster.Spider, Monster.Spider, Monster.Spider_Queen }, 13);
        public static readonly Quest Broken_Bones = new Quest("Broken Bones", 20, new Monster[] { Monster.Skeletal_Ranger, Monster.Skeletal_Ranger, Monster.Skeletal_Ranger, Monster.Skeletal_Warrior }, 14);
        public static readonly Quest Tiny_But_Mighty = new Quest("Tiny But Mighty", 20, new Monster[] { Monster.Dwarf, Monster.Dwarf, Monster.Dwarf, Monster.Dwarf, Monster.Dwarf, Monster.Dwarf, Monster.Dwarf, Monster.Dwarf, Monster.Dwarf, Monster.Dwarf }, 15);
        public static readonly Quest The_Graveyard = new Quest("The Graveyard", 20, new Monster[] { Monster.Skeletal_Warrior, Monster.Skeletal_Warrior, Monster.Undead_Warrior, Monster.Undead_Warrior }, 16);
        public static readonly Quest The_Legion = new Quest("The Legion", 20, new Monster[] { Monster.Felguard, Monster.Felguard, Monster.Felguard, Monster.Felguard }, 17);

        public static Task LoadQuests()
        {
            Quests = new Quest[]
            {
                Lonely_In_The_Sewers, //0
                Rescue_Of_A_Victim, //1
                Attacking_The_Village_Of_Goblins, //2
                Travelling_Through_The_Forest, //3
                Hunt_Of_The_Leader, //4
                Dealing_With_Assassins, //5
                Defending_A_Small_Village, //6
                Stolen_Mines, //7
                Looking_For_Trouble, //8
                Damn_You_Elves, //9
                Through_The_Mountains, //10
                The_Desert, //11
                The_Nest_Of_Spiders, //12
                Putting_The_Queen_To_Rest, //13
                Broken_Bones, //14
                Tiny_But_Mighty, //15
                The_Graveyard, //16
                The_Legion //17
            };

            return Task.CompletedTask;
        }

        private Quest(string name, int duration, Monster[] monsters, int ID)
        {
            Name = name;
            this.ID = ID;
            Duration = duration;
            this.monsters = monsters;

            foreach (Monster m in monsters)
            {
                QuestDifficulty += m.Health * m.Damage;
            }

            QuestDifficulty = QuestDifficulty / 10;
        }

        public static Quest getQuest(Handler handler, int ID)
        {
            if(ID < Quests.Length && ID >= 0)
            {
                return Quests[ID];
            }
            else
            {
                Console.WriteLine($"quest [{ID}] was not found.");
                return null;
            }
        }

        public static string[] getVariousMonsters(Quest quest)
        {
            List<string> variousMonsters = new List<string>();

            foreach (Monster m in quest.monsters)
            {
                bool exists = false;

                foreach (string mName in variousMonsters)
                {
                    if (m.Name == mName) exists = true;
                }

                if (!exists) variousMonsters.Add(m.Name);
            }
            return variousMonsters.ToArray();
        }

        public static async Task OnCompleted(Player player, Quest quest, Handler handler)
        {
            Random random = new Random();

            int totalProtection = player.Character.Protection;
            int totalProtectionMax = totalProtection;

            int player_damage = player.Character.Gear.Sword.Damage + player.Character.Level + player.Character.Gear.Sword.EnchantedLevel;

            //if (player.pet != null) player_damage += (int)((Math.Sqrt(player.pet.level) + (Math.Sqrt(10) * player.pet.level / 3)));

            bool questFailed = false;

            int obtainedGold = 0, obtainedXp = 0;

            int lostHealthOnTrip = 0;
            bool sendQuestCompleted = false;

            player.Character.Stats.Quests.questCooldown = 0;
            sendQuestCompleted = true;

            bool playerTurn = true;
            bool firstEncounter = true;

            foreach (Monster m in quest.monsters)
            {
                firstEncounter = true;

                int mobHp = m.Health;

                while (mobHp > 0)
                {
                    //RANDOM FIRST HIT
                    //50/50 PLAYER OR MOB
                    if (firstEncounter)
                    {
                        int value = random.Next(2);
                        if (value == 0) playerTurn = false;
                        else if (value == 1) playerTurn = true;

                        firstEncounter = false;
                    }

                    if (playerTurn || mobHp - player_damage <= 0) mobHp -= player_damage;
                    else playerTurn = true;

                    //IF MOB HAS HP LEFT, ATTACK THE PLAYER
                    if (mobHp > 0)
                    {
                        if (totalProtection <= 0)
                        {
                            if (player.Character.HP > 0 && player.Character.HP - m.Damage > 0)
                            {
                                player.Character.HP -= m.Damage;
                                lostHealthOnTrip += m.Damage;
                            }
                            else if (player.Character.HP > 0 && player.Character.HP - m.Damage <= 0)
                            {
                                player.Character.HP = 0;
                                lostHealthOnTrip += player.Character.HP;
                                questFailed = true;
                                sendQuestCompleted = true;
                            }
                            else if (player.Character.HP <= 0)
                            {
                                questFailed = true;
                                sendQuestCompleted = true;
                            }
                        }
                        else
                        {
                            if (totalProtection - m.Damage < 0)
                            {
                                int margin = Math.Abs(totalProtection - m.Damage);
                                if (player.Character.HP - margin <= 0)
                                {
                                    lostHealthOnTrip += player.Character.HP;
                                    questFailed = true;
                                    sendQuestCompleted = true;
                                    player.Character.HP = 0;
                                }
                                else
                                {
                                    player.Character.HP -= margin;
                                    lostHealthOnTrip += margin;
                                }
                                totalProtection = 0;
                            }
                            else totalProtection -= m.Damage;
                        }

                        if (player.Character.HP == 0) player.Character.Stats.HP.dead = true;
                    }
                    //IF MOB DEFEATED, REWARD THE PLAYER
                    else
                    {
                        if (!questFailed)
                        {
                            int tempGold = random.Next((m.Level + 1) * 3);
                            player.Character.Gold += tempGold;
                            obtainedGold += tempGold;

                            if (player.Character.Titles.Exists(x => x.name == ITitle.Titles.Supporter.name))
                            {
                                player.Character.TOTAL_XP += (m.Damage * m.Health) * 12 / 10;
                                obtainedXp += (m.Damage * m.Health) * 12 / 10;
                            }
                            else
                            {
                                player.Character.TOTAL_XP += m.Damage * m.Health;
                                obtainedXp += m.Damage * m.Health;
                            }

                            player.Character.Stats.Quests.monstersKilled++;
                            break;
                        }
                    }
                }
                playerTurn = false;
            }

            QuestReport report = new QuestReport(player, quest.Name, sendQuestCompleted, questFailed, totalProtection, totalProtectionMax, obtainedXp, obtainedGold, lostHealthOnTrip);

            if (player.Character.Stats.Quests.questsUnlocked == quest.ID + 1 && !questFailed)
            {
                if (Quests.Length > player.Character.Stats.Quests.questsUnlocked)
                {
                    player.Character.Stats.Quests.questsUnlocked++;
                }
            }

            if (!questFailed) player.Character.Stats.Quests.questsCompleted++;
            player.Character.Stats.Quests.isInQuest = false;

            await SendMessage.Send(player, SendType.Quest, report: report);
        }
    }
}

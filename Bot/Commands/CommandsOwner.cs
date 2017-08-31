using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MrRestructoid.Bot.InteractiveMessages;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.Main.Img;
using MrRestructoid.Bot.WarsOfTanoth;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.Items.Gear;
using MrRestructoid.Bot.WarsOfTanoth.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Commands
{
    public class CommandsOwner : ModuleBase
    {
        public static bool Exiting = false;

        [Command("Exit")]
        [RequireOwner()]
        public async Task Exit()
        {
            Exiting = true;

            while (Program.handler.Saving) await Task.Delay(10);

            IUserMessage msg = await Context.Channel.SendMessageAsync("Goodbye!");

            Environment.Exit(0);
        }

        //soon is to be deleted since is not needed anymore
        [Command("LoadOldStructure")]
        [RequireOwner()]
        public async Task LoadStructure_Old()
        {
            string[] files = Directory.GetFiles("data/Wot/old_structure");
            string[] jsons = new string[files.Length];

            for (int i = 0; i < files.Length; i++)
            {
                StreamReader sr = new StreamReader(files[i]);

                jsons[i] = await sr.ReadToEndAsync();
            }

            foreach (string json in jsons)
            {
                OldPlayer.oldPlayers.Add(JsonConvert.DeserializeObject<OldPlayer>(json));
            }

            string toReturn = "";

            Program.handler.Players.Clear();

            foreach (OldPlayer p in OldPlayer.oldPlayers)
            {
                Player np = new Player();

                //UserInfo
                {
                    np.User.UserID = p.discordID;
                    np.User.ServerID = p.serverID;
                    np.User.phoneMode = p.alertsOn;
                    np.User.additionalAlertsOn = p.additionalAlertsOn;
                    np.User.alertsOn = p.alertsOn;
                }

                np.Character.Name = p.playerName;

                np.Character.TOTAL_XP = p.GetTotalXP();
                np.Character.Gold = p.gold;
                if (p.pet != null)
                {
                    np.Character.Gold += (p.pet.ActionsMade * np.Character.Level);
                }
                np.Character.HP = p.level * 10;

                foreach (OldInventory.InventoryItem Iitem in p.inventory.inventory)
                {
                    np.Character.Inventory.AddItem(Iitem.item.ID, Iitem.amount);
                }

                //Gear
                {
                    np.Character.Gear.Boots = GearBoots.Boots[p.boots.ID];
                    np.Character.Gear.Boots.TOTAL_XP = (ulong)p.boots.TotalXP;

                    np.Character.Gear.Chestplate = GearChestplate.Chestplates[p.body.ID];
                    np.Character.Gear.Chestplate.TOTAL_XP = (ulong)p.body.TotalXP;

                    np.Character.Gear.Gloves = GearGloves.Gloves[p.gloves.ID];
                    np.Character.Gear.Gloves.TOTAL_XP = (ulong)p.gloves.TotalXP;

                    np.Character.Gear.Helmet = GearHelmet.Helmets[p.helmet.ID];
                    np.Character.Gear.Helmet.TOTAL_XP = (ulong)p.helmet.TotalXP;

                    np.Character.Gear.Leggings = GearLeggings.Leggings[p.leggings.ID];
                    np.Character.Gear.Leggings.TOTAL_XP = (ulong)p.leggings.TotalXP;

                    np.Character.Gear.Sword = GearSword.Swords[p.sword.ID];
                    np.Character.Gear.Sword.TOTAL_XP = (ulong)p.sword.TotalXP;
                }

                //Stats
                {
                    //PvP
                    {
                        np.Character.Stats.PvP.bountyKills = p.bountyKills;
                        np.Character.Stats.PvP.bountyTitleTimer = p.bountyTitleTimer;
                        np.Character.Stats.PvP.pvpCooldown = p.pvpCooldown;
                        np.Character.Stats.PvP.reasonlessKills = p.reasonlessKills;
                        np.Character.Stats.PvP.reasonlessKillTimer = p.reasonlessKillTimer;
                    }
                    //Quests
                    {
                        np.Character.Stats.Quests.monstersKilled = p.monstersKilled;
                        np.Character.Stats.Quests.questsCompleted = p.tripsCompleted;
                        np.Character.Stats.Quests.questsUnlocked = p.tripsUnlocked;
                    }
                    // Work
                    {
                        np.Character.Stats.Work.totalWork = p.totalWork;
                    }
                }


                Program.handler.Players.Add(np);
            }

            Player me = Program.handler.Players.Find(x => x.User.UserID == Program.ME);
        }

        [Command("AddTitle")]
        [RequireOwner()]
        public async Task AddTitle(int ID, [Remainder]string player)
        {
            Player p = await Program.handler.GetPlayer(Context, player);
            if (player == null) return;

            if (p.Character.Titles == null) p.Character.Titles = new List<ITitle>();

            if (!p.Character.Titles.Exists(x => x.ID == ID))
            {
                p.Character.Titles.Add(ITitle.TitleCollection[ID]);
                await Context.Channel.SendMessageAsync($"Added **{ITitle.TitleCollection[ID].name}** Title to **{p.Character.Name}'s** Collection");
            }
            else
            {
                await Context.Channel.SendMessageAsync($"Player already has this title.");
            }

        }

        [Command("PlayersWithTitle")]
        [RequireOwner()]
        public async Task PlayersWithTitle(int ID)
        {
            string toReturn = "";

            foreach (Player p in Program.handler.Players)
            {
                if (p.Character.Titles != null)
                {
                    if (p.Character.Titles.Exists(X => X.ID == ITitle.TitleCollection[ID].ID))
                    {
                        toReturn += p.Character.Name + '\n';
                    }
                }
            }

            await Context.Channel.SendMessageAsync(toReturn);
        }

        [Command("say")]
        [RequireOwner()]
        public async Task Say([Remainder]string text)
        {
            await Context.Message.DeleteAsync();

            await Context.Channel.SendMessageAsync(text);
        }

        [Command("Unicode")]
        [RequireOwner()]
        public async Task Unicode(string input)
        {
            byte[] ba = Encoding.Unicode.GetBytes(input);

            string hexBackwards = BitConverter.ToString(ba);

            string[] hexArr = hexBackwards.Split('-');

            string result = "";

            for (int i = hexArr.Length - 1; i >= 0; i--)
            {
                result += hexArr[i];
            }

            IUserMessage msg = await Context.Channel.SendMessageAsync(result);
        }

        [Command("SkipTicks")]
        [Alias("SkipTick")]
        [RequireOwner()]
        public async Task SkipTicks(int amount, string type = null)
        {
            Player p = await Program.handler.GetPlayer(Context);

            if (type == null) type = "handler";

            type = type.ToLower();

            IUserMessage msg = await Context.Channel.SendMessageAsync($"skipping {type} `{amount}` ticks...");

            if (type == "handler")
            {
                for (int i = 0; i < amount; i++)
                {
                    await Program.handler.Update();
                }
            }
            else if (type == "skilling_fields")
            {
                for (int i = 0; i < amount; i++)
                {
                    foreach (Professions.Field sField in Program.handler.Professions_Tier1.Fields)
                    {
                        await sField.GetRewards(true, p);
                    }
                }
            }

            await msg.ModifyAsync((x) =>
            {
                x.Content = $"Skipped `{amount}` ticks";
            });
        }

        [Group("CMD")]
        [RequireOwner()]
        public class CMD : ModuleBase
        {
            [Command("save-all")]
            public async Task SaveAll()
            {
                while (Program.handler.Saving) await Task.Delay(10);

                await Program.handler.SavePlayers();
                await Context.Channel.SendMessageAsync($"Saved all players to `{Handler.path_players}/players_saved.json`");
            }

            [Command("Reacted")]
            public async Task ReactedPlayers()
            {
                string toReturn = "";

                foreach (Professions.Field f in Program.handler.Professions_Tier1.Fields)
                {
                    toReturn += f.targetEmoji + "\n";

                    foreach (Player p in f.ReactedPlayers)
                    {
                        toReturn += p.Character.Name + "\n";
                    }
                }

                await Context.Channel.SendMessageAsync(toReturn);
            }

            [Command("RemoveCooldown")]
            public async Task RemoveCooldown(string name, string cooldown)
            {
                Player player = await Program.handler.GetPlayer(Context, name);
                if (player == null) return;

                switch (cooldown)
                {
                    case "pvp":
                        player.Character.Stats.PvP.pvpCooldown = 0;
                        await Context.Channel.SendMessageAsync($"PvP cooldown was cleared from **{player.Character.Name}**");
                        break;
                    case "quest":
                        player.Character.Stats.Quests.questCooldown = 5;
                        await Context.Channel.SendMessageAsync($"Quest cooldown was cleared from **{player.Character.Name}**");
                        break;
                }
            }

            [Command("SetHP")]
            public async Task SetHP(string name, string amount)
            {
                Player player = await Program.handler.GetPlayer(Context, name);
                if (player == null) return;

                if (amount.ToLower() == "max")
                {
                    player.Character.HP = player.Character.MAX_HP;
                    if (player.Character.Stats.HP.dead)
                    {
                        player.Character.Stats.HP.dead = false;
                        player.Character.Stats.HP.deathCooldown = 0;
                    }
                }
                else
                {
                    if (Library.IsDigit(amount))
                    {
                        player.Character.HP = int.Parse(amount);

                        if (player.Character.HP > player.Character.MAX_HP)
                        {
                            player.Character.HP = player.Character.MAX_HP;
                        }
                        else if (player.Character.HP <= 0)
                        {
                            player.Character.Stats.HP.dead = true;
                            player.Character.Stats.HP.deathCooldown = 0;
                            player.Character.HP = 0;
                        }

                        if (player.Character.HP > 0 && player.Character.Stats.HP.dead)
                        {
                            player.Character.Stats.HP.dead = false;
                            player.Character.Stats.HP.deathCooldown = 0;
                        }
                    }
                }

                await Context.Channel.SendMessageAsync($"Hp set to `{amount}` for **{name}**");
            }

            [Command("AddXp")]
            public async Task AddXp(string name, long amount, string skill = null)
            {
                Player player = await Program.handler.GetPlayer(Context, name);
                if (player == null) return;

                if (skill != null)
                {
                    bool pass = true;

                    switch (skill.ToLower())
                    {
                        case "mining":
                            player.Character.Skills.Mining.TOTAL_XP += (int)amount;
                            break;

                        case "woodcutting":
                            player.Character.Skills.WoodCutting.TOTAL_XP += (int)amount;
                            break;

                        case "fishing":
                            player.Character.Skills.Fishing.TOTAL_XP += (int)amount;
                            break;

                        default: pass = false; break;
                    }

                    if (pass)
                    {
                        await Context.Channel.SendMessageAsync($"Added `{amount}` {skill} xp to **{name}**");
                    }
                }
                else
                {
                    player.Character.TOTAL_XP += amount;

                    await Context.Channel.SendMessageAsync($"Added `{amount}` xp to **{name}**");
                }
            }

            [Command("AddGold")]
            public async Task AddGold(string name, long amount)
            {
                Player player = await Program.handler.GetPlayer(Context, name);
                if (player == null) return;

                player.Character.Gold += amount;

                await Context.Channel.SendMessageAsync($"Added `{amount}` gold to **{name}**");
            }

            [Command("ChangeGear")]
            public async Task ChangeGear(string name, string type, int ID)
            {
                Player player = await Program.handler.GetPlayer(Context, name);
                if (player == null) return;

                ID--;

                if (ID > (Player.LevelCap / 5) || ID < 0)
                {
                    await Context.Channel.SendMessageAsync("ID out of range.");
                }
                else
                {
                    type = type.ToLower();

                    string trueType = "";

                    if (type == "weapon" || type == "sword")
                    {
                        player.Character.Gear.Sword = GearSword.Swords[ID];
                        trueType = "Sword";
                    }
                    else if (type == "helmet")
                    {
                        player.Character.Gear.Helmet = GearHelmet.Helmets[ID];
                        trueType = "Helmet";
                    }
                    else if (type == "chestplate")
                    {
                        player.Character.Gear.Chestplate = GearChestplate.Chestplates[ID];
                        trueType = "Chestplate";
                    }
                    else if (type == "leggings")
                    {
                        player.Character.Gear.Leggings = GearLeggings.Leggings[ID];
                        trueType = "Leggings";
                    }
                    else if (type == "boots")
                    {
                        player.Character.Gear.Boots = GearBoots.Boots[ID];
                        trueType = "Boots";
                    }
                    else if (type == "gloves")
                    {
                        player.Character.Gear.Gloves = GearGloves.Gloves[ID];
                        trueType = "Gloves";
                    }
                    else
                    {
                        return;
                    }

                    await Context.Channel.SendMessageAsync($"Changed **{trueType}** for **{name}** to tier `{ID + 1}`");
                }
            }
        }
    }
}
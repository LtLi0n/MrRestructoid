using Discord;
using Discord.Commands;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.WarsOfTanoth.Adventure;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Commands.Tanoth
{
    public class Info : ModuleBase
    {
        [Command("Skills")]
        public async Task Skills(string action = null, [Remainder]string target = null)
        {
            Player targetP = null;

            if (action != null)
            {
                if(action.ToLower() == "comparewith")
                {
                    if(target != null)
                    {
                        targetP = await Program.handler.GetPlayer(Context, target);
                    }
                }
            }

            Player player = await Program.handler.GetPlayer(Context);
            if (player == null) return;

            EmbedBuilder eb = new EmbedBuilder();

            eb.WithAuthor((x) =>
            {
                x.IconUrl = Context.User.GetAvatarUrl();
                x.Name = $"Skill Stats For {player.Character.Name.First().ToString().ToUpper() + player.Character.Name.Substring(1)}";

                if(targetP != null)
                {
                    x.Name += $", {targetP.Character.Name.First().ToString().ToUpper() + targetP.Character.Name.Substring(1)}";
                }
            });

            //MINING
            eb.AddField((x) =>
            {
                x.Name = ":pick: **Mining**";
                x.Value = $"```swift\n{player.Character.Skills.Mining.Level} [{player.Character.Skills.Mining.XP}/{player.Character.Skills.Mining.XpCap}]```";
                
                if(targetP != null) x.IsInline = true;
            });

            if (targetP != null)
            {
                eb.AddField((x) =>
                {
                    x.Name = ":pick: **Mining**";
                    x.Value = $"```swift\n{targetP.Character.Skills.Mining.Level} [{targetP.Character.Skills.Mining.XP}/{targetP.Character.Skills.Mining.XpCap}]```";

                    x.IsInline = true;
                });

                eb.AddField((x =>
                {
                    x.Name = "\u200b";
                    x.Value = "\u200b";
                    x.IsInline = true;
                }));
            }

            //WOODCUTTING
            eb.AddField((x) =>
            {
                x.Name = ":evergreen_tree: **WoodCutting**";
                x.Value = $"```swift\n{player.Character.Skills.WoodCutting.Level} [{player.Character.Skills.WoodCutting.XP}/{player.Character.Skills.WoodCutting.XpCap}]```";

                if (targetP != null) x.IsInline = true;
            });
            if (targetP != null)
            {
                eb.AddField((x) =>
                {
                    x.Name = ":evergreen_tree: **WoodCutting**";
                    x.Value = $"```swift\n{targetP.Character.Skills.WoodCutting.Level} [{targetP.Character.Skills.WoodCutting.XP}/{targetP.Character.Skills.WoodCutting.XpCap}]```";

                    x.IsInline = true;
                });

                eb.AddField((x =>
                {
                    x.Name = "\u200b";
                    x.Value = "\u200b";
                    x.IsInline = true;
                }));
            }

            //FISHING
            eb.AddField((x) =>
            {
                x.Name = ":fishing_pole_and_fish: **Fishing**";
                x.Value = $"```swift\n{player.Character.Skills.Fishing.Level} [{player.Character.Skills.Fishing.XP}/{player.Character.Skills.Fishing.XpCap}]```";

                if (targetP != null) x.IsInline = true;
            });
            if (targetP != null)
            {
                eb.AddField((x) =>
                {
                    x.Name = ":fishing_pole_and_fish: **Fishing**";
                    x.Value = $"```swift\n{targetP.Character.Skills.Fishing.Level} [{targetP.Character.Skills.Fishing.XP}/{targetP.Character.Skills.Fishing.XpCap}]```";

                    x.IsInline = true;
                });

                eb.AddField((x =>
                {
                    x.Name = "\u200b";
                    x.Value = "\u200b";
                    x.IsInline = true;
                }));
            }

            await Context.Channel.SendMessageAsync("", embed: eb.Build());
        }

        [Command("Inventory")]
        [Alias("inv")]
        public async Task Inventory()
        {
            Player player = await Program.handler.GetPlayer(Context);

            if (player == null) return;

            EmbedBuilder eb = new EmbedBuilder();

            if(player.Character.Inventory.Items.Count == 0)
            {
                await Context.User.SendMessageAsync("*Empty*");
            }
            else
            {
                foreach(InventoryItem it in player.Character.Inventory.Items)
                {
                    eb.AddField((x) =>
                    {
                        x.Name = Item.GetItem(it.ID).Name;
                        x.Value = $"**{it.amount}x**   *worth {(it.Item.Price * it.amount) / 2}*";
                        x.IsInline = true;
                    });

                    eb.WithFooter((x) =>
                    {
                        x.Text = "Worth price is ((Shop Price * Amount) / 2)";
                    });

                }

                await Context.User.SendMessageAsync("", embed: eb.Build());
            }
        }

        /// <summary>
        /// --------------------[ Player Display ]--------------------
        /// </summary>
        [Command("Profile"), Summary("p(rofile)")]
        [Alias("p", "Character")]
        public async Task DisplayPlayer(string target = null)
        {
            Player player = null;
            bool targetting = false;

            if (target != null)
            {
                player = await Program.handler.GetPlayer(Context, target);
                targetting = true;
            }
            else
            {
                player = await Program.handler.GetPlayer(Context);
            }

            if (player == null) return;

            await DisplayPlayer(player, Context, targetting);
        }

        public static async Task DisplayPlayer(Player player, ICommandContext Context, bool inspect = false)
        {
            Quest quest = Quest.getQuest(Program.handler, player.Character.Stats.Quests.questID);
            string name = "";

            int player_damage = player.Character.Gear.Sword.Damage + player.Character.Level + player.Character.Gear.Sword.EnchantedLevel;

            string xpString = string.Format("{0:n0}", player.Character.XP);
            string xpCapString = string.Format("{0:n0}", player.Character.XP_CAP);

            string playerName = player.Character.Name;
            playerName = playerName.First().ToString().ToUpper() + playerName.Substring(1);

            string rankString = "";
            rankString = $"[ #{Program.handler.Players.Count - Program.handler.Players.IndexOf(player)} / {Program.handler.Players.Count} ]";
            if (player.Character.ActiveTitle == null) name += $"{playerName}  {rankString}\n";
            else name += $">>>> ~{player.Character.ActiveTitle.name}~ <<<<\n{playerName}  {rankString}\n";

            string toReturn = $"{name}|+| HP:[ {player.Character.HP}/{player.Character.MAX_HP} ]  Prot:<{{ {player.Character.Protection} }}>  Dmg:<{{ {player_damage} }}>";

            if (player.Character.Level == Player.LevelCap) toReturn += $"\n|+| Level: {player.Character.Level} ( {xpString} )";
            else toReturn += $"\n|+| Level: {player.Character.Level} ( {xpString} / {xpCapString} )";

            toReturn += $"\n|+| Gold: {player.Character.Gold}\n|+| Quests Unlocked: {player.Character.Stats.Quests.questsUnlocked}\n";

            if (player.Character.Stats.Work.isWorking)
            {
                string workLeft = Work.WorkString(player, false);
                toReturn += $"\nCurrently Working. {workLeft}";
            }
            else if (player.Character.Stats.Quests.isInQuest)
            {
                if (quest.Duration - player.Character.Stats.Quests.minutesInQuest > 1) toReturn += $"\nCurrently On The Quest: {quest.Name} - {quest.Duration - player.Character.Stats.Quests.minutesInQuest} Minutes Left";
                else toReturn += $"\nCurrently On The Quest: {quest.Name} - 1 Minute left";
            }

            if (player.Character.Stats.HP.dead && inspect) toReturn += $"\nThis Character Is Dead: {60 - player.Character.Stats.HP.deathCooldown} More Minutes To Recover";
            else if (player.Character.Stats.HP.dead && !inspect) toReturn += $"\nYour Character Is Dead: {60 - player.Character.Stats.HP.deathCooldown} More Minutes To Recover";

            else if (!player.Character.Stats.Quests.isInQuest && player.Character.Stats.Quests.questCooldown < 5)
            {
                if (5 - player.Character.Stats.Quests.questCooldown > 1) toReturn += $"\nQuest Cooldown: {5 - player.Character.Stats.Quests.questCooldown} Minutes Left";
                else toReturn += $"\nQuest Cooldown: 1 Minute Left";
            }

            if (player.Character.Stats.PvP.reasonlessKillTimer > 0) toReturn += $"\nOn Bounty. {Library.GetTimeString(player.Character.Stats.PvP.reasonlessKillTimer % 60, player.Character.Stats.PvP.reasonlessKillTimer / 60)}";

            toReturn += "```";

            if (player.Character.Stats.HP.dead) toReturn += ":skull:";

            await Context.Channel.SendMessageAsync($"```swift\n{toReturn}");
            return;
        }
        /// <summary>
        /// --------------------[ Title ]--------------------
        /// </summary>
        [Command("Title"), Summary("Select/Remove [title]), List")]
        public async Task Title(string action, [Remainder] string chosenTitle = null)
        {
            Player player = await Program.handler.GetPlayer(Context);
            if (player == null) return;

            if (action.ToLower() == "list")
            {
                if (player.Character.Titles == null)
                {
                    await Context.Channel.SendMessageAsync("*Your title list is empty...*"); return;
                }
                if (player.Character.Titles.Count == 0)
                {
                    await Context.Channel.SendMessageAsync("*Your title list is empty...*"); return;
                }

                string toReturn = $"Hello {player.Character.Name}, Here is the list of your Titles:\n";
                bool newSection = false;

                for (int i = 0; i < player.Character.Titles.Count; i++)
                {
                    if (i > 0)
                    {
                        if (player.Character.Titles[i].titleType != player.Character.Titles[i - 1].titleType) newSection = true;
                        else newSection = false;
                    }
                    else if (i == 0) newSection = true;

                    if (newSection)
                    {
                        if (player.Character.Titles[i].titleType == TitleType.MonsterKills) toReturn += $"\n Monster Kills Titles:\n";
                        else toReturn += $"\n {Enum.GetName(typeof(TitleType), player.Character.Titles[i].titleType)} Titles:\n";
                    }

                    toReturn += $"-> [{player.Character.Titles[i].name}]  -  {player.Character.Titles[i].reason}\n";
                }

                await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync($"```Ini\n{toReturn}```");
            }

            else if (action.ToLower() == "select" || action.ToLower() == "set")
            {
                string title = "";

                if (string.IsNullOrEmpty(chosenTitle))
                {
                    await Context.Channel.SendMessageAsync("**Title you want to select can not be null.**"); return;
                }

                title = chosenTitle;

                if (player.Character.Titles.Exists(x => x.name.ToLower() == title.ToLower()))
                {
                    ITitle trueTitle = player.Character.Titles.Find(x => x.name.ToLower() == title.ToLower());

                    player.Character.ActiveTitle = trueTitle;

                    await Context.Channel.SendMessageAsync($":ballot_box_with_check: **Title -** `{trueTitle.name}` **- selected** :ballot_box_with_check: ");
                }
                else
                {
                    await Context.Channel.SendMessageAsync($":bangbang: **Title -** `{title}` **- was not found in your list. Type \"+title list\" to see what titles you have** :bangbang:");
                }
            }

            else if (action.ToLower() == "remove")
            {

                if (player.Character.ActiveTitle == null)
                {
                    await Context.Channel.SendMessageAsync(":exclamation: **No active title has been detected** :exclamation:");
                }
                else if (player.Character.ActiveTitle != null)
                {
                    player.Character.ActiveTitle = null;
                    await Context.Channel.SendMessageAsync(":ballot_box_with_check: **Active title has been removed** :ballot_box_with_check:");
                }
            }
            else
            {
                await Context.Channel.SendMessageAsync($"You are doing something wrong :thinking:\nUsage is: `Select/Remove [title], List`");
            }
            return;
        }

        [Command("CharStats")]
        public async Task Stats()
        {
            Player player = await Program.handler.GetPlayer(Context);
            if (player == null) return;

            EmbedBuilder eb = new EmbedBuilder();

            eb.WithAuthor(x =>
            {
                x.IconUrl = Context.User.GetAvatarUrl();
                x.Name = $"{player.Character.Name} Statistics";
            });

            eb.ThumbnailUrl = Context.User.GetAvatarUrl();

            eb.Color = new Color((int)(((Player.LevelCap / 100.0) * player.Character.Level) * (255 / Player.LevelCap)), 0, 0);

            eb.Description =
                $"```swift\n" +
                $"->| Quests Completed: {player.Character.Stats.Quests.questsCompleted}\n" +
                $"->| Monsters Killed: {player.Character.Stats.Quests.monstersKilled}\n" +
                $"->| Hours Spent Working: {player.Character.Stats.Work.totalWork}\n" +
                $"->| Quests Unlocked: {player.Character.Stats.Quests.questsUnlocked}\n" +
                $"->| Titles Obtained: {player.Character.Titles.Count}\n" +
                $"->| Player Killed: {player.Character.Stats.PvP.bountyKills + player.Character.Stats.PvP.reasonlessKills}\n" +
                $"->| Bounties Collected: {player.Character.Stats.PvP.bountyKills}```";

            await Context.User.SendMessageAsync("", embed: eb.Build());
        }

        [Group("info")]
        public class InfoCommands : ModuleBase
        {
            [Command("quest"), Summary("info q(uest) [ID]")]
            [Alias("q")]
            public async Task InfoQuest(int ID = int.MaxValue)
            {
                Player player = await Program.handler.GetPlayer(Context);
                if (player == null) return;

                int questID = 0;

                if (ID == int.MaxValue)
                {
                    if (player.Character.Stats.Quests.isInQuest) questID = player.Character.Stats.Quests.questID + 1;
                    else { await Context.Channel.SendMessageAsync("You are not in a quest. use ID to check the quest info"); return; }
                }
                else if (ID <= 0) { await Context.Channel.SendMessageAsync("Only positive numbers for IDs"); return; }
                else if (ID > 50) await Context.Channel.SendMessageAsync("Woah, chill out with those numbers!");

                string toReturn = "";

                questID = ID - 1;

                Quest quest = Quest.getQuest(Program.handler, questID);

                if (quest == null)
                {
                    { await Context.Channel.SendMessageAsync("This quest does not yet exist."); return; }
                }

                toReturn += $"Quest: {quest.Name}\n";
                toReturn += $"Difficulty: {quest.QuestDifficulty}\n";
                toReturn += $"Duration: {quest.Duration}\n";
                toReturn += $"expect to find these Monsters: ";

                foreach (string mName in Quest.getVariousMonsters(quest))
                {
                    string name = mName.Replace(" ", "_");
                    toReturn += name + " ";
                }

                await Context.Channel.SendMessageAsync($"```ruby\n{toReturn}```");
            }
        }
    }
}

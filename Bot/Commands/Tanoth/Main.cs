using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MrRestructoid.Bot.InteractiveMessages;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.WarsOfTanoth;
using MrRestructoid.Bot.WarsOfTanoth.Adventure;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Commands.Tanoth
{
    public class Main : ModuleBase
    {
        [Command("Shop")]
        public async Task Shop()
        {
            Player player = await Program.handler.GetPlayer(Context);
            if(player == null) return;

            IUserMessage message = await Context.Channel.SendMessageAsync(ShopIMessage.ShopText.Text1);

            Program.IMessages.Add(new ShopIMessage(Context.User, Context.Channel, message, player));
        }

        [Command("Quest"), Summary("+q(uest) [ID]")]
        [Alias("q")]
        public async Task ChooseQuest(string action = null)
        {
            Player player = await Program.handler.GetPlayer(Context);
            if (player == null) return;

            if (player.Character.Stats.Work.isWorking)
            {
                string workLeft = Work.WorkString(player, true);
                await Context.Channel.SendMessageAsync($"**You can't go on quests while working. **{workLeft}");
            }
            //DEFAULT COMMAND
            else if (action == null)
            {
                //is already in a quest
                if (player.Character.Stats.Quests.isInQuest)
                {
                    int timeLeft = Quest.getQuest(Program.handler, player.Character.Stats.Quests.questID).Duration - player.Character.Stats.Quests.minutesInQuest;

                    await Context.Channel.SendMessageAsync(
                        $"You are in a quest - **{Quest.getQuest(Program.handler, player.Character.Stats.Quests.questID).Name}**. " +
                        $"{string.Format("`{0}`** Minute{1} left.**", timeLeft, timeLeft > 1 ? "s" : string.Empty)}");
                }
                //cooldown is still present
                else if (!player.Character.Stats.Quests.isInQuest && player.Character.Stats.Quests.questCooldown != 5)
                {
                    if (5 - player.Character.Stats.Quests.questCooldown >= 1)
                    {
                        int questRemainingCD = 5 - player.Character.Stats.Quests.questCooldown;

                        await Context.Channel.SendMessageAsync(string.Format("You need to wait **{0}** more minute{1} before going on a quest again.",
                            questRemainingCD,
                            questRemainingCD > 1 ? "s" : string.Empty));
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync($"**Currently you are not in a quest.**");
                }
            }

            else if (player.Character.Stats.Quests.isInQuest && action.ToLower() == "cancel")
            {
                if (player.Character.Stats.Quests.minutesInQuest > Quest.getQuest(Program.handler, player.Character.Stats.Quests.questID).Duration / 2) player.Character.Stats.Quests.questCooldown = 0;

                player.Character.Stats.Quests.isInQuest = false; player.Character.Stats.Quests.minutesInQuest = 0;
                await Context.Channel.SendMessageAsync($":white_check_mark: You have cancelled the quest - **{Quest.getQuest(Program.handler, player.Character.Stats.Quests.questID).Name}**.");
            }
            else if (player.Character.Stats.HP.dead) //DEAD
            {
                await Context.Channel.SendMessageAsync($":skull_crossbones: **You are dead. Wait to revive** :skull_crossbones:");
            }
            else if (!player.Character.Stats.HP.dead && player.Character.HP == 0) //JUST REVIVED
            {
                await Context.Channel.SendMessageAsync($":warning: **You have just revived. You still have 0 hp.** :warning: ");
            }
            else if (!player.Character.Stats.Quests.isInQuest && action.ToLower() == "cancel") //CANCEL WHILE NOT IN A QUEST
            {
                await Context.Channel.SendMessageAsync($"**Currently you are not in the quest.**");
            }
            else if (player.Character.Stats.Quests.isInQuest)
            {
                int questRemainingTime = Quest.getQuest(Program.handler, player.Character.Stats.Quests.questID).Duration - player.Character.Stats.Quests.minutesInQuest;

                await Context.Channel.SendMessageAsync(string.Format("**You are in a quest already. **`{0}` **Minute{1} left.**",
                    questRemainingTime,
                    questRemainingTime > 1 || questRemainingTime == 0 ? "s" : string.Empty));
            }
            else
            {
                if (action.Length < 3 && Library.IsDigit(action))
                {
                    if (player.Character.Stats.Quests.questsUnlocked > int.Parse(action) - 1 && Quest.Quests.Length > int.Parse(action) - 1)
                    {
                        int questID = int.Parse(action) - 1;

                        if (questID >= 0)
                        {
                            if (player.Character.Stats.Quests.questCooldown < 5)
                            {
                                if (5 - player.Character.Stats.Quests.questCooldown > 1)
                                {
                                    await Context.Channel.SendMessageAsync($"**You need to wait**`{5 - player.Character.Stats.Quests.questCooldown}`**more minutes before going on a quest again.**");
                                }
                                else
                                {
                                    await Context.Channel.SendMessageAsync($"**You need to wait**`1`**more minute before going on a quest again.**");
                                }
                            }
                            else
                            {
                                player.Character.Stats.Quests.questID = questID;
                                player.Character.Stats.Quests.minutesInQuest = 0;
                                player.Character.Stats.Quests.isInQuest = true;

                                string emblem = ":trident:";

                                if (Quest.getQuest(Program.handler, questID).ID >= 9)
                                {
                                    emblem = ":star:";
                                }

                                await Context.Channel.SendMessageAsync($"{emblem} **quest - **`{Quest.getQuest(Program.handler, questID).Name}` **- selected** {emblem}");

                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("**Trip ID selected is below what you can select.**");
                        }

                    }
                    else
                    {
                        await Context.Channel.SendMessageAsync(":no_pedestrians: **You have not yet gotten here!** :no_pedestrians:");
                    }
                }
                else if (action.Length > 2 && Library.IsDigit(action))
                {
                    await Context.Channel.SendMessageAsync("Woah, chill out with those numbers!");
                }
                else if (!Library.IsDigit(action))
                {
                    await Context.Channel.SendMessageAsync("only numbers are allowed aside from `cancel` action");
                }
            }
            return;
        }
    }
}

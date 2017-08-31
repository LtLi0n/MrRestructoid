using Discord.Commands;
using MrRestructoid.Bot.Main;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Entity.Players
{
    public class Work : ModuleBase
    {
        public static string WorkString(Player player, bool formatting = true)
        {
            int minutes = (((player.Character.Stats.Work.workToMake * 60) - player.Character.Stats.Work.workProgress) % 60);
            string minutesString = minutes.ToString();
            int hours = player.Character.Stats.Work.workToMake - (player.Character.Stats.Work.workProgress / 60);
            if (((player.Character.Stats.Work.workToMake * 60) - player.Character.Stats.Work.workProgress) % 60 != 0) hours--;
            if (minutesString.Length == 1 && hours > 0) minutesString = "0" + minutesString;

            string pickEmoji = "";

            if (!player.User.phoneMode) pickEmoji = ":pick:";

            if (formatting)
            {
                if (hours == 1) return $"{pickEmoji} `{hours}:{minutesString}`** Hour Left.** {pickEmoji} ";
                else if (hours > 1) return $"{pickEmoji} `{hours}:{minutesString}`** Hours Left.** {pickEmoji} ";
                else if (hours == 0 && minutes != 1) return $"{pickEmoji} `{minutesString}`** Minutes Left.** {pickEmoji} ";
                else if (hours == 0 && minutes == 1) return $"{pickEmoji} `{minutesString}`** Minute Left.** {pickEmoji} ";
            }
            else
            {
                if (hours == 1) return $"{hours}:{minutesString} Hour Left.";
                else if (hours > 1) return $"{hours}:{minutesString} Hours Left.";
                else if (hours == 0 && minutes != 1) return $"{minutesString} Minutes Left.";
                else if (hours == 0 && minutes == 1) return $"{minutesString} Minute Left.";
            }

            return null;
        }

        [Command("Work"), Summary("w(ork) [hours]/cancel")]
        [Alias("w")]
        public async Task WorkCommand(string action = null)
        {
            Player player = await Program.handler.GetPlayer(Context);
            if (player == null) return;

            string emoji_skull = "", emoji_warning = "", emoji_trident = "", emoji_pick = "", emoji_construction_worker = "", emoji_no_entry = "";

            if (!player.User.phoneMode)
            {
                emoji_skull = ":skull:";
                emoji_warning = ":warning:";
                emoji_trident = ":trident:";
                emoji_pick = ":pick:";
                emoji_construction_worker = ":construction_worker:";
                emoji_no_entry = ":no_entry:";
            }

            if (player.Character.Stats.HP.dead) await Context.Channel.SendMessageAsync($"{emoji_skull} **You can't work while you are dead ** {emoji_skull}");
            else if (player.Character.HP == 0) await Context.Channel.SendMessageAsync($"{emoji_warning} **You have just revived. You still have 0 hp.** {emoji_warning}");
            else if (player.Character.Stats.Quests.isInQuest) await Context.Channel.SendMessageAsync($"{emoji_trident} **You can't work while you are on the quest** {emoji_trident}");

            else if (string.IsNullOrEmpty(action) && player.Character.Stats.Work.isWorking)
            {
                string workLeft = WorkString(player);

                await Context.Channel.SendMessageAsync(workLeft);

            }
            else if (string.IsNullOrEmpty(action)) await Context.Channel.SendMessageAsync("specify hours or action\n`cancel`/`[hours]`");

            else
            {
                if (action.ToLower() == "cancel")
                {
                    if (!player.Character.Stats.Work.isWorking) await Context.Channel.SendMessageAsync("**You were not working**");
                    else
                    {
                        await Context.Channel.SendMessageAsync("Work cancelled.");
                        player.Character.Stats.Work.isWorking = false;
                        player.Character.Stats.Work.workProgress = 0;
                    }
                }
                else if (Library.IsDigit(action) && action.Length < 3)
                {
                    short workHours = short.Parse(action);

                    if (workHours > 12 || workHours < 1)
                    {
                        await Context.Channel.SendMessageAsync($"{emoji_pick} **wrong work hours selected. Range ( 1 - 12 )** {emoji_pick}");
                    }
                    else if (player.Character.Stats.Work.isWorking)
                    {
                        await Context.Channel.SendMessageAsync($"{emoji_construction_worker} {emoji_pick} **You are already working. Type {Program.PREFIX}work cancel to select other hours** {emoji_pick} {emoji_construction_worker}");
                    }  
                    else
                    {
                        player.Character.Stats.Work.workToMake = workHours;
                        player.Character.Stats.Work.isWorking = true;
                        player.Character.Stats.Work.workProgress = 0;
                        await Context.Channel.SendMessageAsync($"{emoji_pick} **You are now working for {workHours} hours** :pick:");
                    }
                }
                else if (Library.IsDigit(action) && action.Length > 2) await Context.Channel.SendMessageAsync($"{emoji_no_entry} **wrong work hours selected. Range ( 1 - 12 )** {emoji_no_entry}");
                else if (!Library.IsDigit(action)) await Context.Channel.SendMessageAsync($"{emoji_no_entry} **no numbers in the number selection were found** {emoji_no_entry}");
            }
        }
    }
}

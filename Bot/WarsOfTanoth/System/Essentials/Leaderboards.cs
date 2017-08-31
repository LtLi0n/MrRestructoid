using Discord.Commands;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.System.Essentials
{
    public class Leaderboards : ModuleBase
    {
        [Command("leaderboards")]
        [Alias("leaderboard", "top")]
        public async Task Top(string area = "xp", string action = "", string target = "")
        {
            area = area.ToLower();
            action = action.ToLower();
            target = target.ToLower();

            SortType sortType = SortType.Xp;
            Player player = await Program.handler.GetPlayer(Context);
            if (player == null) return;

            int page = 0;
            bool findPlayer = false;
            Player toFind = null;

            int longestName = int.MinValue;

            int focusAtPersonNumber = Program.handler.Players.Count;
            int userToFindPlace = Program.handler.Players.Count;

            Player[] leaderboard = null;

            string toReturn = "";

            if (area != "")
            {
                if (area == "quests" || area == "quest" || area == "q")
                {
                    leaderboard = Program.handler.TopQuests;
                    toReturn += ":crossed_swords: **-[ Total Quests Completed Leaderboard ]-** :crossed_swords:\n";
                    sortType = SortType.Quest;
                }
                else if (area == "work" || area == "w")
                {
                    leaderboard = Program.handler.TopWork;
                    toReturn += ":hammer_pick: **-[ Total Work Hours Leaderboard ]-** :hammer_pick:\n";
                    sortType = SortType.Work;
                }
                else if (area == "xp" || area == "levels")
                {
                    leaderboard = Program.handler.Players.ToArray();
                    toReturn += ":sparkles: **-[ Total XP Leaderboard ]-** :sparkles:\n";
                }
                else
                {
                    await Context.Channel.SendMessageAsync("Only **Work**, **Quest** and **XP** leaderboards exist.");
                    return;
                }
            }

            if (!string.IsNullOrEmpty(action))
            {
                if (action == "find") findPlayer = true;
                else if (Library.IsDigit(action) && action.Length < 3)
                {
                    page = int.Parse(action) - 1;
                    if (page > (float)leaderboard.Length / 10)
                    {
                        await Context.Channel.SendMessageAsync("*empty field*");
                        return;
                    }
                }
                else
                {
                    await Context.Channel.SendMessageAsync("No characters or long numbers in the page selection are allowed.");
                    return;
                }
            }

            if (findPlayer)
            {
                toFind = await Program.handler.GetPlayer(Context, target);
                if (toFind == null)
                {
                    await Context.Channel.SendMessageAsync($"Player - **{target}** - does not exist");
                    return;
                }

                page = (leaderboard.Length - Array.IndexOf(leaderboard, toFind)) / 10;
                userToFindPlace = leaderboard.Length - Array.IndexOf(leaderboard, toFind);

                if (userToFindPlace < 5) focusAtPersonNumber = 0;
                else if (leaderboard.Length - userToFindPlace <= 5) focusAtPersonNumber = userToFindPlace - 11;
                else focusAtPersonNumber = userToFindPlace - 6;
            }
            else
            {
                focusAtPersonNumber = page * 10;
            }

            toReturn += "```css\n";

            //Longest name search
            for (int i = focusAtPersonNumber; i < focusAtPersonNumber + 11; i++)
            {
                if (i > leaderboard.Length - 1) break;

                if (leaderboard[leaderboard.Length - 1 - i].Character.Name.Length > longestName)
                {
                    longestName = leaderboard[leaderboard.Length - 1 - i].Character.Name.Length;
                }
            }

            //show leaderboard page
            for (int i = focusAtPersonNumber; i < focusAtPersonNumber + 11; i++)
            {
                if (i > leaderboard.Length - 1) break;

                string name = leaderboard[leaderboard.Length - 1 - i].Character.Name;

                string arrow = "";

                if (findPlayer)
                {
                    if (name.ToLower() == toFind.Character.Name.ToLower())
                    {
                        arrow = "  <-----";
                    }
                }

                if (!string.IsNullOrEmpty(name))
                {
                    while (name.Length != longestName) { name += " "; }
                }

                if (i >= 9) name = $"{i + 1}. {name}";
                else name = $"{i + 1}.  {name}";

                if (sortType == SortType.Xp)
                {
                    toReturn += $"{name} - {string.Format("{0:n0}", leaderboard[leaderboard.Length - 1 - i].Character.TOTAL_XP)}{arrow}\n";
                }
                else if (sortType == SortType.Work)
                {
                    toReturn += $"{name} - {string.Format("{0:n0}", leaderboard[leaderboard.Length - 1 - i].Character.Stats.Work.totalWork)}{arrow}\n";
                }
                else if (sortType == SortType.Quest)
                {
                    toReturn += $"{name} - {string.Format("{0:n0}", leaderboard[leaderboard.Length - 1 - i].Character.Stats.Quests.questsCompleted)}{arrow}\n";
                }
            }

            await Context.User.GetOrCreateDMChannelAsync().Result.SendMessageAsync($"{toReturn}```");
        }
    }
}

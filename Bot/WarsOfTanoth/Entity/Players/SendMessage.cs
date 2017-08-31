using Discord;
using Discord.WebSocket;
using MrRestructoid.Bot.WarsOfTanoth.Adventure;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Entity.Players
{
    public enum SendType { Work, Report, LevelUp, Quest, Title, QuestCooldown, Message }

    public static class SendMessage
    {
        public static async Task Send(Player player, SendType type, string message = null, int reward = 0, ITitle title = null, QuestReport report = null)
        {
            if (type == SendType.Title)
            {
                if (!player.Character.Titles.Exists(x => x.ID == title.ID))
                {
                    player.Character.Titles.Add(title);
                    await player.SortTitles();
                }
                return;
            }

            if (Program.botType == Program.BotType.Experimental) return;

            bool found = false;
            foreach (SocketGuild guild in Program.client.Guilds)
            {
                if (found) break;
                foreach (IGuildUser u in guild.Users)
                {
                    if (u.Id == player.User.UserID)
                    {
                        found = true;

                        string toReturn = "";

                        switch (type)
                        {
                            case SendType.LevelUp:
                                toReturn = $"```cs\nYou have just leveled up!\n{player.Character.Level - 1}->{player.Character.Level}```";
                                break;
                            case SendType.Work:
                                toReturn = $":pick: **You have completed your work and got**`{reward}`**gold** :pick:";
                                break;
                            case SendType.QuestCooldown:
                                toReturn = $":alarm_clock: **Your Quest cooldown is over** :alarm_clock:";
                                break;
                            case SendType.Quest:

                                if (report.SendQuestCompleted && !report.QuestFailed)
                                {
                                    toReturn = $"```cs\nHello, you have completed your quest - {report.QuestName}\nHp: [ {player.Character.HP}/{player.Character.MAX_HP} ] - {report.LostHealthOnTrip}\nProtection: [ {report.TotalProtection}/{report.TotalProtectionMax}] - {report.TotalProtectionMax - report.TotalProtection}\nGold(+{report.ObtainedGold})\nXp(+{report.ObtainedXp})```";
                                }
                                else if (report.SendQuestCompleted && report.QuestFailed)
                                {
                                    if (report.ObtainedXp > 0)
                                    {
                                        toReturn = $"```cs\nHello, unfortunately your character has died on the quest - {report.QuestName}\nYou still managed to kill some monsters!\nGold(+{report.ObtainedGold})\nXp(+{report.ObtainedXp})```";
                                    }
                                    else
                                    {
                                        toReturn = $"```cs\nHello, unfortunately your character has died on the quest - {report.QuestName}\nStrong advice - Improve your character first or wait for you character's health to replenish fully before going on this quest again.```";
                                    }
                                }
                                break;

                            case SendType.Message:
                                toReturn = message;
                                break;
                        }

                        await u.SendMessageAsync(toReturn);

                        break;
                    }
                }
            }
        }
    }
}

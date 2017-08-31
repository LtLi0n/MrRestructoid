using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrRestructoid.Bot.WarsOfTanoth.System
{
    public enum SortType { Xp, Work, Quest, PvP, Pet }

    public static class SortPlayers
    {
        public static int UpdateTickCounter { get; set; }

        public static void SortAll(Handler handler)
        {
            try { handler.Players = new List<Player>(SortBy(Program.handler.Players.ToArray(), SortType.Xp)); }
            catch { }

            try { handler.TopQuests = SortBy(Program.handler.TopQuests, SortType.Quest); }
            catch { }

            try { handler.TopWork = SortBy(Program.handler.TopWork, SortType.Work); }
            catch { }

            try { handler.BountyPlayers = new List<Player>(SortBy(Program.handler.BountyPlayers.ToArray(), SortType.PvP)); }
            catch { }
        }

        public static Player[] SortBy(Player[] players, SortType sortType)
        {
            if (players.Length == 0) return players;

            int index = 0;
            while (true)
            {
                for (int i = 0; i < players.Length - index; i++)
                {
                    if (i < players.Length - index - 1)
                    {
                        if(sortType == SortType.Xp)
                        {
                            if (players[i].Character.TOTAL_XP > players[i + 1].Character.TOTAL_XP)
                            {
                                Player tempPlayer = players[i + 1];
                                players[i + 1] = players[i];
                                players[i] = tempPlayer;
                            }
                        }
                        else if (sortType == SortType.Work)
                        {
                            if (players[i].Character.Stats.Work.totalWork > players[i + 1].Character.Stats.Work.totalWork)
                            {
                                Player tempPlayer = players[i + 1];
                                players[i + 1] = players[i];
                                players[i] = tempPlayer;
                            }
                        }
                        else if (sortType == SortType.Quest)
                        {
                            if (players[i].Character.Stats.Quests.questsCompleted > players[i + 1].Character.Stats.Quests.questsCompleted)
                            {
                                Player tempPlayer = players[i + 1];
                                players[i + 1] = players[i];
                                players[i] = tempPlayer;
                            }
                        }

                        else if (sortType == SortType.PvP)
                        {
                            if (players[i].Character.Stats.PvP.reasonlessKillTimer > players[i + 1].Character.Stats.PvP.reasonlessKillTimer)
                            {
                                Player tempPlayer = players[i + 1];
                                players[i + 1] = players[i];
                                players[i] = tempPlayer;
                            }
                        }
                    }
                }
                index++;
                if (index == players.Length) break;
            }

            return players;
        }
    }
}

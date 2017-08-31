using Discord.Commands;
using Discord.WebSocket;
using MrRestructoid.Bot.WarsOfTanoth.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Entity.Players
{
    public enum TitleType { Special, Event, Leaderboard, PvP, Quest, Work, MonsterKills }

    public class ITitle
    {
        public const ulong SupporterRoleId = 279890170315014144;

        public static class Titles
        {
            public static readonly ITitle AlphaPlayer = new ITitle("Alpha Player", "For playing the game in alpha-state.", TitleType.Special, 0);
            public static readonly ITitle Creator = new ITitle("Creator", "The creator of the game.", TitleType.Special, 1);

            public static readonly ITitle Worker100 = new ITitle("Novice Miner", "Mine atleast for 100 hours in total.", TitleType.Work, 2);
            public static readonly ITitle Worker500 = new ITitle("Adept Miner", "Mine atleast for 500 hours in total.", TitleType.Work, 3);
            public static readonly ITitle Worker1000 = new ITitle("Master Miner", "Mine atleast for 1000 hours in total.", TitleType.Work, 4);

            public static readonly ITitle Quester250 = new ITitle("Tired Of Those 5 Minutes", "Complete atleast 250 quests in total.", TitleType.Quest, 5);
            public static readonly ITitle Quester1000 = new ITitle("One More Quest...", "Complete atleast 1000 quests in total.", TitleType.Quest, 6);

            public static readonly ITitle Killer1000 = new ITitle("Monsters Aren't Scary", "Kill atleast 1000 monsters in total.", TitleType.MonsterKills, 7);
            public static readonly ITitle Killer5000 = new ITitle("The Witcher", "Kill atleast 5000 monsters in total.", TitleType.MonsterKills, 8);

            public static readonly ITitle Top1 = new ITitle("Tanoth's Champion", "For being the best in the rankings!", TitleType.Leaderboard, 9);
            public static readonly ITitle Top5 = new ITitle("The Vanguard", "For being in the Top 5", TitleType.Leaderboard, 10);

            public static readonly ITitle ParagonOfTanoth = new ITitle("Paragon Of Tanoth", "For stopping the killers.", TitleType.PvP, 11);
            public static readonly ITitle Unstoppable = new ITitle("Unstopable", "No one dares to stop you.", TitleType.PvP, 12);

            public static readonly ITitle Supporter = new ITitle("Supporter", "For becoming a patron.", TitleType.Special, 13);
        }

        public static IReadOnlyList<ITitle> TitleCollection { get; private set; }

        public static Task LoadTitles()
        {
            TitleCollection = new ITitle[]
            {
                Titles.AlphaPlayer,
                Titles.Creator,
                Titles.Worker100,
                Titles.Worker500,
                Titles.Worker1000,
                Titles.Quester250,
                Titles.Quester1000,
                Titles.Killer1000,
                Titles.Killer5000,
                Titles.Top1,
                Titles.Top5,
                Titles.ParagonOfTanoth,
                Titles.Unstoppable,
                Titles.Supporter
            };

            return Task.CompletedTask;
        }

        [JsonIgnore] public string name { get; set; }
        [JsonIgnore] public string reason { get; set; }
        [JsonIgnore] public TitleType titleType { get; set; }
        public int ID { get; private set; }

        [JsonIgnore]
        public ITitle Title => TitleCollection[ID];

        private ITitle(string name, string reason, TitleType titleType, int ID)
        {
            this.name = name;
            this.reason = reason;
            this.titleType = titleType;
            this.ID = ID;
        }

        [JsonConstructor]
        public ITitle(int ID)
        {
            ITitle t = TitleCollection[ID];

            name = t.name;
            reason = t.reason;
            titleType = t.titleType;
            this.ID = t.ID;
        }

        public static void ResetTitles(Handler handler)
        {
            foreach (Player p in handler.Players)
            {
                bool hasAlphaPlayer = false;

                if (p.Character.Titles.Exists(x => x.name == Titles.AlphaPlayer.name)) hasAlphaPlayer = true;

                p.Character.Titles.Clear();

                if (hasAlphaPlayer) p.Character.Titles.Add(Titles.AlphaPlayer);
            }
        }

        public static void RemoveTile(Player player, ITitle title)
        {
            if (player.Character.ActiveTitle != null)
            {
                if (player.Character.ActiveTitle.name.Equals(title.name)) player.Character.ActiveTitle = null;
            }

            for (int i = 0; i < player.Character.Titles.Count; i++)
            {
                if (player.Character.Titles[i].name == title.name)
                {
                    player.Character.Titles.RemoveAt(i);
                    break;
                }
            }
        }

        public static async Task TitleRewarder(Player player, Handler handler)
        {
            if (player.Character.Titles == null) player.Character.Titles = new List<ITitle>();

            //if (player.Character.Name == "Flame") if (!player.Character.Titles.Exists(x => x.name == LtLi0ns_RightHand.name)) { player.Character.Titles.Add(LtLi0ns_RightHand); }
            if (player.Character.Name == "LtLi0n") if (!player.Character.Titles.Exists(x => x.name == Titles.Creator.name)) { player.Character.Titles.Add(Titles.Creator); }

            bool roleFound = false;

            SocketGuildUser user = Program.client.GetGuild(279715709792288775).GetUser(player.User.UserID);

            if (user != null)
            {
                IEnumerable<SocketRole> roles = Program.client.GetGuild(279715709792288775).GetUser(player.User.UserID).Roles;
                if (roles == null) roleFound = false;
                else if (roles.Count() > 0)
                {
                    foreach (SocketRole r in roles)
                    {
                        if (r.Id == SupporterRoleId)
                        {
                            roleFound = true;
                            await SendMessage.Send(player, SendType.Title, title: Titles.Supporter);
                            break;
                        }
                    }
                }
            }

            if (player.Character.Titles.Exists(x => x.name == Titles.Supporter.name) && !roleFound) RemoveTile(player, Titles.Supporter);

            int rank = handler.Players.Count - handler.Players.IndexOf(player);

            if (rank == 1) await SendMessage.Send(player, SendType.Title, title: Titles.Top1);
            if (rank < 6) await SendMessage.Send(player, SendType.Title, title: Titles.Top5);

            if (rank != 1 && player.Character.Titles.Exists(x => x.ID == Titles.Top1.ID)) RemoveTile(player, Titles.Top1);
            if (rank > 5 && player.Character.Titles.Exists(x => x.ID == Titles.Top5.ID)) RemoveTile(player, Titles.Top5);

            if (player.Character.Stats.PvP.reasonlessKillTimer < 14400 && player.Character.Titles.Exists(x => x.ID == Titles.Unstoppable.ID)) RemoveTile(player, Titles.Unstoppable);
            if (player.Character.Stats.PvP.bountyTitleTimer < 1 && player.Character.Titles.Exists(x => x.ID == Titles.ParagonOfTanoth.ID)) RemoveTile(player, Titles.ParagonOfTanoth);

            //PVP TITLES
            if (player.Character.Stats.PvP.bountyTitleTimer > 0) await SendMessage.Send(player, SendType.Title, title: Titles.ParagonOfTanoth);
            if (player.Character.Stats.PvP.reasonlessKillTimer >= 14400) await SendMessage.Send(player, SendType.Title, title: Titles.Unstoppable);

            //KILLER TITLES
            if (player.Character.Stats.Quests.monstersKilled >= 1000) await SendMessage.Send(player, SendType.Title, title: Titles.Killer1000);
            if (player.Character.Stats.Quests.monstersKilled >= 5000) await SendMessage.Send(player, SendType.Title, title: Titles.Killer5000);
            //WORKER TITLES
            if (player.Character.Stats.Work.totalWork >= 100) await SendMessage.Send(player, SendType.Title, title: Titles.Worker100);
            if (player.Character.Stats.Work.totalWork >= 500) await SendMessage.Send(player, SendType.Title, title: Titles.Worker500);
            if (player.Character.Stats.Work.totalWork >= 1000) await SendMessage.Send(player, SendType.Title, title: Titles.Worker1000);
            //QUESTER TITLES
            if (player.Character.Stats.Quests.questsCompleted >= 250) await SendMessage.Send(player, SendType.Title, title: Titles.Quester250);
            if (player.Character.Stats.Quests.questsCompleted >= 1000) await SendMessage.Send(player, SendType.Title, title: Titles.Quester1000);
        }
    }
}

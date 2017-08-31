using Discord;
using Discord.Commands;
using MrRestructoid.Bot.Main;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Commands.Osu
{
    public class Commands : ModuleBase
    {
        [Command("osu")]
        public async Task Osu(string name = null)
        {
            if(name != null)
            {
                await OsuPlayerData(0, name, Context);
            }
        }

        [Command("ctb")]
        public async Task Ctb(string name = null)
        {
            if (name != null)
            {
                await OsuPlayerData(2, name, Context);
            }
        }

        [Command("Taiko")]
        public async Task Taiko(string name = null)
        {
            if (name != null)
            {
                await OsuPlayerData(1, name, Context);
            }
        }

        [Command("Mania")]
        public async Task Mania(string name = null)
        {
            if (name != null)
            {
                await OsuPlayerData(3, name, Context);
            }
        }

        private static async Task OsuPlayerData(int gamemode, string name, ICommandContext Context)
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    client.Timeout = new TimeSpan(0, 0, 10);

                    string data = await client.GetStringAsync($"https://osu.ppy.sh/api/get_user?k={Library.API_KEYS["Osu"]}&u={name}&m={gamemode}").ContinueWith(x =>
                    {
                         return x.Result.Remove(x.Result.Length - 1, 1).Remove(0, 1);
                    });

                    osuPlayerRaw player = JsonConvert.DeserializeObject<osuPlayerRaw>(data);

                    if (player == null)
                    {
                        await Context.Channel.SendMessageAsync($"Didn't find specified user. Are you sure it's **{name}**? :eyes:");
                    }
                    else if (string.IsNullOrEmpty(player.username))
                    {
                        await Context.Channel.SendMessageAsync($"Didn't find specified user. Are you sure it's **{name}**? :eyes:");
                    }
                    else
                    {
                        uint totalHits = 0;

                        if (!string.IsNullOrEmpty(player.count300)) totalHits += uint.Parse(player.count300);
                        if (!string.IsNullOrEmpty(player.count100)) totalHits += uint.Parse(player.count100);
                        if (!string.IsNullOrEmpty(player.count50)) totalHits += uint.Parse(player.count50);

                        OsuLevelConverter levels = new OsuLevelConverter(totalHits, double.Parse(player.pp_raw));

                        double accuracy = Math.Round(double.Parse(player.accuracy), 2);
                        long ranked_score = long.Parse(player.ranked_score);

                        string player_pp_rank = "???";

                        if (!string.IsNullOrEmpty(player.pp_rank)) player_pp_rank = string.Format("{0:n0}", player.pp_rank);

                        string levelInfo = $"{levels.level} [ {levels.percentProgress} % ]";

                        //levelInfo = $"{levels.level} ( { levels.returnXp} / { levels.returnXpCap} )   [ {levels.percentProgress} % ]";

                        EmbedBuilder eb = new EmbedBuilder();

                        eb.WithAuthor((x) =>
                        {
                            x.IconUrl = $"https://a.ppy.sh/{player.user_id}";
                            x.Name = player.username;
                            x.Url = $"https://osu.ppy.sh/u/{player.user_id}";
                        });

                        eb.Description = $"```Swift\n" +
                            $"Accuracy: {accuracy}%\n" +
                            $"Ranked Score: {string.Format("{0:n0}", ulong.Parse(player.ranked_score))}\n" +
                            $"Total Score: {string.Format("{0:n0}", ulong.Parse(player.total_score))}\n" +
                            $"Total Hits: {string.Format("{0:n0}", totalHits)}\n" +
                            $"Level: {levelInfo}```";

                        eb.AddField((x) =>
                        {
                            x.Name = "Ranking:";
                            x.Value = $"**PP**: [ `{player.pp_raw}` ]    {ReturnFlag(player.country)}**#{player.pp_country_rank}**    :globe_with_meridians:**#{player.pp_rank}**";
                        });

                        eb.WithFooter((x) =>
                        {
                            x.Text = "Level shown is custom and advances as you increase your total hits and performance points.";
                        });

                        await Context.Channel.SendMessageAsync("", embed: eb.Build());
                    }
                }
                catch (Exception ex)
                {
                    await Context.Channel.SendMessageAsync(ex.Message);
                }
            } 
        }

        private static string ReturnFlag(string countryCode)
        {
            return $":flag_{countryCode.ToLower()}:";
        }
    }
}

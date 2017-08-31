using Discord;
using Discord.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Main
{
    public class CommandUsage
    {
        public string cmd { get; private set; }
        public string desc { get; private set; }

        public CommandUsage(string cmd, string desc)
        {
            this.cmd = Program.PREFIX + cmd;
            this.desc = $"*{desc}*";
        }
    }

    public static class Library
    {
        public static RequestOptions rOpts = new RequestOptions();

        public static class Emojis
        {
            public static readonly Emoji arrow_left = new Emoji("🔙");
            public static readonly Emoji arrow_backward = new Emoji("◀");
            public static readonly Emoji arrow_forward = new Emoji("▶");
            public static readonly Emoji shopping_cart = new Emoji("🛒");
            public static readonly Emoji moneybag = new Emoji("💰");
            public static readonly Emoji scroll = new Emoji("📜");
            public static readonly Emoji x = new Emoji("❌");
            public static readonly Emoji gem = new Emoji("💎");
            public static readonly Emoji feet = new Emoji("🐾");

            public static readonly Emoji red_circle = new Emoji("🔴");
            public static readonly Emoji sparkles = new Emoji("✨");

            public static readonly Emoji pick = new Emoji("⛏");
            public static readonly Emoji fishing_pole_and_fish = new Emoji("🎣");
            public static readonly Emoji evergreen_tree = new Emoji("🌲");
        }

        public static bool IsDigit(string x)
        {
            foreach (char c in x)
            {
                if (c > '9' || c < '0')
                {
                    return false;
                }
            }

            return true;
        }

        public static string GetTimeString(int minutes, int hours = 0, int days = 0)
        {
            while (hours > 24)
            {
                hours -= 24;
                days++;
            }

            string toReturn = "";

            string minutesString = minutes.ToString();
            if (minutesString.Length == 1 && hours > 0) minutesString = $"0{minutesString}";

            string hoursString = hours.ToString();
            if (hoursString.Length == 1 && days > 0) hoursString = $"0{hoursString}";

            if (hours == 1) toReturn = $"{hours}:{minutesString} Hour Left.";
            else if (hours > 1) toReturn = $"{hours}:{minutesString} Hours Left.";
            else if (hours == 0 && minutes != 1) toReturn = $"{minutesString} Minutes Left.";
            else if (hours == 0 && minutes == 1) toReturn = $"{minutesString} Minute Left.";

            if (days == 1) toReturn = $"{days} Day, {toReturn}";
            else if (days > 1) toReturn = $"{days} Days, {toReturn}";

            return toReturn;
        }

        public static async Task Say_CommandFailed(ICommandContext Context, params CommandUsage[] usages)
        {
            EmbedBuilder eb = new EmbedBuilder();

            eb.Title = "I did not understand you :sweat:\nCorrect usage is:";

            foreach (CommandUsage usage in usages)
            {
                eb.AddField((x) =>
                {
                    x.Name = usage.cmd;
                    x.Value = usage.desc;
                });
            }

            await Context.Channel.SendMessageAsync("", embed: eb.Build());
        }

        public static async Task LoadKeys(string path = "data/Api_Keys")
        {
            API_KEYS = new ApiDictionary();

            string[] files = Directory.GetFiles(path);

            foreach(string file in files)
            {
                FileStream fs = new FileStream(file, FileMode.Open);
                StreamReader sr = new StreamReader(fs);

                string Key = await sr.ReadToEndAsync();
                string Tag = Path.GetFileNameWithoutExtension(file);
                string[] pathChunks = Tag.Split('/');
                Tag = pathChunks[pathChunks.Length - 1];

                API_KEYS.Add(Tag, Key);

                sr.Close();
                fs.Close();
            }
        }

        public static ApiDictionary API_KEYS;

        public sealed class ApiDictionary : Dictionary<string, string>
        {
            public new void Add(string Tag, string Key) => base.Add(Tag, Key);
            public new string this[string Tag] => base[Tag];
        }
    }
}

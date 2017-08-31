using MrRestructoid.Bot.Commands.Runescape.Osrs;
using MrRestructoid.Bot.Main.Img;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Main
{
    class Boot
    {
        private static Task LoadImgTags(BotHandler botHandler)
        {
            string[] guilds = Directory.GetDirectories(ImgTag.ContentPath);

            foreach(string g in guilds)
            {
                ulong GuildID = ulong.Parse(Regex.Match(g, @"\d+").Value);

                string[] imgs = Directory.GetFiles(g);

                foreach(string img in imgs)
                {
                    string FileName = Regex.Match(img, @"\w+\.(png|jpg)").Value;

                    botHandler.ImageTags.Add(new ImgTag(GuildID, FileName));
                }
            }

            return Task.CompletedTask;
        }

        private static Task LoadGE()
        {
            OsrsGEItem.Items = new Dictionary<int, string>();

            FileStream fs = new FileStream("data/osbuddy/items.json", FileMode.Open);
            StreamReader sr = new StreamReader(fs);

            string json = sr.ReadToEnd();
            dynamic objects = JsonConvert.DeserializeObject(json);

            foreach (var item in objects)
            {
                string value = item.First.name;
                int ID = int.Parse(item.Name);

                OsrsGEItem.Items.Add(ID, value);
            }

            return Task.CompletedTask;
        }

        public static async Task Load(BotHandler botHandler)
        {
            CultureInfo.DefaultThreadCurrentCulture = CultureInfo.InvariantCulture;

            await LoadGE();
            await LoadImgTags(botHandler);
        }
    }
}

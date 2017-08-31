using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

using System.Net;
using System.IO;
using System.Net.Http;
using Discord;

namespace MrRestructoid.Bot.Commands.Runescape.Osrs
{
    public class OsrsGEItem
    {
        public const string GePricesURL = "https://rsbuddy.com/exchange/names.json";
        public const string GeItemID = "http://services.runescape.com/m=itemdb_oldschool/api/catalogue/detail.json?item=";

        public Item item { get; private set; }

        [JsonConstructor]
        public OsrsGEItem(Item item)
        {
            this.item = item;
        }

        public static Dictionary<int, string> Items = new Dictionary<int, string>();

        public class Item
        {
            public string icon { get; private set; }
            public string icon_large { get; private set; }
            public int id { get; private set; }
            public string name { get; private set; }
            public string description { get; private set; }
            public Current current { get; private set; }
            public Today today { get; private set; }
            public Change day30 { get; private set; }
            public Change day90 { get; private set; }
            public Change day180 { get; private set; }

            [JsonConstructor]
            public Item(string icon, int id, string icon_large, string name, string description,
                Current current, Today today, Change day30, Change day90, Change day180)
            {
                this.icon = icon;
                this.id = id;
                this.icon_large = icon_large;
                this.name = name;
                this.description = description;
                this.current = current;
                this.today = today;
                this.day30 = day30;
                this.day90 = day90;
                this.day180 = day180;
            }

            public class Current
            {
                public string trend { get; private set; }
                public string price { get; private set; }

                [JsonConstructor]
                public Current(string trend, string price)
                {
                    this.trend = trend;
                    price = price.Replace('.', ',').ToUpper();

                    if (price.Contains("B"))
                    {
                        price = price.Insert(price.IndexOf('B'), " ");
                    }
                    else if (price.Contains("M"))
                    {
                        price = price.Insert(price.IndexOf('M'), " ");
                    }
                    else if (price.Contains("K"))
                    {
                        price = price.Insert(price.IndexOf('K'), " ");
                    }

                    this.price = price;
                }
            }

            public class Today
            {
                public string trend { get; private set; }
                public string price { get; private set; }

                [JsonConstructor]
                public Today(string trend, string price)
                {
                    this.trend = trend;
                    this.price = price;
                }
            }

            public class Change
            {
                public string trend { get; private set; }
                public string change { get; private set; }

                [JsonConstructor]
                public Change(string trend, string change)
                {
                    this.trend = trend;
                    this.change = change;
                }
            }
        }
    }

    public class Commands : ModuleBase
    {
        [Command("Ge")]
        public async Task GrandExchange([Remainder]string item = null)
        {
            bool found = false;
            int ID = 0;

            if(item == null)
            {
                await Context.Channel.SendMessageAsync("Specify an item name please.");
                return;
            }

            foreach(KeyValuePair<int, string> key in OsrsGEItem.Items)
            {
                if(key.Value.ToLower() == item.ToLower())
                {
                    ID = key.Key;
                    found = true;
                }
            }

            if(!found)
            {
                await Context.Channel.SendMessageAsync($"Could not find **{item}**.");
                return;
            }

                
            using (HttpClient client = new HttpClient())
            {
                string json = await client.GetStringAsync(OsrsGEItem.GeItemID + ID);

                OsrsGEItem geItem = JsonConvert.DeserializeObject<OsrsGEItem>(json);

                EmbedBuilder eb = new EmbedBuilder();

                eb.WithAuthor((x) =>
                {
                    x.IconUrl = geItem.item.icon;
                    x.Name = geItem.item.name;
                    x.Url = $"http://services.runescape.com/m=itemdb_oldschool/Dragon_bones/viewitem?obj={ID}";
                });

                eb.ThumbnailUrl = geItem.item.icon_large;

                eb.Description = $"*{geItem.item.description}*\n```swift\nPrice: {geItem.item.current.price}```";

                eb.AddField((x) =>
                {
                    x.Name = "Today";
                    if (geItem.item.today.trend == "negative") x.Value = "📉 ";
                    else if (geItem.item.today.trend == "positive") x.Value = "📈 ";

                    x.Value += $"{geItem.item.today.price.Replace(" ", "")}";

                    x.IsInline = true;
                });

                eb.AddField((x) =>
                {
                    x.Name = "Over The Past 30 Days";
                    if (geItem.item.day30.trend == "negative") x.Value = "📉 ";
                    else if (geItem.item.day30.trend == "positive") x.Value = "📈 ";

                    x.Value += $"{geItem.item.day30.change}";

                    x.IsInline = true;
                });

                eb.AddField((x) =>
                {
                    x.Name = "Over The Past 90 Days";
                    if (geItem.item.day90.trend == "negative") x.Value = "📉 ";
                    else if (geItem.item.day90.trend == "positive") x.Value = "📈 ";

                    x.Value += $"{geItem.item.day90.change}";

                    x.IsInline = true;
                });

                eb.AddField((x) =>
                {
                    x.Name = "Over The Past 180 Days";
                    if (geItem.item.day180.trend == "negative") x.Value = "📉 ";
                    else if (geItem.item.day180.trend == "positive") x.Value = "📈 ";

                    x.Value += $"{geItem.item.day180.change}";

                    x.IsInline = true;
                });

                await Context.Channel.SendMessageAsync("", embed: eb.Build());
            }
        }
    }
}

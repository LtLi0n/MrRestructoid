using Discord.Commands;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Main.Img
{
    public sealed class ImgTag
    {
        public const string ContentPath = "data/Img";

        public ulong GuildID { get; private set; }
        public string Tag { get; private set; }
        public string Extension { get; private set; }

        public string FileName => $"{Tag}{Extension}";

        public FileStream ImageStream => new FileStream($"{ContentPath}/{GuildID}/{Tag}{Extension}", FileMode.Open);

        public ImgTag(ICommandContext Context, string Tag, string url) => ImgTagConstructor(Context.Guild.Id, Tag, url).GetAwaiter().GetResult();

        private async Task ImgTagConstructor(ulong GuildID, string Tag, string url)
        {
            this.GuildID = GuildID;
            this.Tag = Tag;

            if (Path.HasExtension(url))
            {
                string Extension = Path.GetExtension(url);
                if (Extension.Contains("?"))
                {
                    Extension = Extension.Remove(Extension.IndexOf('?'), Extension.Length - Extension.IndexOf('?'));
                }

                if (Extension == ".png" || Extension == ".jpg" || Extension == ".jpeg")
                {
                    this.Extension = Extension;

                    using (HttpClient client = new HttpClient())
                    {
                        //180 KB limit
                        client.MaxResponseContentBufferSize = 184320;

                        byte[] byteArr = await client.GetByteArrayAsync(url);

                        if (!Directory.Exists($"{ContentPath}/{GuildID}"))
                        {
                            Directory.CreateDirectory($"{ContentPath}/{GuildID}");
                        }

                        FileStream fs = new FileStream($"{ContentPath}/{GuildID}/{Tag}{Extension}", FileMode.Create);

                        await fs.WriteAsync(byteArr, 0, byteArr.Length);

                        fs.Close();
                    }
                }
            }
        }

        public ImgTag(ulong GuildID, string FileName)
        {
            this.GuildID = GuildID;
            Extension = Path.GetExtension(FileName);
            Tag = FileName.Remove(FileName.Length - Extension.Length, Extension.Length);
        }
    }
}

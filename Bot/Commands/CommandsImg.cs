using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MrRestructoid.Bot.Main.Img;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Commands
{
    public class CommandsImg : ModuleBase
    {
        [Command("RemoveImg")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task RemoveImg([Remainder]string tag)
        {
            ImgTag imgTag = Program.BotHandler.ImageTags.Find(x => x.Tag == tag);

            if (imgTag != null)
            {
                Program.BotHandler.ImageTags.Remove(imgTag);
                File.Delete($"{ImgTag.ContentPath}/{Context.Guild.Id}/{imgTag.FileName}");

                await Context.Channel.SendMessageAsync($"Img **{tag}** has been removed from the tag list.");
            }
            else
            {
                await Context.Channel.SendMessageAsync(
                    $"Img **{tag}** doesn't exist in this server" +
                    $"\nCheck All server Img Tags with {Program.PREFIX}Server.");
            }
        }

        [Command("AddImg")]
        [RequireUserPermission(GuildPermission.ManageGuild)]
        public async Task AddImg(string url, [Remainder]string tag)
        {
            bool pass = true;

            if (Context.User.Id != Program.ME)
            {
                int TagCount = Program.BotHandler.ImageTags.FindAll(x => x.GuildID == Context.Guild.Id).Count;
                int UserCount = ((SocketGuild)Context.Guild).Users.Count;
                int UserCountTemp = UserCount;
                int UserCountNeededNext = 0;
                int MaxTagCount = 0;

                while (UserCountTemp >= (MaxTagCount + 1) * 25)
                {
                    UserCountTemp -= (MaxTagCount + 1) * 25;
                    UserCountNeededNext += (MaxTagCount + 1) * 25;
                    MaxTagCount++;
                }

                UserCountNeededNext += (MaxTagCount + 1) * 25;

                if (TagCount >= MaxTagCount)
                {
                    await Context.Channel.SendMessageAsync($"You need **{UserCountNeededNext - UserCount}** more users in the server to add more imgs.");
                    pass = false;
                }
            }

            if (pass)
            {
                if (Program.BotHandler.ImageTags.Exists(x => x.GuildID == Context.Guild.Id && x.Tag == tag))
                {
                    await Context.Channel.SendMessageAsync($"This server already has **{tag}** img.");
                }
                else
                {
                    ImgTag imgTag = new ImgTag(Context, tag, url);
                    Program.BotHandler.ImageTags.Add(imgTag);

                    await Context.Channel.SendMessageAsync("Specified img has been added.\nRemember it is case sensitive and can only be used in this server!");
                }
            }
        }

        [Command("Img")]
        [RequireUserPermission(GuildPermission.AttachFiles)]
        public async Task Img([Remainder]string tag)
        {
            ImgTag imgTag = Program.BotHandler.ImageTags.Find(x => x.GuildID == Context.Guild.Id && x.Tag == tag);
            if (imgTag == null)
            {
                await Context.Channel.SendMessageAsync(
                    $"This Server doesn't have an img of Tag **{tag}** added." +
                    $"\nTo add an Img, use `{Program.PREFIX}AddImg` [`image_url`] [`Tag`]");
                return;
            }

            await Context.Channel.SendFileAsync(imgTag.ImageStream, $"CalledBy-{Context.User.Id}{imgTag.Extension}");
        }

        [Command("ImgFromGuild")]
        [RequireOwner()]
        public async Task Img(ulong guild, [Remainder]string tag)
        {
            ImgTag imgTag = Program.BotHandler.ImageTags.Find(x => x.GuildID == guild && x.Tag == tag);
            if (imgTag == null)
            {
                await Context.Channel.SendMessageAsync($"This Server doesn't have an img of Tag **{tag}** added.");
                return;
            }

            await Context.Channel.SendFileAsync(imgTag.ImageStream, imgTag.FileName);
        }
    }
}

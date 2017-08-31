using Discord.Commands;
using Discord;
using Discord.WebSocket;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Diagnostics;
using MrRestructoid.Bot.Main.Img;

namespace MrRestructoid.Bot.Commands
{
    public class CommandsInfo : ModuleBase
    {
        [Command("ping")]
        public async Task Ping()
        {
            IUserMessage msg = await Context.Channel.SendMessageAsync("Pong!");

            await msg.ModifyAsync((e) =>
           {
               e.Content = $"Pong! - Time taken: **{(msg.CreatedAt - Context.Message.CreatedAt).Milliseconds}ms**";
           });
        }

        [Command("help"), Summary("")]
        public async Task Help()
        {
            EmbedBuilder eb = new EmbedBuilder();

            eb.WithAuthor((x) =>
            {
                x.IconUrl = Program.client.CurrentUser.GetAvatarUrl();
                x.Name = "Commands Help";
            });

            eb.Description =
                $"{{ `example` / `example2` }} means that atleast one of shown options must be entered.\n\n" +
                $"[ `example` ] means that you have to specify an item E.g. [`CharName`] -> **Bob**\n\n" +
                $"( [`example`] ) means that additional input is optional but not necessary unless you want to change how the command will be executed.";

            eb.ThumbnailUrl = Program.client.CurrentUser.GetAvatarUrl();

            //Core Commands
            eb.AddField((x) =>
            {
                x.Name = "▸▸▸ Core ◂◂◂";
                x.Value =
                $"`▸Info`\n" +
                $"      Provides info about essential information E.g. Bot invite, Official server, Patreon.\n" +

                $"`▸User` ( [`@User`] / [`Username`] / [`Nickname`] )\n" +
                $"      Basic user information.\n" +

                $"`▸Server`\n" +
                $"      Info about the server called from.\n" +

                $"`▸Img` [`Tag`]\n" +
                $"      Display an Image from server's tag list (CASE SENSITIVE). Server Tag List -> `Server`\n" +

                $"`▸AddImg` [`Url`] [`Tag`]\n" +
                $"      Requires ServerManage permissions for user to call. Adds an image to server's tag list (CASE SENSITIVE).\n" +

                $"`▸RemoveImg` [`Tag`]\n" +
                $"      Requires ServerManage permissions for user to call. Remove an image from server's tag list (CASE SENSITIVE).\n" +

                $"`▸ping`\n" +
                $"      Returns how much time sending a message took. (It might not look correct since your client has to download the message too. It is correct.)";
            });

            eb.AddField((x) =>
            {
                x.Name = "▸▸▸ Wars Of Tanoth Core ◂◂◂";
                x.Value =
                $"`▸CreateProfile` [`name`]\n" +
                $"      Essential to start playing. Offensive names will not be tolerated and the profile will end up being deleted or user being banned from WoT.\n" +

                $"`▸Profile` ( [`@Mention`] / [`CharName`] )\n" +
                $"      Character profile data.\n" +

                $"`▸Quest` ( [`ID`] )\n" +
                $"      Used to check how much time left for a quest to finish or start a new one. First quest is `ID = 1`.\n" +

                $"`▸Work` ( [`Hours`] )\n" +
                $"      Command your character to work while you are away. Note: you won't be able to attack other players or go on quests while working.\n" +

                $"`▸Shop` **Uses IMessages**\n" +
                $"      Buy, Sell items or check item prices.\n" +

                $"`▸Skills` ( `CompareWith` [ [`CharName`] / [`@Mention`] ] )\n" +
                $"      Character Skill stats.\n" +

                $"`▸Gear` ( `Upgrade` [`Piece`] )\n" +
                $"      Upgrade your gear or inspect it.\n" +

                $"`▸Inventory`\n" +
                $"      Displays a list of items from your inventory.\n" +

                $"`▸PvP ( [`CharName`] / [`@Mention`] )\n" +
                $"      Engage players in combat. (Note: Killing other players without a bounty will put you into the bounty list ).\n";
            });

            eb.AddField((x) =>
            {
                x.Name = "▸▸▸ Wars Of Tanoth Additional ◂◂◂";
                x.Value =
                $"`▸CharStats`\n" +
                $"      Character statistics.\n" +

                $"`▸Title {{ Select/Remove [Title_Name]` / `List` }}\n" +
                $"      Show off your title to show where you stand in the game and/or community.\n" +

                $"`Settings` **Uses IMessages**\n" +
                $"      Change your name, reset profile, configure alerts.\n" +

                $"`▸Leaderboards` ( [ `xp` / `quests` / `work` ( [ `Page` / ( `Find` [ `CharName` ] ) ] ) ] )\n" +
                $"      Navigate the leaderboardss.\n";
            });

            eb.AddField((x) =>
            {
                x.Name = "▸▸▸  Other Commands  ◂◂◂";
                x.Value =
                $"***Osu!***\n" +
                $"  `▸`{{ `Osu` / `CtB` / `Mania` / `Taiko` }} [`Player_Name`]\n" +
                $"          Information about an Osu! player.\n" +

                $"***Old School Runescape***\n" +
                $"  `▸GE` [`Item`]\n" +
                $"          Grand Exchange info about a selected item. Uses the official OSRS API.";
            });

            await Context.User.SendMessageAsync("", embed: eb.Build());
        }

        [Command("info")]
        [Alias("bot", "runtime", "about", "creator", "runtime", "stats")]
        public async Task Info()
        {
            TimeSpan runtime = (DateTime.Now - Process.GetCurrentProcess().StartTime);

            StringBuilder sb = new StringBuilder();

            if (runtime.Days > 0) sb.AppendFormat("{0} day{1} ", runtime.Days, runtime.Days > 1 ? "s" : String.Empty);
            if (runtime.Hours > 0) sb.AppendFormat("{0} hour{1} ", runtime.Hours, runtime.Hours > 1 ? "s" : String.Empty);
            if (runtime.Minutes > 0) sb.AppendFormat("{0} min{1} ", runtime.Minutes, runtime.Minutes > 1 ? "s" : String.Empty);
            if (runtime.Seconds > 0) sb.AppendFormat("{0} sec{1} ", runtime.Seconds, runtime.Seconds > 1 ? "s" : String.Empty);

            EmbedBuilder eb = new EmbedBuilder();

            eb.WithAuthor((x) =>
            {
                x.IconUrl = Program.client.CurrentUser.GetAvatarUrl();
                x.Name = $"Info about {Program.client.CurrentUser.Username}";
                x.Url = "https://discordapp.com/oauth2/authorize?&client_id=202012834035531776&scope=bot&permissions=12659727";
            });
            eb.Description =
                $"```swift\nCreator: LtLi0n#2273\n" +
                $"Users: { Program.UserCount }\n" +
                $"Servers: { Program.client.Guilds.Count }\n" +
                $"Current Runtime: {sb.ToString()}```\n" +
                $"[Official Server](https://discordapp.com/invite/6mqR2z7)        " +
                $"[Invite The Bot](https://discordapp.com/oauth2/authorize?&client_id=202012834035531776&scope=bot&permissions=12659727)        " +
                $"[Patreon](https://www.patreon.com/LtLi0n)\n";

            eb.WithFooter((x) =>
            {
                x.Text = "It is strongly advised to join the official server. Check out new updates and meet with the community!";
            });

            await Context.Channel.SendMessageAsync("", embed: eb.Build());
        }

        [Command("user")]
        [RequireContext(ContextType.Guild)]
        public async Task User([Remainder]string target = null)
        {
            IGuildUser showUser = Context.User as IGuildUser;

            if (Context.Message.MentionedUserIds.Count > 0)
            {
                showUser = await Context.Guild.GetUserAsync(Context.Message.MentionedUserIds.First());
            }
            else if (target != null)
            {
                List<(ulong, string)> potentialTargets = new List<(ulong, string)>();

                IReadOnlyCollection<IGuildUser> users = await Context.Guild.GetUsersAsync();

                foreach (IGuildUser u in users)
                {
                    if (u.Username == target)
                    {
                        potentialTargets.Add((u.Id, u.Username));
                        break;
                    }
                    else if (u.Username.ToLower() == target.ToLower())
                    {
                        potentialTargets.Add((u.Id, u.Username));
                    }
                    else if (u.Nickname != null)
                    {
                        if (u.Nickname == target)
                        {
                            potentialTargets.Add((u.Id, u.Nickname));
                            break;
                        }
                        else if (u.Nickname.ToLower() == target.ToLower())
                        {
                            potentialTargets.Add((u.Id, u.Nickname));
                        }
                    }
                }

                if (potentialTargets.Count == 1)
                {
                    showUser = await Context.Guild.GetUserAsync(potentialTargets[0].Item1);
                }
                else
                {
                    if (potentialTargets.Count == 0)
                    {
                        await Context.Channel.SendMessageAsync("Didn't find sunch user. Perhaps try a @Mention?");
                        return;
                    }
                    else
                    {
                        string toReturn = "Found too many users. Type exact name or @Mention the user instead.\n";

                        foreach ((ulong, string) potentialT in potentialTargets)
                        {
                            toReturn += potentialT.Item2 + "\n";
                        }

                        await Context.Channel.SendMessageAsync(toReturn);
                        return;
                    }
                }
            }

            EmbedBuilder eb = new EmbedBuilder();

            eb.WithThumbnailUrl(showUser.GetAvatarUrl());

            eb.WithAuthor((x) =>
            {
                x.IconUrl = showUser.GetAvatarUrl();
                x.Name = $"{showUser.Username}#{showUser.Discriminator}";
            });

            if (showUser.RoleIds.Count > 0)
            {
                eb.Color = Context.Guild.GetRole(showUser.RoleIds.First()).Color;

                List<IRole> userRoles = new List<IRole>();

                foreach (ulong roleID in showUser.RoleIds)
                {
                    bool skipRole = true;

                    if (((SocketRole)Context.Guild.GetRole(roleID)).IsEveryone) continue;

                    switch (roleID)
                    {
                        case 349875313003724800: break;
                        case 349875385342885889: break;
                        case 349875417106087936: break;

                        default:
                            skipRole = false;
                            break;
                    }

                    if (skipRole) continue;

                    userRoles.Add(Context.Guild.GetRole(roleID));
                }

                IEnumerable<IRole> sortedRoles = userRoles.OrderByDescending(x => x.Position);

                EmbedFieldBuilder roleField = new EmbedFieldBuilder();
                roleField.Name = string.Format("Role{0}", sortedRoles.Count() > 1 ? $"s [{sortedRoles.Count()}]" : string.Empty);

                foreach (IRole role in sortedRoles)
                {
                    roleField.Value += string.Format("{0}{1}", role.Name, sortedRoles.Last().Id == role.Id ? string.Empty : ", ");
                }

                eb.Color = sortedRoles.First().Color;

                eb.AddField(roleField);
            }

            if (showUser.Game != null)
            {
                eb.Description = "Playing " + showUser.Game.Value;
            }

            eb.AddField((x) =>
            {
                x.Name = "Status";
                x.Value = Enum.GetName(typeof(UserStatus), showUser.Status);
                x.IsInline = true;
            });

            eb.AddField((x) =>
            {
                x.Name = "Nickname";
                x.Value = showUser.Nickname == null ? "Not Present" : $"**{showUser.Nickname}**";
                x.IsInline = true;
            });

            eb.AddField((x) =>
            {
                x.Name = "Joined Server At";
                x.Value = showUser.JoinedAt.ToString();
                x.IsInline = true;
            });

            eb.AddField((x) =>
            {
                x.Name = "Joined Discord At";
                x.Value = showUser.CreatedAt.ToString();
                x.IsInline = true;
            });


            eb.WithFooter(x =>
            {
                x.Text = $"ID: {showUser.Id}";
            });

            await Context.Channel.SendMessageAsync("", embed: eb.Build());
        }

        [Command("server"), Summary("returns information about the server.")]
        [Alias("guild")]
        [RequireContext(ContextType.Guild)]
        public async Task Server()
        {
            IGuild guild = Context.Guild;
            IGuildUser owner = await guild.GetOwnerAsync();

            IReadOnlyCollection<IGuildUser> users = await guild.GetUsersAsync();

            List<IGuildUser> onlineUsers = new List<IGuildUser>();

            foreach (IGuildUser user in users)
            {
                if (user.Status > 0)
                {
                    onlineUsers.Add(user);
                }
            }

            EmbedBuilder eb = new EmbedBuilder();

            eb.Title = $"{guild.Name}";
            eb.Description = $"Online: {onlineUsers.Count}/{users.Count}";

            eb.ThumbnailUrl = guild.IconUrl;

            eb.AddField("Owner", $"{owner.Username}#{owner.Discriminator}", true);
            eb.AddField("Region", $"{guild.VoiceRegionId}", true);
            eb.AddField("Creation Date", $"{guild.CreatedAt.ToString().Remove(guild.CreatedAt.ToString().Length - 8, 7)}", true);
            eb.AddField("Joined At", $"{((IGuildUser)Context.User).JoinedAt.ToString().Remove(((IGuildUser)Context.User).JoinedAt.ToString().Length - 8, 7)}", true);

            IEnumerable<ImgTag> tags = Program.BotHandler.ImageTags.FindAll(x => x.GuildID == Context.Guild.Id).OrderBy(e => e.FileName);

            if(tags.Count() > 0)
            {
                eb.AddField((x) =>
                {
                    x.Name = "Img Tags";
                    foreach (ImgTag tag in tags)
                    {
                        x.Value += string.Format("{0}{1}", tag.Tag, tag.Tag == tags.Last().Tag ? "" : ", ");
                    }

                });
            }

            await Context.Channel.SendMessageAsync("", embed: eb.Build());
        }
    }
}

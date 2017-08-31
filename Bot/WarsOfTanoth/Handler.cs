using Discord;
using Discord.Commands;
using Discord.WebSocket;
using MrRestructoid.Bot.Commands;
using MrRestructoid.Bot.Commands.Tanoth;
using MrRestructoid.Bot.InteractiveMessages;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.WarsOfTanoth.Adventure;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Creatures;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.Items;
using MrRestructoid.Bot.WarsOfTanoth.Items.Gear;
using MrRestructoid.Bot.WarsOfTanoth.System;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth
{
    public class Handler
    {
        public List<Player> Players { get; set; }
        public List<Player> BountyPlayers { get; set; }

        public Player[] TopWork { get; set; }
        public Player[] TopQuests { get; set; }

        public int UpdateCounter = 0;

        public Professions Professions_Tier1 { get; private set; }

        public bool updating { get; set; }
        public bool Running { get; private set; }

        public const string path_players = "data/WoT/players";

        public bool Saving { get; private set; }
        public bool Loading { get; private set; }

        public Handler()
        {
            Players = new List<Player>();

            Task.Run(async () =>
            {
                Console.WriteLine("Loading Handler Essential Data...");

                await Item.LoadItems();

                await Monster.LoadMonsters();
                await Quest.LoadQuests();

                await GearSword.LoadSwords();
                await GearHelmet.LoadHelmets();
                await GearChestplate.LoadChestplates();
                await GearGloves.LoadGloves();
                await GearBoots.LoadBoots();
                await GearLeggings.LoadLeggings();

                await ITitle.LoadTitles();

                await LoadPlayers();

                Console.WriteLine("Handler Data Loaded.");

            }).GetAwaiter().GetResult();

            Player p = Players.Find(x => x.Character.Name == "Flame");

            Console.WriteLine("Sorting Players...");

            BountyPlayers = new List<Player>();
            BountyPlayers = Players.FindAll(x => x.Character.Stats.PvP.reasonlessKillTimer > 0);
            TopWork = Players.ToArray();
            TopQuests = Players.ToArray();

            SortPlayers.SortAll(Program.handler);

            Console.WriteLine("Players Sorted.\nStarting WoT.");

            Running = true;

            Task.Run(async () =>
            {
                await KeepAlive();
            });
        }

        private async Task KeepAlive()
        {
            while(Running)
            {
                await Task.Delay(1000);

                UpdateCounter++;
                if(UpdateCounter == 60)
                {
                    UpdateCounter = 0;
                    await Update();

                    int tempUserCount = 0;
                    foreach (SocketGuild guild in Program.client.Guilds)
                    {
                        tempUserCount += guild.Users.Count;
                    }

                    await SavePlayers();
                }
            }
        }

        public async Task Update()
        {
            SortPlayers.SortAll(Program.handler);

            foreach (Player p in Players)
            {
                try
                {
                    await p.Update(this);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    Console.WriteLine($"Problem with updating player {p.Character.Name}");
                }
            }
        }

        public async Task LoadAdditionals()
        {
            if(Program.botType == Program.BotType.Live)
            {
                Professions_Tier1 = new Professions();
            }
        }
        
        public Task<Player> GetPlayer(ulong ID)
        {
            return Task.FromResult(Players.Find(x => x.User.UserID == ID));
        }

        public async Task<Player> GetPlayer(ICommandContext Context, string args = "")
        {
            if(args != null)
            {
                if (args == "")
                {
                    Player player = Program.handler.Players.Find(x => x.User.UserID == Context.User.Id);

                    if (player == null)
                    {
                        await Context.Channel.SendMessageAsync($"Looks like you don't have a profile yet. Use `{Program.PREFIX}CreateProfile [char_name]` first.\n**Offensive nicknames will not be tolerated and will result in deletion of the profile.**");
                        return null;
                    }

                    return player;
                }
                else
                {
                    //Is mention
                    if (Context.Message.MentionedUserIds.Count > 1)
                    {
                        args = args.Remove(0, 2).Remove(args.Length - 3, 1);

                        //If mention was done inside a guild
                        if(Context.Guild != null)
                        {
                            IGuildUser mentionedUser = await Context.Guild.GetUserAsync(ulong.Parse(args));

                            if (mentionedUser != null)
                            {
                                if (mentionedUser.IsBot)
                                {
                                    await Context.Channel.SendMessageAsync(":robot: I think you forgot this game is for humans :robot:");
                                }
                                else
                                {
                                    Player returnPlayer = Players.Find(x => x.User.UserID == ulong.Parse(args));

                                    if (returnPlayer == null)
                                    {
                                        await Context.Channel.SendMessageAsync("This person doesn't seem to be playing Wars of Tanoth. :thinking:");
                                    }
                                    return returnPlayer;
                                }
                            }
                            else
                            {
                                await Context.Channel.SendMessageAsync(":mag: Didn't find the user in the same server.");
                            }
                        }
                        else
                        {
                            await Context.Channel.SendMessageAsync("Can't specify players using mentions in a private DM.");
                        }

                        return null;
                    }
                    else
                    {
                        Player returnPlayer = Players.Find(x => x.Character.Name.ToLower() == args.ToLower());

                        if (returnPlayer == null)
                        {
                            await Context.Channel.SendMessageAsync("Player either does not exist or you misspelled his/her name.\nTry Mention instead. :)");
                        }

                        return returnPlayer;
                    }
                }
            }
            else
            {
                return null;
            }
        }


        public async Task LoadPlayers(string path = path_players)
        {
            if (CommandsOwner.Exiting) return;

            while (Saving) await Task.Delay(10);

            Loading = true;

            if(File.Exists($"{path_players}/players_saved.json"))
            {
                Console.WriteLine($"{ path_players}/ players_saved.json");

                FileStream fs = new FileStream($"{path_players}/players_saved.json", FileMode.Open);

                StreamReader sr = new StreamReader(fs);
                string json = await sr.ReadToEndAsync();
                Console.WriteLine("jsonLength: " + json.Length);
                sr.Close();
                Players = JsonConvert.DeserializeObject<List<Player>>(json);

                Console.WriteLine(Players.Count + " Players Loaded.");
            }

            Loading = false;
        }

        public async Task SavePlayers(string path = path_players)
        {
            if (CommandsOwner.Exiting) return;

            while (Loading) await Task.Delay(10);

            Saving = true;

            string json = JsonConvert.SerializeObject(Players);

            using (StreamWriter sw = new StreamWriter($"{path}/players_saved.json"))
            {
                await sw.WriteAsync(json);
                sw.Close();
            }

            Saving = false;
        }
    }
}

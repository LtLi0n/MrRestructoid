using System;
using Discord;
using Discord.WebSocket;
using Discord.Commands;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using MrRestructoid.Bot.WarsOfTanoth;
using System.Threading;
using System.Threading.Tasks;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.InteractiveMessages;
using System.Collections.Generic;

namespace MrRestructoid.Bot
{
    class Program
    {
        public enum BotType { Experimental = 1, Live = 5};

        public const ulong SpecialRole = 350980878605877250;
        public static List<ulong> SpecialUsers;

        public static bool BotReady { get; private set; }
        public static Random random;
        public static DisplayGame DisplayGame { get; private set; }

        private const string PREFIX_EXPERIMENTAL = "_", PREFIX_LIVE = "+";
        public const ulong ME = 174230278611533824;

        public static string PREFIX
        {
            get
            {
                if (botType == BotType.Live)
                {
                    return PREFIX_LIVE;
                }
                else
                {
                    return PREFIX_EXPERIMENTAL;
                }
            }
        }

        private IServiceProvider services;

        public static CommandService commands;
        public static DiscordSocketClient client;
        public static Handler handler;

        public static int UserCount;

        public static List<InteractMessage> IMessages;
        public static Timer timer1s;

        public static BotType botType { get; private set; }

        public static BotHandler BotHandler { get; private set; }

        static void Main(string[] args) => new Program().MainAsync(args).GetAwaiter().GetResult();

        public async Task MainAsync(string[] args)
        {
            if(args.Length == 0)
            {
                botType = BotType.Experimental;
            }
            else
            {
                botType = (BotType)int.Parse(args[0]);
            }
            await Library.LoadKeys();

            BotHandler = new BotHandler();
            random = new Random();
            await Boot.Load(BotHandler);
            handler = new Handler();

            Library.rOpts.RetryMode = RetryMode.RetryRatelimit;
            Library.rOpts.Timeout = null;

            IMessages = new List<InteractMessage>();

            client = new DiscordSocketClient();
            client.Log += Log;
            client.MessageReceived += MessageReceived;

            client.ReactionAdded += Client_ReactionAdded;
            client.ReactionRemoved += Client_ReactionRemoved;
            client.Ready += Client_Ready;

            commands = new CommandService();
            services = new ServiceCollection().BuildServiceProvider();

            await InstallCommands();

            string token = "";

            switch (botType)
            {
                case BotType.Live: token = Library.API_KEYS["MrDestructoid"]; break;
                case BotType.Experimental: token = Library.API_KEYS["MrRestructoid"]; break;
            }

            await client.LoginAsync(TokenType.Bot, token);
            await client.StartAsync();

            timer1s = new Timer(async(object o) => await Events.OnTimerDown(o), null, 0, 1000);
            await Task.Delay(-1);
        }

        private async Task Client_ReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (arg3.User.Value.IsBot) return;

            if (arg1.Id == handler.Professions_Tier1.msg.Id)
            {
                await handler.Professions_Tier1.ProcessReaction(arg3, false);
            }
        }

        private async Task Client_ReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            if (arg3.User.Value.IsBot) return;

            if (arg1.Id == handler.Professions_Tier1.msg.Id)
            {
                await handler.Professions_Tier1.ProcessReaction(arg3);
            }
        }

        private async Task Client_Ready()
        {

            DisplayGame = new DisplayGame();
            await client.SetGameAsync(DisplayGame.RandomDisplay);
            await handler.LoadAdditionals();

            foreach (SocketGuild guild in client.Guilds)
            {
                UserCount += guild.Users.Count;
            }

            SpecialUsers = new List<ulong>();
            SpecialUsers.Add(ME);

            IReadOnlyCollection<IGuildUser> mainServerUsers = await ((IGuild)client.GetGuild(279715709792288775)).GetUsersAsync();

            foreach (IGuildUser user in mainServerUsers)
            {
                foreach (ulong roleID in user.RoleIds)
                {
                    if (roleID == 350980878605877250)
                    {
                        SpecialUsers.Add(user.Id);
                        break;
                    }
                }
            }

            BotReady = true;
        }

        private async Task MessageReceived(SocketMessage sm)
        {
            foreach (InteractMessage msg in IMessages)
            {
                if (sm.Author.Id == msg.user.Id)
                {
                    await msg.CallProcess((IUserMessage)sm);
                    return;
                }
            }
        }

        private async Task InstallCommands()
        {
            client.MessageReceived += HandleCommand;

            await commands.AddModulesAsync(Assembly.GetEntryAssembly());
        }

        private async Task HandleCommand(SocketMessage sm)
        {
            // Don't process the command if it was a System Message
            var message = sm as SocketUserMessage;
            if (message == null) return;
            if (message.Author.IsBot) return;

            if(botType == BotType.Experimental && sm.Author.Id != ME)
            {
                foreach(CommandInfo ci in commands.Commands)
                {
                    bool found = false;

                    if ($"{PREFIX}" + ci.Name.ToLower() == sm.Content.Split(' ')[0].ToLower())
                    {
                        await sm.Channel.SendMessageAsync("This bot build is only available for owner to use.");
                        found = true;
                    }

                    if(!found)
                    {
                        foreach (string alias in ci.Aliases)
                        {
                            if ($"{PREFIX}" + alias.ToLower() == sm.Content.Split(' ')[0].ToLower())
                            {
                                await sm.Channel.SendMessageAsync("This bot build is only available for owner to use.");
                                found = true;
                                break;
                            }
                        }
                    }

                    if (found) break;
                }

                return;
            }

            if (sm.Author as IGuildUser == null)
            {
                if (!SpecialUsers.Exists(x => x == sm.Author.Id))
                {
                    IDMChannel DM = await message.Author.GetOrCreateDMChannelAsync();
                    await DM.SendMessageAsync("DM commands no longer work unless you are premium on Patreon.");
                    return;
                }
            }
            // Create a number to track where the prefix ends and the command begins
            int argPos = 0;
            // Determine if the message is a command, based on if it starts with '!' or a mention prefix
            if (!(message.HasStringPrefix(PREFIX, ref argPos) || message.HasMentionPrefix(client.CurrentUser, ref argPos)))
            {
                return;
            }
            // Create a Command Context
            var context = new CommandContext(client, message);
            // Execute the command. (result does not indicate a return value, 
            // rather an object stating if the command executed successfully)
            var result = await commands.ExecuteAsync(context, argPos, services);
            if (!result.IsSuccess)
            {
                if(result.Error == CommandError.UnknownCommand != "Unknown command.")
                {
                    await context.Channel.SendMessageAsync(result.ErrorReason);
                }
            }
        }

        private Task Log(LogMessage lm)
        {
            Console.WriteLine(lm.ToString());
            return Task.CompletedTask;
        }
    }
}
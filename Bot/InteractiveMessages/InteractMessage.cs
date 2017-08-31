using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.Items;
using MrRestructoid.Bot.Main;

namespace MrRestructoid.Bot.InteractiveMessages
{
    /// <summary>
    /// A base class to use for any other InteractiveMessage
    /// </summary>
    public abstract class InteractMessage
    {
        public const int SecondsAliveFor = 30;

        public enum IMessageState { Active, Closed }

        protected List<short> Navigation { get; private set; }

        protected IMessageChannel channel { get; set; }
        public IUserMessage message { get; set; }

        public IMessageState state { get; set; }

        public short SecondsToDispose { get; private set; }
        protected bool ignoreUserInactiveMessage = false;
        public IUser user { get; set; }

        protected IReadOnlyDictionary<IEmote, ReactionMetadata> reactions { get; set; }

        protected string text { get; set; }
        protected string EntryText { get; set; }

        public InteractMessage(IUser user, IMessageChannel channel, IUserMessage message)
        {
            this.message = message;
            state = IMessageState.Active;
            this.channel = channel;
            this.user = user;
            SecondsToDispose = 15;

            Navigation = new List<short>();

            bool dispose = false;

            foreach (IMessage imsg in Program.IMessages)
            {
                if (imsg.Author.Id == user.Id)
                {
                    dispose = true;
                }
            }

            Task.Run(async () =>
            {
                await KeepAlive(dispose);
            });
        }

        private async Task KeepAlive(bool dispose = false)
        {
            while(SecondsToDispose != 0 && state == IMessageState.Active && !dispose)
            {
                await Task.Delay(1000);
                SecondsToDispose--;
            }

            state = IMessageState.Closed;
            if (!ignoreUserInactiveMessage && SecondsToDispose == 0)
            {
                await message.DeleteAsync();
                await channel.SendMessageAsync("User inactive. Interactive message has been closed.");
            }
        }

        protected async Task AddReactions(params Emoji[] emojies)
        {
            if(message.Reactions.Count > 0)
            {
                await message.RemoveAllReactionsAsync();
            }

            foreach (Emoji emoji in emojies)
            {
                try
                {
                    await message.AddReactionAsync(emoji);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            }

            message = await message.Channel.GetMessageAsync(message.Id) as IUserMessage;

            reactions = message.Reactions;
        }

        protected virtual Task Process(string input) => Task.CompletedTask;

        public async Task CallProcess(IUserMessage sm)
        {
            string input = (sm == null ? "" : sm.Content.ToLower());

            if (!CheckIfProcess(sm).Result) return;

            if ((input == "back" || input == "go back") && Navigation.Count > 0)
            {
                if (Navigation.Count >= 2)
                {
                    input = Navigation[Navigation.Count - 2].ToString();

                    Navigation.RemoveRange(Navigation.Count - 2, 2);
                }
                else
                {
                    Navigation.RemoveAt(0);

                    await SendMessage(EntryText);
                    return;
                }
            }

            SecondsToDispose = SecondsAliveFor;
            await Process(sm.Content);
        }

        public async Task SendMessage(string text)
        {
            await message.DeleteAsync();
            message = await channel.SendMessageAsync(text);
        }

        public async Task<bool> CheckIfProcess(IUserMessage sm)
        {
            if (sm.Author.Id != user.Id)
            {
                return false;
            }

            bool close = false;

            switch (sm.Content.ToLower())
            {
                case "exit": close = true; break;
                case "close": close = true; break;
                case "kill": close = true; break;
            }

            if (close || state == IMessageState.Closed)
            {
                await message.DeleteAsync();
                state = IMessageState.Closed;

                return false;
            }
            else
            {
                return true;
            }
        }
    }
}

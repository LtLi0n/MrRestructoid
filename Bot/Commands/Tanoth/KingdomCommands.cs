using Discord;
using Discord.Commands;
using MrRestructoid.Bot.WarsOfTanoth.Adventure;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Commands.Tanoth
{
    public class KingdomCommands : ModuleBase
    {
        [Command("Kingdom")]
        [RequireOwner()]
        public async Task Kingdom()
        {
            Player player = await Program.handler.GetPlayer(Context);

            if(player == null) return;

            Program.IMessages.Add(new KingdomIMessage((IGuildUser)Context.User, Context.Channel, Context.Channel.SendMessageAsync(KingdomIMessage.KingdomText.text).Result, player));
        }
    }
}

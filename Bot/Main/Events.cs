using Discord.WebSocket;
using MrRestructoid.Bot.InteractiveMessages;
using MrRestructoid.Bot.WarsOfTanoth;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.System;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Main
{
    public static class Events
    {
        public static bool updating = false;
        public static int timerTickCounter = 0;

        public static async Task OnTimerDown(object o, bool skipping = false)
        {
            for(int i = 0; i < Program.IMessages.Count; i++)
            {
                if (Program.IMessages[i].state == InteractMessage.IMessageState.Closed)
                {
                    Program.IMessages[i] = null;
                    Program.IMessages.Remove(Program.IMessages[i]);
                    i--;
                }
            }

            if (Program.BotReady)
            {
                if (Program.botType == Program.BotType.Live)
                {
                    Program.handler.Professions_Tier1.Update().GetAwaiter().GetResult();
                }

                if (timerTickCounter % 60 == 0)
                {
                    await Program.client.SetGameAsync(Program.DisplayGame.RandomDisplay);
                }
            }

            timerTickCounter++;
            updating = false;
        }
    }
}

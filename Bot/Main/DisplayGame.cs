using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Main
{
    public class DisplayGame
    {
        private Func<string>[] Displays { get; set; }
        private int? ActiveDisplay { get; set; }

        private Task LoadDisplays()
        {
            Displays = new Func<string>[]
            {
                () => $"[ {Program.PREFIX}info, {Program.PREFIX}help ] Bot Completely Rewritten!",
                () => $"[ Servers: {Program.client.Guilds.Count} ]",
                () => $"Join the official server and keep in touch with updates! {Program.PREFIX}info",
            };

            return Task.CompletedTask;
        }

        public DisplayGame()
        {
            ActiveDisplay = null;
            LoadDisplays().GetAwaiter().GetResult();
        }

        public string RandomDisplay
        {
            get
            {
                int tempID = Program.random.Next(Displays.Length);

                if (ActiveDisplay != null)
                {
                    if (tempID == ActiveDisplay.Value)
                    {
                        int decider = 0;

                        if (tempID > 0 && tempID < Displays.Length - 1) decider = Program.random.Next(2) + 1;
                        else if (tempID == 0 && tempID + 1 != Displays.Length) decider = 2;
                        else if (tempID == Displays.Length - 1) decider = 1;

                        switch (decider)
                        {
                            case 1: tempID--; break;
                            case 2: tempID++; break;
                        }
                    }
                }

                ActiveDisplay = tempID;

                return Displays[tempID]();
            }
        }
    }
}

using MrRestructoid.Bot.InteractiveMessages;
using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;

namespace MrRestructoid.Bot.WarsOfTanoth.Adventure
{
    public class KingdomIMessage : InteractMessage
    {
        //Abandoned, might come back if it peaks my interest

        public Player Player { get; private set; }

        public KingdomIMessage(IUser user, IMessageChannel channel, IUserMessage message, Player player) : base(user, channel, message)
        {
            Player = player;
            EntryText = KingdomText.text;
        }

        override protected async Task Process(string input)
        {
            switch (Navigation.Count)
            {
                case 0:
                    if (input == "1" || input == "enter the palace")
                    {
                        Navigation.Add(1);
                        await SendMessage(KingdomText.Text_1.text);
                    }

                    else if (input == "2" || input == "train skills")
                    {
                        Navigation.Add(2);
                        await SendMessage(KingdomText.Text_2.text);
                    }

                    break;

                case 1:
                    break;
            }
        }


        public static class KingdomText
        {
            public const string text =
                "1) Enter The Palace\n" +
                "2) Train Skills\n" +
                "3) Manage Kingdom";

            public static class Text_1
            {
                public const string text =
                    "1) Quests\n" +
                    "2) Daily Missions\n" +
                    "3) Treasury";
            }

            public static class Text_2
            {
                public const string text =
                    "1) WoodCutting\n" +
                    "2) Mining\n" +
                    "3) Fishing";
            }

            public static class Text_3
            {

            }
        }
    }
}

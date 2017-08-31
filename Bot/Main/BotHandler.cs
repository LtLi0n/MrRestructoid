using MrRestructoid.Bot.Main.Img;
using System;
using System.Collections.Generic;
using System.Text;

namespace MrRestructoid.Bot.Main
{
    class BotHandler
    {
        public List<ImgTag> ImageTags { get; private set; }

        public BotHandler()
        {
            ImageTags = new List<ImgTag>();
        }
    }
}

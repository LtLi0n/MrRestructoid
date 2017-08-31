using Discord;
using Discord.WebSocket;
using MrRestructoid.Bot.InteractiveMessages;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.Items;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Adventure
{
    public class ShopIMessage : InteractMessage
    {
        private int quantity { get; set; }

        private int itemID { get; set; }
        private Item Item => Item.GetItem(itemID);

        private string ConfirmationSeed { get; set; }
        private Player Player { get; set; }

        public ShopIMessage(IUser user, IMessageChannel channel, IUserMessage message, Player player) : base(user, channel, message)
        {
            itemID = -1;

            Player = player;
            EntryText = ShopText.Text1;
        }

        override protected async Task Process(string input)
        {
            switch (Navigation.Count)
            {
                case 0:
                    if (input.ToLower() == "buy" || input == "1")
                    {
                        await SendMessage(ShopText.BuyText.Text1);

                        Navigation.Add(1);
                    }
                    else if (input.ToLower() == "sell" || input == "2")
                    {
                        await SendMessage(ShopText.SellText.Text1);

                        Navigation.Add(2);
                    }
                    else if (input.ToLower() == "list" || input == "3")
                    {
                        await SendMessage(ShopText.ListText.Text1);

                        Navigation.Add(3);
                    }
                    else if (input == "4")
                    {
                        state = IMessageState.Closed;
                        return;
                    }
                    break;
                case 1:
                    //selling or buying
                    if (Navigation[0] == 1 || Navigation[0] == 2)
                    {
                        foreach (Item i in Item.Items)
                        {
                            //If attempting to buy a discontinued item, prevent
                            if (!i.Purchasable && Navigation[0] == 1) continue;

                            if (i.Name.ToLower() == input)
                            {
                                itemID = i.ID;
                                break;
                            }
                        }

                        if (itemID == -1)
                        {
                            await SendMessage($"Item `{input}` was not found.\nPlease type again.");
                        }
                        else
                        {
                            text = string.Format(
                                "Item: **{0}**{1}\n{2}Price: **{3}**", Item.Name,
                                Navigation[0] == 2 ? $"  ( `{Player.Character.Inventory.Items.Find(x => x.ID == Item.ID).amount}` )" : string.Empty,
                                Navigation[0] == 2 ? "Sell " : string.Empty,
                                Navigation[0] == 1 ? Item.Price : (Item.Price / 2));

                            if (Navigation[0] == 1) await SendMessage($"{text}\n{ShopText.BuyText.Text2}");
                            else if (Navigation[0] == 2) await SendMessage($"{text}\n{ShopText.SellText.Text2}");

                            Navigation.Add(1);
                        }
                    }
                    //listing
                    else if (Navigation[0] == 3)
                    {
                        if (input == "1")
                        {
                            await SendMessage(ShopText.ListText.GemSelection);

                            Navigation.Add(1);
                        }
                    }
                    break;
                case 2:
                    //Selling or Buying (Amount selection)
                    if (Navigation[0] == 1 || Navigation[0] == 2)
                    {
                        if (!Library.IsDigit(input))
                        {
                            if (Navigation[0] == 1) await SendMessage($"Numbers only!\n{text}\n{ShopText.BuyText.Text2}");
                            else if (Navigation[0] == 2) await SendMessage($"Numbers only!\n{text}\n{ShopText.SellText.Text2}");
                        }
                        else if (input.Length > 2)
                        {
                            if (Navigation[0] == 1) await SendMessage($"Maximum amount you can purchase at once is `99`\n{text}\n{ShopText.BuyText.Text2}");
                            else if (Navigation[0] == 2) await SendMessage($"Maximum amount you can purchase at once is `99`\n{text}\n{ShopText.SellText.Text2}");
                        }
                        else
                        {
                            if (int.Parse(input) < 1)
                            {
                                if (Navigation[0] == 1) await SendMessage($"You must select atleast `1` item to purchase\n{text}\n{ShopText.BuyText.Text2}");
                                else if (Navigation[0] == 2) await SendMessage($"You must select atleast `1` item to sell\n{text}\n{ShopText.SellText.Text2}");

                                return;
                            }

                            quantity = int.Parse(input);

                            bool pass = true;

                            if (Navigation[0] == 1)
                            {
                                if (quantity * Item.Price > Player.Character.Gold)
                                {
                                    text = $"You still lack `{(quantity * Item.Price) - Player.Character.Gold}` more gold.";

                                    await SendMessage($"{text}\n\n{ShopText.BuyText.Text1}");

                                    pass = false;
                                }
                            }
                            else if (Navigation[0] == 2)
                            {
                                if (quantity > Player.Character.Inventory.Items.Find(x => x.ID == Item.ID).amount)
                                {
                                    text = $"Your inventory doesn't contain `{quantity}x` of **{Item.Name}**.";

                                    await SendMessage($"{text}\n\n{ShopText.SellText.Text1}");

                                    pass = false;
                                }
                            }

                            if (pass)
                            {
                                Random random = new Random();

                                for (int i = 0; i < 4; i++) ConfirmationSeed += random.Next(10);

                                if (Navigation[0] == 1)
                                {
                                    text = 
                                        $"Item: `{quantity}x` **{Item.Name}**\n" +
                                        $"Total Price: **{string.Format("{0:n0}", quantity * Item.Price)}**\n" +
                                        $"Your Gold: **{string.Format("{0:n0}", Player.Character.Gold)}**\n\n" +
                                        $"Type **{ConfirmationSeed}** to proceed.";
                                }
                                else if (Navigation[0] == 2)
                                {
                                    text = 
                                        $"Item: **{quantity}x** {Item.Name}\n" +
                                        $"Total Selling Price: **{string.Format("{0:n0}", (quantity * Item.Price) / 2)}**\n" +
                                        $"Your Gold: **{string.Format("{0:n0}", Player.Character.Gold)}**\n\n" +
                                        $"Type **{ConfirmationSeed}** to proceed.";
                                }

                                Navigation.Add(1);
                            }

                            await SendMessage(text);
                        }
                    }
                    //Listing (Gems)
                    else if (Navigation[0] == 3 && Navigation[1] == 1)
                    {
                        if (input.ToLower() == "ruby" || input == "1")
                        {
                            string rubyList =
                                $"`{Item.Ruby.Ruby_Level_1.Name}`   - **{Item.Ruby.Ruby_Level_1.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_2.Name}`   - **{Item.Ruby.Ruby_Level_2.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_3.Name}`   - **{Item.Ruby.Ruby_Level_3.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_4.Name}`   - **{Item.Ruby.Ruby_Level_4.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_5.Name}`   - **{Item.Ruby.Ruby_Level_5.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_6.Name}`   - **{Item.Ruby.Ruby_Level_6.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_7.Name}`   - **{Item.Ruby.Ruby_Level_7.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_8.Name}`   - **{Item.Ruby.Ruby_Level_8.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_9.Name}`   - **{Item.Ruby.Ruby_Level_9.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_10.Name}` - **{Item.Ruby.Ruby_Level_10.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_11.Name}` - **{Item.Ruby.Ruby_Level_11.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_12.Name}` - **{Item.Ruby.Ruby_Level_12.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_13.Name}` - **{Item.Ruby.Ruby_Level_13.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_14.Name}` - **{Item.Ruby.Ruby_Level_14.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_15.Name}` - **{Item.Ruby.Ruby_Level_15.Price}**\n" +
                                $"`{Item.Ruby.Ruby_Level_16.Name}` - **{Item.Ruby.Ruby_Level_16.Price}**\n";

                            await SendMessage(rubyList);
                            state = IMessageState.Closed;
                        }
                        else if (input.ToLower() == "emerald" || input == "2")
                        {
                            string emeraldList = $"`{Item.Emerald.Emerald_Level_1.Name}`  - **{Item.Emerald.Emerald_Level_1.Price}**\n";
                            emeraldList += $"`{Item.Emerald.Emerald_Level_2.Name}`  - **{Item.Emerald.Emerald_Level_2.Price}**\n";
                            emeraldList += $"`{Item.Emerald.Emerald_Level_3.Name}`  - **{Item.Emerald.Emerald_Level_3.Price}**\n";
                            emeraldList += $"`{Item.Emerald.Emerald_Level_4.Name}`  - **{Item.Emerald.Emerald_Level_4.Price}**\n";

                            await SendMessage(emeraldList);
                            state = IMessageState.Closed;
                        }
                    }

                    break;
                case 3:
                    //Buying or Selling (Checking confirmation input)
                    if (Navigation[0] == 1 || Navigation[0] == 2)
                    {
                        if (input == ConfirmationSeed)
                        {
                            if (Navigation[0] == 1)
                            {
                                Player.Character.Gold -= (Item.Price * quantity);
                                Player.Character.Inventory.AddItem(Item.ID, quantity);
                                await SendMessage($"`{quantity}x` of **{Item.Name}** bought!");
                                state = IMessageState.Closed;
                            }
                            else if (Navigation[0] == 2)
                            {
                                Player.Character.Gold += ((Item.Price * quantity) / 2);
                                Player.Character.Inventory.RemoveItem(Item.ID, quantity);
                                await SendMessage($"`{quantity}x` of **{Item.Name}** sold!");
                                state = IMessageState.Closed;
                            }
                        }
                    }
                    break;
            }
        }

        public static class ShopText
        {
            public const string Text1 = 
                ":scales: **SHOP** :scales:\n       " +
                ":one: ⇒ :shopping_cart:\n       " +
                ":two: ⇒ :moneybag:\n       " +
                ":three: ⇒ :newspaper:\n       " +
                ":four: ⇒ :back:";
            public const string ExitText = ":shopping_cart: Shop was successfully exited.";

            public static class BuyText
            {
                public const string Text1 = "Enter item name you are looking for: ";
                public const string Text2 = "Enter an amount you want to purchase: ";
            }
            public static class SellText
            {
                public const string Text1 = "Enter item name you wish to sell: ";
                public const string Text2 = "Enter an amount you want to sell: ";
            }
            public static class ListText
            {
                public const string Text1 =
                    ":page_with_curl: **SHOP LIST**\n       " +
                    ":one: ⇒ :gem:\n       ";
                public const string GemSelection = 
                    ":gem: **GEM LIST**\n       " +
                    ":one: ⇒ :red_circle:\n       " +
                    ":two: ⇒ :sparkles:";
            }
        }
    }
}

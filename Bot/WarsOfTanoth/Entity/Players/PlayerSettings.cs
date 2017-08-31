using Discord.Commands;
using MrRestructoid.Bot.InteractiveMessages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Discord;
using MrRestructoid.Bot.WarsOfTanoth.Items.Gear;

namespace MrRestructoid.Bot.WarsOfTanoth.Entity.Players
{
    public class PlayerSettingsCommand : ModuleBase
    {
        [Command("settings")]
        public async Task Settings()
        {
            Player player = await Program.handler.GetPlayer(Context);
            if (player == null) return;

            IUserMessage message = await Context.Channel.SendMessageAsync(PlayerSettings.SettingsText.text);

            Program.IMessages.Add(new PlayerSettings(Context.User, Context.Channel, message, player));
        }
    }

    public class PlayerSettings : InteractMessage
    {
        public static List<string> ChangingNames = new List<string>();

        private string ConfirmationSeed { get; set; }

        public Player Player { get; set; }

        private string ChangeNameTo { get; set; }

        public PlayerSettings(IUser user, IMessageChannel channel, IUserMessage message, Player player) : base(user, channel, message)
        {
            Player = player;
        }

        override protected async Task Process(string input)
        {
            switch (Navigation.Count)
            {
                case 0:

                    if (input == "1" || input.ToLower() == "profile settings")
                    {
                        await SendMessage(SettingsText.ProfileSettingsText.text);
                        Navigation.Add(1);
                    }
                    else if (input == "2" || input.ToLower() == "alert settings")
                    {
                        await SendMessage(SettingsText.AlertSettingsText.text);
                        Navigation.Add(2);
                    }
                    else if (input == "3")
                    {
                        state = IMessageState.Closed;
                        await message.DeleteAsync();
                        return;
                    }

                    break;

                case 1:
                    //Profile Settings
                    if (Navigation[0] == 1)
                    {
                        //Reset Name
                        if (input == "1")
                        {
                            if (Player.Character.NameChangedAt != default(DateTime))
                            {
                                TimeSpan passedTime = DateTime.Now - Player.Character.NameChangedAt;

                                if (passedTime.Days != 30)
                                {
                                    await SendMessage(string.Format("You have changed your Character Name recently. Wait {0} More Day{1}.",
                                        (30 - passedTime.Days),
                                        passedTime.Days != 1 ? "s" : string.Empty));

                                    state = IMessageState.Closed;
                                    return;
                                }
                            }

                            await SendMessage(SettingsText.ProfileSettingsText.ResetNameText.text);
                            Navigation.Add(1);
                        }
                        //Reset Profile
                        else if (input == "2")
                        {
                            await SendMessage(SettingsText.ProfileSettingsText.ResetProfileText.text);
                            Navigation.Add(2);
                        }
                    }
                    //Alert Settings
                    else if(Navigation[0] == 2)
                    {
                        if (input.ToLower() == "alerts" ||input == "1")
                        {
                            await SendMessage(SettingsText.AlertSettingsText.options);
                            Navigation.Add(1);
                        }
                        else if (input.ToLower() == "additional alerts" || input == "2")
                        {
                            await SendMessage(SettingsText.AlertSettingsText.options);
                            Navigation.Add(2);
                        }
                    }
                    break;

                case 2:
                    //Profile Settings
                    if (Navigation[0] == 1)
                    {
                        //Reset Name
                        if (Navigation[1] == 1)
                        {
                            //yes
                            if (input == "1")
                            {
                                await SendMessage(SettingsText.ProfileSettingsText.ResetNameText.text2);
                                Navigation.Add(1);
                            }
                            //no
                            else if (input == "2")
                            {
                                Navigation.Clear();
                                await SendMessage(EntryText);
                                return;
                            }
                        }
                        //Reset Profile
                        else if (Navigation[1] == 2)
                        {
                            //Proceed
                            if (input == "1")
                            {
                                for (int i = 0; i < 4; i++) ConfirmationSeed += Program.random.Next(10);

                                await SendMessage($"Enter **{ConfirmationSeed}** to confirm profile reset.\n**Beyond this point, there is no return.**");
                                Navigation.Add(1);
                            }
                            else
                            {
                                Navigation.Clear();
                                await SendMessage(EntryText);
                                return;
                            }
                        }
                    }
                    //Alert Settings
                    else if (Navigation[0] == 2)
                    {
                        //Alerts
                        if (Navigation[1] == 1)
                        {
                            if (input.ToLower() == "yes" || input == "1")
                            {
                                if (!Player.User.alertsOn)
                                {
                                    Player.User.alertsOn = true;
                                    await SendMessage("Alerts state has been changed to `On` state.");
                                    state = IMessageState.Closed;
                                }
                                else
                                {
                                    await SendMessage("You have Alerts set to `On` already.");
                                    state = IMessageState.Closed;
                                }
                            }
                            else if(input.ToLower() == "no" || input == "2")
                            {
                                if (Player.User.alertsOn)
                                {
                                    Player.User.alertsOn = false;
                                    await SendMessage("Alerts state has been changed to `Off` state.");
                                    state = IMessageState.Closed;
                                }
                                else
                                {
                                    await SendMessage("You have Alerts set to `Off` already.");
                                    state = IMessageState.Closed;
                                }
                            }
                        }
                        //Additional Alerts
                        else if (Navigation[1] == 2)
                        {
                            if (input.ToLower() == "yes" || input == "1")
                            {
                                if (!Player.User.additionalAlertsOn)
                                {
                                    Player.User.additionalAlertsOn = true;
                                    await SendMessage("Additional Alerts state has been changed to `On` state.");
                                    state = IMessageState.Closed;
                                }
                                else
                                {
                                    await SendMessage("You have Additional Alerts set to `On` already.");
                                    state = IMessageState.Closed;
                                }
                            }
                            else if (input.ToLower() == "no" || input == "2")
                            {
                                if (Player.User.additionalAlertsOn)
                                {
                                    Player.User.additionalAlertsOn = false;
                                    await SendMessage("Additional Alerts state has been changed to `Off` state.");
                                    state = IMessageState.Closed;
                                }
                                else
                                {
                                    await SendMessage("You have Additional Alerts set to `Off` already.");
                                    state = IMessageState.Closed;
                                }
                            }
                        }
                    }
                    break;

                case 3:
                    //Profile Settings
                    if (Navigation[0] == 1)
                    {
                        //Reset Name
                        if (Navigation[1] == 1)
                        {
                            if (Navigation[2] == 1)
                            {
                                ChangeNameTo = input;

                                if (Program.handler.Players.Exists(x => x.Character.Name.ToLower() == input.ToLower()))
                                {
                                    await SendMessage("There is a player with such Character Name already.\nPlease choose something different:");
                                }
                                else if (input.Length > 15)
                                {
                                    await SendMessage("Your chosen name is too long.\nPlease choose something different:");
                                }
                                else if (input.Length < 5)
                                {
                                    await SendMessage("Your chosen name is too short.\nPlease choose something different:");
                                }
                                else if (input.Contains("rank") || input.Contains("back"))
                                {
                                    await SendMessage("Names with **rank** or **back** in them are impossible to create");
                                }
                                else
                                {
                                    ChangingNames.Add(ChangeNameTo);

                                    await SendMessage(
                                        $"Are you sure you want to change your character name from **{Player.Character.Name}** to **{ChangeNameTo}**?\n" +
                                        $":one: ⇒ ✅\n" +
                                        $":two: ⇒ ❌\n" +
                                        $"**This warning is the last and final. You won't be able to change your name for 30 days.**");
                                    Navigation.Add(1);
                                }
                            }
                            else if (Navigation[2] == 2)
                            {
                                Navigation.Clear();
                                await SendMessage(EntryText);
                                return;
                            }
                        }
                        //Reset Profile
                        if(Navigation[1] == 2)
                        {
                            if(input == ConfirmationSeed)
                            {
                                Player resetPlayer = new Player();
                                resetPlayer.User.UserID = Player.User.UserID;
                                resetPlayer.User.ServerID = Player.User.ServerID;
                                resetPlayer.User.alertsOn = Player.User.alertsOn;
                                resetPlayer.User.additionalAlertsOn = Player.User.additionalAlertsOn;
                                resetPlayer.Character.Name = Player.Character.Name;
                                resetPlayer.Character.NameChangedAt = Player.Character.NameChangedAt;

                                resetPlayer.Character.Gear.Sword = new GearSword(GearSword.Short_Sword);
                                resetPlayer.Character.Gear.Helmet = new GearHelmet(GearHelmet.Leather_Cap);
                                resetPlayer.Character.Gear.Chestplate = new GearChestplate(GearChestplate.Leather_Armor);
                                resetPlayer.Character.Gear.Leggings = new GearLeggings(GearLeggings.Leather_Leggings);
                                resetPlayer.Character.Gear.Gloves = new GearGloves(GearGloves.Leather_Gloves);
                                resetPlayer.Character.Gear.Boots = new GearBoots(GearBoots.Leather_Boots);

                                if (Player.Character.Titles.Find(x => x.name == "Alpha Player") != null)
                                {
                                    resetPlayer.Character.Titles.Add(ITitle.Titles.AlphaPlayer);
                                }

                                Program.handler.Players.Remove(Program.handler.Players.Find(x => x.User.UserID == Player.User.UserID));
                                Program.handler.Players.Insert(0, resetPlayer);

                                await SendMessage("Your profile has been successfully resetted!");
                            }
                            else if(input != ConfirmationSeed)
                            {
                                await SendMessage("Confirmation seeds did not match. Profile restart aborted.");
                            }
                            state = IMessageState.Closed;
                        }
                    }
                    break;

                case 4:
                    //Profile Settings
                    if (Navigation[0] == 1)
                    {
                        //Reset Name
                        if (Navigation[1] == 1 && Navigation[2] == 1)
                        {
                            if (Navigation[3] == 1)
                            {
                                Player.Character.Name = ChangeNameTo;
                                Player.Character.NameChangedAt = DateTime.Now;
                                ChangingNames.Remove(ChangeNameTo);

                                await SendMessage($"Your name has been successfully changed to **{Player.Character.Name}**!");
                                state = IMessageState.Closed;
                            }
                            else if (Navigation[3] == 2)
                            {
                                ChangingNames.Remove(ChangeNameTo);
                                Navigation.Clear();
                                await SendMessage(SettingsText.text);
                                return;
                            }
                        }
                    }
                    break;
            }
        }

        public static class SettingsText
        {
            public readonly static string text =
                $":one: ⇒ Profile Settings\n" +
                $":two: ⇒ Alert Settings\n" +
                $":three: ⇒:back:";

            public static class ProfileSettingsText
            {
                public readonly static string text =
                    $":one: ⇒ Reset Name  *( Only available every 30 days )*\n" +
                    $":two: ⇒ Reset Profile\n";

                public static class ResetNameText
                {
                    public readonly static string text =
                                        $"**Are you sure you want to do this?**\nYou will not be able to Reset your name again in 30 days.\n" +
                                        $":one: ⇒ ✅\n" +
                                        $":two: ⇒ ❌\n";

                    public readonly static string text2 = $"Type chosen name:";
                }

                public static class ResetProfileText
                {
                    public readonly static string text =
                                        $"**Are you sure you want to do this?**\nYour progress will be **deleted** and old data won't be **restorable**.\n" +
                                        $":one: ⇒ ✅\n" +
                                        $":two: ⇒ ❌\n";
                }
            }

            public static class AlertSettingsText
            {
                public readonly static string text =
                    $":one: ⇒ Alerts\n" +
                    $":two: ⇒ Additional Alerts\n";

                public readonly static string options =
                    $":one: ⇒ Turn on\n" +
                    $":two: ⇒ Turn off";
            }
        }
    }
}

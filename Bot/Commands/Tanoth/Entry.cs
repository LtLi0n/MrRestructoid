using Discord.Commands;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.Items.Gear;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.Commands.Tanoth
{
    public class ProfileCreator
    {
        public string Name { get; private set; }
        public ulong ID { get; private set; }
        public ulong SERVER_ID { get; private set; }

        public ICommandContext Context { get; private set; }

        public ProfileCreator(string Name, ulong ID, ulong SERVER_ID)
        {
            this.Name = Name;
            this.ID = ID;
            this.SERVER_ID = SERVER_ID;
        }

        public ProfileCreator(string Name, ICommandContext Context)
        {
            this.Name = Name;
            ID = Context.User.Id;
            SERVER_ID = Context.Guild.Id;

            this.Context = Context;
        }
    }

    public class Entry : ModuleBase
    {
        public static async Task CreateProfile(ProfileCreator profile)
        {
            bool noErrors = true;

            if (profile.Context != null)
            {
                noErrors = false;

                if (profile.Name == null)
                {
                    await profile.Context.Channel.SendMessageAsync("You have not entered the name for your character.");
                }
                else if (profile.Name.Length > 15)
                {
                    await profile.Context.Channel.SendMessageAsync($"**The maximum amount of letters in character name is 15.**");
                }
                else if (profile.Name.Length < 5)
                {
                    await profile.Context.Channel.SendMessageAsync("Chosen name must be longer than 5 characters.");
                }
                else
                {
                    foreach (char x in profile.Name.ToCharArray())
                    {
                        if (x == ' ') continue;
                        else if (x < 48 || x > 126)
                        {
                            await profile.Context.Channel.SendMessageAsync($":x:  Some or all letters are unsupported in the name - **{profile.Name}** :x:"); return;
                        }
                    }

                    if (profile.Name.ToLower().Contains("rank") || profile.Name.ToLower().Contains("back"))
                    {
                        await profile.Context.Channel.SendMessageAsync($"Can't create profile with a name that contains **rank** or **back**.");
                    }
                    else
                    {
                        noErrors = true;
                    }
                }
            }

            if (noErrors)
            {
                if (Program.handler.Players.Exists(x => x.User.UserID == profile.Context.User.Id))
                {
                    if (profile.Context != null)
                    {
                        await profile.Context.Channel.SendMessageAsync($"You already have profile a created. Go to `{Program.PREFIX}settings` to reset your profile.");
                    }
                }
                else if (Program.handler.Players.Exists(x => x.Character.Name == profile.Name))
                {
                    if(profile.Context != null)
                    {
                        await profile.Context.Channel.SendMessageAsync("Profile with such name already exists.");
                    }
                }
                else
                {
                    Player player = new Player();

                    player.User.ServerID = profile.SERVER_ID;
                    player.User.UserID = profile.ID;

                    player.Character.Name = profile.Name;

                    player.Character.Gear.Sword = new GearSword(GearSword.Short_Sword);
                    player.Character.Gear.Helmet = new GearHelmet(GearHelmet.Leather_Cap);
                    player.Character.Gear.Chestplate = new GearChestplate(GearChestplate.Leather_Armor);
                    player.Character.Gear.Leggings = new GearLeggings(GearLeggings.Leather_Leggings);
                    player.Character.Gear.Gloves = new GearGloves(GearGloves.Leather_Gloves);
                    player.Character.Gear.Boots = new GearBoots(GearBoots.Leather_Boots);

                    Program.handler.Players.Insert(0, player);

                    if (profile.Context != null)
                    {
                        await profile.Context.Channel.SendMessageAsync(
                            $"***{player.Character.Name}*** ** succesfully created!**\n" +
                            $"Check {Program.PREFIX}help to learn more!" +
                            $"\n`Note: You have alerts on by default meaning you will receive updates about the game. If you wish to turn this feature off, write {Program.PREFIX}settings`, navigate to `alerts` and turn them off");
                    }
                }
            }
        }

        [Command("CreateProfile"), Summary("CreateProfile [char_name]")]
        public async Task CreateProfileCommand([Remainder] string name = null)
        {
            await CreateProfile(new ProfileCreator(name, Context));
        }
    }
}

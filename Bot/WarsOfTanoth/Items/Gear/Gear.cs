using Discord;
using Discord.Commands;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Items.Gear
{
    public abstract class IGear
    {
        private enum XP_ConversionType { ToLevelXp, ToXpCap, ToLevel }

        [JsonIgnore] public string Name { get; set; }
        public int ID { get; set; }
        [JsonIgnore] public int Level { get; set; }
        [JsonIgnore] public int MinLevel { get; set; }
        public ulong TOTAL_XP = 0;

        [JsonIgnore] public ulong XP => ConvertTotalXpTo(XP_ConversionType.ToLevelXp);
        [JsonIgnore] public int EnchantedLevel => (int)ConvertTotalXpTo(XP_ConversionType.ToLevel);
        [JsonIgnore] public ulong XpCap => ConvertTotalXpTo(XP_ConversionType.ToXpCap);

        private ulong ConvertTotalXpTo(XP_ConversionType type)
        {
            ulong tempXp = TOTAL_XP;
            ulong tempLevel = 0;
            ulong tempXpCap = 1000;

            while (tempXp >= tempXpCap)
            {
                tempXp -= tempXpCap;
                tempLevel++;
                tempXpCap += tempLevel * 1000;
            }

            switch(type)
            {
                case XP_ConversionType.ToLevel: return tempLevel;
                case XP_ConversionType.ToLevelXp: return tempXp;
                default: return tempXpCap;
            }
        }
    }

    public class Gear
    {
        public GearSword Sword { get; set; }

        public GearHelmet Helmet { get; set; }
        public GearChestplate Chestplate { get; set; }
        public GearLeggings Leggings { get; set; }
        public GearBoots Boots { get; set; }
        public GearGloves Gloves { get; set; }

        public Gear() { }

        [JsonConstructor]
        public Gear(GearSword sword, GearHelmet helmet, GearChestplate chestplate, GearLeggings leggings, GearBoots boots, GearGloves gloves)
        {
            Sword = sword;
            Helmet = helmet;
            Chestplate = chestplate;
            Leggings = leggings;
            Boots = boots;
            Gloves = gloves;
        }
    }

    public class GearCommand : ModuleBase
    {
        private EmbedFieldBuilder GetFieldFromProtGear(ProtectionGear gear)
        {
            EmbedFieldBuilder field = new EmbedFieldBuilder();

            field.Name = $"{gear.Name}   ({gear.MinLevel / 5 + 1})";
            field.Value = $"```swift\nProtection: {gear.Protection}\n" +
                    $"Minimum_Level: {gear.MinLevel}\n" +
                    $"Power_Level: {gear.EnchantedLevel} [{gear.XP} / {gear.XpCap}]```";

            field.IsInline = true;

            return field;
        }

        [Command("gear")]
        public async Task Gear(string action = "", string piece = "", string amount = "")
        {
            Player player = await Program.handler.GetPlayer(Context);
            if (player == null) return;

            action = action.ToLower();
            amount = amount.ToLower();

            //Show Gear
            if (action == "" && piece == "")
            {
                EmbedBuilder eb = new EmbedBuilder();

                eb.Color = new Color((int)(((Player.LevelCap / 100.0) * player.Character.Level) * (255 / Player.LevelCap)), 0, 0);

                eb.WithAuthor((x) =>
                {
                    x.IconUrl = Context.User.GetAvatarUrl();
                    x.Name = "Gear Stats";
                });

                eb.Title = "\u200b";

                eb.AddField((x) =>
                {
                    x.Name = $"{player.Character.Gear.Sword.Name}   ({player.Character.Gear.Sword.MinLevel / 5 + 1})";
                    x.Value = $"```swift\nDamage: {player.Character.Gear.Sword.Damage}\n" +
                    $"Minimum_Level: {player.Character.Gear.Sword.MinLevel}\n" +
                    $"Power_Level: {player.Character.Gear.Sword.EnchantedLevel} [{player.Character.Gear.Sword.XP} / {player.Character.Gear.Sword.XpCap}]```";
                    x.IsInline = true;
                });

                eb.AddField(GetFieldFromProtGear(player.Character.Gear.Helmet));
                eb.AddField(GetFieldFromProtGear(player.Character.Gear.Chestplate));
                eb.AddField(GetFieldFromProtGear(player.Character.Gear.Leggings));
                eb.AddField(GetFieldFromProtGear(player.Character.Gear.Boots));
                eb.AddField(GetFieldFromProtGear(player.Character.Gear.Gloves));

                eb.WithFooter((e) =>
                {
                    e.Text = $"Total Protection: {player.Character.Protection}";
                }).Build();

                await ((IGuildUser)Context.User).SendMessageAsync("", embed: eb.Build());
            }
            else if (action == "upgrade")
            {
                await Context.Channel.SendMessageAsync($"You have successfully upgraded your **{piece.Substring(0,1).ToUpper() + piece.Remove(0,1)}** to tier **{player.Character.Inventory.UpgradeGear(piece)}**.");
            }
            else
            {
                await Library.Say_CommandFailed(Context,
                    new CommandUsage("gear", "Gets info about your gear"),
                    new CommandUsage("gear upgrade [gear_piece]", "Upgrade gear using rubies in your inventory."));
            }
        }
    }
}

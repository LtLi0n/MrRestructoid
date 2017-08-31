using System;
using System.Collections.Generic;
using System.Text;
using Discord;
using System.Threading.Tasks;
using Discord.WebSocket;
using MrRestructoid.Bot.Main;
using MrRestructoid.Bot.WarsOfTanoth.Entity.Players;
using MrRestructoid.Bot.WarsOfTanoth.Items;

namespace MrRestructoid.Bot.WarsOfTanoth.System
{
    public enum SkillField { Mining, WoodCutting, Fishing }

    public class Professions
    {
        public IUserMessage msg { get; set; }
        public IMessageChannel channel { get; private set; }
        public IGuild guild { get; private set; }

        public List<Field> Fields { get; private set; }

        public async Task Update()
        {
            foreach(Field f in Fields)
            {
                await f.Update();
            }
        }

        public async Task ProcessReaction(SocketReaction sr, bool reactionAdded = true)
        {
            Player player = await Program.handler.GetPlayer(sr.UserId);

            if (player == null) return;

            foreach(Field field in Fields)
            {
                if(field.targetEmoji.Name == sr.Emote.Name)
                {
                    if(!field.ReactedPlayers.Contains(player))
                    {
                        if(reactionAdded)
                        {
                            field.ReactedPlayers.Add(player);

                            await guild.GetUserAsync(player.User.UserID).Result.AddRoleAsync(guild.GetRole(field.ROLE_ID));
                        }
                    }
                    else if(!reactionAdded)
                    {
                        if(field.ReactedPlayers.Contains(player))
                        {
                            field.ReactedPlayers.Remove(player);

                            await guild.GetUserAsync(player.User.UserID).Result.RemoveRoleAsync(guild.GetRole(field.ROLE_ID));
                        }
                    }
                }
            }
        }

        public EmbedBuilder EmbedBuilder => GetEmbedBuilder();

        public string EmbedBuilderDescription { get; set; }

        private EmbedBuilder GetEmbedBuilder()
        {
            EmbedBuilder eb = new EmbedBuilder();

            eb.Description = EmbedBuilderDescription;

            foreach(Field f in Fields)
            {
                eb.AddField(f.EmbedField);
            }

            return eb;
        }

        public async Task ManageReactions()
        {
            foreach(Field f in Fields)
            {
                if(!f.active)
                {
                    bool sent = false;

                    while(true)
                    {
                        try
                        {
                            await msg.RemoveAllReactionsAsync();
                            break;
                        }
                        catch (Exception e)
                        {
                            if (!sent)
                            {
                                await ((IMessageChannel)guild.GetChannelAsync(280769235549159426).Result).SendMessageAsync(e.Message);
                                sent = true;
                            }
                        }
                    }

                    break;
                }
            }

            foreach (Field f in Fields)
            {
                if (f.active)
                {
                    bool sent = false;

                    while (true)
                    {
                        try
                        {
                            await msg.AddReactionAsync(f.targetEmoji);
                            break;
                        }
                        catch(Exception e)
                        {
                            if(!sent)
                            {
                                await ((IMessageChannel)guild.GetChannelAsync(280769235549159426).Result).SendMessageAsync(e.Message);
                                sent = true;
                            }
                        }
                    }
                }
            }
        }

        public Professions()
        {
            Fields = new List<Field>();

            try
            {
                Task.Run(async () =>
                {
                    guild = Program.client.GetGuild(279715709792288775);
                    channel = await guild.GetChannelAsync(349298772460699650) as IMessageChannel;

                    IEnumerable<IMessage> messages = await channel.GetMessagesAsync().Flatten();

                    foreach (IMessage m in messages)
                    {
                        if (m.Author.Id == Program.client.CurrentUser.Id)
                        {
                            await m.DeleteAsync();
                        }
                    }

                    Fields.Add(new Field(SkillField.Mining, 1, this,
                        new Field.Reward(
                            new Field.Reward.RewardItem(Item.Ruby.Ruby_Level_3, 3),
                            new Field.Reward.RewardItem(Item.Ruby.Ruby_Level_4, 1)
                            )));

                    Fields.Add(new Field(SkillField.WoodCutting, 1, this,
                        new Field.Reward(
                            new Field.Reward.RewardItem(Item.WoodCutting.Oak_Logs, 3),
                            new Field.Reward.RewardItem(Item.WoodCutting.Willow_Logs, 1)
                            )));

                    Fields.Add(new Field(SkillField.Fishing, 1, this,
                        new Field.Reward(
                            new Field.Reward.RewardItem(Item.Fishing.Raw_Shrimp, 8),
                            new Field.Reward.RewardItem(Item.Fishing.Raw_Trout, 1)
                            )));

                    msg = await channel.SendMessageAsync("", embed: EmbedBuilder.Build());

                    await ManageReactions();

                    msg = await channel.GetMessageAsync(msg.Id) as IUserMessage;

                    IReadOnlyCollection<IGuildUser> users = await guild.GetUsersAsync();
                    foreach (IGuildUser user in users)
                    {
                        foreach(ulong roleID in user.RoleIds)
                        {
                            foreach(Field f in Fields)
                            {
                                if(roleID == f.ROLE_ID)
                                {
                                    await user.RemoveRoleAsync(guild.GetRole(roleID));
                                }
                            }
                        }
                    }

                }).GetAwaiter().GetResult();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public class Field
        {
            public SkillField Type { get; private set; }
            public EmbedFieldBuilder EmbedField { get; private set; }
            public Professions Professions { get; private set; }
            public List<Player> ReactedPlayers { get; private set; }

            public Emoji targetEmoji { get; set; }

            public bool active { get; private set; }
            public int Tier { get; private set; }

            public Reward Rewards { get; private set; }

            private int timer { get; set; }

            private int timerActive { get; set; }
            private int timerIdle { get; set; }

            public ulong ROLE_ID => GetRoleID(this);

            public async Task GetRewards(bool setActive = true, Player includeIntoRewards = null)
            {
                if(includeIntoRewards != null)
                {
                    if(!ReactedPlayers.Contains(includeIntoRewards))
                    {
                        ReactedPlayers.Add(includeIntoRewards);
                    }
                }

                if(setActive)
                {
                    active = true;
                    timer = timerActive - 1;
                }

                await Update();

                if(setActive)
                {
                    timer = timerIdle - 1;
                }
            }

            private static ulong GetRoleID(Field field)
            {
                if (field.Tier == 1)
                {
                    switch (field.Type)
                    {
                        case SkillField.Mining: return 349875313003724800;
                        case SkillField.WoodCutting: return 349875385342885889;
                        case SkillField.Fishing: return 349875417106087936;
                    }
                }

                return 0;
            }

            private enum TimerType { idle, active }

            private int getNewTimer(TimerType timerType)
            {
                switch(timerType)
                {
                    case TimerType.active: return Program.random.Next(61) + 120;
                    case TimerType.idle: return Program.random.Next(361) + 480;
                }

                return -1;
            }

            public Field(SkillField field, int Tier, Professions professions, Reward rewards = null)
            {
                active = true;

                ReactedPlayers = new List<Player>();

                Type = field;
                this.Tier = Tier;

                EmbedField = new EmbedFieldBuilder();
                Professions = professions;

                Rewards = rewards;

                timerIdle = getNewTimer(TimerType.idle);
                timerActive = getNewTimer(TimerType.active);

                SetupField(true);
            }

            public async Task Update(bool modifyField = true)
            {
                timer++;

                if(active)
                {
                    if(timer == timerActive)
                    {
                        Random random = new Random();

                        timer = 0;
                        active = false;
                        timerActive = getNewTimer(TimerType.active);
                        if (modifyField) SetupField();

                        foreach (Player p in ReactedPlayers)
                        {
                            await Professions.guild.GetUserAsync(p.User.UserID).Result.RemoveRoleAsync(Professions.guild.GetRole(ROLE_ID));

                            if (Type == SkillField.Mining)
                            {
                                if(p.Character.Skills.Mining.Level >= (Tier - 1) * 10)
                                {
                                    p.Character.Skills.Mining.TOTAL_XP += Tier * 50;
                                }
                            }
                            else if(Type == SkillField.WoodCutting)
                            {
                                if (p.Character.Skills.WoodCutting.Level >= (Tier - 1) * 10)
                                {
                                    p.Character.Skills.WoodCutting.TOTAL_XP += Tier * 50;
                                }
                            }
                            else if(Type == SkillField.Fishing)
                            {
                                if (p.Character.Skills.Fishing.Level >= (Tier - 1) * 10)
                                {
                                    p.Character.Skills.Fishing.TOTAL_XP += Tier * 50;
                                }
                            }

                            if(Rewards != null)
                            {
                                p.Character.Inventory.AddItem(Rewards.PullRandomReward());
                            }
                        }

                        ReactedPlayers.Clear();
                    }
                }
                else
                {
                    if (timer == timerIdle)
                    {
                        Random random = new Random();

                        timer = 0;
                        active = true;
                        timerIdle = getNewTimer(TimerType.idle);

                        if (modifyField) SetupField();
                    }
                }
            }

            private bool FoundReaction(bool OnLoad)
            {
                if (OnLoad) return false;

                foreach (KeyValuePair<IEmote, ReactionMetadata> reaction in Professions.msg.Reactions)
                {
                    if (reaction.Key.Name == targetEmoji.Name)
                    {
                        return true;
                    }
                }

                return false;
            }

            public void SetupField(bool OnLoad = false)
            {
                Professions.EmbedBuilderDescription = $"```swift\n Tier {Tier} Skilling Field.```";

                switch (Type)
                {
                    case SkillField.Mining:
                        EmbedField.Name = $"Mining Field";
                        targetEmoji = Library.Emojis.pick;
                        break;

                    case SkillField.WoodCutting:
                        EmbedField.Name = $"WoodCutting Field";
                        targetEmoji = Library.Emojis.evergreen_tree;
                        break;

                    case SkillField.Fishing:
                        EmbedField.Name = $"Fishing Field";
                        targetEmoji = Library.Emojis.fishing_pole_and_fish;
                        break;
                }

                if (active)
                {
                    EmbedField.Name += " ✅";

                    if(Rewards != null)
                    {
                        if (Rewards.RewardItems.Length > 0)
                        {
                            EmbedField.Value = "\n";

                            for (int i = 0; i < Rewards.RewardItems.Length; i++)
                            {
                                if (Rewards.RewardItems.Length - 1 != i)
                                {
                                    EmbedField.Value += $"{Rewards.RewardItems[i].item.Item.Name}, ";
                                }
                                else
                                {
                                    EmbedField.Value += $"{Rewards.RewardItems[i].item.Item.Name}.";
                                }
                            }
                        }
                    }
                    else
                    {
                        EmbedField.Value = "*No Rewards*";
                    }
                }
                else
                {
                    EmbedField.Name += " 💤";
                    EmbedField.Value = "\u200b";
                }

                if(!OnLoad)
                {
                    Professions.msg.ModifyAsync((x) =>
                    {
                        x.Embed = new Optional<Embed>(Professions.EmbedBuilder.Build());
                    }).GetAwaiter().GetResult();

                    Professions.ManageReactions().GetAwaiter().GetResult();

                    Professions.msg = Professions.channel.GetMessageAsync(Professions.msg.Id).GetAwaiter().GetResult() as IUserMessage;
                }
            }

            public class Reward
            {
                public RewardItem[] RewardItems { get; private set; }
                private int TotalChance { get; set; }

                public Reward(params RewardItem[] rewards)
                {
                    RewardItems = rewards;

                    foreach(RewardItem ri in RewardItems)
                    {
                        TotalChance += ri.chance;
                    }
                }

                public class RewardItem
                {
                    public InventoryItem item { get; private set; }
                    public int chance { get; private set; }

                    public RewardItem(Item item, int chance, int amount = 1)
                    {
                        this.item = new InventoryItem(item.ID, amount);
                        this.chance = chance;
                    }
                }

                public InventoryItem PullRandomReward()
                {
                    int reward = Program.random.Next(TotalChance) + 1;

                    foreach(RewardItem ri in RewardItems)
                    {
                        reward -= ri.chance;

                        if(reward <= 0)
                        {
                            return ri.item;
                        }
                    }

                    return null;
                }
            }
        }
    }
}

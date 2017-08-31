using Discord.Commands;
using MrRestructoid.Bot.Main;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MrRestructoid.Bot.WarsOfTanoth.Entity.Players
{
    public class PvP : ModuleBase
    {
        [Command("bounties")]
        [RequireOwner()]
        public async Task DisplayBounties()
        {
            string toReturn = "";
            int longestName = int.MinValue;

            if (!Program.handler.Players.Exists(x => x.Character.Stats.PvP.reasonlessKillTimer > 0)) { await Context.Channel.SendMessageAsync("**No players with bounties at the moment.**"); return; }

            Player[] playersWithBounty = Program.handler.Players.FindAll(x => x.Character.Stats.PvP.reasonlessKillTimer > 0).ToArray();

            //playersWithBounty = PlayerSort.SortBy(playersWithBounty, SortType.PvP);

            foreach (Player p in playersWithBounty)
            {
                if (p.Character.Name.Length > longestName) longestName = p.Character.Name.Length;
            }


            toReturn += ">~[ **Bounties** ]~<\n";

            for (int i = playersWithBounty.Length - 1; i >= 0; i--)
            {
                string name = playersWithBounty[i].Character.Name;

                while (name.Length < longestName) { name += " "; }

                toReturn += $":chains: **{name}** :chains:\n";
            }

            await Context.Channel.SendMessageAsync(toReturn);

        }

        [Command("pvp")]
        public async Task PvP_Command([Remainder]string name = null)
        {
            bool cancelPvP = true;

            Player targetPlayer = await Program.handler.GetPlayer(Context, name);

            Random random = new Random();

            bool killForBounty = false;

            Player attacker = await Program.handler.GetPlayer(Context);
            if(attacker == null) return;
            if(name != null && targetPlayer == null) return;

            if (attacker.Character.Level < 10 && Context.User.Id != Program.ME) await Context.Channel.SendMessageAsync("You must be level **10** or above to attack other players.");

            else if (string.IsNullOrEmpty(name))
            {
                if (attacker.Character.Stats.PvP.pvpCooldown != 0)
                {
                    await Context.Channel.SendMessageAsync($"You are in PvP cooldown. `{Library.GetTimeString(attacker.Character.Stats.PvP.pvpCooldown % 60, attacker.Character.Stats.PvP.pvpCooldown / 60)}`");
                    return;
                }
                else
                {
                    await Context.Channel.SendMessageAsync("You don't have a PvP cooldown on you.");
                    return;
                }
            }
            else if (targetPlayer == null) return;

            else if (targetPlayer.Character.Level < 10 && Context.User.Id != Program.ME) await Context.Channel.SendMessageAsync("You cannot attack players, who have lower level than **10**.");

            else if (targetPlayer.Character.Name.ToLower() == attacker.Character.Name.ToLower()) await Context.Channel.SendMessageAsync("You cannot attack yourself!");

            else if (attacker.Character.HP == 0 && attacker.Character.Stats.HP.dead) await Context.Channel.SendMessageAsync(":skull: You cannot attack other players while you are dead. :skull:");
            else if (attacker.Character.HP == 0) await Context.Channel.SendMessageAsync("You have just revived and still have **0** HP.");
            else if (targetPlayer.Character.HP == 0 && targetPlayer.Character.Stats.HP.dead) await Context.Channel.SendMessageAsync(":skull: This player is dead. :skull:");
            else if (targetPlayer.Character.HP == 0) await Context.Channel.SendMessageAsync("This player has just revived and still has **0** hp.");

            else if (attacker.Character.Stats.Work.isWorking) await Context.Channel.SendMessageAsync($"You are still working. Type `{Program.PREFIX}work cancel` or wait to finish.");
            else if (attacker.Character.Stats.Quests.isInQuest) await Context.Channel.SendMessageAsync($"You are on a quest. Type `{Program.PREFIX}quest cancel` or wait to finish.");

            else if (attacker.Character.Stats.PvP.pvpCooldown != 0)
            {
                int minutes = attacker.Character.Stats.PvP.pvpCooldown % 60;
                string minutesString = minutes.ToString();
                int hours = attacker.Character.Stats.PvP.pvpCooldown / 60;
                if (minutesString.Length == 1 && hours > 0) minutesString = "0" + minutesString;

                string toReturn = "";

                if (hours == 1) toReturn = $"{hours}:{minutesString} Hour Left.";
                else if (hours > 1) toReturn = $"{hours}:{minutesString} Hours Left.";
                else if (hours == 0 && minutes != 1) toReturn = $"{minutesString} Minutes Left.";
                else if (hours == 0 && minutes == 1) toReturn = $"{minutesString} Minute Left.";

                await Context.Channel.SendMessageAsync($"**You are in PvP cooldown.** {toReturn}");

                return;
            }

            else cancelPvP = false;

            if (cancelPvP) return;

            if (targetPlayer.Character.Stats.PvP.reasonlessKillTimer > 0) killForBounty = true;

            int attacksUsed = 0;

            int attackerProtection = attacker.Character.Protection;
            int attackerProtectionMax = attackerProtection;
            int targetProtection = targetPlayer.Character.Protection;
            int targetProtectionMax = targetProtection;

            int attackerMaxHp = attacker.Character.HP;
            int targetMaxHp = targetPlayer.Character.HP;

            bool tired = false;

            attacker.Character.Stats.PvP.pvpCooldown = 300;

            //COMBAT
            while (true)
            {
                if (attacksUsed < 15)
                {
                    int attackDecider = random.Next(100) + 1;
                    int targetAttackDecider = random.Next(100) + 1;

                    int attackerDamage = attacker.Character.Gear.Sword.Damage + attacker.Character.Level + attacker.Character.Gear.Sword.EnchantedLevel;
                    int targetDamage = targetPlayer.Character.Gear.Sword.Damage + targetPlayer.Character.Level + targetPlayer.Character.Gear.Sword.EnchantedLevel;

                    if (attackDecider < 51) attackerDamage /= 4;
                    else if (attackDecider > 50 && attackDecider < 85) attackerDamage /= 2;

                    if (targetAttackDecider <= 50) targetDamage /= 4;
                    else if (targetAttackDecider > 50 && targetAttackDecider <= 85) targetDamage /= 2;

                    //ATTACKER COMBAT MOVE
                    if (attacker.Character.HP > 0)
                    {
                        //WITH PROTECTION
                        if (targetProtection - attackerDamage >= 0)
                        {
                            targetProtection -= attackerDamage;
                        }
                        else if (targetProtection - attackerDamage < 0 && targetProtection > 0)
                        {
                            int margin = attackerDamage - targetProtection;
                            targetProtection = 0;

                            if (targetPlayer.Character.HP - margin < 0) targetPlayer.Character.HP = 0;
                            else targetPlayer.Character.HP -= margin;
                        }
                        //WITHOUT PROTECTION 
                        else if (targetProtection == 0)
                        {
                            if (targetPlayer.Character.HP - attackerDamage < 0) targetPlayer.Character.HP = 0;
                            else targetPlayer.Character.HP -= attackerDamage;
                        }
                    }
                    //IF ATTACKER GOT KILLED
                    else
                    {
                        attacker.Character.Stats.HP.dead = true;
                        attacker.Character.HP = 0;
                        break;
                    }

                    //TARGET COMBAT RESPONSE
                    if (targetPlayer.Character.HP > 0)
                    {
                        //WITH PROTECTION
                        if (attackerProtection - targetDamage >= 0)
                        {
                            attackerProtection -= targetDamage;
                        }
                        else if (attackerProtection - targetDamage < 0 && attackerProtection > 0)
                        {
                            int margin = targetDamage - attackerProtection;
                            attackerProtection = 0;

                            if (attacker.Character.HP - margin < 0) targetPlayer.Character.HP = 0;
                            else attacker.Character.HP -= margin;
                        }
                        //WITHOUT PROTECTION 
                        else if (attackerProtection == 0)
                        {
                            if (attacker.Character.HP - targetDamage < 0) attacker.Character.HP = 0;
                            else attacker.Character.HP -= targetDamage;
                        }
                    }
                    //IF TARGET GOT KILLED
                    else
                    {
                        targetPlayer.Character.Stats.Quests.isInQuest = false;
                        targetPlayer.Character.Stats.Quests.minutesInQuest = 0;
                        targetPlayer.Character.Stats.Work.isWorking = false;
                        targetPlayer.Character.Stats.Work.workProgress = 0;

                        targetPlayer.Character.Stats.HP.dead = true;
                        targetPlayer.Character.HP = 0;
                        break;
                    }

                    attacksUsed++;
                }
                else
                {
                    tired = true;
                    break;
                }
            }

            //REPORTING BOTH - ATTACKER AND THE TARGET

            string toReturnAttacker = "```cs\n", toReturnTarget = "```cs\n";

            if (targetPlayer.Character.Stats.HP.dead)
            {
                if (killForBounty)
                {
                    attacker.Character.Stats.PvP.bountyKills++;
                    attacker.Character.Stats.PvP.bountyTitleTimer += 2880;

                    targetPlayer.Character.Stats.PvP.reasonlessKillTimer = 0;
                }
                else
                {
                    attacker.Character.Stats.PvP.reasonlessKills++;
                    attacker.Character.Stats.PvP.reasonlessKillTimer += 2880;
                }
                toReturnAttacker += $"You have Killed player - {targetPlayer.Character.Name}\n";
                toReturnTarget += $"Hello, you have been Killed by the player - {attacker.Character.Name}\n";
            }
            else if (attacker.Character.Stats.HP.dead)
            {
                toReturnAttacker += $"You Died while trying to kill - {targetPlayer.Character.Name}\n";
                toReturnTarget += $"Hello, you have been attacked by the player - {attacker.Character.Name}, who got Killed in your response to the attack\n";

                targetPlayer.Character.Stats.PvP.reasonlessKills++;
            }

            if (tired)
            {
                toReturnAttacker += $"Both you and the attacker got tired from a long fight. You will get {targetPlayer.Character.Name} next time!\n";
                toReturnTarget += $"The fight was too long not only for your hero, but the attacker too!\n";
            }

            toReturnAttacker += "\nYour stats:\n";
            toReturnAttacker += $"Hp [ {attacker.Character.HP}/{attacker.Character.MAX_HP} ] - {attackerMaxHp - attacker.Character.HP}\n";
            toReturnAttacker += $"Protection: [ {attackerProtection}/{attackerProtectionMax} ] - {attackerProtectionMax - attackerProtection}\n";
            toReturnAttacker += "\nTarget stats:\n";
            toReturnAttacker += $"Hp [ {targetPlayer.Character.HP}/{targetPlayer.Character.MAX_HP} ] - {targetMaxHp - targetPlayer.Character.HP}\n";
            toReturnAttacker += $"Protection: [ {targetProtection}/{targetProtectionMax} ] - {targetProtectionMax - targetProtection}```";

            toReturnTarget += "\nYour stats:\n";
            toReturnTarget += $"Hp [ {targetPlayer.Character.HP}/{targetPlayer.Character.MAX_HP} ] - {targetMaxHp - targetPlayer.Character.HP}\n";
            toReturnTarget += $"Protection: [ {targetProtection}/{targetProtectionMax} ] - {targetProtectionMax - targetProtection}\n";
            toReturnTarget += "\nAttacker stats:\n";
            toReturnTarget += $"Hp [ {attacker.Character.HP}/{attacker.Character.MAX_HP} ] - {attackerMaxHp - attacker.Character.HP}\n";
            toReturnTarget += $"Protection: [ {attackerProtection}/{attackerProtectionMax} ] - {attackerProtectionMax - attackerProtection}```";

            toReturnTarget += "\n**+alerts additional off** to stop reports of being attacked";

            await SendMessage.Send(attacker, SendType.Message, toReturnAttacker);
            if (targetPlayer.User.additionalAlertsOn) await SendMessage.Send(targetPlayer, SendType.Message, toReturnTarget);
        }
    }
}

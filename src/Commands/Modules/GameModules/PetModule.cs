﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using PacManBot.Constants;
using PacManBot.Extensions;
using PacManBot.Games.Concrete;

namespace PacManBot.Commands.Modules.GameModules
{
    [Name(ModuleNames.Games), Remarks("3")]
    public class PetModule : BaseGameModule<PetGame>
    {
        private static readonly IEnumerable<MethodInfo> PetMethods = typeof(PetModule).GetMethods()
            .Where(x => x.Get<PetCommandAttribute>()?.VerifyMethod(x) != null)
            .ToArray();

        [AttributeUsage(AttributeTargets.Method)]
        private class RequiresPetAttribute : Attribute { }

        [AttributeUsage(AttributeTargets.Method)]
        private class PetCommandAttribute : Attribute
        {
            public string[] Names { get; }
            public PetCommandAttribute(params string[] names)
            {
                Names = names;
            }

            // Runtime check that all commands are valid
            public object VerifyMethod(MethodInfo method)
            {
                if (method.ReturnType != typeof(Task<string>) || method.GetParameters().Length != 0)
                {
                    throw new InvalidOperationException($"{method.Name} does not match the expected {GetType().Name} signature.");
                }
                return this;
            }
        }




        public string AdoptPetMessage => $"You don't have a pet yet! Do `{Context.Prefix}pet adopt` to adopt one.";

        public string ExtraArg { get; private set; }


        [Command("pet"), Alias("gotchi", "wakagotchi", "clockagotchi"), Parameters("[command]"), Priority(5)]
        [Remarks("Adopt your own pet!")]
        [Summary("**__Pet Commands__**\n\n" +
                 "**{prefix}pet** - Check on your pet or adopt if you don't have one\n" +
                 "**{prefix}pet stats [user]** - Check your pet or another's statistics and unlocks\n" +
                 "**{prefix}pet name <name>** - Name your pet\n" +
                 "**{prefix}pet image <image>** - Give your pet an image\n\n" +
                 "**{prefix}pet feed** - Fills your pet's Satiation and restores a little Energy\n" +
                 "**{prefix}pet clean** - Fills your pet's Hygiene\n" +
                 "**{prefix}pet play [user]** - Fills your pet's Happinness. It requires Energy, and consumes a little of every stat. " +
                 "You can make your pet play with another user's pet, in which case they get Happiness for free\n" +
                 "**{prefix}pet sleep/wakeup** - Sleep to restore Energy over time\n\n" +
                 "**{prefix}pet help** - This list of commands\n" +
                 "**{prefix}pet pet** - Pet your pet\n" +
                 "**{prefix}pet user <user>** - See another person's pet\n" +
                 "**{prefix}pet adopt [name]** - Adopt a new pet!\n" +
                 "**{prefix}pet release** - Gives your pet to a loving family that will take care of it (Deletes pet forever)")]
        public async Task PetMaster(string commandName = "", [Remainder]string args = "")
        {
            commandName = commandName.ToLowerInvariant();

            var command = PetMethods
                .FirstOrDefault(x => x.GetCustomAttribute<PetCommandAttribute>().Names.Contains(commandName));

            if (command == null)
            {
                await ReplyAsync($"Unknown pet command! Do `{Context.Prefix}pet help` for help");
            }
            else
            {
                if (Game == null && command.Get<RequiresPetAttribute>() != null)
                {
                    await ReplyAsync(AdoptPetMessage);
                    return;
                }

                ExtraArg = args;
                string response = await command.Invoke<Task<string>>(this);
                if (response != null) await ReplyAsync(response);
            }
        }




        [PetCommand(""), RequiresPet]
        public async Task<string> SendProfile()
        {
            await ReplyAsync(Game.GetContent(), Game.GetEmbed(Context.User as IGuildUser));
            return null;
        }


        [PetCommand("exact", "precise", "decimals", "float", "double")]
        public async Task<string> SendExact()
        {
            var user = Context.User as SocketGuildUser;
            var pet = Game;

            if (ExtraArg != "")
            {
                user = await Context.ParseUserAsync(ExtraArg);
                if (user == null) return "Can't find that user!";

                pet = Games.GetForUser<PetGame>(user.Id);
                if (pet == null) return "This person doesn't have a pet :(";
            }

            if (pet == null) return AdoptPetMessage;

            await ReplyAsync(pet.GetContent(), pet.GetEmbed(user, decimals: true));
            return null;
        }


        [PetCommand("stats", "statistics", "achievements", "unlocks")]
        public async Task<string> SendStats()
        {
            var user = Context.User as SocketGuildUser;
            var pet = Game;

            if (ExtraArg != "")
            {
                user = await Context.ParseUserAsync(ExtraArg);
                if (user == null) return "Can't find that user!";

                pet = Games.GetForUser<PetGame>(user.Id);
                if (pet == null) return "This person doesn't have a pet :(";
            }

            if (pet == null) return AdoptPetMessage;

            await ReplyAsync(pet.GetEmbedAchievements(user));
            return null;
        }


        [PetCommand("feed", "food", "eat", "hunger", "satiation"), RequiresPet]
        public async Task<string> Feed()
        {
            if (!await Game.TryFeedAsync()) return $"{CustomEmoji.Cross} Your pet is already full! (-1 energy)";
            await Context.Message.AddReactionAsync(Program.Random.Choose(Content.petFoodEmotes).ToEmoji());
            return null;
        }


        [PetCommand("clean", "hygiene", "wash"), RequiresPet]
        public async Task<string> Clean()
        {
            if (!await Game.TryCleanAsync()) return $"{CustomEmoji.Cross} Your pet is already clean! (-1 energy)";
            await Context.Message.AddReactionAsync(Program.Random.Choose(Content.petCleanEmotes).ToEmoji());
            return null;
        }


        [PetCommand("play", "fun", "happy", "happiness"), RequiresPet]
        public async Task<string> Play()
        {
            PetGame otherPet = null;
            if (ExtraArg != "")
            {
                var otherUser = await Context.ParseUserAsync(ExtraArg);
                if (otherUser == null)
                {
                    return "Can't find that user to play with!";
                }
                else if ((otherPet = Games.GetForUser<PetGame>(otherUser.Id)) == null)
                {
                    return "This person doesn't have a pet :(";
                }
                else
                {
                    otherPet.UpdateStats();
                    if (otherPet.happiness.Ceiling() == PetGame.MaxStat) return "This person's pet doesn't want to play right now!";
                }
            }

            if (await Game.TryPlayAsync())
            {
                var playEmote = Program.Random.Choose(Content.petPlayEmotes).ToEmoji();

                if (otherPet == null) await Context.Message.AddReactionAsync(playEmote);
                else
                {
                    otherPet.happiness = PetGame.MaxStat;
                    await Games.SaveAsync(otherPet);

                    await ReplyAsync($"{CustomEmoji.PetRight}{playEmote}{CustomEmoji.PetLeft}");
                    await ReplyAsync($"{Game.petName} and {otherPet.petName} are happy to play together!");
                }
                return null;
            }
            else
            {
                string message = Game.happiness.Ceiling() == PetGame.MaxStat
                    ? "Your pet doesn't want to play anymore! (-1 energy)"
                    : "Your pet is too tired! It needs 5 energy, or for someone else's pet to encourage it to play.";

                return $"{CustomEmoji.Cross} {message}";
            }
        }


        [PetCommand("sleep", "rest", "energy"), RequiresPet]
        public async Task<string> Sleep()
        {
            Game.UpdateStats();
            if (Game.energy.Ceiling() == PetGame.MaxStat && !Game.asleep)
            {
                Game.happiness = Math.Max(0, Game.happiness - 1);
                await SaveGameAsync();
                return $"{CustomEmoji.Cross} Your pet is not tired! (-1 happiness)";
            }

            string message = Game.asleep ? "Your pet is already sleeping." : "Your pet is now asleep.";
            if (!Game.asleep) await Game.ToggleSleepAsync();
            return $"{Program.Random.Choose(Content.petSleepEmotes)} {message}";
        }


        [PetCommand("wake", "wakeup", "awaken", "awake"), RequiresPet]
        public async Task<string> WakeUp()
        {
            Game.UpdateStats();
            var message = Game.asleep ? "🌅 You wake up your pet." : "🌅 Your pet is already awake.";
            if (Game.asleep) await Game.ToggleSleepAsync();
            return message;
        }


        [PetCommand("name"), RequiresPet]
        public async Task<string> SetName()
        {
            var msg = Context.Message;
            string name = ExtraArg;

            if (name == "")
            {
                await ReplyAsync("Say your pet's new name:");

                msg = await GetResponseAsync();
                if (msg == null) return "Timed out 💨";

                name = msg.Content;
                if (string.IsNullOrWhiteSpace(name)) return null;
            }

            if (name.Length > PetGame.NameCharLimit)
                return $"{CustomEmoji.Cross} Pet name can't go above 32 characters!";


            await Game.SetPetNameAsync(name);
            await msg.AutoReactAsync();
            return null;
        }


        [PetCommand("image", "url"), RequiresPet]
        public async Task<string> SetImage()
        {
            var msg = Context.Message;
            string url = msg.Attachments.FirstOrDefault()?.Url ?? ExtraArg;

            if (string.IsNullOrWhiteSpace(url))
            {
                await ReplyAsync("Send your pet's new image or image URL, or \"reset\" to reset it.");

                msg = await GetResponseAsync(120);
                if (msg == null) return "Timed out 💨";

                url = msg.Attachments.FirstOrDefault()?.Url ?? msg.Content;
                if (string.IsNullOrWhiteSpace(url)) return null;
            }

            if (url.ToLowerInvariant() == "reset")
            {
                await Game.TrySetImageUrlAsync(null);
                return $"{CustomEmoji.Check} Pet image reset!";
            }

            if (!await Game.TrySetImageUrlAsync(url)) return $"{CustomEmoji.Cross} Invalid image!";

            await msg.AutoReactAsync();
            return null;
        }


        [PetCommand("help", "h")]
        public async Task<string> SendHelp()
        {
            await ReplyAsync(Commands.GetCommandHelp("pet", Context));
            return null;
        }


        [PetCommand("pet", "pat", "pot", "p", "wakagotchi", "gotchi"), RequiresPet]
        public async Task<string> PetPet()
        {
            var now = DateTime.Now;
            var passed = now - Game.petTimerStart;
            if (passed > TimeSpan.FromMinutes(1))
            {
                Game.petTimerStart = now;
                Game.timesPetSinceTimerStart = 0;
            }

            int limit = Context.Guild == null ? 10 : 1;

            if (Game.timesPetSinceTimerStart >= limit)
            {
                if (Game.timesPetSinceTimerStart < limit + 3) // Reattempts
                {
                    Game.timesPetSinceTimerStart += 1;

                    string response = Context.Guild == null
                        ? $"{CustomEmoji.Cross} That's enough petting! {60 - (int)passed.TotalSeconds} seconds left."
                        : $"{CustomEmoji.Cross} You may pet once a minute in guilds. You can pet more in DMs with the bot.";
                        
                    await ReplyAsync(response);
                }
                return null;
            }
            else
            {
                Game.timesPetSinceTimerStart += 1;
                return await Game.DoPetAsync();
            }
        }


        [PetCommand("abuse"), RequiresPet]
        public Task<string> Meme()
        {
            if (Game.timesPetSinceTimerStart > 0) return null;
            Game.timesPetSinceTimerStart = 10;
            return Task.FromResult("no");
        }


        [PetCommand("user", "u")]
        public async Task<string> SendUser()
        {
            if (ExtraArg == "") return "You must specify a user!";

            var user = await Context.ParseUserAsync(ExtraArg);
            if (user == null) return "Can't find that user!";

            var pet = Games.GetForUser<PetGame>(user.Id);
            if (pet == null) return "This person doesn't have a pet :(";

            await ReplyAsync(pet.GetContent(), pet.GetEmbed(user));
            return null;
        }


        [PetCommand("adopt", "get")]
        public async Task<string> StartGame()
        {
            if (Game != null) return $"You already have a pet!";

            StartNewGame(new PetGame(Context.User.Id, Services));
            await SendProfile();
            await SaveGameAsync();
            return null;
        }


        [PetCommand("release"), RequiresPet]
        public async Task<string> DeleteGame()
        {
            if (string.IsNullOrWhiteSpace(Game.petName))
            {
                Games.Remove(Game);
                return $"Goodbye {Game.GameName}!";
            }

            await ReplyAsync(
                $"❗ Are you sure you want to release **{Game.petName}**?\n" +
                $"It will be gone forever, along with your stats and achievements, and you can't get it back.\n" +
                $"Release your pet? (Yes/No)");

            if (await GetYesResponseAsync() ?? false)
            {
                Games.Remove(Game);
                return $"Goodbye {Game.petName}!";
            }
            return "Pet not released ❤";
        }
    }
}

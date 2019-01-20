﻿using System;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using PacManBot.Constants;
using PacManBot.Extensions;
using PacManBot.Services;

namespace PacManBot.Commands
{
    /// <summary>
    /// The base for Pac-Man Bot modules, including their main services and some utilities.
    /// </summary>
    /// <remarks>Service properties are loaded lazily.</remarks>
    public abstract class PmBaseModule : ModuleBase<PmCommandContext>
    {
        /// <summary>Consistent and sane <see cref="RequestOptions"/> to be used in most Discord requests.</summary>
        public static readonly RequestOptions DefaultOptions = PmBot.DefaultOptions;


        private PmContent internalContent;
        private LoggingService internalLogger;
        private StorageService internalStorage;
        private GameService internalGames;
        private PmCommandService internalCommands;


        /// <summary>All of this program's services, required to supply new objects such as games.</summary>
        public IServiceProvider Services { get; }

        /// <summary>Contents used throughout the bot.</summary>
        public PmContent Content => internalContent ?? (internalContent = Services.Get<PmConfig>().Content);
        /// <summary>Logs everything in the console and on disk.</summary>
        public LoggingService Logger => internalLogger ?? (internalLogger = Services.Get<LoggingService>());
        /// <summary>Gives access to the bot's database.</summary>
        public StorageService Storage => internalStorage ?? (internalStorage = Services.Get<StorageService>());
        /// <summary>Gives access to active games.</summary>
        public GameService Games => internalGames ?? (internalGames = Services.Get<GameService>());
        /// <summary>Provides help for commands.</summary>
        public PmCommandService Commands => internalCommands ?? (internalCommands = Services.Get<PmCommandService>());


        protected PmBaseModule(IServiceProvider services)
        {
            Services = services;
        }


        protected override async void AfterExecute(CommandInfo command)
        {
            await Logger.Log(LogSeverity.Verbose, LogSource.Command,
                             $"Executed {command.Name} for {Context.User.FullName()} in {Context.Channel.FullName()}");
        }


        /// <summary>Sends a message in the current context, using the default options if not specified.</summary>
        protected override async Task<IUserMessage> ReplyAsync(string message = null, bool isTTS = false, Embed embed = null, RequestOptions options = null)
        {
            return await base.ReplyAsync(message, isTTS, embed, options ?? DefaultOptions);
        }


        /// <summary>Sends a message in the current context, containing text and an embed, and using the default options if not specified.</summary>
        public async Task<IUserMessage> ReplyAsync(object text, EmbedBuilder embed, RequestOptions options = null)
            => await ReplyAsync(text?.ToString(), false, embed?.Build(), options);


        /// <summary>Sends a message in the current context containing only text, and using the default options if not specified.</summary>
        public async Task<IUserMessage> ReplyAsync(object text, RequestOptions options = null)
            => await ReplyAsync(text.ToString(), false, null, options);


        /// <summary>Sends a message in the current context containing only an embed, and using the default options if not specified.</summary>
        public async Task<IUserMessage> ReplyAsync(EmbedBuilder embed, RequestOptions options = null)
            => await ReplyAsync(null, false, embed.Build(), options);



        /// <summary>Reacts to the command's calling message with a check or cross.</summary>
        public async Task AutoReactAsync(bool success = true)
            => await Context.Message.AutoReactAsync(success);


        /// <summary>Creates an instance of a different module. This shouldn't ever be used. I'm a madman.</summary>
        public TModule GetModule<TModule>() where TModule : PmBaseModule
            => ModuleBuilder<TModule, PmCommandContext>.Create(Context, Services);
    }
}

using DSharpPlus;
using DSharpPlus.EventArgs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.CommandsNext;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using DSharpPlus.Entities;
using DiscordBotTut.Commands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBotTut
{
    public class Bot
    {
        public DiscordClient Client { get; private set; }
        public CommandsNextExtension Commands { get; private set; }
        public InteractivityExtension Interactivity { get; private set; }

        public async Task RunAsync()
        {
            var json = string.Empty;
            using (var fs = File.OpenRead("config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug,
                
            };
            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });
            var commandsConfig = new CommandsNextConfiguration
            {
                StringPrefixes = new string[] { configJson.Prefix },
                EnableDms = false,
                EnableMentionPrefix = true
            };

            Commands = Client.UseCommandsNext(commandsConfig);
           
            Commands.RegisterCommands<FunCommands>();
            Commands.RegisterCommands<TeamCommands>();

            Client.MessageCreated += async (s, e) =>
            {
                if (e.Message.Content.ToLower().StartsWith("hej"))
                    await e.Message.RespondAsync("siemka!");
                else if (e.Message.Content.ToLower().StartsWith("dzien dobry"))
                    await e.Message.RespondAsync("dobry wieczor");
                else if (e.Message.Content.ToLower().StartsWith("test"))
                    await e.Message.RespondAsync("no co jest?");

            };



            await Client.ConnectAsync();

            await Task.Delay(-1);
        }
        private Task OnClientReady(object sender, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }

    }
}

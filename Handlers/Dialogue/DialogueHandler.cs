using DiscordBotTut.Handlers.Dialogue.Steps;
using DSharpPlus;
using DSharpPlus.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTut.Handlers.Dialogue
{
    public class DialogueHandler
    {
        private readonly DiscordClient _client;
        private readonly DiscordChannel _channel;
        private readonly DiscordUser _user;
        private IDialogueStep _currentStep;

        public DialogueHandler(DiscordClient discordClient, DiscordChannel discordChannel, DiscordUser user, IDialogueStep startingStep)
        {
            _client = discordClient;
            _channel = discordChannel;
            _user = user;
            _currentStep = startingStep;
        }

        private readonly List<DiscordMessage> messages = new List<DiscordMessage>();

        public async Task <bool> ProcessDialogue()
        {
            while(_currentStep != null)
            {
                _currentStep.OnMessageAdded += (message) => messages.Add(message); // subuj event i dodawaj message do listy

                bool cancled = await _currentStep.ProcessStep(_client, _channel, _user).ConfigureAwait(false);

                if (cancled)
                {  await DeleteMessage().ConfigureAwait(false);

                    var cancelEmbed = new DiscordEmbedBuilder
                    {
                        Title = "The dialog has succesfully been canceled",
                        Description = _user.Mention,
                        Color = DiscordColor.Green
                    };

                    await _channel.SendMessageAsync(embed: cancelEmbed).ConfigureAwait(false);

                    return false;
                }

                _currentStep = _currentStep.NextStep;   
            }
            await DeleteMessage().ConfigureAwait(false);

            return true;
        }
        private async Task DeleteMessage()
        {
            if(_channel.IsPrivate)
            { return; }

            foreach (var mess in messages)
            {
                await mess.DeleteAsync().ConfigureAwait(false);
            }
        }

    }
}

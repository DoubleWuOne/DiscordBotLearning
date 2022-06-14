using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTut.Handlers.Dialogue.Steps
{
    public class IntStep : DialogueStepBase
    {
        private  IDialogueStep _nextStep;
        private readonly int? _minValue;
        private readonly int? _maxValue;

        public IntStep(string content, IDialogueStep nextStep, int? minLength = null, int? maxLength = null) : base(content)
        {
            _nextStep = nextStep;
            _minValue = minLength;
            _maxValue = maxLength;
        }

        public Action<int> OnValidResult { get; set; } = delegate { };

        public override IDialogueStep NextStep => _nextStep;

        public void SetNextStep (IDialogueStep nextStep)
        {
            _nextStep=nextStep; 
        }
        public override async Task<bool>ProcessStep(DiscordClient client, DiscordChannel channel, DiscordUser user)
        {
            var embedBuilder = new DiscordEmbedBuilder
            {
                Title = "Pls respond below",
                Description = $"{user.Mention}, {_content}"
            };
            embedBuilder.AddField("To stop the dialog", "use the ^cancel command");
            
            if (_minValue.HasValue)
            {
                embedBuilder.AddField("Min Value: ", $"{_minValue.Value} ");
            }
            if (_maxValue.HasValue)
            {
                embedBuilder.AddField("Max Value: ", $"{_maxValue.Value} ");
            }

            var interactivity = client.GetInteractivity();

            while (true)
            {
                var embed = await channel.SendMessageAsync(embed: embedBuilder).ConfigureAwait(false);

                OnMessageAdded(embed);

                var messageResult = await interactivity.WaitForMessageAsync(x => 
                x.ChannelId == channel.Id && 
                x.Author.Id == user.Id)
                    .ConfigureAwait(false);

                OnMessageAdded(messageResult.Result);

                if (messageResult.Result.Content.Equals("^cancel",StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }

                if (!int.TryParse(messageResult.Result.Content, out int inputValue))
                {
                    await TryAgain(channel, $"Your input is  not an integer").ConfigureAwait(false);
                    continue;

                }

                if (_minValue.HasValue)
                {
                    if(inputValue < _minValue)
                    {
                        await TryAgain(channel, $"Your input value  {inputValue} is smaller than {_minValue}").ConfigureAwait(false);
                        continue;
                    }
                }
                if (_maxValue.HasValue)
                {
                    if (inputValue > _maxValue)
                    {
                        await TryAgain(channel, $"Your input value  {inputValue} is greater than {_maxValue}").ConfigureAwait(false);
                        continue;
                    }
                }

                OnValidResult(inputValue);

                return false;
            }
        }
    }
}

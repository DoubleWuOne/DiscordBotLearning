using DiscordBotTut.Handlers.Dialogue;
using DiscordBotTut.Handlers.Dialogue.Steps;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordBotTut.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("cipa")]
        public async Task Ping(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync("Chuj").ConfigureAwait(false); //await znaczy ze funkcja zrobi ta
                                                                              //linie ale nie pojdzie dalej zanim
                                                                              //nie skonczy tej lini

        }
        [Command("add")]
        [Description("dodaje dwie liczby lol")]
        public async Task Add(CommandContext ctx, [Description("pierwsza liczba")] int numb1, [Description("druga liczba")] int numb2)
        {
            await ctx.Channel.SendMessageAsync((numb1 + numb2).ToString()).ConfigureAwait(false);
        }

        [Command("debil")]
        public async Task Debil(CommandContext ctx, string numb1, string numb2)
        {
            Random rnd = new Random();
            var temp = rnd.Next(1, 3);

            var cond = temp > 1 ? numb1 : numb2;
            var cond2 = temp > 1 ? numb2 : numb1;

            var iq = rnd.Next(1, 250);
            var iq2 = rnd.Next(iq, 251);
            await ctx.Channel.SendMessageAsync("Glupszy jest " + cond + " bo ma " + iq +
                " IQ a ten drugi debil " + cond2 + " ma " + iq2 + " IQ").ConfigureAwait(false);
        }
        [Command("resp")]
        public async Task Resp(CommandContext ctx)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var message = await interactivity.WaitForMessageAsync(x => x.Channel == ctx.Channel).ConfigureAwait(false);
            await ctx.Channel.SendMessageAsync(message.Result.Content);
        }
        [Command("poll")]
        public async Task Poll(CommandContext ctx, TimeSpan time, params DiscordEmoji[] emojiOptions)
        {
            var interactivity = ctx.Client.GetInteractivity();
            var options = emojiOptions.Select(x => x.ToString());

            var embed = new DiscordEmbedBuilder
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            var pollMess = await ctx.Channel.SendMessageAsync(embed: embed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMess.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMess).ConfigureAwait(false);
            var distincResult = result.Distinct();

            var results = distincResult.Select(x => $"{x.Emoji}: {x.Total}");
            await ctx.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);
        }
        [Command("dialogue")]
        public async Task Dialogue(CommandContext ctx)
        {
            var inputStep = new TextStep("Enter smth interesting", null);
            var funnyStep = new IntStep("haha, funny", null, 100);

            string input = string.Empty;
            int value = 0;

            inputStep.OnValidResult += (result) =>

            {
                input = result;
                if (result == "something interesting")
                {
                    inputStep.SetNextStep(funnyStep);
                }
            };

            funnyStep.OnValidResult += (result) => value = result;

            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);
            var inputDialogueHandler = new DialogueHandler(
                ctx.Client,
                userChannel,
                ctx.User,
                inputStep
                );
            bool succeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);

            if (!succeded) { return; }

            await ctx.Channel.SendMessageAsync(input).ConfigureAwait(false);

            await ctx.Channel.SendMessageAsync(value.ToString()).ConfigureAwait(false);

        }
        [Command("emojidialogue")]
        public async Task EmojiDialogue(CommandContext ctx)
        {
            var yesStep = new TextStep("you chose yes", null);
            var noStep = new TextStep("you chose no", null);
            var emojiStep = new ReactionStep("Yes or No?", new Dictionary<DiscordEmoji, ReactionStepData>
            {
                {DiscordEmoji.FromName(ctx.Client,":thumbsup:"), new ReactionStepData{Content = "this means yes", NextStep = yesStep} },
                {DiscordEmoji.FromName(ctx.Client,":thumbsdown:"), new ReactionStepData{Content = "this means no", NextStep = noStep} }

            });
            var userChannel = await ctx.Member.CreateDmChannelAsync().ConfigureAwait(false);

            var inputDialogueHandler = new DialogueHandler(
            ctx.Client,
            userChannel,
            ctx.User,
             emojiStep
             );

            bool succeeded = await inputDialogueHandler.ProcessDialogue().ConfigureAwait(false);
            if (!succeeded) { return; }
        }
    }
}

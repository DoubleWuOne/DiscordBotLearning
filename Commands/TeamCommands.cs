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
    public class TeamCommands : BaseCommandModule
    {
        [Command("joinn")]
        public async Task Join(CommandContext ctx)
        {
            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Dołączysz?",
                Thumbnail = new DiscordEmbedBuilder.EmbedThumbnail { Url = ctx.Client.CurrentUser.AvatarUrl },
                Color = DiscordColor.Green
            };

            var joinMess = await ctx.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            var thumbsUpEmoji = DiscordEmoji.FromName(ctx.Client, ":smile:");
            var thumbsDownEmoji = DiscordEmoji.FromName(ctx.Client, ":grin:");

            await joinMess.CreateReactionAsync(thumbsUpEmoji).ConfigureAwait(false);
            await joinMess.CreateReactionAsync(thumbsDownEmoji).ConfigureAwait(false);

            var interactivity = ctx.Client.GetInteractivity();

            var result = await interactivity.WaitForReactionAsync(
            x => x.Message == joinMess &&
            x.User == ctx.User &&
            (x.Emoji == thumbsUpEmoji || x.Emoji == thumbsDownEmoji))
                .ConfigureAwait(false);

            if (result.Result.Emoji == thumbsUpEmoji)
            {
                await ctx.Channel.SendMessageAsync("asdf ").ConfigureAwait(false);
            }
            else if (result.Result.Emoji == thumbsDownEmoji)
            {
                await ctx.Channel.SendMessageAsync("ghjkl ").ConfigureAwait(false);
            }

            await joinMess.DeleteAsync().ConfigureAwait(false);
            await ctx.Message.DeleteAsync().ConfigureAwait(false);
        }
    }
}

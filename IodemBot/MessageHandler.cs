﻿using System.Reflection;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;
using System;
using IodemBot.Core.Leveling;
using System.Text.RegularExpressions;
using Discord;
using System.Collections.Generic;
using IodemBot.Core.UserManagement;

namespace IodemBot
{
    public class MessageHandler
    {
        private DiscordSocketClient client;
        private CommandService service;
        ulong[] whiteList = { 1234 };
        private List<AutoResponse> responses;

        public async Task InitializeAsync(DiscordSocketClient client)
        {
            this.client = client;
            service = new CommandService();
            await service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
            client.MessageReceived += HandleMessageAsync;
            //badWords = File.ReadAllLines("Resources/bad_words.txt");

            responses = new List<AutoResponse>();
            responses.Add(new AutoResponse(
                new Regex("[Hh][y][a]*[h][o]*", RegexOptions.Compiled),
                new Reaction("",
                    Emote.Parse("<:Keelhaul:537265959442841600>")),
                5));
            responses.Add(new AutoResponse(
                new Regex("[H][Y][A]*[H][O]*", RegexOptions.Compiled),
                new Reaction("",
                    Emote.Parse("<:Vicious_Chop:537265959384121366>")),
                5));
            responses.Add(new AutoResponse(
                new Regex("Bubebo", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                new Reaction("Do you feel the earth rumbling? It must be Lord Babi rolling in his grave.",
                    Emote.Parse("<:sad:490015818063675392>")),
                60));
            responses.Add(new AutoResponse(
                new Regex("(Air).*(Rock).*(Sol Blade)|(Sol Blade).*(Air).*(Rock)", RegexOptions.Compiled|RegexOptions.IgnoreCase),
                new Reaction("I assume you are talking about the Air's Rock Glitch where you can get an early Sol Blade. Check TLPlexas video about it! https://www.youtube.com/watch?v=AIdt53_mqXQ&t=1s"),
                30));
            responses.Add(new AutoResponse(
                new Regex(@"\¡\!", RegexOptions.Compiled | RegexOptions.IgnoreCase),
                new Reaction("If you want to summon me to seek my assistance, use the prefix `i!` as in **I**odem."),
                30));
            responses.Add(new AutoResponse(
                new Regex(@"(\#\^\@\%\!)", RegexOptions.Compiled),
                new CurseReaction(),
                2));
            responses.Add(new AutoResponse(
                new Regex(@" 420 ", RegexOptions.Compiled),
                new Reaction("",
                    Emote.Parse("<:Herb:543043292187590659>")),
                60));
        }

        private async Task HandleMessageAsync(SocketMessage s)
        {
            SocketUserMessage msg = s as SocketUserMessage;
            if (msg == null) return;
            var context = new SocketCommandContext(client, msg);
            if (context.User.IsBot) return;
            //Check for Profanity here
            responses.ForEach(async r => await r.Check(msg));
            Leveling.UserSentMessage((SocketGuildUser)context.User, (SocketTextChannel)context.Channel);
            await Task.CompletedTask;
        }

        private class CurseReaction : Reaction
        {
            public CurseReaction() : base("",Emote.Parse("<:curse:538074679492083742>"))
            {
            }

            public override async Task ReactAsync(SocketUserMessage msg)
            {
                await base.ReactAsync(msg);
                var userAccount = UserAccounts.GetAccount(msg.Author);
                userAccount.hasWrittenCurse = true;
                UserAccounts.SaveAccounts();
                await ServerGames.UserHasCursed((SocketGuildUser) msg.Author, (SocketTextChannel) msg.Channel);
            }
        }

        private bool ContainsBadWord(SocketUserMessage msg)
        {
            //you should do this once and not every function call
            return false;
        }

        internal async Task CheckProfanity(SocketUserMessage msg)
        {
            await Task.CompletedTask;
        }

        internal class AutoResponse
        {
            public Regex trigger;
            public Reaction reaction;
            public DateTime lastUse;
            public int timeOut;

            public AutoResponse(Regex regex, Reaction reaction, int timeOut)
            {
                trigger = regex;
                this.reaction = reaction;
                this.timeOut = timeOut;
                lastUse = DateTime.MinValue;
            }

            public async Task Check(SocketUserMessage msg)
            {
                if (trigger.IsMatch(msg.Content))
                {
                    if ((DateTime.Now - lastUse).TotalSeconds < timeOut) return;
                    await reaction.ReactAsync(msg);
                    lastUse = DateTime.Now;
                }
            }
        }
        internal class Reaction
        {
            private string text;
            private IEmote[] emotes;

            public Reaction(string text)
            {
                this.text = text;
                emotes = new IEmote[] { };
            }

            public Reaction(string text, IEmote[] emotes)
            {
                this.text = text;
                this.emotes = emotes;
            }

            public Reaction(string text, IEmote emote)
            {
                this.text = text;
                this.emotes = new IEmote[] { emote };
            }

            public virtual async Task ReactAsync(SocketUserMessage msg)
            {
                if (emotes.Length > 0) await msg.AddReactionsAsync(emotes);
                if (text != "")
                {
                    var embed = new EmbedBuilder();
                    embed.WithColor(Colors.get("Iodem"));
                    embed.WithDescription(text);
                    await msg.Channel.SendMessageAsync("", false, embed.Build());
                }
            }
        }
    }
}

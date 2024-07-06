using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.Entities;
using DSharpPlus.CommandsNext.Exceptions;
using DSharpPlus.CommandsNext.Attributes;
using HEX.HEX.Helpers;
using System.Diagnostics;
using System.Windows;

namespace HEX.HEX.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class DCServer
    {
        public static DiscordClient? Client { get; set; }
        public static CommandsNextExtension? Commands { get; set; }
        
        /// <summary>
        /// Initializes the Discord client with configuration settings, event handlers, and command settings.
        /// Connects the client to Discord and keeps it running indefinitely.
        /// </summary>
        public static async Task Initialize() 
        {
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
            };

            Client = new DiscordClient(discordConfig);


            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });


            Client.Ready += DCServerSupport.Client_Ready;
            Client.MessageCreated += DCServerSupport.Message_Created;
            Client.MessageCreated += RequestRecieved;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = true,
                DmHelp = false,
                IgnoreExtraArguments = true,
            };

            Commands = Client.UseCommandsNext(commandsConfig);


            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private static async Task RequestRecieved(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (e.Message.Author.Id == 1255046184419201034 || e.Message.Author.Id == 739867365763710996)
            {
                await HandleRequestAsync(e);
            }

        }

        public static async Task HandleRequestAsync(MessageCreateEventArgs e)
        {
            var request = await DCServerSupport.DeserializeRequestObjectAsync(e.Message.Content);

            await DCServerSupport.HandleRequestAsync(request, e.Message);
        }

        public static async Task<bool> HandleResponseAsync(ResponseObject response)
        {
            var Destination = await DCServerSupport.GetDestinationAsync(response.Response.RequestType);

            var serializedResponse = await DCServerSupport.SerializeRequestObjectAsync(response);

            return await PostResponseAsync(serializedResponse, Destination);
        }

        private static async Task<bool> PostResponseAsync(string responseObject, DiscordChannel discordChannel)
        {
            var responseMSG =  await discordChannel.SendMessageAsync($"{responseObject}");

            return await WaitForConfirmation(discordChannel, responseMSG);
        }

        private static async Task<bool> WaitForConfirmation(DiscordChannel channel, DiscordMessage discordMessage)
        {
            var interactivity = Client.GetInteractivity();

            var waitTime = TimeSpan.FromSeconds(5);

            var result = await interactivity.WaitForReactionAsync(
                            x => x.Message == discordMessage, waitTime);

            if (result.Result != null)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}

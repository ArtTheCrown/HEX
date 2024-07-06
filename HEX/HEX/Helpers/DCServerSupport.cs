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
using HEX.HEX.Services;
using Newtonsoft.Json;
using System.Windows;
using System.Diagnostics;

namespace HEX.HEX.Helpers
{
    public static class DCServerSupport
    {
        public static async Task<string> SerializeRequestObjectAsync(ResponseObject responseObject)
        {
            return JsonConvert.SerializeObject(responseObject);
        }

        public static async Task<RequestObject?> DeserializeRequestObjectAsync(string responseObject)
        {
            return JsonConvert.DeserializeObject<RequestObject>(responseObject);
        }

        public static Task Message_Created(DiscordClient sender, MessageCreateEventArgs e)
        {
            if (!e.Author.IsBot)
            {
                // Handle non-bot messages here.
            }
            return Task.CompletedTask;
        }

        public static async Task Client_Ready(DiscordClient sender, ReadyEventArgs e)
        {
            sender.UpdateStatusAsync(new DiscordActivity("雷電様の命令に", ActivityType.ListeningTo), UserStatus.DoNotDisturb);
            
            Console.WriteLine("========================================================================");

            var channel = await DCServer.Client.GetChannelAsync(1255056967655751790);

            var result = await SerializeRequestObjectAsync(new ResponseObject
            {
                Authentication = new Authentication
                {
                    User = new User 
                    { 
                        UserID = "ArtTheCrown",
                        Password = "professorHEX"
                    }
                }
            });

            await channel.SendMessageAsync(result);
        }

        public static async Task<DiscordChannel> GetDestinationAsync(RequestType requestType)
        {
            ulong Address = await GetRequestChannelAsync(requestType);

            var Destination = await DCServer.Client.GetChannelAsync(Address);

            return Destination;
        }

        private static async Task<ulong> GetRequestChannelAsync(RequestType requestType)
        {
            switch (requestType)
            {
                case RequestType.System:
                    return 1255786057832599583;
                case RequestType.Request:
                    return 1255779753294561281;
                case RequestType.Authentication:
                    return 1255779696432644116;
                case RequestType.Registration:
                    return 1255779910027444224;
                case RequestType.Communication:
                    return 1255786428412068002;
                case RequestType.Kaizen:
                    return 1255785863699501097;
                case RequestType.Report:
                    return 1255813320976498730;
                case RequestType.Debug:
                    return 1255056967655751790;
                default:
                    return 1255056967655751790;
            }
        }
        public static async Task HandleRequestAsync(RequestObject request)
        {
            switch (request.Request?.RequestType)
            {
                case RequestType.System:
                    await RequestsHandler.SystemAsync(request);
                    break;
                case RequestType.Request:
                    await RequestsHandler.RequestAsync(request);
                    break;
                case RequestType.Authentication:
                    await RequestsHandler.AuthenticationAsync(request);
                    break;
                case RequestType.Registration:
                    await RequestsHandler.RegistrationAsync(request);
                    break;
                case RequestType.Communication:
                    await RequestsHandler.CommunicationAsync(request);
                    break;
                case RequestType.Kaizen:
                    await RequestsHandler.KaizenAsync(request);
                    break;
                case RequestType.Report:
                    await RequestsHandler.ReportAsync(request);
                    break;
                case RequestType.Debug:
                    await RequestsHandler.DebugAsync(request);
                    break;
                default:
                    await RequestsHandler.DebugAsync(request);
                    break;
            }
        }
    }
}

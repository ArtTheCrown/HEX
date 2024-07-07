using DSharpPlus.Entities;
using HEX.HEX.Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace HEX.HEX.Services
{
    public static class RequestsHandler
    {
        public static async Task<bool> SystemAsync(RequestObject request)
        {

            return false;
        }
        public static async Task<bool> RequestAsync(RequestObject request)
        {

            return false;
        }
        public static async Task<bool> AuthenticationAsync(RequestObject request)
        {
            var _user = request.Authentication?.User;


            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();

            var engine = new DBEngine(jsonReader.connectionString);

            var result = await engine.GetUserByAuthenticationAsync(_user.UserID, _user.Password);

            if (result.Item1)
            {
                return await Program.Respond(new ResponseObject
                {
                    Response = new Response { RequestType = RequestType.Authentication },
                    Authentication = new Authentication { Status = result.Item3, User = result.Item2 }
                });
            }
            else
            {
                return await Program.Respond(new ResponseObject
                {
                    Response = new Response { RequestType = RequestType.Authentication },
                    Authentication = new Authentication { Status = result.Item3 }
                });
            }
        }
        public static async Task<bool> RegistrationAsync(RequestObject request)
        {
            var _user = request.Registration?.User;

            var user = new User
            {
                Pfp = _user.Pfp == null ? "null" : _user.Pfp,
                UserID = _user.UserID,
                Username = _user.Username == null ? "null" : _user.Username,
                Password = _user.Password,
                Email = _user.Email == null ? "null" : _user.Email,
                Status = _user.Status == null ? "null" : _user.Status,
                About = _user.About == null ? "null" : _user.About,
            };

            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();
            
            var engine = new DBEngine(jsonReader.connectionString);

            var result = await engine.StoreUserAsync(user);

            if(result.Item1)
            {
                return await Program.Respond(new ResponseObject
                {
                    Response = new Response { RequestType = RequestType.Registration},
                    Registration = new Registration { Status = result.Item2 }
                });
            }
            else
            {
                return await Program.Respond(new ResponseObject
                {
                    Registration = new Registration { Status = result.Item2 }
                });
            }
        }
        public static async Task<bool> CommunicationAsync(RequestObject request)
        {

            return false;
        }
        public static async Task<bool> KaizenAsync(RequestObject request)
        {

            return false;
        }
        public static async Task<bool> ReportAsync(RequestObject request)
        {

            return false;
        }
        public static async Task<bool> DebugAsync(RequestObject request)
        {

            return false;
        }
        public static async Task<bool> FilesAsync(RequestObject request, DiscordMessage discordMessage)
        {
            if (request.Files?.FilesType == FilesType.ProfilePicture)
            {
                string url = discordMessage.Attachments[0].Url;
                var _user = request.Authentication?.User;
                var jsonReader = new JSONReader();
                await jsonReader.ReadJSON();
                var engine = new DBEngine(jsonReader.connectionString);
                var result = await engine.GetUserByAuthenticationAsync(_user.UserID, _user.Password);

                if (result.Item1)
                {
                    try
                    {
                        // Define the path to save the file
                        string folderPath = "Data/ProfilePictures/";
                        string filePath = Path.Combine(folderPath, $"{_user.Username}_pfp.png");

                        // Ensure the directory exists
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Download the file
                        using (HttpClient client = new HttpClient())
                        using (HttpResponseMessage response = await client.GetAsync(url))
                        using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                        using (Stream streamToWriteTo = File.Open(filePath, FileMode.Create))
                        {
                            await streamToReadFrom.CopyToAsync(streamToWriteTo);
                        }

                        // You can add additional logic here if needed after saving the file
                        return true;
                    }
                    catch (Exception ex)
                    {
                        // Log or handle the exception as needed
                        Console.WriteLine($"An error occurred: {ex.Message}");
                    }
                }
            }
            return false;
        }
    }
}

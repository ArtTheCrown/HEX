using HEX.HEX.Services;
using HEX.HEX.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace HEX.HEX
{
    public static class Program
    {
        public static async void Initialize()
        {
            await DCServer.Initialize();

            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        public static async Task<bool> Respond(ResponseObject response)
        {
            ResponseStatus res = response.Response.RequestType == RequestType.Authentication? response.Authentication.Status : response.Registration.Status;


            var result = await DCServer.HandleResponseAsync(response);

            ResponseStatus respresult = result ? ResponseStatus.Success : ResponseStatus.Failure;


            //Console.WriteLine($"[+] {str} : [{reqresult}] : ({respresult})");

            return result;
        }
    }
}

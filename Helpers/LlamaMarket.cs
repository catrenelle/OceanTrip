using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace OceanTripPlanner.Helpers
{
	public static class LlamaMarket
    {


        public static void OpenMarketSettings()
        {
            var loader = BotManager.Bots.FirstOrDefault(x => x.EnglishName == "Llama Market");
            if (loader == null) 
            {
                string llamaURL = "https://llamamagic.net/botbases/llamamarket/";
                try
                {
                    Process.Start(llamaURL);
                }
                catch
                {
                    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                        Process.Start(new ProcessStartInfo(llamaURL.Replace("&", "^&")) { UseShellExecute = true });
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                        Process.Start("xdg-open", llamaURL);
                    else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
                        Process.Start("open", llamaURL);
                }

                return; 
            }

            var checkVentureTask = loader.GetType().GetMethod("OnButtonPress");
            checkVentureTask.Invoke(loader, null);
        }

    }
}
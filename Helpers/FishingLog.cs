using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using ff14bot;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using LlamaLibrary.RemoteAgents;
using OceanTripPlanner.Definitions;

namespace OceanTrip
{
    public static class FishingLog
    {
        private static string name = "IKDFishingLog";
        private static int elementCount => LlamaElements.ElementCount(name);
        private static TwoInt[] Elements => LlamaElements.___Elements(name);

        public static string AreaName 
        { 
            get 
            {
                if (elementCount > 0)
                    return Core.Memory.ReadString((IntPtr)Elements[2].Data, Encoding.UTF8);

                return ""; 
            } 
        }

        public static int Points
        {
            get 
            {
                if (elementCount > 0)
                    return Elements[7].TrimmedData;

                return 0;
            }
        }

        public static uint LastFishCaught
        {
            get
            {
                if (elementCount > 0)
                    return (uint)Elements[8].TrimmedData;

                return 0;
            }
        }

        public static async Task<List<uint>> GetFishLog(int[] oceanFish)
        {
            var fishList = await AgentFishGuide2.Instance.GetFishList();
            var recordedFish = fishList.Where(x => x.HasCaught).Select(x => (int)x.FishItem).ToList();

            fishList = null;


            // Convert the list to uint
            List<uint> newOceanFishList = oceanFish.Except(recordedFish).ToList().ConvertAll(x => (uint)x);
            return newOceanFishList;
        }

        public static void SaveMissingFishLog(List<uint> missingFish)
        {
            var file = Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripMissingFish.txt");

            if (File.Exists(file))
                File.Delete(file);

            File.WriteAllLines(file, missingFish.Select(x => x.ToString()));
            return;
        }

        public static void LoadMissingFishLog(out List<uint> missingFish)
        {
            var file = Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripMissingFish.txt");
            missingFish = File.ReadAllLines(file).Select(x => (uint)Convert.ToInt32(x)).ToList();

            //List<string> missingFishNames = new List<string>();
            //foreach (var fish in missingFish)
            //    missingFishNames.Add(DataManager.GetItem(fish).EnglishName);

            //settings.updateMissingFish(missingFishNames);
            //missingFishNames.Clear();

            return;
        }
    }
}

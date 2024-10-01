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
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using Ocean_Trip.Definitions;
using OceanTripPlanner.Definitions;

namespace OceanTrip
{
    public static class FishingLog
    {
        private static string name = "IKDFishingLog";
        private static int elementCount => LlamaElements.ElementCount(name);
        private static TwoInt[] Elements => LlamaElements.___Elements(name);


        private static string fileName = Path.Combine(JsonSettings.CharacterSettingsDirectory, "OceanTripMissingFish.txt");
        private static List<uint> _cachedMissingFishList;
        public static List<uint> MissingFish()
        {
            return _cachedMissingFishList;
        }


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

        public static void InvalidateCache()
        {
            _cachedMissingFishList = null;
        }

        public static void AddFish(uint fishId)
        {
            if (!_cachedMissingFishList.Contains(fishId))
                _cachedMissingFishList.Add(fishId);
        }

        public static void RemoveFish(uint fishId)
        {
            if (_cachedMissingFishList.Contains(fishId))
                _cachedMissingFishList.Remove(fishId);
        }


        public static async Task InitializeFishLog()
        {

            if (_cachedMissingFishList != null)
                _cachedMissingFishList = null;

            if (!File.Exists(fileName))
            {

                var fishList = await AgentFishGuide2.Instance.GetFishList();
                var recordedFish = fishList.Where(x => x.HasCaught).Select(x => (int)x.FishItem).ToList();
                var oceanFish = FishDataCache.GetFish().Select(x => x.FishID).ToList();

                // Convert the list to uint
                List<uint> newOceanFishList = oceanFish.Except(recordedFish).ToList().ConvertAll(x => (uint)x);

                Logging.Write($"  Ocean Fish: {oceanFish.Count()}");
                Logging.Write($"Missing Fish: {newOceanFishList.Count()}");

                fishList = null;
                recordedFish = null;
                oceanFish = null;

                _cachedMissingFishList = newOceanFishList;
                SaveMissingFishLog();
            }
            else
                LoadMissingFishLog();
        }

        public static void SaveMissingFishLog()
        {
            if (File.Exists(fileName))
                File.Delete(fileName);

            File.WriteAllLines(fileName, _cachedMissingFishList.Select(x => x.ToString()));
            return;
        }

        public static void LoadMissingFishLog()
        {
            if (_cachedMissingFishList != null)
                _cachedMissingFishList = null;

            if (File.Exists(fileName))
                _cachedMissingFishList = File.ReadAllLines(fileName).Select(x => (uint)Convert.ToInt32(x)).ToList();
        }
    }
}

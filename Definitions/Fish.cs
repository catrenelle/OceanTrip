using ff14bot.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocean_Trip.Definitions
{
    public class Fish
    {
        public uint RouteID { get; set; }
        public string RouteShortName { get; set; }
        public int FishID { get; set; }
        public string FishName { get; set; }
        public int IconX { get; set; }
        public int IconY { get; set; }
        public TugType BiteType { get; set; }
        public string Rarity { get; set; }
        public uint FavoriteBait { get; set; }
        public bool CausesSpectral { get; set; }
        public bool SpectralFish { get; set; }
        public float BiteStart { get; set; }
        public float BiteEnd { get; set; }
        public int Points { get; set; }
        public int DHBonus { get; set; }
        public int THBonus { get; set; }
        public string Achievement { get; set; }
        public string WeatherExclusion1 { get; set; }
        public string WeatherExclusion2 { get; set; }
        public string TimeOfDayExclusion1 { get; set; }
        public string TimeOfDayExclusion2 { get; set; }


        public static List<Fish> GetFish()
        {
            List<Fish> fish;

            var file = Path.Combine(Environment.CurrentDirectory, $@"BotBases\Ocean-Trip\Resources\fishList.json");
            string json = File.ReadAllText(file);
            fish = JsonConvert.DeserializeObject<List<Fish>>(json);

            return fish;
        }
    }
}


# OceanTrip

This BotBase will take the most complicated parts of Ocean Fishing and simplify it for you. It utilizes logic from [Zeke's guide](https://docs.google.com/spreadsheets/d/17A_IIlSO0wWmn8I3-mrH6JRok0ZIxiNFaDH2MhN63cI/edit#gid=1411459855) and does all of the hard work for you.

For best results, you should be level 90 with 900+GP available to you. In my own runs, I'm using fully pentamelded gathering gear with 973 GP (after eating Crab Cakes). Please note that as far as food goes, the focus should be on GP+ rather than Perception/Gathering. If you can do GP+ and Perception (IE: Crab Cakes or Peppered Popotoes) then you are all set!

Please note that your success rates will vary. Some runs will be very low scoring, others can be very high. Ocean Trip has less to do with stats and everything to do with RNG. You can manipulate some factors, such as double hooking at the right time and identical casting when required. I've hit 20k points using this bot many times now, so I know it's easily accomplished.

Ideally, you should aim for the following point values and can use this to gauge your potential success rate:


| Bonus Multiplier | 10k Mount | 16k Title | 20k Title |
| :--------------: | :-------: | :-------: | :-------: |
| 30 | 7963 | 12308 | 15385 |
| 40 | 7193 | 11429 | 14286 |
| 50 | 6667 | 10668 | 13334 |
| 60 | 6250 | 10000 | 12500 |
| 70 | 5883 | 9418 | 11765 |
| 80 | 5556 | 8889 | 11112 |
| 90 | 5264 | 8422 | 10527 |

## BotBase Settings
When Idling:

| Idle Stuff | Values | Description|
| :--- | :--- | :--- |
| Craft Food | True/False | Should the bot craft lvl 90 food while passing the time? Currently crafts Tsai Tou Vounou, Pumpkin Ratatouille, Archon Burger, Pumpkin Potage, and Thavnairian Chai. |
| Craft Mats | True/False | Should the bot craft lvl 80 stuff while waiting? Right now, this only exchanges scripts for Immutable Solution.
| Craft Potions | None, Grade7, Grade6 | Should the bot craft lvl 90 potions while passing the time? Currently crafts Grade 6/7 Tinctures of Strength, Dexterity, and Intelligence. Grade 7 requires Palaka Mistletoe, which can only be obtained using battle scrips (Astronomy). Ocean Trip will NOT gather Astronomy scrips for you. | 
| Custom Order | True/False | Should the bot read your "BoatOrder.json" file in the root folder of RebornBuddy? |
| Gather Shards | True/False | Should the bot gather shards and crystals while passing the time? |
| Get Materia | True/False | Should the bot purchase IX and X materia using scripts while waiting for the boat? |
| Lisbeth Food | None, Stone Soup, Seafood Stew (Normal/HQ), Chili Crab (HQ), Tasi tou Vounou (HQ), Calamari Ripieni (Normal/HQ) | What food should be used while crafting? |
| Refill Scrips | True/False | Should the bot refill your white/purple crafter scrips while passing the time? Currently refills up to 3000 scrips. |
| Resume Order | True/False | If something was left mid-progress in Lisbeth, should it continue where it left off? |
| Retaining | None, Any city with a summoning bell | Should the bot automatically complete and reassign ventures? |

When Ocean Fishing:

| Ocean Fishing | Values | Description |
| :--- | :--- | :--- |
| Bait Restock Amount | 0 or higher | How much bait should the bot keep in inventory? Set to 0 to disable restocking of bait (not advised). |
| Bait Restock Threshold | 0 or higher | How low on bait should the bot be before it purchases more? Set to 0 to disable restocking of bait (not advised). |
| Exchange Fish | Sell, Desynth, None | What should the bot do with the fish after finishing an Ocean Trip? No blues will be sold or desynthesized. |
| Fish Priority | FishLog, Points, Auto | What should the focus be when fishing on the boat? FishLog may give a really low score as it focuses entirely on missing fish entries. No doublehooking will occur during spectral events while this is set to FishLog. Points will focus on point values, regardless of intuition or missing fish. Double Hooking will occur during spectral events based on Zeke's rules. Auto is a blend of FishLog and Points mode, where it will focus on completing the Fishing Log if possible. If no fish are available at the location/time, then Auto will operate in Points mode. |
| Full GP Action | None, Chum, DoubleHook | What should the bot do when you have full GP? Chum is a good choice while going for your Fishing Log. Double Hook is useless unless it's a spectral event or you're trying to trigger intuition for certain fish. The bot handles that already. | 
| Late Queue | True, False | Should the bot queue up as soon as the boat becomes available or 13 minutes after? If set to true, the bot will wait until the 13 minute mark before attempting to queue. Late queues may contribute to better points, as it is easier to get higher scores and multipliers when less people are on the boat. |
| Ocean Food | None, Peppered Popotoes, Crab Cakes | What food should be used for Ocean Trips? These items give +GP and +Perception. The more GP you have, the better.  If you have less than 10 of the selected food in stock, Lisbeth will attempt to craft 40 more. |

When out in the open world trying to complete your fish log, you can settle down at a fishing hole and allow the bot to assist you in fishing:

| Open World Fishing | Values | Description |
| :--- | :--- | :--- |
| Assisted Fishing | True/False | Should the bot assist when fishing in the open world? You will need to do the initial cast, and the bot will take over until you decide to use the quit ability. The bot will auto-quit and queue up when the boat becomes available. Assisted fishing will auto-mooch, cast, hook, and use Thaliak's favor. |


## Installation

In the plugins tab of RebornBuddy, go to "repoBuddy" and click settings.

In the bottom of the window, you'll see 3 text boxes. Enter the following and click "Add Row".
Repo Name: OceanTrip
Dropdown:  BotBase
Repo URL:  https://github.com/catrenelle/OceanTrip.git/trunk

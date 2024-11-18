
# OceanTrip

This BotBase will take the most complicated parts of Ocean Fishing and simplify it for you. Whether you are going on the original Indigo route or the Ruby route, the BotBase utilizes the publicly available data found on Lulu's Ocean Fishing website. This includes the Ocean Fishing Spreadsheet that is managed by Tyo'to Tayuun and bite timers from Teamcraft as well as data that I've collected during my testing and development of this BotBase.

For best results, you should be level 90 or higher with 900+GP available to use. In my own runs, I'm using fully pentamelded gathering gear with 973 GP. Please note that as far as food goes, the focus is on GP+ and not on actual stats like perception or gathering. This is because stats have little to no influence in what you catch while on the boat.

Please note that your success rates will vary. Some runs will be very low scoring, others can be very high. Ocean Trip has less to do with stats and everything to do with RNG. You can manipulate some factors, such as double hooking at the right time and identical casting when required. I've hit 20k points using this bot many times now, so I know it's easily accomplished.

## Installation

Installation is as simple as opening UpdateBuddy and checking the box for Ocean Trip, then restarting RebornBuddy. If you have an old version from before UpdateBuddy, it's recommended you delete it and install through UpdateBuddy.


## Indigo Route Point Targets
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


## Ruby Route Point Targets
Ideally, you should aim for the following point values and can use this to gauge your potential success rate:

| Bonus Multiplier | 5k Achievement | 10k Minion | 16k Title |
| :--------------: | :-------: | :-------: | :-------: |
| 30 | 3847 | 7693 | 12309 |
| 40 | 3573 | 7144 | 11430 |
| 50 | 3335 | 6668 | 10668 |
| 60 | 3126 | 6251 | 10001 |
| 70 | 2942 | 5883 | 9413 |
| 80 | 2779 | 5557 | 8890 |
| 90 | 2633 | 5264 | 8422 |


## BotBase Settings
When Idling:

During downtime between fishing trips, the BotBase can perform the following actions (Requires Lisbeth):
* Gather Materials
* Gather Aethersand
* Craft Raid Food
* Craft Raid Potions
* Restock Crystals
* Purchase Materia (Grade IV - Grade XII)
	* For Grade XI and XII, you will need to complete the Dawntrail quests and unlock them before the BotBase can farm them.

When Ocean Fishing:
* Specify Fishing Priority
	* Automatic (Default) - Will focus on Fishing Log when missing fish are available, otherwise focuses on points.
	* Points - Self explainatory, will focus on getting points based on DH / TH values of fish available.
	* Fishing Log - Self explainatory, will focus on catching fish you have not caught.
	* Achievements (Currently Disabled - Work in Progress) - Focus on the various fishing achievements such as catching Jellyfish.
	* Ignore Boat - This will allow you to utilize the BotBase in order to focus on the "Idle" activities such as raid preparation.

* Fishing Route
	* Indigo - The "original" route for Ocean Fishing, with zones such as Rothlyt Sound
	* Ruby - This is the route introduced after Endwalker, the "Ruby Sea"
* Full GP Actions 
	* None - Don't do anything outside of normal logic when player GP is full. This is the recommended setting.
	* Double/Triple Hook - This will automatically double or triple hook (based on GP and skill availability) when GP is full.
	* Chum - This will automatically chum when the player is full on GP.
* Trip Options
	* Late Queue - This will queue up for the boat around the 13 minute mark. If disabled, will queue up as soon as the boat is available.
	* Ocean Food - This will have the player eat a predefined food that grants the most GP upon boarding the boat. As of DawnTrail 7.0 release, this is Nasi Goreng. This is subject to change with future releases and may not match what I've listed in this readme.

* Use Patience Skill
	* Default Logic - Will only activate Patience for certain fish that require it. Recommended if level 90 or above.
	* Spectral Only - Will activate Patience Skill for Spectral events and try to keep it active during the spectral.
	* Always - Will always try to keep Patience active. Recommended if below level 90 to catch large fish.

* Fish Exchange
	* None - Keep all the fish you catch.
	* Desynthesize - As it sounds, will Desynthesize all ocean fish you have caught except for the Spectral Blue's.
	* Sell - Sell all the ocean fish you have caught except for the Spectral Blue's.
	
* Indigo Achievement Focus (only if "Achievements" is set for Priority - Currently disabled due to being work in progress)
	* Mantas (Solo) - Attempts to catch 25 mantas
	* Octopods - Attempts to catch 150 octopods (requires party)
	* Sharks - Attempts to catch 200 sharks (requires party)
	* Jellyfish - Attempts to catch 150 jellyfish (requires party)
	* Seadragons - Attempts to catch 100 seadragons (requires party)
	* Balloons - Attempts to catch 250 fugu (requires party)
	* Crabs - Attempts to catch 250 crabs (requires party)

* Ruby Achievement Focus (only if "Achievements" is set for Priority - Currently disabled due to being work in progress)
	* Shrimp (Solo) - Attempts to catch 50 shrimp
	* Shellfish - Attempts to catch 350 shellfish (requires party)
	* Squid - Attempts to catch 400 squid (requires party)

There is an additional option to enable or disable "Open World Fishing". This allows you to settle down at a fishing hole, cast your line with whatever bait you wish and have the bot take over to auto-hook, auto-cast, or auto-mooch. It will not manage your bait for you, but will give you a brief pause before auto-casting to allow you to change bait or use abilities such as Prize Catch. This is not intended to catch big-fish, but will allow you to sit back and relax while filling your fishing log.


## Tacklebox

In the Ocean Settings screen, there is a Tacklebox panel. This panel will show you all the bait required for Ocean Fishing. This is combined between Indigo and Ruby. It is recommended that you have all the necessary bait to go fishing, even if you are under level 90. You can mouse-over the icon for each bait to know what the bait is. The count that appears under the bait icon is how much bait is in your inventory. Ocean Trip will not factor in any bait that you have in a retainer, so make sure to keep it in your inventory or else it may try to restock

* Restock Threshold - If the bait in your inventory goes below this number, the BotBase will try to restock before the next Ocean Trip begins.

* Restock Amount - If the BotBase goes below the threshold and requires restocking, it will purchase bait until you have this amount in your inventory. In case the bait is a lure, it will restock up to 15. This is because you can occasionally lose a lure during fishing (although uncommon).


## Ocean Fishing Achievements

While this feature is currently a work in progress and Achievement focus is disabled, you can track your progress with this panel in the Ocean Settings screen. From a high-level view, you'll know what points achievements you have or need for each route and what your progress is towards the "World Class Troller" title. If you have any of the special focus achievements such as Mantas or Octopods, they will also be displayed here.

If an icon is greyed out, you do not have the achievement or the BotBase was unable to determine that you have the achievement. If it's in full-color, the BotBase was able to determine that you have the achievement.


## Support Development

A lot of work goes into maintaining this and adding support for new patches. The botbase is free to all but if you want to help support me, why not buy me a coffee/tea/drink? 
https://bmc.link/cathousegames


## Special Thanks

This BotBase would not be possible without the work of Antony256, the original author of Ocean Trip. I also want to thank anyone who contributes suggestions, ideas, bug fixes, etc. This includes nt153113 for Llama Library and lots of advice/help provided, as well as Saga for his work on Lisbeth. In addition, anyone that appears as a contributer on the GitHub Repo or has provided support/assistance in the Project BR discord.

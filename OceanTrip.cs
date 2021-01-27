using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Timers;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot;
using ff14bot.AClasses;
using ff14bot.Behavior;
using ff14bot.Enums;
using ff14bot.Helpers;
using ff14bot.Managers;
using ff14bot.Navigation;
using ff14bot.Pathing.Service_Navigation;
using ff14bot.RemoteWindows;
using GreyMagic;
using OceanTripPlanner.RemoteWindows;
using OceanTripPlanner.Helpers;
using TreeSharp;

namespace OceanTripPlanner
{
	public class OceanTrip : BotBase
	{
		private Composite _root;

		public OceanTrip()
		{
		}

		private static readonly Vector3[] fishSpots =
		{
			new Vector3(-7.541584f, 6.74677f, -7.7191f),
			new Vector3(-7.419403f, 6.73973f, -2.7815f),
			new Vector3(7.538965f, 6.745806f, -10.44607f),
			new Vector3(7.178741f, 6.749996f, -4.165483f),
			new Vector3(7.313677f, 6.711103f, -8.10146f),
			new Vector3(7.53893f, 6.745699f, 1.881091f)
		};
		private static readonly float[] headings = new[] { 4.622331f, 4.684318f, 1.569952f, 1.509215f, 1.553197f, 1.576235f };

		private readonly uint[] oceanFish = new uint[] {
			28937,28938,29739,29722,29723,28941,29728,29729,29730,29736,29737,29738,29719,28942,28939,29724,28940,29725, 29731,29735,29733,29718,29740,29741,29734,29721,29726,29727,29742,29732,29720,29743,29744,29745,29746,29747,29766,29749,29750,29751,29752,29753,29754,29755, 29756,29757,29758,29759,29779,29761,29760,29763,29764,29765,29775,29767,29768,29769,29770,29771,29772,29780,29774,29748,29776,29777,29778,29773,29762,29781,29782,29783,29784,29785,29786,29787,29788,29789,29790,29791,32055,32056,32057,32058,32059,32060,32061,32062,32063,32064,32065,32066,32067,32068,32069,32070,32071,32072,32073,32074,32075,32076,32077,32078,32079,32080,32081,32082,32083,32084,32085,32086,32087,32088,32089,32090,32091,32092,32093,32094,32095,32096,32097,32098,32099,32100,32101,32102,32103,32104,32105,32106,32107,32108,32109,32110,32111,32112,32113,32114
		};
		private static readonly int[] fishForSale = new[]
		{
			28937,28938,28939,28940,28941,28942,29718,29719,29720,29721,29722,29723,29724,29725,29726,29727,29728,29729,29730,29731,29732,29733,29734,29735,29736,29737,29738,29739,29740,29741,29742,29743,29744,29745,29746,29747,29748,29749,29750,29751,29752,29753,29754,29755,29756,29757,29758,29759,29760,29761,29762,29763,29764,29765,29766,29767,29768,29769,29770,29771,29772,29773,29774,29775,29776,29777,29778,29779,29780,29781,29782,29784,29785,29786,29787,32055,32056,32057,32058,32059,32060,32061,32062,32063,32064,32065,32066,32067,32068,32069,32070,32071,32072,32073,32075,32076,32077,32078,32079,32080,32081,32082,32083,32084,32085,32086,32087,32088,32089,32090,32091,32092,32093,32095,32096,32097,32098,32099,32100,32101,32102,32103,32104,32105,32106,32107,32108,32109,32110,32111,32112,32113
		};

		private static readonly int[] fullPattern = new[]
		{
			7,10,1,4,8,11,2,5,12,3,6,7,10,1,4,8,11,2,5,9,3,6,7,10,1,4,8,11,2,5,9,12,6,7,10,1,4,8,11,2,5,9,12,3,7,10,1,4,8,11,2,5,9,12,3,6,10,1,4,8,11,2,5,9,12,3,6,7,1,4,8,11,2,5,9,12,3,6,7,10,4,8,11,2,5,9,12,3,6,7,10,1,8,11,2,5,9,12,3,6,7,10,1,4,11,2,5,9,12,3,6,7,10,1,4,8,2,5,9,12,3,6,7,10,1,4,8,11,5,9,12,3,6,7,10,1,4,8,11,2,9,12,3,6
		};

		private static readonly Tuple<string, string>[] sharkCoral = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Sunset"),
			new Tuple<string, string>("south", "Night"),
			new Tuple<string, string>("rhotano", "Day")
		};
		private static readonly Tuple<string, string>[] sothisElasmo = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Sunset"),
			new Tuple<string, string>("galadion", "Night"),
			new Tuple<string, string>("north", "Day")
		};
		private static readonly Tuple<string, string>[] sothisStone = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Night"),
			new Tuple<string, string>("south", "Day"),
			new Tuple<string, string>("rhotano", "Sunset")
		};
		private static readonly Tuple<string, string>[] seadragonCoral = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Night"),
			new Tuple<string, string>("galadion", "Day"),
			new Tuple<string, string>("north", "Sunset")
		};
		private static readonly Tuple<string, string>[] jellyfish = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("galadion", "Day"),
			new Tuple<string, string>("south", "Sunset"),
			new Tuple<string, string>("rhotano", "Night")
		};
		private static readonly Tuple<string, string>[] octopus = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("south", "Day"),
			new Tuple<string, string>("galadion", "Sunset"),
			new Tuple<string, string>("north", "Night")
		};

		private static readonly Tuple<string, string>[] hafgufaElasmo = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Night"),
			new Tuple<string, string>("north", "Day"),
			new Tuple<string, string>("blood", "Sunset")
		};
		private static readonly Tuple<string, string>[] mantas = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Day"),
			new Tuple<string, string>("north", "Sunset"),
			new Tuple<string, string>("blood", "Night")
		};
		private static readonly Tuple<string, string>[] toadCrab = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Sunset"),
			new Tuple<string, string>("north", "Night"),
			new Tuple<string, string>("blood", "Day")
		};
		private static readonly Tuple<string, string>[] hafgufaPlacodus = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Night"),
			new Tuple<string, string>("rhotano", "Day"),
			new Tuple<string, string>("sound", "Sunset")
		};
		private static readonly Tuple<string, string>[] ballonStonescale = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Day"),
			new Tuple<string, string>("rhotano", "Sunset"),
			new Tuple<string, string>("sound", "Night")
		};
		private static readonly Tuple<string, string>[] ballonManta = new Tuple<string, string>[3]
		{
			new Tuple<string, string>("ciel", "Sunset"),
			new Tuple<string, string>("rhotano", "Night"),
			new Tuple<string, string>("sound", "Day")
		};

		private static readonly Tuple<uint, Vector3>[] SummoningBells = new Tuple<uint, Vector3>[]
		{
			new Tuple<uint, Vector3>(129, new Vector3(-123.888062f, 17.990356f, 21.469421f)), //Limsa
			new Tuple<uint, Vector3>(131, new Vector3(148.91272f, 3.982544f, -44.205383f)), //Ul'dah
			new Tuple<uint, Vector3>(133, new Vector3(160.234863f, 15.671021f, -55.649719f)), //Old Gridania(Gridania) 
			new Tuple<uint, Vector3>(156, new Vector3(11.001709f, 28.976807f, -734.554077f)), //Mor Dhona(Mor Dhona) 
			new Tuple<uint, Vector3>(419, new Vector3(-151.171204f, -12.64978f, -11.764771f)), //The Pillars(Ishgard) 
			new Tuple<uint, Vector3>(478, new Vector3(34.775269f, 208.148193f, -50.858398f)), //Idyllshire(Dravania)  
			new Tuple<uint, Vector3>(628, new Vector3(19.394226f, 4.043579f, 53.025024f)), //Kugane(Kugane) 
			new Tuple<uint, Vector3>(635, new Vector3(-57.633362f, -0.01532f, 49.30188f)), //Rhalgr's Reach(Gyr Abania) 
			new Tuple<uint, Vector3>(819, new Vector3(-69.840576f, -7.705872f, 123.491211f)), //The Crystarium
			new Tuple<uint, Vector3>(820, new Vector3(7.186951f, 83.17688f, 31.448853f)) //Eulmore(Eulmore) 
		};

		private int posOnSchedule = 0;

		private static Random rnd = new Random();
		private int spot = rnd.Next(6);
		private Stopwatch biteTimer = new Stopwatch();
		private bool doubleHooked = false;
		private bool spectraled = false;
		private Tuple<string, string>[] schedule = new Tuple<string, string>[3];

		private List<uint> missingFish = new List<uint>();

		static PatternFinder patternFinder = new PatternFinder(Core.Memory);
		static int HomeWorldOffset = patternFinder.Find("0F B7 81 ? ? ? ? 66 89 44 24 ? 48 8D 4C 24 ? Add 3 Read32").ToInt32();
		public static ushort HomeWorld = Core.Memory.NoCacheRead<ushort>(Core.Me.Pointer + HomeWorldOffset);

		System.Timers.Timer execute = new System.Timers.Timer();

		public override string Name => "Ocean Trip";
		public override PulseFlags PulseFlags => PulseFlags.All;

		public override bool IsAutonomous => true;
		public override bool RequiresProfile => false;

		public override Composite Root => _root;

		public override bool WantButton { get; } = true;

		private SettingsForm settings;
		public override void OnButtonPress()
		{
			if (settings == null || settings.IsDisposed)
				settings = new SettingsForm();
			try
			{
				settings.Show();
				settings.Activate();
			}
			catch
			{
			}
		}

		public override void Start()
		{
			TimeSpan stop = new TimeSpan();

			if ((DateTime.UtcNow.Hour % 2 == 0) && (DateTime.UtcNow.Minute > 10))
			{
				stop = new TimeSpan(DateTime.UtcNow.Hour + 2, 10, 0);
			}
			else
			{
				stop = new TimeSpan(DateTime.UtcNow.Hour + DateTime.UtcNow.Hour % 2, 10, 0);
			}

			TimeSpan timeLeftUntilFirstRun = stop - DateTime.UtcNow.TimeOfDay;

			execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;
			execute.Elapsed += new ElapsedEventHandler(KillLisbeth);
			execute.Start();

			Log($"Passing the time for {Math.Round(timeLeftUntilFirstRun.TotalMinutes)} minutes");


			_root = new ActionRunCoroutine(r => Run());
		}

		private async void KillLisbeth(object sender, ElapsedEventArgs e)
		{
			TimeSpan stop = new TimeSpan(DateTime.UtcNow.Hour + 2, 10, 0);

			schedule = GetSchedule();

			//if ((OceanTripSettings.Instance.FishPriority != FishPriority.FishLog) || ((OceanTripSettings.Instance.FishPriority == FishPriority.FishLog) && ((missingFish.Contains(29788) && (schedule == sothisElasmo || schedule == sothisStone)) || (missingFish.Contains(29789) && (schedule == sharkCoral || schedule == seadragonCoral)) || (missingFish.Contains(29790) && (schedule == sothisStone)) || (missingFish.Contains(29791) && (schedule == sothisElasmo)) || (missingFish.Contains(32074) && (schedule == hafgufaElasmo || schedule == hafgufaPlacodus)) || (missingFish.Contains(32094) && (schedule == toadCrab)) || (missingFish.Contains(32114) && (schedule == hafgufaPlacodus)))))
			//{
				Log("Stop!");
				Lisbeth.StopGently();
				PassTheTime.freeToCraft = false;
			//}
			//else
			//{
			//	Log("Not getting on the boat, no fish needed");
			//}
			TimeSpan timeLeftUntilFirstRun = stop - DateTime.UtcNow.TimeOfDay;

			execute.Interval = timeLeftUntilFirstRun.TotalMilliseconds;
			execute.Start();
		}

		public override void Stop()
		{
			execute.Elapsed -= new ElapsedEventHandler(KillLisbeth);
			_root = null;
		}

		private async Task<bool> Run()
		{
			Navigator.PlayerMover = new SlideMover();
			Navigator.NavigationProvider = new ServiceNavigationProvider();

			await OceanFishing();
			return true;
		}

		private async Task OceanFishing()
		{
			if (WorldManager.RawZoneId != 900)
			{
				missingFish = await GetFishLog();

				if (Core.Me.CurrentJob == ClassJobType.Fisher)
				{
					if (OceanTripSettings.Instance.ExchangeFish == ExchangeFish.Sell)
					{
						await LandSell(fishForSale);
					}
					else if (OceanTripSettings.Instance.ExchangeFish == ExchangeFish.Desynth)
					{
						await PassTheTime.DesynthOcean(fishForSale);
					}

					await Lisbeth.SelfRepairWithMenderFallback();
					await LandRepair(90);	
				}

				await RestockBait(150, 500);
				if (OceanTripSettings.Instance.EmptyScrips)
				{
					await EmptyScrips(12669, 1000, 0);
				}

				if (OceanTripSettings.Instance.Venturing != Venturing.None)
				{
					await Retaining();
				}

				Log($"Passing the time");
				PassTheTime.freeToCraft = true;
				await PassTheTime.Craft();

				missingFish = await GetFishLog();
				Log($"Missing Fish:");
				foreach (var fish in missingFish)
				{
					if (oceanFish.Contains(fish))
					{
						Log($"{fish} {DataManager.GetItem(fish).CurrentLocaleName}");
					}
				}

				await SwitchToJob(ClassJobType.Fisher);
				await Lisbeth.SelfRepairWithMenderFallback();
				Log("Waiting for the boat...");
				while (!((DateTime.UtcNow.Hour % 2 == 0) && (DateTime.UtcNow.Minute == 13)))
				{
					await Coroutine.Sleep(1000);
					if (OceanTripSettings.Instance.Venturing != Venturing.None)
					{
						await Retaining();
					}
				}
				
				await SwitchToJob(ClassJobType.Fisher);
				await Navigation.GetTo(129, new Vector3(-410.1068f, 3.999944f, 74.89863f));

				if (DataManager.GetItem(27870).ItemCount() >= 1 && OceanTripSettings.Instance.OceanFood == true)
				{
					Log("Eating food...");
					do
					{
						foreach(BagSlot slot in InventoryManager.FilledSlots)	
						{		
							if(slot.RawItemId == 27870)	
							{	
								slot.UseItem();	
							}
						}
						await Coroutine.Sleep(3000);
					} while (!Core.Player.Auras.Any(x => x.Value == 10419));
					await Coroutine.Sleep(1000);
				}
				else if (OceanTripSettings.Instance.OceanFood == true)
				{
					Log("Out of food!");
				}

				await GetOnBoat();

				spot = rnd.Next(6);
				posOnSchedule = 0;

				schedule = GetSchedule();
			}

			while ((WorldManager.ZoneId == 900) && !ChatCheck("[NPCAnnouncements]","measure your catch"))
			{
				if (ChatCheck("[NPCAnnouncements]","southern Strait"))
				{
					Log($"Southern Merlthor, {schedule[posOnSchedule].Item2}");
					await GoFish(29715, 2613, "south", schedule[posOnSchedule].Item2);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]","Galadion"))
				{
					Log($"Galadion Bay, {schedule[posOnSchedule].Item2}");
					await GoFish(29716, 2603, "galadion", schedule[posOnSchedule].Item2);		
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]","northern Strait"))
				{
					Log($"Northern Merlthor, {schedule[posOnSchedule].Item2}");
					await GoFish(29714, 2619, "north", schedule[posOnSchedule].Item2);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]","Rhotano Sea"))
				{
					Log($"Rhotano Sea, {schedule[posOnSchedule].Item2}");
					await GoFish(29714, 2591, "rhotano", schedule[posOnSchedule].Item2);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]", "Cieldalaes"))
				{
					Log($"Cieldalaes, {schedule[posOnSchedule].Item2}");
					await GoFish(29714, 27590, "ciel", schedule[posOnSchedule].Item2);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]", "Bloodbrine"))
				{
					Log($"Bloodbrine, {schedule[posOnSchedule].Item2}");
					await GoFish(29715, 2587, "blood", schedule[posOnSchedule].Item2);
					posOnSchedule++;
				}
				if (ChatCheck("[NPCAnnouncements]", "Rothlyt Sound"))
				{
					Log($"Rothlyt Sound, {schedule[posOnSchedule].Item2}");
					await GoFish(29716, 29714, "sound", schedule[posOnSchedule].Item2);
					posOnSchedule++;
				}
				await Coroutine.Sleep(500);
			}

			AtkAddonControl windowByName = RaptureAtkUnitManager.GetWindowByName("IKDResult");
			if (windowByName != null)
			{
				await Coroutine.Sleep(12000);
				windowByName.SendAction(1, 3, 0);
				if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
				{
					await Coroutine.Yield();
					await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
				}
			}
			await Coroutine.Sleep(1000);
		}

		private async Task ChangeBait(ulong baitId)
		{
			if ((baitId != FishingManager.SelectedBaitItemId) && (DataManager.GetItem((uint)baitId).ItemCount() > 20) && (DataManager.GetItem((uint)baitId).RequiredLevel <= Core.Me.ClassLevel))
			{
				AtkAddonControl baitWindow = RaptureAtkUnitManager.GetWindowByName("Bait");
				if(baitWindow == null)
				{
					ActionManager.DoAction(288, GameObjectManager.LocalPlayer);
					await Coroutine.Sleep(1000);
					baitWindow = RaptureAtkUnitManager.GetWindowByName("Bait");
				}							
				if(baitWindow != null)
				{
					baitWindow.SendAction(4, 0, 0, 0, 0, 0, 0, 1, baitId);
					Log($"Applied {DataManager.GetItem((uint) baitId).CurrentLocaleName}");
					await Coroutine.Sleep(1000);
					ActionManager.DoAction(288, GameObjectManager.LocalPlayer);
				}	
			}
		}

		private async Task GoFish(ulong baitId, ulong spectralbaitId, string location, string timeOfDay)
		{
			spectraled = false;
			Navigator.PlayerMover.MoveTowards(fishSpots[spot]);
			while (fishSpots[spot].Distance2DSqr(Core.Me.Location) > 2)
			{
				Navigator.PlayerMover.MoveTowards(fishSpots[spot]);
				await Coroutine.Sleep(100);
			}
			Navigator.PlayerMover.MoveStop();
			await Coroutine.Sleep(500);
			Core.Me.SetFacing(headings[spot]);
			await Coroutine.Sleep(1000);

			while ((WorldManager.ZoneId == 900) && !ChatCheck("[NPCAnnouncements]","Weigh the anchors") && !ChatCheck("[NPCAnnouncements]","measure your catch"))
			{
				if (WorldManager.CurrentWeatherId != 145) 
				{
					spectraled = false;
				}
				else
				{
					spectraled = true;
				}

				if (!FishingManager.CanMooch && !ChatCheck("[2115]","Mooch II") && (Core.Me.CurrentGP < 500) && spectraled)
				{
					await UseCordial();
				}

				if (FishingManager.State == FishingState.None || FishingManager.State == FishingState.PoleReady)
				{
					//Identical Cast for Blue fish
					if ((ChatCheck("You land a","gugrusaurus") || ChatCheck("You land a","heavenskey") || ChatCheck("You land a", "grandmarlin")) && (Core.Me.CurrentGP >= 350) && !Core.Player.HasAura(568))
					{
						await Coroutine.Sleep(100);
						if (ActionManager.CanCast(4596, Core.Me))
						{
							Log("Identical Cast!");
							ActionManager.DoAction(4596, Core.Me);
						}
					}

					if (FishingManager.CanMooch && WorldManager.CurrentWeatherId == 145 && spectraled)
					{
						Log("Mooch!");
						FishingManager.Mooch();
						biteTimer.Start();
					}
					else if (ChatCheck("[2115]","Mooch II") && ActionManager.CanCast(268, Core.Me) && Core.Me.CurrentGP >= 200 && WorldManager.CurrentWeatherId == 145 && spectraled)
					{
						Log("Mooch II!");
						ActionManager.DoAction(268, Core.Me);
						biteTimer.Start();
					}
					else
					{
						//<!-- plump 29716 -->
						//<!-- krill 29715 -->
						//<!-- ragworm 29714 -->
						if (WorldManager.CurrentWeatherId == 145)
						{
							//Bait for Blue fish
							if (((location == "galadion") && (timeOfDay == "Night") && missingFish.Contains(29788)) || ((location == "south") && (timeOfDay == "Night") && missingFish.Contains(29789)) || ((location == "north") && (timeOfDay == "Day") && missingFish.Contains(29791)) || ((location == "rhotano") && (timeOfDay == "Sunset") && missingFish.Contains(29790)) || ((location == "ciel") && (timeOfDay == "Night") && missingFish.Contains(32074)) || ((location == "blood") && (timeOfDay == "Day") && missingFish.Contains(32094)) || ((location == "sound") && (timeOfDay == "Sunset") && missingFish.Contains(32114)))
							{
								await ChangeBait(spectralbaitId);
							}
							else if (((location == "galadion") && (timeOfDay == "Sunset")) || ((location == "rhotano") && (timeOfDay == "Day")) || ((location == "north") && (timeOfDay == "Day")) || ((location == "south") && (timeOfDay == "Night")) || ((location == "ciel") && (timeOfDay == "Sunset")))
							{
								await ChangeBait(29716); //plump
							}
							else if (((location == "south") && (timeOfDay == "Day")) || ((location == "rhotano") && (timeOfDay == "Night") && ((OceanTripSettings.Instance.FishPriority != FishPriority.FishLog) || !missingFish.Contains(29774))) || ((location == "north") && (timeOfDay == "Sunset") && missingFish.Contains(29783) && (OceanTripSettings.Instance.FishPriority == FishPriority.FishLog)) || ((location == "north") && (timeOfDay == "Night") && ((OceanTripSettings.Instance.FishPriority != FishPriority.FishLog) || !missingFish.Contains(29777))) || ((location == "galadion") && (timeOfDay == "Night")) || ((location == "ciel") && (timeOfDay == "Day")))
							{
								await ChangeBait(29715); //krill
							}
							else if (((location == "galadion") && (timeOfDay == "Day")) || ((location == "south") && (timeOfDay == "Sunset")) || ((location == "north") && (timeOfDay == "Sunset")) || ((location == "north") && (timeOfDay == "Night")) || ((location == "rhotano") && (timeOfDay == "Night")))
							{
								await ChangeBait(29714); //ragworm
							}
							else
							{
								await ChangeBait(spectralbaitId);
							}
						}
						else
						{
							await ChangeBait(baitId);
						}
						biteTimer.Start();
						FishingManager.Cast();	
					}
					await Coroutine.Sleep(50);
					doubleHooked = false;
				}
				while ((FishingManager.State != FishingState.PoleReady) && !ChatCheck("[NPCAnnouncements]","Weigh the anchors") && !ChatCheck("[NPCAnnouncements]","measure your catch"))
				{
					await Coroutine.Sleep(50);

					//Spectral popped, don't wait for normal fish
					if (WorldManager.CurrentWeatherId == 145 && !spectraled)
					{
						spectraled = true;
						if (FishingManager.CanHook)
						{
							FishingManager.Hook();
						}
					}
					if (FishingManager.CanHook && FishingManager.State == FishingState.Bite)
					{
						biteTimer.Stop();
						Log($"Bite Time: {biteTimer.Elapsed.TotalSeconds:F1}s");
						if ((((location == "galadion") && (((biteTimer.Elapsed.TotalSeconds >= 7) && (FishingManager.TugType != TugType.Medium) && (timeOfDay != "Night")) || ((biteTimer.Elapsed.TotalSeconds > 1) && (biteTimer.Elapsed.TotalSeconds <= 4) && (FishingManager.TugType == TugType.Medium)))) || ((location == "south") && (((biteTimer.Elapsed.TotalSeconds >= 6) && (timeOfDay == "Sunset") && (FishingManager.TugType == TugType.Light)) || ((biteTimer.Elapsed.TotalSeconds >= 2) && (timeOfDay == "Sunset") && (FishingManager.TugType == TugType.Heavy) && (FishingManager.MoochLevel == 1)) || ((biteTimer.Elapsed.TotalSeconds >= 2) && (biteTimer.Elapsed.TotalSeconds <= 6) && (timeOfDay == "Night") && (FishingManager.TugType == TugType.Medium) && (FishingManager.MoochLevel == 1)) || ((biteTimer.Elapsed.TotalSeconds >= 4) && (biteTimer.Elapsed.TotalSeconds <= 7) && (timeOfDay == "Day") && (FishingManager.TugType == TugType.Medium)))) || ((location == "north") && (((biteTimer.Elapsed.TotalSeconds >= 5) && (biteTimer.Elapsed.TotalSeconds <= 9) && (timeOfDay == "Night") && (FishingManager.TugType != TugType.Light)) || ((biteTimer.Elapsed.TotalSeconds >= 7) && (biteTimer.Elapsed.TotalSeconds <= 12) && (timeOfDay == "Sunset") && (FishingManager.TugType == TugType.Light)) || ((biteTimer.Elapsed.TotalSeconds >= 6) && (biteTimer.Elapsed.TotalSeconds <= 9) && (timeOfDay == "Sunset") && (FishingManager.TugType == TugType.Medium)))) || ((location == "rhotano") && (((biteTimer.Elapsed.TotalSeconds >= 7) && (biteTimer.Elapsed.TotalSeconds <= 11) && (timeOfDay == "Night") && (FishingManager.TugType == TugType.Light)) || ((biteTimer.Elapsed.TotalSeconds >= 6) && (biteTimer.Elapsed.TotalSeconds <= 10) && (timeOfDay == "Day") && (FishingManager.TugType == TugType.Heavy))))) && (WorldManager.CurrentWeatherId == 145) && ActionManager.CanCast(269, Core.Me) && (Core.Me.CurrentGP >= 400))
						{
							Log("Double Hook!");
							ActionManager.DoAction(269, Core.Me);
							doubleHooked = true;
						}
						else if (FishingManager.HasPatience)
						{
							if (FishingManager.TugType == TugType.Light)
							{
								ActionManager.DoAction(4179, Core.Me);
							}
							else
							{
								ActionManager.DoAction(4103, Core.Me);
							}
						}
						else
						{
							FishingManager.Hook();
						}
						biteTimer.Reset();
					}
				}
			}

			spectraled = false;
			await Coroutine.Sleep(2000);
			Log("Waiting for next stop...");
			if (FishingManager.State != FishingState.None)
			{
				ActionManager.DoAction(299, Core.Me);
			}
		}

		private static async Task GetOnBoat()
		{	
			var Dryskthota = GameObjectManager.GetObjectByNPCId(1005421);

			if (Dryskthota != null)
			{
				Dryskthota.Interact();
				if (await Coroutine.Wait(5000, () => Talk.DialogOpen))
				{
					Talk.Next();
				}

				await Coroutine.Wait(5000, () => SelectString.IsOpen);

				if (SelectString.IsOpen)
				{
					SelectString.ClickSlot(0);
					await Coroutine.Wait(5000, () => SelectYesno.IsOpen);
					SelectYesno.Yes();

					await Coroutine.Wait(1000000, () => ContentsFinderConfirm.IsOpen);

					await Coroutine.Yield();
					while (ContentsFinderConfirm.IsOpen)
					{
						DutyManager.Commence();
						await Coroutine.Yield();
						if (await Coroutine.Wait(30000, () => CommonBehaviors.IsLoading))
						{
							await Coroutine.Yield();
							await Coroutine.Wait(Timeout.Infinite, () => !CommonBehaviors.IsLoading);
						}
					}
					while (WorldManager.ZoneId != 900)
					{
						await Coroutine.Sleep(1000);
					}
					await Coroutine.Sleep(2500);
					Logging.Write(Colors.Aqua, "On the boat");
				}
			}
		}

		private static async Task SwitchToJob(ClassJobType job)
		{
			if (Core.Me.CurrentJob == job) return;

			var gearSets = GearsetManager.GearSets.Where(i => i.InUse);

			if (gearSets.Any(gs => gs.Class == job))
			{
				Logging.Write(Colors.Fuchsia, $"[ChangeJob] Found GearSet");
				gearSets.First(gs => gs.Class == job).Activate();
				await Coroutine.Sleep(1000);
			}
		}		

		private async Task UseCordial()
		{
			if (Core.Me.CurrentGP < 500)
			{
				await Coroutine.Sleep(500);
				foreach(ff14bot.Managers.BagSlot slot in ff14bot.Managers.InventoryManager.FilledSlots)	
				{	
					if(slot.RawItemId == 12669)	
					{	
						slot.UseItem();	
					}
				}
				await Coroutine.Sleep(2000);
			}
		}

		private static async Task LandRepair(int repairThreshold)
		{
			if (InventoryManager.EquippedItems.Where(r => r.IsFilled && r.Condition < repairThreshold).Count() > 0)
			{	
				Logging.Write(Colors.Aqua, "Starting repair...");
				await Navigation.GetTo(129, new Vector3(-398.5143f, 3.099996f, 81.47765f));
				await Coroutine.Sleep(1000);
				GameObjectManager.GetObjectByNPCId(1005422).Interact();
				await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
				if (SelectIconString.IsOpen)
				{
					SelectIconString.ClickSlot(1);
					await Coroutine.Wait(3000, () => Repair.IsOpen);
					if (Repair.IsOpen)
					{
						Repair.RepairAll();
						await Coroutine.Wait(3000, () => SelectYesno.IsOpen);
						if (SelectYesno.IsOpen)
						{
							SelectYesno.ClickYes();
						}
						Repair.Close();
						await Coroutine.Wait(5000, () => !Repair.IsOpen);
					}
				}
				Logging.Write(Colors.Aqua, "Repair complete");
			}
		}

		private async Task RestockBait(int baitThreshold, uint baitCap)
		{
			List<uint> itemsToBuy = new List<uint>();
			if (DataManager.GetItem(29714).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(29714);
			}
			if (DataManager.GetItem(29715).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(29715);
			}
			if (DataManager.GetItem(29716).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(29716);
			}
			if (DataManager.GetItem(2591).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(2591);
			}
			if (DataManager.GetItem(2603).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(2603);
			}
			if (DataManager.GetItem(2619).ItemCount() < 5)
			{
				itemsToBuy.Add(2619);
			}
			if (DataManager.GetItem(2613).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(2613);
			}
			if (DataManager.GetItem(2587).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(2587);
			}
			if (DataManager.GetItem(27590).ItemCount() < baitThreshold)
			{
				itemsToBuy.Add(27590);
			}

			if (itemsToBuy.Any())
			{
				Log("Restocking bait...");
				if (itemsToBuy.Contains(29714) || itemsToBuy.Contains(29715) || itemsToBuy.Contains(29716))
				{
					await Navigation.GetTo(129, new Vector3(-398.5143f, 3.099996f, 81.47765f));
					await Coroutine.Sleep(1000);
					GameObjectManager.GetObjectByNPCId(1005422).Interact();
					await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
					if (SelectIconString.IsOpen)
					{
						SelectIconString.ClickSlot(0);
						await Coroutine.Wait(5000, () => Shop.Open);
						foreach (uint item in itemsToBuy)
						{
							if ((item == 29714) || (item == 29715) || (item == 29716))
							{
								Shop.Purchase(item, (baitCap - DataManager.GetItem(item).ItemCount()));
								await Coroutine.Wait(2000, () => SelectYesno.IsOpen);
								SelectYesno.ClickYes();
							}
							await Coroutine.Sleep(1000);
						}
					}
					await Coroutine.Sleep(1000);
					Shop.Close();
					await Coroutine.Wait(5000, () => !Shop.Open);
				}
				if (itemsToBuy.Contains(2591) || itemsToBuy.Contains(2603) || itemsToBuy.Contains(2613) || itemsToBuy.Contains(2587))
				{
					await Navigation.GetTo(129, new Vector3(-247.6223f, 16.2f, 39.87407f));
					await Coroutine.Sleep(1000);
					GameObjectManager.GetObjectByNPCId(1003254).Interact();
					await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
					if (SelectIconString.IsOpen)
					{
						SelectIconString.ClickSlot(2);
						await Coroutine.Wait(5000, () => Shop.Open);
						foreach (uint item in itemsToBuy)
						{
							if ((item == 2591) || (item == 2603) || (item == 2613) || (item == 2587))
							{
								Shop.Purchase(item, (baitCap - DataManager.GetItem(item).ItemCount()));
								await Coroutine.Wait(2000, () => SelectYesno.IsOpen);
								SelectYesno.ClickYes();
							}
							await Coroutine.Sleep(1000);
						}
					}
					await Coroutine.Sleep(1000);
					Shop.Close();
					await Coroutine.Wait(5000, () => !Shop.Open);
				}

				if (itemsToBuy.Contains(2619) && Core.Me.Levels[ClassJobType.Goldsmith] >= 36)
				{
					await PassTheTime.IdleLisbeth(2619, 20, "Goldsmith", "true", (int)OceanTripSettings.Instance.LisbethFood);
				}
				if (itemsToBuy.Contains(27590))
				{
					await PassTheTime.IdleLisbeth(27590, 300, "Exchange", "true", (int)OceanTripSettings.Instance.LisbethFood);
				}
				Log("Restocking bait complete");
			}
		}

		private async Task LandSell(int[] itemIds)
		{
			var itemsToSell = InventoryManager.FilledSlots.Where(bs => bs.IsSellable && itemIds.Contains((int)bs.RawItemId));
			if (itemsToSell.Count() != 0)
			{
				Log("Selling fish...");
				await Navigation.GetTo(129, new Vector3(-398.5143f, 3.099996f, 81.47765f));
				await Coroutine.Sleep(1000);
				GameObjectManager.GetObjectByNPCId(1005422).Interact();
				await Coroutine.Wait(3000, () => SelectIconString.IsOpen);
				if (SelectIconString.IsOpen)
				{
					SelectIconString.ClickSlot(0);
					await Coroutine.Wait(5000, () => Shop.Open);
					foreach (var item in itemsToSell)
					{		
						if(item.Value <= 18)
						{
							var name = item.Name;
							await CommonTasks.SellItem(item);
							await Coroutine.Wait(10000, () => !item.IsFilled || !item.Name.Equals(name));
							await Coroutine.Sleep(1000);	
						}
					}
					Shop.Close();
					await Coroutine.Wait(2000, () => !Shop.Open);
				}
				Log("Sale complete");	
			}
		}

		private async Task EmptyScrips(uint itemId, int scripThreshold, uint slot)
		{
			//TODO: Buy other stuff with scrip
			if (SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency) 17834) > scripThreshold)
			{
				Log($"Buying {DataManager.GetItem(itemId).CurrentLocaleName}s");
				await Navigation.GetTo(129, new Vector3(-406.1322f, 3.099996f, 67.07707f));
				var ScripStore = GameObjectManager.GetObjectByNPCId(1005423);
				if (ScripStore != null)
				{
					ScripStore.Interact();
				}
				uint count = SpecialCurrencyManager.GetCurrencyCount((SpecialCurrency) 17834)/20;
				await Coroutine.Wait(5000, () => SelectIconString.IsOpen);

				if (SelectIconString.IsOpen)
				{
					SelectIconString.ClickSlot(slot);

					await Coroutine.Wait(5000, () => ShopExchangeCurrency.Open);

					if (ShopExchangeCurrency.Open)
					{
						ShopExchangeCurrency.Purchase(itemId, count);
						await Coroutine.Wait(2000, () => SelectYesno.IsOpen || Request.IsOpen);

						if (SelectYesno.IsOpen)
						{
							SelectYesno.Yes();
							await Coroutine.Sleep(1000);
						}
					}

					await Coroutine.Wait(2000, () => ShopExchangeCurrency.Open);
					if (ShopExchangeCurrency.Open)
					{
						ShopExchangeCurrency.Close();
					}
					await Coroutine.Sleep(1000);
				}
				Log("Purchase complete");
			}
		}

		private async Task Retaining()
		{
			if(OceanTripSettings.Instance.VentureTime < DateTime.Now)
			{
				await Navigation.GetTo(SummoningBells[(int)OceanTripSettings.Instance.Venturing].Item1, SummoningBells[(int)OceanTripSettings.Instance.Venturing].Item2);

				foreach (var unit in GameObjectManager.GameObjects.OrderBy(r => r.Distance()))
				{
					if (unit.NpcId == 2000401 || unit.NpcId == 2000441)
					{
						unit.Interact();
						break;
					}
				}
				await Coroutine.Sleep(2000);
				string bell = Lua.GetReturnVal<string>(string.Format("local values = '' for key,value in pairs(_G) do if string.match(key, '{0}:') then return key;   end end return values;", "CmnDefRetainerBell")).Trim();
				int numOfRetainers = 0;

				if (bell.Length > 0)
				{
					numOfRetainers = Lua.GetReturnVal<int>(string.Format("return _G['{0}']:GetRetainerEmployedCount();", bell));
				}

				AtkAddonControl retainerWindow = RaptureAtkUnitManager.GetWindowByName("RetainerList");
				while (retainerWindow == null)
				{
					await Coroutine.Sleep(1000);
					retainerWindow = RaptureAtkUnitManager.GetWindowByName("RetainerList");
				}

				int count = 0;
				while (count < numOfRetainers)
				{
					retainerWindow = null;
					while (retainerWindow == null)
					{	
						await Coroutine.Sleep(1000);
						retainerWindow = RaptureAtkUnitManager.GetWindowByName("RetainerList");
					}
					retainerWindow.SendAction(2, 3UL, 2UL, 3UL, (ulong) count);
					if (await Coroutine.Wait(15000, () => Talk.DialogOpen))
					{
						Talk.Next();
					}
					if (await Coroutine.Wait(20000, () => SelectString.IsOpen))
					{
						if (SelectString.Lines().Contains("View venture report. (Complete)"))
						{
							SelectString.ClickLineEquals("View venture report. (Complete)");
							if (await Coroutine.Wait(20000, () => RetainerTaskResult.IsOpen))
							{
								RetainerTaskResult.Reassign();
								if (await Coroutine.Wait(10000, () => RetainerTaskAsk.IsOpen))
								{
									RetainerTaskAsk.Confirm();
									if (await Coroutine.Wait(10000, () => Talk.DialogOpen))
									{
										Talk.Next();
									}
								}
							}
						}
						await Coroutine.Wait(20000, () => SelectString.IsOpen);
						if(SelectString.Lines().Any(x => x.Contains("View venture report. (Complete on")))
						{
							Regex r = new Regex(@"(\d+[-.\/]\d+ \d+:\d+)");
							Match m = r.Match(SelectString.Lines().FirstOrDefault(x => x.Contains("View venture report. (Complete on")).ToString());
							if(m.Success)
							{
								DateTime ventureTime = DateTime.ParseExact(m.Value, "d/M H:mm", null);
								if ((ventureTime < OceanTripSettings.Instance.VentureTime && OceanTripSettings.Instance.VentureTime > DateTime.Now) || (ventureTime > OceanTripSettings.Instance.VentureTime && OceanTripSettings.Instance.VentureTime < DateTime.Now))
								{
									OceanTripSettings.Instance.VentureTime = ventureTime;
								}

							}
						}
						if (await Coroutine.Wait(20000, () => SelectString.IsOpen))
						{
							SelectString.ClickLineEquals("Quit.");
						}
						if (await Coroutine.Wait(10000, () => Talk.DialogOpen))
						{
							Talk.Next();
						}					
					}
					count++;
				}
				retainerWindow = null;
				while (retainerWindow == null)
				{	
					await Coroutine.Sleep(1000);
					retainerWindow = RaptureAtkUnitManager.GetWindowByName("RetainerList");
				}
				retainerWindow.SendAction(1, 3UL, (ulong) uint.MaxValue);
				await Coroutine.Sleep(3000);
			}
		}

		private async Task<List<uint>> GetFishLog()
		{
			List<uint> recordedFish = new List<uint>();
			if (!FishGuide.IsOpen)
			{
				FishGuide.Toggle();
				await Coroutine.Wait(5000, () => FishGuide.IsOpen);
			}

			if (FishGuide.IsOpen)
			{
				for (int i = 33; i < FishGuide.TabCount; i++)
				{
					FishGuide.ClickTab(i);
					await Coroutine.Sleep(10);
					var list = FishGuide.GetTabList();
					foreach (var fishy in list.Select(x => x.FishItem))
					{
						if (fishy != 0 && fishy != uint.MaxValue)
						{
							recordedFish.Add(fishy);
						}
					}
				}

				FishGuide.Toggle();
				await Coroutine.Wait(5000, () => !FishGuide.IsOpen);
			}
			return oceanFish.Except(recordedFish).ToList();
		}

		private bool ChatCheck(string chattype, string chatmessage)
		{
			try
			{
				return GamelogManager.CurrentBuffer.Last(chatline => chatline.FullLine.Contains(chattype)).FullLine.Contains(chatmessage);
			}
			catch
			{
				return false;
			}
		}

		private Tuple<string, string>[] GetSchedule()
		{
			var epoch = (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
			int twoHourChunk = (int)(((epoch / 1000 / 7200) + 103) % 143);
			
			//Log(twoHourChunk.ToString());
			//Log(fullPattern[twoHourChunk].ToString());
			//Log(fullPattern[twoHourChunk+1].ToString());
			//Log(fullPattern[twoHourChunk+2].ToString());
			//Log(fullPattern[twoHourChunk+3].ToString());
			//Log(fullPattern[twoHourChunk+4].ToString());
			//Log(fullPattern[twoHourChunk+5].ToString());

			switch (fullPattern[twoHourChunk])
			{
				case 1:
					return seadragonCoral;
				case 2:
					return octopus;
				case 3:
					return sothisElasmo;
				case 4:
					return sothisStone;
				case 5:
					return jellyfish;
				case 6:
					return sharkCoral;
				case 7:
					return hafgufaElasmo;
				case 8:
					return mantas;
				case 9:
					return toadCrab;
				case 10:
					return hafgufaPlacodus;
				case 11:
					return ballonStonescale;
				case 12:
					return ballonManta;
			}
			return null;
		}

		//private Tuple<string, string>[] GetSchedule()
		//{
		//	long lulu_epoch = 1593302400000;

		//	long epoch = (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalMilliseconds;
		//	long adjustedDate = epoch + 29700000;
		//	long day = adjustedDate - lulu_epoch / 86400000;
		//	long hour = DateTimeOffset.FromUnixTimeMilliseconds(adjustedDate).Hour;

		//	hour += ((hour & 1) == 1) ? 2 : 1;

		//	if (hour > 23)
		//	{
		//		day++;
		//		hour -= 24;
		//	}

		//	long voyageNumber = hour >> 1;
		//	long destIndex = (day + voyageNumber) % 4;
		//	long timeIndex = (day + voyageNumber) % 12;

		//	switch (fullPattern[twoHourChunk])
		//	{
		//		case 1:
		//			return seadragonCoral;
		//		case 2:
		//			return octopus;
		//		case 3:
		//			return sothisElasmo;
		//		case 4:
		//			return sothisStone;
		//		case 5:
		//			return jellyfish;
		//		case 6:
		//			return sharkCoral;
		//		case 7:
		//			return hafgufaElasmo;
		//		case 8:
		//			return mantas;
		//		case 9:
		//			return toadCrab;
		//		case 10:
		//			return hafgufaPlacodus;
		//		case 11:
		//			return ballonStonescale;
		//		case 12:
		//			return ballonManta;
		//	}
		//	return null;
		//}

		private void Log(string text, params object[] args)
		{
			var msg = string.Format("[" + Name + "] " + text, args);
			Logging.Write(Colors.Aqua, msg);
		}

	}
}
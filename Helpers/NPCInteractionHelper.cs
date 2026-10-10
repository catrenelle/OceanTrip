using System;
using System.Threading.Tasks;
using Buddy.Coroutines;
using Clio.Utilities;
using ff14bot.Managers;
using ff14bot.RemoteWindows;
using OceanTripPlanner.Definitions;

namespace OceanTripPlanner.Helpers
{
	/// <summary>
	/// Helper class for common NPC interaction patterns
	/// Reduces duplication in navigation and dialog handling
	/// </summary>
	public static class NPCInteractionHelper
	{
		/// <summary>
		/// Navigate to Limsa Lominsa Lower Decks Fishing Merchant/Mender
		/// </summary>
		public static async Task NavigateToLimsaMender()
		{
			await Navigation.GetTo(Zones.LimsaLominsaLowerDecks, new Vector3(-398.5143f, 3.099996f, 81.47765f));
			await Coroutine.Sleep(FishingConstants.STANDARD_DELAY_MS);
		}

		/// <summary>
		/// Interact with the Limsa Fishing Merchant/Mender NPC
		/// </summary>
		public static void InteractWithLimsaMender()
		{
			GameObjectManager.GetObjectByNPCId(NPC.LimsaFishingMerchantMender).Interact();
		}

		/// <summary>
		/// Wait for a dialog window to open with specified timeout
		/// </summary>
		/// <typeparam name="T">Type that has an IsOpen property</typeparam>
		/// <param name="condition">Condition to wait for</param>
		/// <param name="timeoutMs">Timeout in milliseconds</param>
		/// <returns>True if window opened, false if timeout</returns>
		public static async Task<bool> WaitForWindow(Func<bool> condition, int timeoutMs)
		{
			return await Coroutine.Wait(timeoutMs, condition);
		}

		/// <summary>
		/// Complete interaction with Limsa mender: navigate, interact, wait for menu
		/// </summary>
		/// <returns>True if menu opened successfully, false otherwise</returns>
		public static async Task<bool> InteractWithMenderAndWaitForMenu()
		{
			await NavigateToLimsaMender();
			InteractWithLimsaMender();
			return await WaitForWindow(() => SelectIconString.IsOpen, FishingConstants.DIALOG_WINDOW_TIMEOUT_MS);
		}

		/// <summary>
		/// Select an option from SelectIconString menu by slot index
		/// </summary>
		/// <param name="slotIndex">0-based slot index to click</param>
		public static void SelectIconStringSlot(uint slotIndex)
		{
			if (SelectIconString.IsOpen)
				SelectIconString.ClickSlot(slotIndex);
		}

		/// <summary>
		/// Cancel a SelectIconString dialog by clicking "Nothing"
		/// </summary>
		public static void CancelSelectIconString()
		{
			if (SelectIconString.IsOpen)
				SelectIconString.ClickLineEquals("Nothing");
		}

		/// <summary>
		/// Wait for the Repair window to open, then repair all equipment
		/// </summary>
		/// <returns>True if repair completed successfully, false otherwise</returns>
		public static async Task<bool> RepairAllEquipment()
		{
			if (!await WaitForWindow(() => Repair.IsOpen, FishingConstants.DIALOG_WINDOW_TIMEOUT_MS))
				return false;

			if (!Repair.IsOpen)
				return false;

			Repair.RepairAll();

			// Wait for confirmation dialog
			if (await WaitForWindow(() => SelectYesno.IsOpen, FishingConstants.DIALOG_WINDOW_TIMEOUT_MS))
			{
				if (SelectYesno.IsOpen)
					SelectYesno.ClickYes();
			}

			Repair.Close();
			await Coroutine.Wait(FishingConstants.REPAIR_WINDOW_TIMEOUT_MS, () => !Repair.IsOpen);

			return true;
		}

		/// <summary>
		/// Wait for shop window to open
		/// </summary>
		/// <returns>True if shop opened, false otherwise</returns>
		public static async Task<bool> WaitForShopOpen()
		{
			return await WaitForWindow(() => Shop.Open, FishingConstants.SHOP_WINDOW_TIMEOUT_MS);
		}

		/// <summary>
		/// Close the shop window and wait for it to close
		/// </summary>
		public static async Task CloseShop()
		{
			Shop.Close();
			await Coroutine.Wait(FishingConstants.SHOP_CLOSE_TIMEOUT_MS, () => !Shop.Open);
		}
	}
}

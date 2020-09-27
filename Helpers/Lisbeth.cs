using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace OceanTripPlanner.Helpers
{
	public static class Lisbeth
	{
		private static object _lisbeth;
		private static MethodInfo _orderMethod;
		private static Func<Task> _equipOptimalGear, _selfRepairWithMenderFallback;
		private static Func<Task> _stopGently;
		private static Action<string, Func<Task>> _addHook;
		private static Action<string> _removeHook;
		private static Func<List<string>> _getHookList;
		private static Action _openWindow;


		static Lisbeth()
		{
			FindLisbeth();
		}

		internal static void FindLisbeth()
		{
			var loader = BotManager.Bots
				.FirstOrDefault(c => c.Name == "Lisbeth");

			if (loader == null) return;

			var lisbethObjectProperty = loader.GetType().GetProperty("Lisbeth");
			var lisbeth = lisbethObjectProperty?.GetValue(loader);
			var orderMethod = lisbeth?.GetType().GetMethod("ExecuteOrders");
			var apiObject = lisbeth.GetType().GetProperty("Api")?.GetValue(lisbeth);
			if (lisbeth == null || orderMethod == null) return;
			if (apiObject != null)
			{
				var m = apiObject.GetType().GetMethod("GetCurrentAreaName");
				if (m != null)

				{
					try
					{
						_stopGently = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "StopGently");
						//_stopGentlyAndWait = (Func<Task>) Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "StopGentlyAndWait");
						_addHook = (Action<string, Func<Task>>)Delegate.CreateDelegate(typeof(Action<string, Func<Task>>), apiObject, "AddHook");
						_removeHook = (Action<string>)Delegate.CreateDelegate(typeof(Action<string>), apiObject, "RemoveHook");
						_getHookList = (Func<List<string>>)Delegate.CreateDelegate(typeof(Func<List<string>>), apiObject, "GetHookList");
						_selfRepairWithMenderFallback = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "SelfRepairWithMenderFallback");
						_openWindow = (Action)Delegate.CreateDelegate(typeof(Action), apiObject, "OpenWindow");
					}
					catch (Exception e)
					{
						Logging.Write(e.ToString());
					}
				}
			}

			_orderMethod = orderMethod;
			_lisbeth = lisbeth;

			Logging.Write("Lisbeth found.");
		}

		internal static async Task<bool> ExecuteOrders(string json)
		{
			if (_orderMethod != null) return await (Task<bool>)_orderMethod.Invoke(_lisbeth, new object[] { json, false });

			FindLisbeth();
			if (_orderMethod == null)
				return false;

			return await (Task<bool>)_orderMethod.Invoke(_lisbeth, new object[] { json, false });
		}

		public static async Task StopGently()
		{
			await _stopGently();
		}


		public static void AddHook(string name, Func<Task> function)
		{
			_addHook?.Invoke(name, function);
		}

		public static void RemoveHook(string name)
		{
			_removeHook?.Invoke(name);
		}

		public static List<string> GetHookList()
		{
			return _getHookList?.Invoke();
		}

		public static async Task EquipOptimalGear()
		{
			await _equipOptimalGear();
		}

		public static async Task SelfRepairWithMenderFallback()
		{
			await _selfRepairWithMenderFallback();
		}

		public static void OpenWindow()
		{
			_openWindow();
		}
	}
}
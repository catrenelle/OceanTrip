using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace OceanTripPlanner.Helpers
{
	public static class Lisbeth
    {
        private static Func<Task> _stopGently, _selfRepairWithMenderFallback;
        private static Action _openWindow;

        private static object _lisbeth;
        private static MethodInfo _orderMethod;

        static Lisbeth()
        {
            FindLisbeth();
        }

        public static async Task<bool> ExecuteOrders(string json)
        {
            if (_orderMethod == null) { return false; }

            return await (Task<bool>)_orderMethod.Invoke(_lisbeth, new object[] { json, false });
        }

        private static object GetLisbethBotObject()
        {
            var loader = BotManager.Bots
                .FirstOrDefault(c => c.Name == "Lisbeth");

            if (loader == null) { return null; }

            var lisbethObjectProperty = loader.GetType().GetProperty("Lisbeth");
            var lisbeth = lisbethObjectProperty?.GetValue(loader);

            return lisbeth;
        }

        private static void FindLisbeth()
        {
            var lisbeth = GetLisbethBotObject();
            if (lisbeth == null) { return; }

            var orderMethod = lisbeth.GetType().GetMethod("ExecuteOrders");
            if (orderMethod == null) { return; }

            _orderMethod = orderMethod;
            _lisbeth = lisbeth;

            var apiObject = lisbeth.GetType().GetProperty("Api")?.GetValue(lisbeth);

            if (apiObject != null)
            {
                _stopGently = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "StopGently");
                _selfRepairWithMenderFallback = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "SelfRepairWithMenderFallback");
                _openWindow = (System.Action)Delegate.CreateDelegate(typeof(System.Action), apiObject, "OpenWindow");
            }

            Logging.Write("Lisbeth found.");
        }

        public static async Task StopGently()
        {
            if (_stopGently == null) { return; }
            await _stopGently();
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
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using ff14bot.Helpers;
using ff14bot.Managers;

namespace OceanTripPlanner.Helpers
{
	public static class LlamaMarket
    {
        private static Func<Task> _stopGently, _selfRepairWithMenderFallback;
        private static Func<int> _getEmptyInventorySlotCount;
        private static Action _openWindow;

        private static object _llamamarket;
        private static MethodInfo _orderMethod;

        static LlamaMarket()
        {
            FindLlamaMarket();
        }

        public static async Task<bool> ExecuteOrders(string json)
        {
            if (_orderMethod == null) { return false; }

            return await (Task<bool>)_orderMethod.Invoke(_llamamarket, new object[] { json, false });
        }

        private static object GetLlamaMarketBotObject()
        {
            var loader = BotManager.Bots
                .FirstOrDefault(c => c.Name == "Llama Market");

            if (loader == null) { return null; }

            var lisbethObjectProperty = loader.GetType().GetProperty("LlamaMarket");
            var lisbeth = lisbethObjectProperty?.GetValue(loader);

            return lisbeth;
        }

        private static void FindLlamaMarket()
        {
            try
            {
                var llamamarket = GetLlamaMarketBotObject();
                if (llamamarket == null) { return; }

                var orderMethod = llamamarket.GetType().GetMethod("ExecuteOrders");
                if (orderMethod == null) { return; }

                _orderMethod = orderMethod;
                _llamamarket = llamamarket;

                var apiObject = llamamarket.GetType().GetProperty("Api")?.GetValue(llamamarket);

                if (apiObject != null)
                {
                    _stopGently = (Func<Task>)Delegate.CreateDelegate(typeof(Func<Task>), apiObject, "StopGently");
                    _openWindow = (System.Action)Delegate.CreateDelegate(typeof(System.Action), apiObject, "OpenWindow");
                }

                Logging.Write("LlamaMarket found.");
            }
            catch (Exception)
            {
                Logging.Write("LlamaMarket could not be found/hooked. LlamaMarket functionality will be disabled.");
            }
        }

        public static async Task StopGently()
        {
            if (_stopGently == null) { return; }
            await _stopGently();
        }

        public static void OpenWindow()
        {
            _openWindow();
        }
    }
}
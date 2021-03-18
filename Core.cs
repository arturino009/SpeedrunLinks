using ExileCore;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.InventoryElements;
using System.Collections.Generic;

namespace SpeedrunLinks
{
    public class Core : BaseSettingsPlugin<Settings>
    {
        public override void Render()
        {
            base.Render();

            var purchaseWindow = GameController?.Game?.IngameState?.IngameUi?.PurchaseWindow;

            if (purchaseWindow == null || purchaseWindow.Address == 0x0 || !purchaseWindow.IsVisible)
                return;

            var allShopTabs = purchaseWindow.GetChildFromIndices(7,1).Children;

            List<NormalInventoryItem> items = null;

            foreach (var tab in allShopTabs)
            {
                if (!tab.IsVisible)
                {
                    continue;
                }
                items = tab.GetChildAtIndex(0).GetChildrenAs<NormalInventoryItem>();
                break;
            }

            
            //var items = GameController.Game.IngameState.IngameUi.StashElement.VisibleStash?.VisibleInventoryItems;
            if (items == null || items.Count == 0)
                return;
            
            foreach (var vendorItem in items)
            {
                if (IsLinkedItem(vendorItem))
                {
                    DrawBorder(vendorItem);
                }
            }
        }

        private void DrawBorder(NormalInventoryItem inventoryItem)
        {
            var rect = inventoryItem.GetClientRect();

            var borderColor = Settings.BorderColor.Value;

            var borderWidth = Settings.BorderWidth.Value;

            Graphics.DrawFrame(rect, borderColor, borderWidth);
        }

        private bool IsLinkedItem(NormalInventoryItem inventoryItem)
        {
            if (inventoryItem == null || inventoryItem.Address == 0x0)
                return false;

            var item = inventoryItem.Item;
            if (item == null || item.Address == 0x0) return false;

            if (!item.HasComponent<Sockets>()) return false;

            var links = item.GetComponent<Sockets>().LargestLinkSize;
            if (links == 0) return false;

            return links >= Settings.LinkCount;
        }
    }
}
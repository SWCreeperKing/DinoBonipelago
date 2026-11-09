using Archipelago.MultiClient.Net.Models;

namespace DinoBonipelago.Archipelago;

public static class ItemHandler
{
    public static HashSet<string> CratesUnlocked = [];

    public static void HandleItem(ItemInfo item)
    {
        if (item.ItemName is "Dust") return;
        CratesUnlocked.Add(item.ItemName);
    }
}
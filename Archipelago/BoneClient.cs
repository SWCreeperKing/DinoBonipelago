using Archipelago.MultiClient.Net.Enums;
using CreepyUtil.Archipelago;
using CreepyUtil.Archipelago.ApClient;
using IndieSouls.Save;
using JetBrains.Annotations;
using Newtonsoft.Json;

namespace DinoBonipelago.Archipelago;

public static class BoneClient
{
    public static Dictionary<string, string> CratePathToCrate = [];
    public static Dictionary<string, int> BoneCrates = [];
        
    public static ApClient Client = new(new TimeSpan(0, 1, 0));
    public static ApData Data = new();
    public static string GameUUID = "";

    public static void Init()
    {
        if (File.Exists("ApConnection.json"))
        {
            Data = JsonConvert.DeserializeObject<ApData>(File.ReadAllText("ApConnection.json").Replace("\r", ""));
        }

        Client.OnConnectionLost += () =>
        {
            Core.Log.Error("Lost Connection to Ap");
        };

        Client.OnConnectionEvent += _ =>
        {
            try
            {
                ItemHandler.CratesUnlocked.Clear();
                GameUUID = (string)Client.SlotData["uuid"];
                SavePaths.OverrideRootForTests($"{Directory.GetCurrentDirectory()}/Archipelago/{GameUUID}");
                CheckGoal();
            }
            catch (Exception e) { Core.Log.Error(e); }
        };

        Client.ItemHandlerInitialized += handler =>
        {
            handler.OnItemReceived += (item, _) => ItemHandler.HandleItem(item);
        };

        Client.OnConnectionErrorReceived += (e, _) => Core.Log.Error(e);
    }

    [CanBeNull]
    public static string[] TryConnect(string addressPort, string password, string slotName)
    {
        var addressSplit = addressPort.Split(':');

        if (addressSplit.Length != 2) return ["Address Field is incorrect"];
        if (!int.TryParse(addressSplit[1], out var port)) return ["Port is incorrect"];

        var login = new LoginInfo(port, slotName, addressSplit[0], password);

        return Client.TryConnect(login, "DinoBones Tidy Up the Museum", ItemsHandlingFlags.AllItems);
    }

    public static void SaveFile() => File.WriteAllText("ApConnection.json", JsonConvert.SerializeObject(Data));

    public static void Update()
    {
        try
        {
            if (Client is null) return;
            Client.UpdateConnection();

            if (!Client.IsConnected) return;
            Client.UpdateItemHandler();
        }
        catch (Exception e) { Core.Log.Error(e); }
    }

    public static void SendLocation(string loc)
    {
        if (!Client.IsMissingLocation(loc)) return;
        Client.SendLocation(loc);
    }

    public static void CheckGoal()
    {
        if (Client is null) return;
        if (!Client.IsConnected) return;
        if (Client.MissingLocations.Any()) return;
        Client.TryGoal();
    }
}
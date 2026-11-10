using DinoBonipelago.Added;
using DinoBonipelago.Archipelago;
using DinoBonipelago.CopyPastas;
using MelonLoader;

[assembly: MelonInfo(typeof(DinoBonipelago.Core), "DinoBonipelago", "0.1.0", "SW_CreeperKing", null)]
[assembly: MelonGame("Indie Souls Studio", "DinoBones")]

namespace DinoBonipelago;

public class Core : MelonMod
{
    public const string DataFolder = "Mods/SW_CreeperKing.DinoBonipelago/Data";

    public static MelonLogger.Instance Log;
    public static string Scene;
    public static APGui APGui = new();
    public static TrackingUI TrackingUI = new();

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        HarmonySetup.Init(MelonAssembly.Assembly, HarmonyInstance);

        BoneClient.CratePathToCrate = File.ReadAllLines($"{DataFolder}/bonepaths.txt").Select(s => s.Split('|'))
                                          .ToDictionary(arr => arr[1], arr => arr[0]);

        BoneClient.BoneCrates = File.ReadAllLines($"{DataFolder}/bonecrates.txt").Select(s => s.Split('|'))
                                    .ToDictionary(arr => arr[0], arr => int.Parse(arr[1]));

        BoneClient.Init();

        Log.Msg("Initialized.");
    }

    public override void OnUpdate()
    {
        KeyRegistry.Update();
        BoneClient.Update();
    }

    public override void OnGUI()
    {
        if (Scene is not "Museum") APGui.Render();
        if (Scene is "Museum") TrackingUI.Render();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Scene = sceneName;
        Log.Msg($"Scene Loaded [{Scene}]");
        BoneClient.CheckGoal();
    }
}
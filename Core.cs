using DinoBonipelago.Archipelago;
using DinoBonipelago.CopyPastas;
using MelonLoader;

[assembly: MelonInfo(typeof(DinoBonipelago.Core), "DinoBonipelago", "0.1.0", "SW_CreeperKing", null)]
[assembly: MelonGame("Indie Souls Studio", "DinoBones")]

namespace DinoBonipelago;

public class Core : MelonMod
{
    public static MelonLogger.Instance Log;
    public static Inspector Inspector = new(true);
    public const string DataFolder = "Mods/SW_CreeperKing.DinoBonipelago/Data";
    public static string Scene;
    public static APGui APGui = new();

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        HarmonySetup.Init(MelonAssembly.Assembly, HarmonyInstance);

        // KeyRegistry.TryAddKey(
        //     KeyCode.F7, Key.F7,
        //     () =>
        //     {
        //         File.WriteAllText(
        //             "Dinobones.txt", string.Join("\n", GrabData.Bones.OrderBy(s => s, StringComparer.Ordinal)).Replace('·', '-')
        //         );
        //         File.WriteAllText(
        //             "DinobonePaths.txt", string.Join("\n", GrabData.BonePaths.OrderBy(s => s, StringComparer.Ordinal)).Replace('·', '-')
        //         );
        //     }
        // );

        BoneClient.CratePathToCrate = File.ReadAllLines($"{DataFolder}/bonepaths.txt").Select(s => s.Split('|'))
                                          .ToDictionary(arr => arr[1], arr => arr[0]);

        BoneClient.Init();
        
        Log.Msg("Initialized.");
    }

    public override void OnUpdate()
    {
        // KeyRegistry.Update(); 
        BoneClient.Update();
    }

    public override void OnGUI()
    {
        // Inspector.Render(); 
        if (Scene is not "Museum") APGui.Render();
    }

    public override void OnSceneWasLoaded(int buildIndex, string sceneName)
    {
        Scene = sceneName;
        Log.Msg($"Scene Loaded [{Scene}]");
        BoneClient.CheckGoal();
    }
}
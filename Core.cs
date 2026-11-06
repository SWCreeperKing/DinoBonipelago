using DinoBonipelago.CopyPastas;
using DinoBonipelago.Patches.Debug;
using MelonLoader;
using UnityEngine;
using UnityEngine.InputSystem;

[assembly: MelonInfo(typeof(DinoBonipelago.Core), "DinoBonipelago", "0.1.0", "SW_CreeperKing", null)]
[assembly: MelonGame("Indie Souls Studio", "DinoBones")]

namespace DinoBonipelago;

public class Core : MelonMod
{
    public static MelonLogger.Instance Log;
    public static Inspector Inspector = new(true);

    public override void OnInitializeMelon()
    {
        Log = LoggerInstance;

        HarmonySetup.Init(MelonAssembly.Assembly, HarmonyInstance);

        KeyRegistry.TryAddKey(
            KeyCode.F7, Key.F7,
            () =>
            {
                File.WriteAllText(
                    "Dinobones.txt",
                    string.Join(
                        "\n\n",
                        GrabData.Bones.Select(kv
                            => $"{kv.Key}\n  - {string.Join("\n  - ", kv.Value.OrderBy(s => s, StringComparer.Ordinal))}"
                        )
                    )
                );
            }
        );

        Log.Msg("Initialized.");
    }

    public override void OnUpdate() { KeyRegistry.Update(); }

    public override void OnGUI() { Inspector.Render(); }
}
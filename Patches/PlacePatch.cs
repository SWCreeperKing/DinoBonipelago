using DinoBonipelago.Archipelago;
using DinoBonipelago.CopyPastas;
using DinoMuseum.Blueprint;
using DinoMuseum.Crates;
using HarmonyLib;

namespace DinoBonipelago.Patches;

[PatchAll]
public static class PlacePatch
{
    [HarmonyPatch(typeof(BonePlacer), "ArriveCore"), HarmonyPrefix]
    public static void Place(object f)
    {
        var bone = f.GetPublicField<BoneBody>("Bone");
        var label = bone.Label;
        BoneClient.SendLocation($"Place {label.Species} {label.Region}: {label.Bone.Replace('·', '-')}");
        BoneClient.CheckGoal();
    }
}

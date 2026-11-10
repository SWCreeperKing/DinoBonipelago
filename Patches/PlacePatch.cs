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
        var location = $"Place {label.Species} {label.Region}: {label.Bone.Replace('·', '-')}";
        if (BoneClient.BoneCrates.TryGetValue(location, out var crate)) location = $"{location} (Crate {crate})";
        BoneClient.SendLocation(location);
        BoneClient.CheckGoal();
    }
}

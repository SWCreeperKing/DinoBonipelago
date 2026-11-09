using DinoBonipelago.CopyPastas;
using DinoMuseum.Crates;
using HarmonyLib;

namespace DinoBonipelago.Patches.Debug;

// [PatchAll]
public static class GrabData
{
    public static HashSet<string> Bones = [];
    public static HashSet<string> BonePaths = [];
    
    [HarmonyPatch(typeof(BoneBody), "Awake"), HarmonyPrefix]
    public static void GrabBoneData(BoneBody __instance, string ___species, string ___labelSpecies, string ___labelRegion, string ___labelBone)
    {
        // Core.Log.Msg($"[{___species}] | [{___labelSpecies}] | [{___labelRegion}] | [{___labelBone}]");

        var label = __instance.Label;
        Bones.Add($"{label.Species}|{label.Region}|{label.Bone}");

        var rawPath = Inspector.GetGameObjectPath(__instance.gameObject).Split('/');
        if (rawPath[0] is "Paletler") return;
        var path = string.Join('/', rawPath[..^4]);
        BonePaths.Add($"{path}|{label.Species}|{label.Region}|{label.Bone}");
    }
}
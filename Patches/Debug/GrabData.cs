using DinoBonipelago.CopyPastas;
using DinoMuseum.Crates;
using HarmonyLib;

namespace DinoBonipelago.Patches.Debug;

[PatchAll]
public static class GrabData
{
    public static Dictionary<string, HashSet<string>> Bones = [];
    
    [HarmonyPatch(typeof(BoneBody), "Awake"), HarmonyPrefix]
    public static void GrabBoneData(string ___species, string ___labelSpecies, string ___labelRegion, string ___labelBone)
    {
        // Core.Log.Msg($"[{___species}] | [{___labelSpecies}] | [{___labelRegion}] | [{___labelBone}]");

        if (!Bones.ContainsKey(___labelSpecies)) Bones[___labelSpecies] = [];
        Bones[___labelSpecies].Add($"{___labelRegion} - {___labelBone}");
    }
}
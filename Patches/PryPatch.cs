using DinoBonipelago.Archipelago;
using DinoBonipelago.CopyPastas;
using DinoMuseum.Crates;
using HarmonyLib;
using UnityEngine;

namespace DinoBonipelago.Patches;

[PatchAll]
public static class PryPatch
{
    [HarmonyPatch(typeof(BoneGrabber), "UpdatePry"), HarmonyPrefix]
    public static bool StopUpdatePry(Camera cam, CrateBody crate, Vector3 hitPoint,
        bool keyDown)
    {
        if (crate is null) return true;
        var parent = crate.GetParent();
        var crateItem = BoneClient.CratePathToCrate[Inspector.GetGameObjectPath(parent.gameObject)];
        return ItemHandler.CratesUnlocked.Contains(crateItem);
    }
}
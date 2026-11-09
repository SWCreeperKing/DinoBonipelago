using DinoBonipelago.CopyPastas;
using HarmonyLib;
using IndieSouls.Save;

namespace DinoBonipelago.Patches;

[PatchAll]
public static class SavePatch
{
    [HarmonyPatch(typeof(MuseumSaveFiles), "IsThisMachine"), HarmonyPrefix]
    public static bool WhySoSerious(ref bool __result) // no need to know if this is the correct machine, kinda stupid
    {
        __result = true;
        return false;
    }
}
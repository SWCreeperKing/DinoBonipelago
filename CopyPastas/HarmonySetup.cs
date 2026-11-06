using System.Reflection;

namespace DinoBonipelago.CopyPastas;

public static class HarmonySetup
{
    public static void Init(Assembly assembly, HarmonyLib.Harmony harmonyInstance)
    {
        var classesToPatch = assembly.GetTypes()
                                     .Where(t => t.GetCustomAttributes(typeof(PatchAllAttribute), false).Any())
                                     .ToArray();

        Core.Log.Msg($"Loading [{classesToPatch.Length}] Class patches");

        foreach (var patch in classesToPatch) harmonyInstance.PatchAll(patch);
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class PatchAllAttribute : Attribute;
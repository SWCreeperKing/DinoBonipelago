using System.Reflection;
using UnityEngine;

namespace DinoBonipelago.CopyPastas;

public class DebugWindow(Assembly melonAssembly) : GuiBuilder
{
    public bool Show;
    public ScrollWindow ConfigBox = new(0, 20, 20, 350, 200, "Debug Toggles");

    protected override void InitGUI()
    {
        KeyRegistry.AddKey(KeyCode.F5, () => Show = !Show);
        
        try
        {
            var debugVars = melonAssembly
                           .GetTypes().SelectMany(t => t.GetFields())
                           .Where(t => t.GetCustomAttributes(typeof(DebugVariableMarkerAttribute), false).Any())
                           .Where(f => f.FieldType == typeof(bool))
                           .ToArray();

            foreach (var field in debugVars)
            {
                ConfigBox.AddChildren(
                    new Toggle(
                        new SetterGetter<bool>(
                            () => (bool)field.GetValue(null),
                            b => field.SetValue(null, b)
                        ), field.GetCustomAttribute<DebugVariableMarkerAttribute>().Text, 20
                    )
                );
            }
        }
        catch (Exception e) { Core.Log.Error(e); }
    }

    protected override void OnGUI() => ConfigBox.Render();
}

[AttributeUsage(AttributeTargets.Field)]
public class DebugVariableMarkerAttribute(string text) : Attribute
{
    public readonly string Text = text;
}
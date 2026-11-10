using DinoBonipelago.Archipelago;
using DinoBonipelago.CopyPastas;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DinoBonipelago.Added;

public class TrackingUI : GuiBuilder
{
    public Window Window;
    public int Page = 0;
    public Label[] Labels = new Label[5];

    protected override void InitGUI()
    {
        Window = new Window(0, 300, 155, 200, "Crates");
        KeyRegistry.TryAddKey(KeyCode.LeftArrow, Key.LeftArrow, NavPrev);
        KeyRegistry.TryAddKey(KeyCode.UpArrow, Key.UpArrow, NavPrev);
        KeyRegistry.TryAddKey(KeyCode.RightArrow, Key.RightArrow, NavNext);
        KeyRegistry.TryAddKey(KeyCode.DownArrow, Key.DownArrow, NavNext);

        for (var i = 0; i < Labels.Length; i++)
        {
            var i1 = i;
            Labels[i] = new Label(
                new TextRef(() =>
                    {
                        var crate = 5 * Page + i1 + 1;
                        return crate > 28 ? "" : $"Crate {crate}";
                    }
                ), 30
            );
            Window.AddChild(Labels[i]);
        }

        Window.AddChild(new Label("Use arrow keys for Nav", 25));
    }

    protected override void OnGUI()
    {
        if (BoneClient.Client is null) return;
        if (!BoneClient.Client.IsConnected) return;
        
        for (var i = 0; i < Labels.Length; i++)
        {
            var crate = 5 * Page + i + 1;
            Labels[i].SetColor(ItemHandler.CratesUnlocked.Contains($"Crate {crate}") ? Color.green : Color.red);
        }
        
        Window.Render();
    }

    public void NavPrev()
    {
        Page--;
        if (Page < 0) Page = 5;
    }

    public void NavNext()
    {
        Page++;
        Page %= 6;
    }
}
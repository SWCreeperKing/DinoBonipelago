using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace DinoBonipelago.CopyPastas;

public static class KeyRegistry
{
    private static bool IsActiveInput;
    private static Dictionary<KeyCode, bool> Previous = [];
    private static Dictionary<Key, bool> PreviousActive = [];
    public static event Action<KeyCode>? OnKeyPressed;
    public static event Action<Key>? OnKeyPressedActive;

    public static void Update()
    {
        if (!IsActiveInput)
        {
            if (!Previous.Any()) return;
            UpdateLegacy();
        }
        else
        {
            if (!PreviousActive.Any()) return;
            UpdateActive();
        }
    }

    public static void UpdateLegacy()
    {
        foreach (var key in Previous.Keys.ToArray())
        {
            var isKey = Input.GetKey(key);
            if (isKey == Previous[key]) continue;
            if (isKey)
            {
                try { OnKeyPressed?.Invoke(key); }
                catch (Exception e)
                {
                    Core.Log.Msg($"Error when running action on [{key}]");
                    Core.Log.Error(e);
                }
            }

            Previous[key] = isKey;
        }
    }

    public static void UpdateActive()
    {
        foreach (var key in PreviousActive.Keys.ToArray())
        {
            var isKey = Keyboard.current[key].isPressed;

            if (isKey == PreviousActive[key]) continue;
            if (isKey)
            {
                try { OnKeyPressedActive?.Invoke(key); }
                catch (Exception e)
                {
                    Core.Log.Msg($"Error when running action on [{key}]");
                    Core.Log.Error(e);
                }
            }

            PreviousActive[key] = isKey;
        }
    }

    public static void AddKey(KeyCode key, Action action)
    {
        IsActiveInput = false;
        try { Input.GetKey(key); }
        catch (InvalidOperationException)
        {
            IsActiveInput = true;
            Core.Log.Error("USING ACTIVE INPUT NOT LEGACY");
            return;
        }

        Previous[key] = false;
        OnKeyPressed += keycode =>
        {
            if (keycode != key) return;
            action();
        };

        Core.Log.Msg($"Added Debug keybind for: [{key}]");
    }

    public static void AddKeyActive(Key key, Action action)
    {
        IsActiveInput = true;
        PreviousActive[key] = false;
        OnKeyPressedActive += keycode =>
        {
            if (keycode != key) return;
            action();
        };

        Core.Log.Msg($"Added Debug keybind for: [{key}]");
    }

    public static void TryAddKey(KeyCode keyCode, Key key, Action action)
    {
        if (!IsActiveInput) AddKey(keyCode, action);
        if (IsActiveInput) AddKeyActive(key, action);
    }
}
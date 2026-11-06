using UnityEngine;
using UnityEngine.SceneManagement;
using Key = UnityEngine.InputSystem.Key;

namespace DinoBonipelago.CopyPastas;

public class Inspector(bool keyboardInput = false) : GuiBuilder
{
    public bool Show = false;
    public bool Cached = false;
    public GameObject[] RootObjects;
    public GameObject[] InspectObjects;
    public GameObject CurrentObj;
    public ScrollWindow SceneView;
    public ScrollWindow ObjectView;
    public int RootSelected = 0;
    public int InspectSelected = 0;

    protected override void InitGUI()
    {
        SceneManager.activeSceneChanged += (cur, next) => Cached = false;
        KeyRegistry.TryAddKey(KeyCode.Z, Key.Z, () => Show = !Show);
        KeyRegistry.TryAddKey(KeyCode.Delete, Key.Delete, () => Cached = false);
        KeyRegistry.TryAddKey(KeyCode.F5, Key.F5, ToggleMouse);

        if (!keyboardInput) return;
        KeyRegistry.TryAddKey(KeyCode.LeftArrow, Key.LeftArrow, () => Input(0)); // open/close obj inspect
        KeyRegistry.TryAddKey(KeyCode.UpArrow, Key.UpArrow, () => Input(1)); // nav 1
        KeyRegistry.TryAddKey(KeyCode.RightArrow, Key.RightArrow, () => Input(2)); // child inspect
        KeyRegistry.TryAddKey(KeyCode.DownArrow, Key.DownArrow, () => Input(3)); // nav 2 
        KeyRegistry.TryAddKey(KeyCode.C, Key.C, () => Input(4)); // copy
        KeyRegistry.TryAddKey(KeyCode.T, Key.T, () => Input(5)); // toggle visible
        KeyRegistry.TryAddKey(KeyCode.Backspace, Key.Backspace, () => Input(6)); // parent inspect
    }

    protected override void OnGUI()
    {
        if (!Show) return;
        if (!Cached)
        {
            RootObjects = SceneManager.GetActiveScene().GetRootGameObjects();

            SceneView = new ScrollWindow(0, 50, 50, 240, 400, "Scene View", dragable: true);
            for (var i = 0; i < RootObjects.Length; i++)
            {
                var i1 = i;

                if (!keyboardInput)
                    SceneView.AddChild(
                        new UiButton(new TextRef(() => RootObjects[i1].name), 30, () => InspectObject(i1))
                    );
                else
                    SceneView.AddChild(
                        new Label(
                            new TextRef(() => RootSelected % RootObjects.Length == i1 ? $"> {RootObjects[i1].name} <"
                                : $"  {RootObjects[i1].name}  "
                            ), 30
                        )
                    );
            }

            Cached = true;
            ObjectView = null;
            CurrentObj = null;
        }

        SceneView?.Render();
        try { ObjectView?.Render(); }
        catch (NullReferenceException)
        {
            ObjectView = null;
            CurrentObj = null;
        }
    }

    public void InspectObject(int obj)
    {
        if (RootObjects is null || RootObjects.Length <= obj || RootObjects[obj] is null)
        {
            Cached = false;
            return;
        }
        InspectObject(RootObjects[obj]);
    }

    public void InspectObject(GameObject obj)
    {
        if (obj is null) return;
        InspectSelected = 0;
        CurrentObj = obj;

        var parent = obj.transform.parent is not null ? obj.GetParent() : null;
        ObjectView = new ScrollWindow(1, 500, 50, 400, 400, $"View [{obj.name}]", dragable: true);

        if (!keyboardInput)
        {
            if (parent is not null) ObjectView.AddChild(new UiButton("View Parent", 30, () => InspectObject(parent)));
            var scrollH = parent is null ? 334 : 301;

            ObjectView.AddChildren(
                new UiButton("Copy Path", 30, () => GUIUtility.systemCopyBuffer = GetGameObjectPath(obj)),
                new Toggle(new SetterGetter<bool>(() => obj.activeSelf, obj.SetActive), "Self Active", 30),
                new HBox()
                   .AddChildren(
                        new ScrollBox(scrollH)
                           .AddChildren(
                                [
                                    .. obj.GetChildren().Select(o => new UiButton(
                                        $"{o.name}", 30, () => InspectObject(o)
                                    )
                                    )
                                ]
                            ),
                        new ScrollBox(scrollH)
                           .AddChildren(
                                [.. obj.GetComponents<MonoBehaviour>().Select(c => new Label(c.GetType().Name, 30))]
                            )
                    )
            );
        }
        else
        {
            InspectObjects = obj.GetChildren();
            var objScroll = new ScrollBox(334);
            for (var i = 0; i < InspectObjects.Length; i++)
            {
                var i1 = i;

                objScroll.AddChild(
                    new Label(
                        new TextRef(() => InspectSelected % InspectObjects.Length == i1 ? $"> {InspectObjects[i1].name} <"
                            : $"  {InspectObjects[i1].name}  "
                        ), 30
                    )
                );
            }

            ObjectView.AddChildren(
                new Toggle(new SetterGetter<bool>(() => obj.activeSelf, obj.SetActive), "Self Active", 30),
                new HBox()
                   .AddChildren(
                        objScroll,
                        new ScrollBox(334)
                           .AddChildren(
                                [.. obj.GetComponents<MonoBehaviour>().Select(c => new Label(c.GetType().Name, 30))]
                            )
                    )
            );
        }
    }

    public void Input(int dir)
    {
        if (!Show) return;

        switch (dir)
        {
            case 0 when ObjectView is not null:
                ObjectView = null;
                CurrentObj = null;
                InspectSelected = 0;
                break;
            case 0 when ObjectView is null: InspectObject(RootObjects[RootSelected]); break;
            case 1 when ObjectView is null: RootSelected--; break;
            case 1 when ObjectView is not null: InspectSelected--; break;
            case 2 when ObjectView is not null && InspectObjects.Length > 0:
                InspectObject(InspectObjects[InspectSelected]); break;
            case 3 when ObjectView is null: RootSelected++; break;
            case 3 when ObjectView is not null: InspectSelected++; break;
            case 4 when ObjectView is not null: GUIUtility.systemCopyBuffer = GetGameObjectPath(CurrentObj); break;
            case 5 when ObjectView is not null: CurrentObj.SetActive(!CurrentObj.activeSelf); break;
            case 6 when ObjectView is not null && CurrentObj.transform.parent is not null:
                InspectObject(CurrentObj.GetParent()); break;
        }
        
        var max = ObjectView is null ? RootObjects.Length : InspectObjects.Length;
        if (RootSelected == -1) RootSelected = max - 1;
        if (RootObjects.Length is 0) RootSelected = 0;
        else RootSelected %= RootObjects.Length;
        
        if (ObjectView is null) return;
        if (InspectSelected == -1) InspectSelected = max - 1;
        if (InspectObjects.Length is 0) InspectSelected = 0;
        else InspectSelected %= InspectObjects.Length;
    }

    public static string GetGameObjectPath(GameObject obj)
    {
        var transform = obj.transform;
        var path = transform.name;
        while ((transform = transform.parent) is not null) path = $"{transform.name}/{path}";
        return path;
    }

    public static void ToggleMouse()
        => Cursor.lockState = Cursor.lockState is not CursorLockMode.None ? CursorLockMode.None : CursorLockMode.Locked;
}
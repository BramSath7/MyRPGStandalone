using System.Collections.Generic;
using UnityEngine;

public class Game_UI : MonoBehaviour {
    public static Game_UI instance;
    public Transform UIRoot;
    public Transform PopupRoot;
    public Transform NotificationRoot;
    public Transform LoadingRoot;
    [Header("UI")]
    public List<UI_Container> UIs = new();
    public readonly Dictionary<string, GameObject> cache = new();
    private readonly Stack<string> History = new();
    private string currentUI;
    [Header("POPUP")]
    public List<UI_Container> PopUps = new();
    public readonly Dictionary<string, GameObject> cachePopup = new();
    public readonly Stack<GameObject> popupStack = new();
    private void Awake() {
        instance = this;
        UIRoot = transform.Find("RootUI");
        PopupRoot = transform.Find("RootPop");
        NotificationRoot = transform.Find("RootNotif");
        LoadingRoot = transform.Find("RootLoad");
        foreach (var ui in UIs) {
            if (ui.Prefab == null) continue;
            var go = Instantiate(ui.Prefab, UIRoot);
            go.SetActive(false);
            cache[ui.ID] = go;
        }
        foreach (var pop in PopUps) {
            if (pop.Prefab == null) continue;
            var go = Instantiate(pop.Prefab, PopupRoot);
            go.SetActive(false);
            cachePopup[pop.ID] = go;
        }
        GameNotify.OnOpenUI += Open;
        GameNotify.OpenUI("Start");
    }
    private void OnDestroy() {
        GameNotify.OnOpenUI -= Open;
    }
    public void Open(string id) {
        if (!string.IsNullOrEmpty(currentUI)) {
            History.Push(currentUI);
        }
        Show(id);
    }
    public void Back() {
        if (History.Count == 0) return;
        string previous = History.Pop();
        Show(previous, false);
    }
    public void Show(string id, bool saveCurrent = true) {
        HideAll();
        if (cache.TryGetValue(id, out var panel)) {
            panel.SetActive(true);
            currentUI = id;
        }
    }
    public void HideAll() {
        foreach (var p in cache.Values) {
            if (p.activeSelf) {
                p.SetActive(false);
            }
        }
    }
    //
    public GameObject OpenPopup(string id) {
        if (!cachePopup.TryGetValue(id, out var popup))
            return null;
        popup.SetActive(true);
        popup.transform.SetAsLastSibling();
        popupStack.Push(popup);
        return popup;
    }
    public void ClosePopup() {
        if (popupStack.Count == 0) return;
        var popup = popupStack.Pop();
        popup.SetActive(false);
    }
    public void ClosePopup(string id) {
        if (!cachePopup.TryGetValue(id, out var popup))
            return;
        popup.SetActive(false);
    }
    public void CloseAllPOPUP() {
        while (popupStack.Count > 0) {
            var popup = popupStack.Pop();
            popup.SetActive(false);
        }
    }
}

[System.Serializable]
public class UI_Container {
    public string ID;
    public GameObject Prefab;
}

public class UI_Pop {
    public string Name;
    public string Message;
    public string[] Label;
    public event System.Action<int> OnPop;
    public void Trigger(int index) 
        => OnPop?.Invoke(index);
}
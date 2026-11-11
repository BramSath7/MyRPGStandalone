using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
    public static GameManager instance;
    public Object_Character LoadedObject;
    public Unit LoadedRuntime;
    public ItemCollection Coin;
    public ItemCollection Bread;
    //
    public GameObject Prefab;
    public Transform SpawnPos;
    //
    private void Awake() {
        instance = this;
    }
    public void StartGame(Unit data) {
        Coin = new ItemCollection {
            Name = "Coin",
            Icon = Resources.Load<Sprite>("Icons/Currency_Coin"),
            Current = 500,
            Limit = 5000000
        };
        Bread = new ItemCollection {
            Name = "Bread",
            Icon = Resources.Load<Sprite>("Icons/Consumable_Bread"),
            Current = 5, 
            Limit = 100
        };
        //
        LoadedRuntime = data;
        LoadedRuntime.IsPlayer = true;
        GameObject obj = Instantiate(Prefab, SpawnPos.position, Quaternion.identity);
        LoadedObject = obj.GetComponent<Object_Character>();
        LoadedObject.Bind(LoadedRuntime);
        GameNotify.LoadGame();
        //
        GameNotify.OpenUI("Combat");
        Combat_UI.instance?.Bind(LoadedRuntime);
        Game_Camera.Instance?.SetTarget(LoadedObject.transform);
        Game_Inventory.instance?.Bind(new List<ItemCollection> { Coin, Bread });
        //
        LoadedRuntime.OnCombatStart();
        LoadedObject.OnCombatStart();
    }
}
[System.Serializable]
public class ItemCollection {
    public string Name;
    public Sprite Icon;

    public int Current;
    public int Limit = 999;
    public event System.Action<int> OnChanged;
    public void Gain(int amount) {
        Current = Mathf.Min(Current + amount, Limit);
        OnChanged?.Invoke(Current);
    }
    public bool Spend(int amount) {
        if (Current < amount) return false;
        Current -= amount;
        OnChanged?.Invoke(Current);
        return true;
    }
}
public enum AbilityType
{
    None, Basic, Skill, Spell, Ultimate
}
public static class GameNotify {
    public static event System.Action<Vector2> OnMove;
    public static void UpdateMove(Vector2 dir) => OnMove?.Invoke(dir);
    //
    public static event System.Action OnGameLoaded;
    public static void LoadGame() => OnGameLoaded?.Invoke();
    //
    public static event System.Action<string> OnOpenUI;
    public static void OpenUI(string id) => OnOpenUI?.Invoke(id);
}
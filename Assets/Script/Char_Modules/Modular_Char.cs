using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
[CreateAssetMenu(menuName = "Data/Char/Modular")]
public class Char_Data: ScriptableObject {
    public string ID;
    public string Name;
    public Sprite Portrait;
    public GameObject Prefab;
    public List<AssetReferenceT<Ability_Data>> Abilities;
    public BaseStats Base;
}
[System.Serializable]
public class BaseStats {
    public float Health, Energy;
    public float Attack, Power, Intensity;
}
[System.Serializable]
public class Char_Runtime {
    public Char_Data Data;
    //
    public bool IsPlayer;
    public bool IsInit;
    //
    public Char_Stats Stats;
    public Char_Status Status = new();
    public Char_Buff Buffs;
    public Char_Ability Ability = new();
    //
    public Module_Char_Obj LoadedObj;
    public Vector2 CurrentPos => LoadedObj ? LoadedObj.transform.position : Vector2.zero;
    public Vector2 CurrentDir => LoadedObj ? LoadedObj.CurrentDir : Vector2.zero;
    //
    public void Initialize(Char_Data newData) {
        Data = newData;
        //
        Buffs = new Char_Buff(this);
        Stats = new Char_Stats(this);

        //
        IsInit = true;
    }
    // koneksi object 
    public void OnCombatStart()
    {

    }
    public void OnCombatEnd() { }
    //
    public void OnAI() { }
    public void SetDir(Vector2 dir) => LoadedObj?.SetDirection(dir);
    public void Update(float dt)
    {
        if (!IsInit) return;
    }
}
// Sub perlu tambah owner ? 
[System.Serializable]
public class Char_Stats
{
    public GamePoint Health;
    public GamePoint Energy;
    public GamePoint Attack;
    public Dictionary<string, GamePoint> Stats = new();
    [System.NonSerialized] public Char_Runtime Owner;
    public Char_Stats(Char_Runtime newOwner) {  Owner = newOwner; }
    public void Initialize() {

    }
}
[System.Serializable]
public class Char_Status
{

}
[System.Serializable]
public class Char_Buff {
    public List<Buff_Instance> Active = new();
    [System.NonSerialized] private Char_Runtime Owner;
    public Char_Buff(Char_Runtime newOwner) => Owner = newOwner;
    public void Add(Buff_Data data) {

    }
}
[System.Serializable]
public class Char_Ability {
    public List<AbilityRuntime> Abilities = new();
    public AbilityRuntime Get(int index)
    {
        if (index < 0 || index >= Abilities.Count) return null;
        return Abilities[index];
    }
}

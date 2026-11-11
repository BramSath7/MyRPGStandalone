using System.Collections.Generic;
using UnityEngine;

public class GameData {
    public Unit Unit;
    public List<ItemCollection> Items = new();

    public void StartNew(Unit selectedUnit, string id) {
        Unit = selectedUnit;



    }
    //
}
public class GameRuntime {
    public GameData Data = new();

    public void Load() {

    }
    public void Save() {

    }
    public void Reset()
    {

    }

}
//

public class Data_Character {
    public string ID;
    public string Name;
}
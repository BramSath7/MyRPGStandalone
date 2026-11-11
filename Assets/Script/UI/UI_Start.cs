using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Start : MonoBehaviour {
    private List<Unit> UnitReady = new();
    private Unit SelectedUnit;
    private UI_Start_Slot selectedSlot;
    //
    public GameObject unitPrefabUI;
    public Transform unitParent;
    //
    public Button btnStart;
    public Button btnLoad;
    public Button btnSetting;
    public Button btnExit;
    private void Awake() {
        UnitReady.Add(new Adventurer());
        //
        var pnl = transform.Find("Pnl");
        btnStart = pnl.Find("BtnStart").GetComponent<Button>();
        btnLoad = pnl.Find("BtnLoad").GetComponent<Button>();
        btnSetting = pnl.Find("BtnSetting").GetComponent<Button>();
        btnExit = pnl.Find("BtnExit").GetComponent<Button>();
        //
        unitParent = transform.Find("PnlUnit");
        unitPrefabUI = Resources.Load<GameObject>("Prefab/Btn_Slot_Start");
        //
        OnGameLoaded();
    }
    private void OnGameLoaded() {
        foreach (var u in UnitReady) {
            var ui = Instantiate(unitPrefabUI, unitParent).GetComponent<UI_Start_Slot>();
            ui.Setup(u, SelectUnit);
        }
        //
        btnStart.onClick.RemoveAllListeners();
        btnLoad.onClick.RemoveAllListeners();
        btnSetting.onClick.RemoveAllListeners();
        btnExit.onClick.RemoveAllListeners();
        //
        btnStart.onClick.AddListener(StartGame);
    }
    private void SelectUnit(UI_Start_Slot slot) {
        if (selectedSlot != null)
            selectedSlot.SetSelected(false);
        selectedSlot = slot;
        SelectedUnit = slot.Data;
        selectedSlot.SetSelected(true);
    }
    public void StartGame() {
        if (selectedSlot == null) return;
        GameManager.instance?.StartGame(SelectedUnit);
    }
}

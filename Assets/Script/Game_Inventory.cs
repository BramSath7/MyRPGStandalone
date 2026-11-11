using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Inventory : MonoBehaviour {
    public static Game_Inventory instance;
    public List<Btn_Slot_Inventory> Slots = new();
    public int Slot = 25;
    public GameObject prefab;
    public Transform parent;
    public Button exit;
    private void Awake() {
        instance = this;
        parent = transform.Find("ViewPort/Content");
        exit = transform.Find("BtnExit").GetComponent<Button>();
        exit.onClick.RemoveAllListeners();
        exit.onClick.AddListener(() => { Game_UI.instance?.Back(); });
        prefab = Resources.Load<GameObject>("Prefab/Btn_Slot_Inventory");
    }
    public void Bind(List<ItemCollection> list) {
        //
        while (Slots.Count < list.Count) {
            var go = Instantiate(prefab, parent);
            Slots.Add(go.GetComponent<Btn_Slot_Inventory>());
        }
        //
        for (int i = 0;i < Slots.Count;i++) {
            if (i < list.Count) {
                Slots[i].gameObject.SetActive(true);
                var item = list[i];
                Slots[i].Setup(item);
            }
            else {
                Slots[i].gameObject.SetActive(false);
            }
        }
    }

}

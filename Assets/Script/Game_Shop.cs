using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game_Shop: MonoBehaviour {
    public static Game_Shop instance;
    [Header("Data")]
    public List<Shop_Item> items = new();
    [Header("UI")]
    public GameObject Prefab;
    public Transform Content;
    public readonly List<BtnShop_Slot> slots = new();
    private void Awake() {
        instance = this;
        Content = transform.Find("PnlList").Find("ItemList").Find("Content");
        gameObject.SetActive(false);
        Test();
        Generate();
    }
    private void Test() {
        items = new List<Shop_Item> {
            new Shop_Bread()
        };
        foreach (var item in items) {
            item.Restock();
        }
    }
    public void Generate() {
        foreach (Transform child in Content)
            Destroy(child.gameObject);
        slots.Clear();
        foreach (var item in items) {
            var go = Instantiate(Prefab, Content);
            var slot = go.GetComponent<BtnShop_Slot>();
            slot.Setup(item);
            slots.Add(slot);
        }
    }
    public void Open() {
        gameObject.SetActive(true);
    }
    public void Hide() {
        Game_UI.instance?.Back();
    }
}



[System.Serializable]
public class Shop_Item {
    public string ID;
    public string Name;
    [TextArea] public string Desc;
    public Sprite Icon;
    // Jumlah yang dimiliki shop
    public int AvailableStock;  // Jumlah umum
    public int Stack;
    // Jumlah yang akan dibeli
    [Header("Runtime")]
    public int CurrentStock { get; private set; }
    public int SelectedValue { get; private set; }
    // yang dibutuhkan
    [SerializeField]
    public string ReqItem;
    public int ReqValue;
    //
    public int RemainingStock => CurrentStock - SelectedValue;
    public bool CanBuy => 
        SelectedValue > 0 && 
        CurrentStock >= SelectedValue && 
        GameManager.instance.Coin.Current >= TotalPrice;
    public int TotalPrice => SelectedValue * ReqValue;
    //
    public event System.Action OnChanged;
    public void Buy() {
        if (!CanBuy) return;
        GameManager.instance.Coin.Spend(TotalPrice);
        OnBuy();
        CurrentStock -= SelectedValue;
        SelectedValue = 0;
        OnChanged?.Invoke();
    }
    public virtual void OnBuy() { }
    public void IncreaseSelected() {
        SelectedValue = Mathf.Min(CurrentStock, SelectedValue + 1);
        OnChanged?.Invoke();
    }
    public void DecreaseSelected() {
        SelectedValue = Mathf.Max(0, SelectedValue - 1);
        OnChanged?.Invoke();
    }
    public void Restock() {
        CurrentStock = AvailableStock;
        OnChanged?.Invoke();
    }
}
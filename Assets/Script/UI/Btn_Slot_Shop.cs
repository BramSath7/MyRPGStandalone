using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BtnShop_Slot : MonoBehaviour {
    public Shop_Item Data;
    //
    public Image Icon;
    //
    public Button Add;
    public Button Dec;
    public Button Buy;
    //
    public TextMeshProUGUI Title;
    public TextMeshProUGUI Description;
    public TextMeshProUGUI Value;
    private void Awake() {
        Icon = transform.Find("Img").Find("Icon").GetComponent<Image>();
        Dec = transform.Find("BtnDecrease").GetComponent <Button>();
        Add = transform.Find("BtnIncrease").GetComponent<Button>();
        Buy = transform.Find("BtnBuy").GetComponent<Button>();
        //
        Title = transform.Find("TxtTitle").GetComponent<TextMeshProUGUI>();
        Description = transform.Find("TxtDesc").GetComponent<TextMeshProUGUI>();
        Value = transform.Find("BtnBuy").Find("TxtValue").GetComponent<TextMeshProUGUI>();
    }
    public void Setup(Shop_Item newData) {
        Add.onClick.RemoveAllListeners();
        Dec.onClick.RemoveAllListeners();
        Buy.onClick.RemoveAllListeners();
        if (Data != null) Data.OnChanged -= UpdateData;
        Data = newData;
        if (Data != null) Data.OnChanged += UpdateData;
        Icon.sprite = Data.Icon;
        Title.text = Data.Name;
        Description.text = Data.Desc;
        Add.onClick.AddListener(Data.IncreaseSelected);
        Dec.onClick.AddListener(Data.DecreaseSelected);
        Buy.onClick.AddListener(Data.Buy);
        UpdateData();
        Data.OnChanged += UpdateData;
    }
    private void OnDestroy() {
        if (Data != null) 
        Data.OnChanged -= UpdateData;
    }
    public void UpdateData() {
        if (Data == null) return;
        Value.text = 
            $"{Data.CurrentStock}/{Data.AvailableStock}" +
            $"(-{Data.SelectedValue} || ${Data.TotalPrice})";
        Add.interactable = Data.SelectedValue < Data.CurrentStock;
        Dec.interactable = Data.SelectedValue > 0;
        Buy.interactable = Data.CanBuy;
    }
}

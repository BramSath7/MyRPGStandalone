using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Btn_Slot_Inventory : MonoBehaviour {
    public ItemCollection Data;
    public Image Icon;
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtValue;
    private void Awake() {
        Icon = transform.Find("Icon").GetComponent<Image>();
        txtName = transform.Find("TxtName").GetComponent<TextMeshProUGUI>();
        txtValue = transform.Find("TxtValue").GetComponent<TextMeshProUGUI>();   
    }
    public void Setup(ItemCollection item) {
        if (Data != null)
            Data.OnChanged -= UpdateData;
        Data = item;
        txtName.text = item.Name;
        Icon.sprite = item.Icon;
        txtValue.text = Data.Current.ToString();
        Data.OnChanged += UpdateData;
    }
    private void OnDestroy() {
        if (Data != null)
            Data.OnChanged -= UpdateData;
    }
    private void UpdateData(int value) {
        txtValue.text = Data.Current.ToString();
    }
}

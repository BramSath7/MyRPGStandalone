using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Start_Slot : MonoBehaviour
{
    public Unit Data;
    public Button btnSelect;
    public Button btnInfo;
    public TextMeshProUGUI txtName;
    private void Awake() {
        btnSelect = GetComponent<Button>();
        btnInfo = transform.Find("BtnInfo").GetComponent<Button>();
        txtName = transform.Find("TxtName").GetComponent<TextMeshProUGUI>();
    }
    public void Setup(Unit data, System.Action<UI_Start_Slot> onclick) {
        Data = data;    
        txtName.text = data.Name;
        btnSelect.onClick.RemoveAllListeners();
        btnSelect.onClick.AddListener(() => onclick?.Invoke(this));
        SetSelected(false);
        btnInfo.onClick.RemoveAllListeners();
    }
    public void SetSelected(bool check) {
        // highlight
    }
}

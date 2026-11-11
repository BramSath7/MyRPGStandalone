using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Popup : MonoBehaviour {
    public TextMeshProUGUI title;
    public TextMeshProUGUI Message;
    public Button[] buttons;
    private UI_Pop data;
    public void Initialize(UI_Pop pop) {
        data = pop;
        title.text = pop.Name;
        for (int i = 0; i < buttons.Length; i++) {
            int index = i;
            buttons[i].GetComponentInChildren<TextMeshProUGUI>().text = pop.Label[i];
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].onClick.AddListener(() => {
                pop.Trigger(index);
                Game_UI.instance.ClosePopup();
            });
        }
    }
}

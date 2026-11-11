using System.Collections.Generic;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Game_Dialogue : MonoBehaviour {
    public static Game_Dialogue instance;
    [Header("UI")]
    public Transform pnlDialog;
    public TextMeshProUGUI Speaker;
    public TextMeshProUGUI Detail;
    public Button btnNext;
    public Button btnAuto;
    [Header("Choice")]
    public Transform choiceContainer;
    public Button choicePrefab;
    [Header("Character")]
    public List<Dialogue_Obj> Characters;
    [Header("Config")]
    public bool IsAuto;
    public float SpeedAuto = 2f;
    public float SpeedAutoMin = 1f;
    public float SpeedAutoPerChar = 0.03f;
    public float SpeedType = 0.02f;
    //
    public Dialogue_Run Runtime = new();
    private Coroutine typingCoroutine;
    private Coroutine autoCoroutine;
    //
    private bool IsPaused;
    private bool IsSkip;
    private bool IsHideUI;
    private bool IsTyping => typingCoroutine != null;
    private bool IsWaitingDelay;
    private void Awake() {
        instance = this;
        var panel = transform.Find("Panel");
        Speaker = panel.Find("PnlTxt").Find("TxtSpeaker").GetComponent<TextMeshProUGUI>();
        Detail = panel.Find("PnlTxt").Find("TxtDetail").GetComponent<TextMeshProUGUI>();
        choiceContainer = panel.Find("PnlChoice");
        //
        btnNext = GetComponent<Button>();
        btnNext.onClick.RemoveAllListeners();
        btnNext.onClick.AddListener(OnClickDialogue);
        // 
        btnAuto = transform.Find("BtnAuto").GetComponent<Button>();
        btnAuto.onClick.RemoveAllListeners();
        btnAuto.onClick.AddListener(() => { IsAuto = true; });
        // 
    }
    private void OnEnable() {
        var newData = new Dialogue_Data {
            Background = Resources.Load<Sprite>("Sprite/BG_Dialogue_001"),
            Nodes = new List<Dialogue_Node> {
                    new Dialogue_Node {
                        IDNode = "Test_001",
                        Speaker = "Narator",
                        Text = "Test dialog",
                        Camera = "StandardZoom",
                        Delay = 1f,
                        Chars = null,
                    },
                    new Dialogue_Node {
                        IDNode = "Test_002",
                        Speaker = "Narator",
                        Text = "Test Char",
                        Delay = 1.5f,
                        Choices = new List<Dialogue_Choice> {

                        }
                    },
                    new Dialogue_Node {
                        IDNode = "Test_003",
                        Speaker = "Luna",
                        Text = "Hari ini terasa sangat sepi",
                        Delay = 1f,
                        Chars = new() {
                            new Dialogue_CharState {
                                ID = "Luna",
                                Visible = true,
                                Focus = true,
                                Expression = DialogueExpression.Idle,
                                 Anim = "EnterLeft"
                            }
                        }
                    }
                }
        };
        StartDialogue(newData);
    }
    public Dialogue_Obj GetChar(string id) 
        => Characters.Find(x => x.ID == id);
    //
    public void StartDialogue(Dialogue_Data data) {
        pnlDialog.gameObject.SetActive(true);
        choiceContainer.gameObject.SetActive(false);
        Runtime = new Dialogue_Run {
            Data = data,
            CurrentNode = 0,
            IsRunning = true
        };
        Runtime.History.Clear();
        OpenNode(Runtime.SelectedNode);
    }
    public void OpenNode(Dialogue_Node node) {
        if (node == null) return;
        if (autoCoroutine != null) {
            StopCoroutine(autoCoroutine);
            autoCoroutine = null;
        }
        choiceContainer.gameObject.SetActive(false);
        Speaker.text = node.Speaker;
        ApplyChar(node);
        StartTyping(node.Text);
        Runtime.History.Add(node.Text);
    }
    public void NextNode() {
        var current = Runtime.SelectedNode;
        if (current == null) return;
        //
        if (current.Choices != null) {
            if (current.Choices.Count > 1) {
                OpenChoices(current);
                return;
            }
            if (current.Choices.Count == 1) {
                JumpToNode(current.Choices[0].NextNode);
                return;
            }
        }
        Runtime.CurrentNode++;
        if (Runtime.CurrentNode >= Runtime.Data.Nodes.Count) {
            EndDialogue();
            return;
        }
        OpenNode(Runtime.SelectedNode);
    }
    public void JumpToNode(string id) {
        int index = Runtime.Data.Nodes.FindIndex(x => x.IDNode == id);
        if (index < 0) {
            Debug.LogWarning($"Node [{id}] not found.");
            return;
        }
        Runtime.CurrentNode = index;
        OpenNode(Runtime.SelectedNode);
    }
    //
    private void ApplyChar(Dialogue_Node node) {
        if (node.Chars == null) {
            foreach (var character in Characters)
                character.SetVisible(false);
            return;
        }
        foreach (var state in node.Chars) {
            Runtime.Characters[state.ID] = state;
            var view = GetChar(state.ID);
            if (view == null) continue;
            //
            view.SetExpression(state.Expression);
            view.SetVisible(state.Visible);
            view.SetFocus(state.Focus);
            if (!string.IsNullOrEmpty(state.Anim)) {
                view.PlayAnim(state.Anim);
            }
        }
    }
    private void StartTyping(string text) {
        if (typingCoroutine != null) {
            StopCoroutine(typingCoroutine);
        }
        IsSkip = false;
        typingCoroutine = StartCoroutine(TypeText(text));
    }
    private IEnumerator TypeText(string fullText) {
        IsSkip = false;
        Detail.text = "";
        WaitForSeconds wait = new WaitForSeconds(SpeedType);
        foreach (var c in fullText) {
            while (IsPaused) yield return null;
            if (IsSkip) {
                Detail.text = fullText;
                break;
            }
            Detail.text += c;
            yield return wait;
        }
        Detail.text = fullText;
        typingCoroutine = null;
        var node = Runtime.SelectedNode;
        if (node != null && node.Delay > 0) {
            IsWaitingDelay = true;
            yield return new WaitForSeconds(node.Delay);
            IsWaitingDelay = false;
        }
        ShowChoiceOrContinue();
        if (IsAuto) {
            autoCoroutine = StartCoroutine(AutoContinue());
        }
    }
    private IEnumerator AutoContinue() {
        string text = Runtime.SelectedNode?.Text ?? "";
        float delay = SpeedAutoMin + (text.Length * SpeedAutoPerChar);
        yield return new WaitForSeconds(delay);
        if (!IsTyping && Runtime.IsRunning) {
            NextNode();
        }
        autoCoroutine = null;
    }
    //
    public void OpenChoices(Dialogue_Node node) {
        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);
        choiceContainer.gameObject.SetActive(true);
        foreach (var choice in node.Choices) {
            Dialogue_Choice cache = choice;
            Button btn = Instantiate(choicePrefab, choiceContainer);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = cache.text;
            btn.onClick.AddListener(() => {
                SelectChoice(cache);
            });
        }
    }
    private void SelectChoice(Dialogue_Choice choice) {
        choiceContainer.gameObject.SetActive(false);
        JumpToNode(choice.NextNode);
    }
    private void ShowChoiceOrContinue() {
        var node = Runtime.SelectedNode;
        //
        if (node?.Choices == null) return;
        //
        if (node.Choices.Count > 0) {
            OpenChoices(node);
        }
    }
    //
    public void EndDialogue() {
        Runtime.IsRunning = false;
        pnlDialog.gameObject.SetActive(false);
        choiceContainer.gameObject.SetActive(false);
    }
    //
    public void HideUI() {
        IsHideUI = !IsHideUI;
        pnlDialog.gameObject.SetActive(!IsHideUI);
    }
    private void OnClickDialogue() {
        if (IsHideUI) return;
        if (IsWaitingDelay) return;
        //
        if (IsTyping) {
            IsSkip = true;
            return;
        }
        NextNode();
    }
}
public class Dialogue_Run {
    public Dialogue_Data Data;
    public int CurrentNode;
    public Dialogue_Node SelectedNode {
        get {
            if (Data == null) 
                return null;
            if (Data.Nodes == null) 
                return null;
            if (CurrentNode < 0 || CurrentNode >= Data.Nodes.Count)
                return null;
            return Data.Nodes[CurrentNode];
        }
    }
    public bool IsRunning;
    public readonly List<string> History = new();
    public readonly Dictionary<string, Dialogue_CharState>
        Characters = new();
}
//
[System.Serializable]
public class Dialogue_Data {
    public Sprite Background;
    public List<Dialogue_Node> Nodes = new();
}
[System.Serializable]
public class Dialogue_Node {
    public string IDNode;
    public string Speaker;
    [TextArea] public string Text;
    public string SFX;
    public string Camera;
    public float Delay;
    //
    public List<Dialogue_CharState> Chars; // if null => hide all char
    public List<Dialogue_Choice> Choices; // if choice.count > 2 = show choice 
}
[System.Serializable]
public class Dialogue_Choice {
    public string NextNode;
    public string text;
}
[System.Serializable]
public class Dialogue_CharState {
    public string ID;
    public DialogueExpression Expression;
    public string Anim;
    public AudioClip Clip;
    public bool Visible = true;
    public bool Focus = true;
}
public enum DialogueExpression {
    Idle, Happy, Angry, Talk, Shake, Jump, 
    EnterLeft, EnterRight, ExitLeft, ExitRight,
}
public enum DialogueBranch { }
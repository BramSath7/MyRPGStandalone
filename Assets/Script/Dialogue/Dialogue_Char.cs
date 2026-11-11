using UnityEngine;
using UnityEngine.UI;

public class Dialogue_Obj : MonoBehaviour {
    public string ID;
    public Image Portrait;
    public Animator anim;
    //
    public CanvasGroup Group;
    public Sprite Idle, Happy, Angry;
    private void Awake() {
        
    }
    public void SetExpression(DialogueExpression exp)
    {
        switch (exp)
        {
            case DialogueExpression.Idle:
                Portrait.sprite = Idle;
                break;
        }
    }
    public void PlayAnim(string anim)
    {

    }
    public void SetFocus(bool focus)
        => Group.alpha = focus ? 1f : 0.5f;
    public void SetVisible(bool value)
        => gameObject.SetActive(value);
}

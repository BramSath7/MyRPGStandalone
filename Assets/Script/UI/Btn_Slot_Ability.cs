using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Btn_Ability : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler {
    public Ability ability;

    [SerializeField] private Image Icon;
    [SerializeField] private TextMeshProUGUI cooldown;

    [Header("Drag")]
    [SerializeField] private float dragRadius = 50f;
    [SerializeField] private RectTransform dragHandle;
    private Vector2 startPos;
    private Sprite currentIcon;
    public bool dragging {  get; private set; }
    bool canAIM => ability != null && ability.IsAim;
    public void Binding(Ability ability) {
        this.ability = ability;
        currentIcon = ability.CurrentIcon;
        Icon.sprite = currentIcon;
        if (dragHandle != null) {
            dragHandle.gameObject.SetActive(false);
            dragHandle.anchoredPosition = Vector2.zero;
        }
    }
    public void OnPointerDown(PointerEventData eventData) {
        if (ability == null) return; 
        if (!ability.CanCast) return;

        dragging = false;
        startPos = eventData.position;
        ability.Press();
        if (canAIM && dragHandle) {
            dragHandle.gameObject.SetActive(true);
            dragHandle.anchoredPosition = Vector2.zero;
        }
    }
    public void OnDrag(PointerEventData eventData) {
        if (ability == null) return;
        if (!canAIM) return;
        dragging = true;
        Vector2 delta = eventData.position - startPos;
        delta = Vector2.ClampMagnitude(delta, dragRadius);

        ability.UpdateAIM(delta);
        if (dragHandle) {
            dragHandle.anchoredPosition = delta;
        }
    }
    public void OnPointerUp(PointerEventData eventData) {
        if (ability == null) return;
        dragging= false;
        ability.ReleasePress();
        if (dragHandle) {
            dragHandle.gameObject.SetActive(false);
            dragHandle.anchoredPosition = Vector2.zero;
        }
    }
    public void OnUpdate(float dt) {
        if (ability == null) return;
        cooldown.text = ability.IsCooldown ? ability.CooldownTimer.ToString("0.0") : string.Empty;
        if (currentIcon != ability.CurrentIcon) {
            currentIcon = ability.CurrentIcon;
            Icon.sprite = currentIcon;
        }
    }
}

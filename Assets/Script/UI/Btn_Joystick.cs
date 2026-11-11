using UnityEngine;
using UnityEngine.EventSystems;
public class Btn_Joystick : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler {
    public RectTransform Handle;
    public float Radius = 100f;
    public Vector2 Input;
    public bool IsActive => Input.sqrMagnitude > 0.01f;
    private void Awake() {
        Handle = transform.Find("ImgHandle").GetComponent<RectTransform>();
    }
    public void OnPointerDown(PointerEventData eventData) {
        OnDrag(eventData);  
    }
    public void OnDrag(PointerEventData eventData) {
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle((RectTransform)transform, eventData.position, eventData.pressEventCamera, out pos);
        pos = Vector2.ClampMagnitude(pos, Radius);
        Handle.anchoredPosition = pos;
        Input = pos / Radius;
        GameNotify.UpdateMove(Input);
    }
    public void OnPointerUp(PointerEventData eventData) {
        Handle.anchoredPosition = Vector2.zero;
        Input = Vector2.zero;
        GameNotify.UpdateMove(Input);
    }
}

using UnityEngine;

public class Game_NPC : MonoBehaviour
{
    public Animator anim;
    private void Awake()
    {
        
    }
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.TryGetComponent<IDamageable>(out var target)) {
            if (target.IsPlayer) {
                var pop = new UI_Pop
                {
                    Name = "Exit",
                    Message = "Keluar dari Game?",
                    Label = new[] { "YES", "NO" }
                };
                pop.OnPop += index => {
                    if (index == 0) {
                        Application.Quit();
                    }
                };
                var ui = Game_UI.instance?.OpenPopup("Confirm");
                ui.GetComponent<UI_Popup>().Initialize(pop);
            }
        }
    }
}

public class NPC
{

}
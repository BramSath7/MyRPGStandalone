using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Combat_UI : MonoBehaviour {
    public static Combat_UI instance;
    private Unit runtime;
    [Header("Ability")]
    public Btn_Ability Basic;
    public Btn_Ability Skill;
    public Btn_Ability Spell;
    public Btn_Ability Ultimate;
    [Header("Stats")]
    public Image HealthBar;
    public TextMeshProUGUI HealthText;
    public Image EnergyBar;
    public TextMeshProUGUI EnergyText;
    [Header("Feature")]
    public Button Inventory;
    public Button Shop;
    public Button Dialog;
    private void Awake() {
        instance = this;
        var pnlAbility = transform.Find("PnlAbility");
        Basic = pnlAbility.Find("BtnBasic").GetComponent<Btn_Ability>();
        Skill = pnlAbility.Find("BtnSkill").GetComponent<Btn_Ability>();
        Spell = pnlAbility.Find("BtnSpell").GetComponent<Btn_Ability>();
        Ultimate = pnlAbility.Find("BtnUltimate").GetComponent<Btn_Ability>();
        //
        var pnlStat = transform.Find("PnlStat");
        var pnlHealth = pnlStat.Find("PnlHealth");
        var pnlEnergy = pnlStat.Find("PnlEnergy");
        HealthBar = pnlHealth.Find("HealthBar").GetComponent<Image>();
        HealthText = pnlHealth.Find("HealthText").GetComponent<TextMeshProUGUI>();
        EnergyBar = pnlEnergy.Find("EnergyBar").GetComponent<Image>();
        EnergyText = pnlEnergy.Find("EnergyText").GetComponent<TextMeshProUGUI>();
        //
        Inventory = transform.Find("BtnInventory").GetComponent<Button>();
        Shop = transform.Find("BtnShop").GetComponent<Button>();
        Dialog = transform.Find("BtnDialog").GetComponent<Button>();
        Inventory.onClick.RemoveAllListeners();
        Shop.onClick.RemoveAllListeners();
        Dialog.onClick.RemoveAllListeners();
        //
    }
    public void Bind(Unit newRuntime) {
        // Rebind
        if (runtime != null) {
            runtime.Health.OnValue += OnHealthChanged;
            runtime.Health.OnCapacity += OnHealthChanged;

            runtime.Energy.OnValue += OnEnergyCHanged;
            runtime.Energy.OnCapacity += OnEnergyCHanged;
        }

        runtime = newRuntime;
        runtime.Health.OnValue += OnHealthChanged;
        runtime.Health.OnCapacity += OnHealthChanged;
        runtime.Energy.OnValue += OnEnergyCHanged;
        runtime.Energy.OnCapacity += OnEnergyCHanged;
        RefreshBindings();

        UpdateHealth();
        UpdateEnergy();
        NotifyTime.OnTick += OnUpdate;
        Inventory.onClick.AddListener(() => GameNotify.OpenUI("Inventory"));
        Shop.onClick.AddListener(() => { GameNotify.OpenUI("Shop"); });
        Dialog.onClick.AddListener(() => { GameNotify.OpenUI("Dialog"); });
    }
    public void OnDestroy()
    {
        NotifyTime.OnTick -= OnUpdate;
    }
    private void OnHealthChanged(float a, float b) => UpdateHealth();
    private void OnEnergyCHanged(float a, float b) => UpdateEnergy();
    private void UpdateHealth()
    {
        HealthBar.fillAmount = runtime.Health.Ratio;
        HealthText.text =
            $"Health: {runtime.Health.Current:0}/{runtime.Health.CurrentTotal:0}";
    }

    private void UpdateEnergy()
    {
        EnergyBar.fillAmount = runtime.Energy.Ratio;
        EnergyText.text =
            $"Energy: {runtime.Energy.Current:0}/{runtime.Energy.CurrentTotal:0}";
    }
    private void RefreshBindings() {
        Basic.Binding(runtime.Basic);
        Skill.Binding(runtime.Skill);
        Spell.Binding(runtime.Spell);
        Ultimate.Binding(runtime.Ultimate);
    }
    public void OnUpdate(float dt) {
        Basic.OnUpdate(dt);
        Skill.OnUpdate(dt);
        Spell.OnUpdate(dt);
        Ultimate.OnUpdate(dt);
    }
}

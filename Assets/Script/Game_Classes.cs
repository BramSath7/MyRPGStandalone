using UnityEngine;

[System.Serializable]
public class Level
{
    public int CurrentLevel = 1;
    public int MaxLevel = 100;
    public float CurrentExp = 0;
    public float ReqExp => 100f * Mathf.Pow(CurrentLevel, 1.5f);
    public void AddExp(float amount)
    {
        if (CurrentLevel >= MaxLevel) return;
        CurrentExp += amount;
        while (CurrentExp >= ReqExp)
        {
            CurrentExp -= ReqExp;
            CurrentLevel++;
            if (CurrentLevel >= MaxLevel)
            {
                CurrentLevel = MaxLevel;
                CurrentExp = 0;
                break;
            }
            OnLevelUp();
        }
    }
    private void OnLevelUp() { }
}

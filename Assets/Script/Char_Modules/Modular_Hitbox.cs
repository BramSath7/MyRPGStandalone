using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Ability/Hitbox")]
public class Hitbox_Data : ScriptableObject {
    public GameObject Prefab;
    public float Duration;
    public bool FollowCaster;
    public Vector2 Offset;
    //
    public Sprite Icon;
    public AudioClip SpawnSFX;
    [SerializeReference]
    [SubclassSelector]
    public List<Ability_Effect> Effects = new();


    public Vector2 Size;
    public float Distance;
    public bool MultiHit;
    public float HitInterval;
}

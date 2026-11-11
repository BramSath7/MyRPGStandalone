using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class Game_Addressable : MonoBehaviour
{
    public static Game_Addressable Instance;
    private void Awake()
    {
        Instance = this;
    }
    public async Task<Ability_Data> LoadAbility(AssetReferenceT<Ability_Data> reference) {
        var handle = reference.LoadAssetAsync();
        return await handle.Task;
    }
    //
    public async Task<Char_Runtime> Spawn(Char_Data data) {
        var runtime = new Char_Runtime();
        runtime.Data = data;
        var basicData = await LoadAbility(data.Abilities[0]);
        runtime.Ability.Abilities[0] = new AbilityRuntime(basicData, runtime);
        return runtime;
    }
}
public class GameAsset
{
    Dictionary<object, object> cache = new();
    public async Task<T> Load<T>(AssetReferenceT<T> reference) where T : Object
    {
        if (cache.TryGetValue(reference.RuntimeKey, out var obj)) {
            return (T)obj;
        }
        var handle = reference.LoadAssetAsync();
        var result = await handle.Task;
        cache[reference.RuntimeKey] = result;
        return result;
    }
}
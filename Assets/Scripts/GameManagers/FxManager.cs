using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cysharp.Threading.Tasks;

public class FxManager : SingletonWithMonoBehaviour<FxManager> {
    private static readonly string FxPrefabLabel = "Effects";
    private Dictionary<string, ParticleSystem> _fxDictionary;
    private void Awake() {
        AssetLoader.Instance.LoadAssetsAsync<GameObject>(FxPrefabLabel, (op) => {
            _fxDictionary = op.Result
                        .Select(x => x.GetComponent<ParticleSystem>())
                        .ToDictionary(x => x.gameObject.name);
        });
        
    }

    // TODO: 최적화
    public void SpawnParticle(string name, Vector3 position, double duration, Transform parent = null) {
        if (_fxDictionary.TryGetValue(name, out ParticleSystem fx)) {
            var instance = Instantiate(fx);
            instance.transform.position = position;
            instance.transform.SetParent(parent);

            RemoveParticle(instance, duration).Forget();
        }
    }

    private async UniTaskVoid RemoveParticle(ParticleSystem instance, double duration) {
        await UniTask.Delay(System.TimeSpan.FromSeconds(duration));

        DestroyImmediate(instance);
    }
}

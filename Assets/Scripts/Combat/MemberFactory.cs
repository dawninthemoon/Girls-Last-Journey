using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MemberFactory : MonoBehaviour {
    private List<SOAttackConfig> _attackConfigList;
    private List<Sprite> _memberBodySprites;
    private List<EntityStatus> _statusList;
    private void Awake() {
        var assetLoader = AssetLoader.Instance;

        assetLoader.LoadAssetsAsync<Sprite>("MemberInfo", (x) => {
            _memberBodySprites = x.Result as List<Sprite>;
        });
        assetLoader.LoadAssetsAsync<SOAttackConfig>("MemberInfo", (x) => {
            _attackConfigList = x.Result as List<SOAttackConfig>;
        });
        assetLoader.LoadAssetsAsync<EntityStatus>("MemberInfo", (x) => {
            _statusList = x.Result as List<EntityStatus>;
        });
    }

    public EntityInfo GetRandomMember() {
        EntityStatus status = _statusList[Random.Range(0, _statusList.Count)];
        return CreateEntityInfo(status);
    }

    public EntityInfo CreateEntityInfo(EntityStatus status) {
        EntityInfo info = EntityInfo.CreateWithStatus(status);
        info.BodySprite = _memberBodySprites[Random.Range(0, _memberBodySprites.Count)];
        info.EntityAttackConfig = _attackConfigList[Random.Range(0, _attackConfigList.Count)];
        info.Synergy1 = GetRandomSynergy();
        info.Synergy2 = GetRandomSynergy();
        return info;

        SynergyType GetRandomSynergy() {
            int start = (int)SynergyType.None + 1;
            int end = (int)SynergyType.Count;
            return (SynergyType)Random.Range(start, end);
        }
    }
}

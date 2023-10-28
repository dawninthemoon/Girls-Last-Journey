using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SynergyType {
    None,
    SMG,
    Handgun,
    Shotgun,
    Sniper,
    Grenade,
    Flame,
    AcademyA,
    AcademyB,
    AcademyC,
    AcademyD,
    Count
}

[CreateAssetMenu(menuName = "ScriptableObjects/SynergyConfig", fileName = "NewSynergy")]
public class SynergyConfig : ScriptableObject {
    [SerializeField] private SynergyType _type;
    [SerializeField] private BuffConfig _synergyBuff;
    [SerializeField] private float _extraItemDropPercent;
    [SerializeField] private float _extraGoldGainPercent;

    public SynergyType Type {
        get { return _type; }
    }
    public BuffConfig Buff {
        get { return _synergyBuff; }
    }

    public float ExtraItemDropPercent {
        get { return _extraItemDropPercent; }
    }
    public float ExtraGoldGainPercent {
        get { return _extraGoldGainPercent; }
    }
}
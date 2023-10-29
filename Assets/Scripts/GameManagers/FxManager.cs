using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FxManager : SingletonWithMonoBehaviour<FxManager> {
    // 나중에 수정
    [SerializeField] private ParticleSystem[] _fxPrefabs;

    private void Awake() {
        
    }
}

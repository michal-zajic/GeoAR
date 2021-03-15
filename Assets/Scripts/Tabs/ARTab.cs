using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum LockMode {
    unlocked, locked
}

public class ARTab : TabView
{
    [SerializeField] PlaceMapOnARPlane _placer = null;
    [SerializeField] Button _lockButton = null;
    [SerializeField] Button _unlockButton = null;

    LockMode _lockMode = LockMode.unlocked;
    
    // Start is called before the first frame update
    void Start()
    {
        _unlockButton.onClick.AddListener(() => {            
            SetLockModeTo(LockMode.locked);
        });
        _lockButton.onClick.AddListener(() => {
            SetLockModeTo(LockMode.unlocked);
        });
        SetLockModeTo(LockMode.unlocked);
    }

    void SetLockModeTo(LockMode mode) {
        _lockMode = mode;
        _lockButton.gameObject.SetActive(_lockMode == LockMode.locked);
        _unlockButton.gameObject.SetActive(_lockMode == LockMode.unlocked);
        _placer.LockStateChangeTo(_lockMode);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

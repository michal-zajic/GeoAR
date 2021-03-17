using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingToggle : SettingOption
{
    [SerializeField] Image _imageOn = null;
    [SerializeField] Image _imageOff = null;
       
    bool _isOn = false;

    bool isOn {
        get {
            return _isOn;
        }
        set {
            _isOn = value;
            UpdateToggle();
        }
    }
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        var on = Settings.instance.GetValue(setting);
        if (on != null) {
            isOn = (bool)on;
        } else {
            isOn = false;
        }
    }

    void UpdateToggle() {
        _imageOn.gameObject.SetActive(isOn);
        _imageOff.gameObject.SetActive(!isOn);
        Settings.instance.Set(setting, isOn);
    }

    protected override void OnClick() {
        isOn = !isOn;        
    }
}

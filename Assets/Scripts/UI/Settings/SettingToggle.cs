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
        UpdateUI();
    }

    void UpdateToggle() {
        _imageOn.gameObject.SetActive(isOn);
        _imageOff.gameObject.SetActive(!isOn);
        Settings.instance.Set(setting, isOn);
    }

    public override void UpdateUI() {
        base.UpdateUI();
        var on = Settings.instance.GetValue(setting);
        if (on != null) {
            isOn = (bool)on;
        } else {
            isOn = false;
        }
    }

    protected override void OnClick() {
        isOn = !isOn;        
    }
}

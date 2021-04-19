using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

//individual option in setting picker setting
public class SettingDetailItem : MonoBehaviour
{
    [SerializeField] Image _checkImage = null;
    [SerializeField] Button _button = null;
    [SerializeField] Text _text = null;
    
    public void Initialize(string title, bool isActive, UnityAction onClick) {
        _text.text = title;
        _checkImage.enabled = isActive;
        _button.onClick.AddListener(onClick);
    }

    public void UpdateChanged(bool isActive) {
        _checkImage.enabled = isActive;
    }
}

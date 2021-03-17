using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingOption : MonoBehaviour
{
    [SerializeField] Button _button = null;
    [SerializeField] Settings.Setting _setting;

    protected Settings.Setting setting => _setting;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        _button.onClick.AddListener(() => {
            OnClick();
        });
    }

    protected virtual void OnClick() {

    }
}

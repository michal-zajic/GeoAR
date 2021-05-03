////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: SettingOption.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//main setting option class - other options inherit this class
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

    public virtual void UpdateUI() {

    }
}

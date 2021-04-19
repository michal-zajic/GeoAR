using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//individual module entry in module panel
public class ModulePanelEntry : MonoBehaviour
{
    [SerializeField] Text _titleTxt = null;
    [SerializeField] Text _descriptionTxt = null;
    [SerializeField] Image _iconImg = null;
    [SerializeField] Image _checkImg = null;
    [SerializeField] Image _backgroundImg = null;
    [SerializeField] Button _button = null;

    public bool active = false;

    public new string name {
        get {
            return _titleTxt.text;
        }
    }

    public void Init(string title, string description, Sprite icon, Action<string> onClick) {
        _titleTxt.text = title;
        _descriptionTxt.text = description;
        _iconImg.sprite = icon;
        _button.onClick.AddListener(() => {
            onClick(title);
        });

        SetActive(false);
    }

    public void SetActive(bool active) {
        this.active = active;
        _backgroundImg.color = active ? new Color(1, 1, 1, 0.3f) : Color.clear;
        _checkImg.gameObject.SetActive(active);
    }
}

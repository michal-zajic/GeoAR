﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class TabBarButton : MonoBehaviour {
    Color _activeColor {
        get {
            return new Color32(50, 120, 246, 255);
        }
    }
    Color _inactiveColor {
        get {
            return new Color32(160, 160, 160, 255);
        }
    }

    public Button button => _button;

    [SerializeField] Image _image = null;
    [SerializeField] Text _text = null;
    [SerializeField] TabView _tabGameObject = null;
    Button _button = null;

    private void Start() {
        _button = GetComponent<Button>();
    }

    public void SetActive(bool active) {
        _image.color = active ? _activeColor : _inactiveColor;
        _text.color = active ? _activeColor : _inactiveColor;
        _tabGameObject.gameObject.SetActive(active);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder {
    public static ModuleMgr moduleMgr => GetModuleMgr();
    public static UIMgr uiMgr => GetUIMgr();
    public static Camera arCamera => GetARCamera();

    static ModuleMgr _moduleMgr = null;
    static UIMgr _uiMgr = null;
    static Camera _arCamera = null;

    static ModuleMgr GetModuleMgr() {
        if (_moduleMgr == null) {
            _moduleMgr = GameObject.Find("ModuleMgr").GetComponent<ModuleMgr>();
        }
        return _moduleMgr;
    }

    static UIMgr GetUIMgr() {
        if (_uiMgr == null) {
            _uiMgr = GameObject.Find("UI").GetComponent<UIMgr>();
        }
        return _uiMgr;
    }

    static Camera GetARCamera() {
        if(_arCamera == null) {
            _arCamera = GameObject.Find("ARCamera").GetComponent<Camera>();
        }
        return _arCamera;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder {
    public static ModuleMgr moduleMgr => GetModuleMgr();
    public static UIMgr uiMgr => GetUIMgr();

    static ModuleMgr _moduleMgr = null;
    static UIMgr _uiMgr = null;

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
}

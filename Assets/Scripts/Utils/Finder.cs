using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finder {
    public static ModuleMgr moduleMgr => GetModuleMgr();

    static ModuleMgr _moduleMgr = null;

    static ModuleMgr GetModuleMgr() {
        if (_moduleMgr == null) {
            _moduleMgr = GameObject.Find("ModuleMgr").GetComponent<ModuleMgr>();
        }
        return _moduleMgr;
    }
}

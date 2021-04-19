using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class which helps get access to some MonoBehaviour objects in scene
public class Finder {

    private static Finder _instance = null;
    private static FinderLink _link = null;

    public static Finder instance {
        get {
            if(_instance == null) {
                _instance = new Finder();
                //only one GameObject.Find call instead of call for each needed object
                _link = GameObject.Find("FinderLink").GetComponent<FinderLink>();
            }
            return _instance;
        }
    }

    public ModuleMgr moduleMgr => _link.moduleMgr;
    public UIMgr uiMgr => _link.uiMgr;
    public Camera arCamera => _link.arCamera;
    public MeshRenderer particleEmitPlane => _link.particleEmitPlane;
}

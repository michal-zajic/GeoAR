using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//single tab - parent class to AR, Map and Settings tab
public class TabView : MonoBehaviour
{
    [SerializeField] Camera _camera = null;
    [SerializeField] GameObject _objectToHide = null;

    protected Camera camera => _camera;    

    //some tabs use different cameras, which need to be set active
    public void SetActive(bool active) {
        if (active)
            ActivateCamera();
        else
            DeactivateCamera();
        
        _objectToHide.SetActive(active);
        if(active)
            OnTabSelection();
    }

    protected virtual void OnTabSelection() {

    }

    void ActivateCamera() {
        _camera.tag = "MainCamera";
    }
    void DeactivateCamera() {
        _camera.tag = "Untagged";
    }
}

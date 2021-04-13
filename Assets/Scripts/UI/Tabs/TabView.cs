using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour
{
    [SerializeField] Camera _camera = null;
    [SerializeField] GameObject _objectToHide = null;
    [SerializeField] GameObject _tutorialObject = null;

    protected Camera camera => _camera;    

    public void SetActive(bool active) {
        if (active)
            ActivateCamera();
        else
            DeactivateCamera();
        
        _objectToHide.SetActive(active);
        if(active)
            OnTabSelection();
    }

    protected void CreateTutorialPopup() {
        if (_tutorialObject != null) {
            GameObject obj = Instantiate(_tutorialObject, Finder.instance.uiMgr.transform);
            TutorialPanel tutorial = obj.GetComponent<TutorialPanel>();
            tutorial.Init();
        }
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

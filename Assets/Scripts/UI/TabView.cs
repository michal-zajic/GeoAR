using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour
{
    [SerializeField] Camera _camera = null;
    [SerializeField] GameObject _objectToHide = null;

    protected new Camera camera => _camera;
    

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

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour
{
    [SerializeField] Camera _camera = null;
    [SerializeField] GameObject _objectToHide = null;
    

    public void SetActive(bool active) {
        if (active)
            ActivateCamera();
        else
            DeactivateCamera();

        _objectToHide.SetActive(active);
    }

    void ActivateCamera() {
        _camera.tag = "MainCamera";
    }
    void DeactivateCamera() {
        _camera.tag = "Untagged";
    }
}

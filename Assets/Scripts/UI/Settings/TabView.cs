using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabView : MonoBehaviour
{
    [SerializeField] Camera _camera = null;
    [SerializeField] GameObject _objectToHide = null;
    [SerializeField] GameObject _tutorialObject = null;

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

    protected void CreateTutorialPopup(Settings.Setting setting) {
        if (_tutorialObject == null)
            return;
        var showTutorial = Settings.instance.GetValue(setting);
        if (showTutorial != null && (bool)showTutorial == false)
            return;
        GameObject obj = Instantiate(_tutorialObject, Finder.instance.uiMgr.transform);
        TutorialPanel tutorial = obj.GetComponent<TutorialPanel>();
        tutorial.Init(okAction: null, notAgainAction: () => {
            Settings.instance.Set(setting, false);
        });
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

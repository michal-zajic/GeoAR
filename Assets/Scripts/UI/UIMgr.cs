using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{
    [SerializeField] GameObject _noConnectionAlert = null;
    [SerializeField] Button _noConnectionAlertButton = null;
    [SerializeField] GameObject _modulePanel = null;
    [SerializeField] Button _helpButton = null;

    private void Start() {
        _helpButton.onClick.AddListener(() => {
            if(Finder.instance.moduleMgr.activeModule != null)
                Instantiate(Finder.instance.moduleMgr.activeModule.GetTutorialObject(), transform);
        });
    }        

    public void SetModulePanel(bool active) {
        _modulePanel.SetActive(active);
        UpdateHelpButton();
    }

    public void UpdateHelpButton() {
        _helpButton.gameObject.SetActive(Finder.instance.moduleMgr.activeModule != null && _modulePanel.activeInHierarchy);
    }

    public void ShowNoConnectionAlert(bool ar) {
        if ((ar && !AppState.instance.allowARConnectionAlert) || (!ar && !AppState.instance.allowMapConnectionAlert))
            return;
        _noConnectionAlert.gameObject.SetActive(true);
        _noConnectionAlertButton.onClick.AddListener(() => {
            _noConnectionAlert.gameObject.SetActive(false);
        });
    }
}

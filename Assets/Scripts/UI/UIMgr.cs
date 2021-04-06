using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIMgr : MonoBehaviour
{
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
        _helpButton.gameObject.SetActive(active);
    }
}

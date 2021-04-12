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
    [SerializeField] Image _loadingImage = null;

    List<ModuleDataLoader> _loaders = new List<ModuleDataLoader>();
    bool _showLoader = true;

    private void Start() {
        _helpButton.onClick.AddListener(() => {
            if(Finder.instance.moduleMgr.activeModule != null)
                Instantiate(Finder.instance.moduleMgr.activeModule.GetTutorialObject(), transform);
        });
    }

    public void AddLoader(ModuleDataLoader loader) {
        if (!_loaders.Contains(loader))
            _loaders.Add(loader);
    }

    public void RemoveLoader(ModuleDataLoader loader) {
        if (_loaders.Contains(loader)) {
            _loaders.Remove(loader);
        }
    }

    public void SetModulePanel(bool active) {
        _modulePanel.SetActive(active);
        UpdateHelpButton();
    }

    public void SetLoadingImage(bool active) {
        _showLoader = active;
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

    private void Update() {
        if(_loaders.Count > 0 && _showLoader) {
            if (!_loadingImage.gameObject.activeInHierarchy)
                _loadingImage.gameObject.SetActive(true);
            _loadingImage.transform.Rotate(Vector3.forward, -8);
        } else {
            if(_loadingImage.gameObject.activeInHierarchy)
                _loadingImage.gameObject.SetActive(false);
        }
    }
}

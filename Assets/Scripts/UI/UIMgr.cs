////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: UIMgr.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//class controlling UI common to more than one tab
public class UIMgr : MonoBehaviour
{
    [SerializeField] GameObject _noConnectionAlert = null;
    [SerializeField] Button _noConnectionAlertButton = null;
    [SerializeField] GameObject _modulePanel = null;
    [SerializeField] Button _helpButton = null;     //this button calls module legend, not onboarding tutorial
    [SerializeField] Image _loadingImage = null;
    [SerializeField] OnboardingUI _onboardingUI = null;
    [SerializeField] OnboardingUI _onboardingUIModified = null;

    List<ModuleDataLoader> _loaders = new List<ModuleDataLoader>();
    bool _showLoader = true;
        
    private void Start() {
        _helpButton.onClick.AddListener(() => {
            if(Finder.instance.moduleMgr.activeModule != null)
                Instantiate(Finder.instance.moduleMgr.activeModule.GetTutorialObject(), transform);
        });

        if ((bool)Settings.instance.GetValue(Settings.Setting.showOnboarding)) {
            ShowOnboarding(false);
        }
    }

    //adds data loader to active list
    public void AddLoader(ModuleDataLoader loader) {
        if (!_loaders.Contains(loader))
            _loaders.Add(loader);
    }
    //removes loader from active list
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

    public void ShowOnboarding(bool modified) {
        (modified ? _onboardingUIModified : _onboardingUI).Show();
    }

    public void UpdateHelpButton() {
        _helpButton.gameObject.SetActive(Finder.instance.moduleMgr.activeModule != null && _modulePanel.activeInHierarchy);
    }
    //shows alert informing user about bad internet connection or different connection problem
    public void ShowNoConnectionAlert(bool ar) {
        if ((ar && !AppState.instance.allowARConnectionAlert) || (!ar && !AppState.instance.allowMapConnectionAlert))
            return;
        _noConnectionAlert.gameObject.SetActive(true);
        _noConnectionAlertButton.onClick.AddListener(() => {
            _noConnectionAlert.gameObject.SetActive(false);
        });
    }

    //if there is at least one data loader loading data, loading indicator spins in top left corner
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

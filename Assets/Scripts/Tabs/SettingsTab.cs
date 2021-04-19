using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTab : TabView
{
    [SerializeField] Button _resetTutorialsButton = null;
    [SerializeField] Transform _settingsParent = null;

    private void Start() {
        _resetTutorialsButton.onClick.AddListener(() => {
            Settings.instance.Set(Settings.Setting.showPlacementHint, true);
        });
    }

    protected override void OnTabSelection() {
        base.OnTabSelection();
        Finder.instance.uiMgr.SetModulePanel(false);
        Finder.instance.uiMgr.SetLoadingImage(false);

        foreach(Transform setting in _settingsParent) {
            setting.GetComponent<SettingOption>().UpdateUI();
        }
    }
}

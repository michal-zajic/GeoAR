using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsTab : TabView
{
    [SerializeField] Button _resetTutorialsButton = null;

    private void Start() {
        _resetTutorialsButton.onClick.AddListener(() => {
            Settings.instance.Set(Settings.Setting.showARTutorial, true);
            Settings.instance.Set(Settings.Setting.showMapTutorial, true);
            Settings.instance.Set(Settings.Setting.showPlacementHint, true);
        });
    }

    protected override void OnTabSelection() {
        base.OnTabSelection();
        Finder.instance.uiMgr.SetModulePanel(false);
    }
}

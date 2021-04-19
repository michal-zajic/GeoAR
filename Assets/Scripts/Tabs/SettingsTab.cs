using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//Controller for settings tab
public class SettingsTab : TabView
{
    [SerializeField] Transform _settingsParent = null;

    //when tab clicked, check individual settings and update them if needed
    //also show/hide settings specific UI
    protected override void OnTabSelection() {
        base.OnTabSelection();
        Finder.instance.uiMgr.SetModulePanel(false);
        Finder.instance.uiMgr.SetLoadingImage(false);

        foreach(Transform setting in _settingsParent) {
            setting.GetComponent<SettingOption>().UpdateUI();
        }
    }
}

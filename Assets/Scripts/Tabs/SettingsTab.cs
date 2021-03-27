using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsTab : TabView
{
    protected override void OnTabSelection() {
        base.OnTabSelection();
        Finder.uiMgr.SetModulePanel(false);
    }
}

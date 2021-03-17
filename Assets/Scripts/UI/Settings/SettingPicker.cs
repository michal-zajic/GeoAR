using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingPicker : SettingOption
{
    [SerializeField] GameObject _detailScreenPrefab = null;
    [SerializeField] Text _settingText = null;
    [SerializeField] Text _choiceText = null;
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _choiceText.text = (string)Settings.instance.GetValue(setting);
    }

    protected override void OnClick() {
        List<string> items = Settings.instance.GetItemsFor(setting);
        if(items == null) {
            return;
        }

        Transform canvas = GameObject.FindGameObjectWithTag("SettingsCanvas").transform;
        GameObject screen = Instantiate(_detailScreenPrefab as GameObject, canvas);
        SettingDetailScreen detailScreen = screen.GetComponent<SettingDetailScreen>();
        detailScreen.Initialize(_settingText.text, Settings.instance.GetDescriptionFor(setting), items, _choiceText.text, (item => {
            _choiceText.text = item;
            Settings.instance.Set(setting, item);
        }));
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public sealed class Settings {
    public enum Setting {
        gpsDefault,
        moduleDefault,
        zoomDefault,
        useSatellite
    }

    public static Settings instance {
        get {
            if (_instance == null) {
                _instance = new Settings();
            }
            return _instance;
        }
    }
    private static Settings _instance = null;

    Settings() {
        Load();
    }

    Dictionary<Setting, object> _dict;
    string path {
        get {
            return Application.persistentDataPath + "/settings.json";
        }
    }

    public List<string> GetItemsFor(Setting setting) {
        switch (setting) {
            case Setting.zoomDefault:
                return new List<string> { "Maximální", "Střední", "Minimální" };
            default:
                return null;
        }
    }

    public bool IsGPSDefault() {
        return (bool)GetValue(Setting.gpsDefault);
    }

    public float GetDefaultZoom() {
        switch (GetValue(Setting.zoomDefault)) {
            case "Maximální":
                return 1;                
            case "Střední":
                return 0.5f;
            case "Minimální":
                return 0;
            default:
                return 0;                
        }
    }

    public string GetDescriptionFor(Setting setting) {
        switch (setting) {
            case Setting.zoomDefault:
                return "Nastavte výchozí přiblížení mapy v rozšířené realitě. Větší přiblížení zobrazí menší oblast na stejném úseku mapy, než než menší přiblížení.";
            default:
                return "";
        }
    }

    public void Add(Setting key, object value) {
        _dict.Add(key, value);
        Save();
    }

    public object GetValue(Setting key) {
        if (_dict != null && _dict.ContainsKey(key)) {
            return _dict[key];
        } else {
            return null;
        }
    }

    public void Set(Setting key, object value) {
        if (_dict == null) {
            SetDefaults();
        }
        if (_dict.ContainsKey(key)) {
            _dict[key] = value;
            Save();
        } else {
            Add(key, value);
        }
    }

    private int GetTypeFor(Setting setting) {
        //0 = bool
        //1 = int
        //2 = float
        //3 = string
        switch (setting) {
            case Setting.gpsDefault:
                return 0;
            case Setting.moduleDefault:
                return 3;
            case Setting.zoomDefault:
                return 3;
            case Setting.useSatellite:
                return 0;
            default:
                return -1;
        }
    }

    private void SetDefaults() {
        _dict = new Dictionary<Setting, object>();

        _dict.Add(Setting.gpsDefault, false);
        _dict.Add(Setting.moduleDefault, "Mraky");
        _dict.Add(Setting.zoomDefault, "Střední");
        _dict.Add(Setting.useSatellite, true);

        Save();
    }

    private void Load() {
        if (!File.Exists(path)) {
            SetDefaults();
            return;
        }
        string text = File.ReadAllText(path);
        if (string.IsNullOrEmpty(text)) {
            SetDefaults();
            return;
        }
        JSONObject json = new JSONObject(text);
        _dict = new Dictionary<Setting, object>();

        foreach (Setting setting in Enum.GetValues(typeof(Setting))) {
            if (json.HasField(setting.ToString())) {
                //0 = bool
                //1 = int
                //2 = float
                //3 = string
                switch (GetTypeFor(setting)) {
                    case 0:
                        Add(setting, json.GetField(setting.ToString()).b);
                        break;
                    case 1:
                        Add(setting, (int)json.GetField(setting.ToString()).i);
                        break;
                    case 2:
                        Add(setting, json.GetField(setting.ToString()).f);
                        break;
                    case 3:
                        Add(setting, json.GetField(setting.ToString()).str);
                        break;
                    default:
                        Add(setting, json.GetField(setting.ToString()));
                        break;
                }

            }
        }
    }

    private void Save() {
        JSONObject json = new JSONObject();
        foreach (KeyValuePair<Setting, object> pair in _dict) {
            JSONObject setting = new JSONObject();
            object val = _dict[pair.Key];
            string key = pair.Key.ToString();

            switch (GetTypeFor(pair.Key)) {
                case 0:
                    json.AddField(key, (bool)val);
                    break;
                case 1:
                    json.AddField(key, (int)val);
                    break;
                case 2:
                    json.AddField(key, (float)val);
                    break;
                case 3:
                    json.AddField(key, (string)val);
                    break;
                default:
                    json.AddField(pair.Key.ToString(), JsonUtility.ToJson(val));
                    break;
            }
        }


        File.WriteAllText(path, json.ToString());
    }

}

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.Networking;

public class PollenDataLoader : ModuleDataLoader
{
    string baseAddress = "https://api.ambeedata.com/latest/pollen/by-lat-lng?";
    string fileName = "pollenInfo.json";

    string filePath {
        get {
            return Application.persistentDataPath + "/" + fileName;
        }
    }

    [HideInInspector]
    public List<PollenInfo> pollenInfos = new List<PollenInfo>();

    List<Vector2d> startLocations = new List<Vector2d> {
        new Vector2d(50.1103664, 13.0473083),
        new Vector2d(50.4333703, 14.2338317),
        new Vector2d(50.2440467, 15.623602),
        new Vector2d(49.7603358, 16.4146181),
        new Vector2d(49.8702147, 17.3704283),
        new Vector2d(49.2000158, 16.9254822),
        new Vector2d(49.0274272, 14.4260925),
        new Vector2d(49.5220025, 13.7504333),
        new Vector2d(49.9162189, 14.6732850),
        new Vector2d(49.4613444, 15.5851503)
    };

    int startLocationsCounter = 0;

    private void Start() {
        if (!LoadDataFromDisc()) {
            location = startLocations[0];
            GetData();
        }        
    }

    public override void Init(AbstractMap map, bool ar = false) {
        base.Init(map, ar);
    }

    public override void GetData(Action onFinish = null) {
        if (IsLocationNearExisting(location)) {
            if (onFinish != null)
                onFinish();
        } else {
            StartCoroutine(LoadJSON(location, onFinish, ar));
        }        
    }

    public PollenInfo NearestInfoTo(Vector2d location) {
        double dist = double.PositiveInfinity;
        PollenInfo nearestInfo = pollenInfos[0];
        for(int i = 0; i < pollenInfos.Count; i++) {
            double tempDist = Tools.GetDistanceBetweenPoints(location.x, location.y, pollenInfos[i].coordinates.x, pollenInfos[i].coordinates.y);
            if (tempDist < dist) {
                dist = tempDist;
                nearestInfo = pollenInfos[i];
            }
        }
        return nearestInfo;
    }

    IEnumerator LoadJSON(Vector2d location, Action onFinish, bool ar) {        
        string locationString = "lat=" + location.x + "&lng=" + location.y;
        
        Uri address = new Uri(baseAddress + locationString);

        UnityWebRequest request = UnityWebRequest.Get(address);
        request.SetRequestHeader("x-api-key", "M9rf3WXvhx2PlxGAIDKgyv1N5KQnGev8ZtR2AzHj");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            Debug.Log(request.error);
            Finder.instance.uiMgr.ShowNoConnectionAlert(ar);
            yield break;
        } else {
            string s = request.downloadHandler.text;
            JSONObject json = new JSONObject(s);

            ProcessJSON(json, location, onFinish);

            startLocationsCounter++;
            if(startLocationsCounter < startLocations.Count - 1) {
                location = startLocations[startLocationsCounter];
                GetData();
            } else if(startLocationsCounter == startLocations.Count - 1) {
                location = startLocations[startLocationsCounter];
                GetData(SaveDataToDisc);
            }
        }
    }

    void ProcessJSON(JSONObject json, Vector2d coordinates, Action onFinish) {
        if(json.GetField("message").str != "Success")
            return;

        JSONObject info = json["data"][0];
        JSONObject count = info["Count"];
        JSONObject danger = info["Risk"];

        PollenInfo pollenInfo = new PollenInfo();
        pollenInfo.coordinates = coordinates;
        pollenInfo.grassCount = (int)count["grass_pollen"].i;
        pollenInfo.treeCount = (int)count["tree_pollen"].i;
        pollenInfo.weedCount = (int)count["weed_pollen"].i;
        pollenInfo.grassDanger = PollenInfo.DangerFromString(danger["grass_pollen"].str);
        pollenInfo.treeDanger = PollenInfo.DangerFromString(danger["tree_pollen"].str);
        pollenInfo.weedDanger = PollenInfo.DangerFromString(danger["weed_pollen"].str);

        pollenInfos.Add(pollenInfo);
        if(onFinish != null)
            onFinish();
    }

    bool IsLocationNearExisting(Vector2d location) {
        var locations = pollenInfos.Select(info => { return info.coordinates; });

        for(int i = 0; i < locations.ToArray().Length; i++) {
            var loc = locations.ToArray()[i];
            if (Tools.GetDistanceBetweenPoints(location.x, location.y, loc.x, loc.y) < 40000) {
                return true;
            }
        }        

        return false;
    }

    bool LoadDataFromDisc() {
        if (!File.Exists(filePath))
            return false;

        var text = File.ReadAllText(filePath);
        JSONObject json = new JSONObject(text);

        if(json.IsArray && Mathf.Abs((int)json[0]["day"].i - DateTime.Now.DayOfYear) > 5) {
            return false;
        }

        json.list.ForEach(infoJSON => {
            PollenInfo info = new PollenInfo();
            Vector2d coordinates = new Vector2d(infoJSON["latitude"].f, infoJSON["longitude"].f);
            JSONObject pollenJSON = infoJSON["pollen"];
            info.coordinates = coordinates;
            info.grassCount = (int)pollenJSON["grass_pollen"].i;
            info.treeCount = (int)pollenJSON["tree_pollen"].i;
            info.weedCount = (int)pollenJSON["weed_pollen"].i;
            info.grassDanger = PollenInfo.DangerFromString(pollenJSON["grass_danger"].str);
            info.treeDanger = PollenInfo.DangerFromString(pollenJSON["tree_danger"].str);
            info.weedDanger = PollenInfo.DangerFromString(pollenJSON["weed_danger"].str);

            pollenInfos.Add(info);
        });

        return true;
    }    

    void SaveDataToDisc() {
        JSONObject json;
        if(!File.Exists(filePath)) {
            json = new JSONObject();
        } else {
            var text = File.ReadAllText(filePath);
            json = new JSONObject(text);
        }

        if(json.IsArray) {

        } else {
            pollenInfos.ForEach(info => {
                JSONObject infoJSON = new JSONObject();
                infoJSON.AddField("day", DateTime.Now.DayOfYear);
                JSONObject pollen = new JSONObject();
                pollen.AddField("grass_pollen", info.grassCount);
                pollen.AddField("tree_pollen", info.treeCount);
                pollen.AddField("weed_pollen", info.weedCount);
                pollen.AddField("grass_danger", info.grassDanger.ToString());
                pollen.AddField("tree_danger", info.treeDanger.ToString());
                pollen.AddField("weed_danger", info.weedDanger.ToString());
                infoJSON.AddField("pollen", pollen);
                infoJSON.AddField("latitude", (float)info.coordinates.x);
                infoJSON.AddField("longitude", (float)info.coordinates.y);

                json.Add(infoJSON);
            });
        }

        File.WriteAllText(filePath, json.ToString());
        
    }
}

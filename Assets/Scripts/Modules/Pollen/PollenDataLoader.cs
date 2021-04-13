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

    const string grassJSONKey = "grass_pollen";
    const string treeJSONKey = "tree_pollen";
    const string weedJSONKey = "weed_pollen";
    const string grassDangerJSONKey = "grass_danger";
    const string treeDangerJSONKey = "tree_danger";
    const string weedDangerJSONKey = "weed_danger";

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
    bool initialized = false;
    bool firstTime = true;
    Vector2d tmpLocation = Vector2d.zero;

    public override void Init(AbstractMap map, bool ar = false) {
        base.Init(map, ar);
        initialized = LoadDataFromDisc();       
    }

    public override void GetData(Action onFinish = null) {
        if (firstTime) {
            tmpLocation = location;
            firstTime = false;
        }
        if (!initialized) {            
            location = startLocations[0];
            LoadJSON((json) => { RequestComplete(json, onFinish); });
            return;
        }
        if (IsLocationNearExisting(location)) {
            if (onFinish != null)
                onFinish();
        } else {
            LoadJSON((json) => { RequestComplete(json, onFinish); });
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

    protected override UnityWebRequest GetRequest() {
        string locationString = "lat=" + location.x + "&lng=" + location.y;

        Uri address = new Uri(baseAddress + locationString);

        UnityWebRequest request = UnityWebRequest.Get(address);
        request.SetRequestHeader("x-api-key", "M9rf3WXvhx2PlxGAIDKgyv1N5KQnGev8ZtR2AzHj");
        return request;
    }

    void RequestComplete(JSONObject json, Action onFinish) {             
        ProcessJSON(json, location);

        startLocationsCounter++;
        if(startLocationsCounter < startLocations.Count - 1) {
            location = startLocations[startLocationsCounter];
            GetData();
            return;
        } else if(startLocationsCounter == startLocations.Count - 1) {
            location = startLocations[startLocationsCounter];
            GetData(SaveDataToDisc);
            initialized = true;
            if (tmpLocation != Vector2d.zero) {
                location = tmpLocation;
                tmpLocation = Vector2d.zero;
                GetData();
            }
            return;
        }

        Stop();
        if (onFinish != null)
            onFinish();
    }

    void ProcessJSON(JSONObject json, Vector2d coordinates) {
        if(json.GetField("message").str != "Success")
            return;

        JSONObject info = json["data"][0];
        JSONObject count = info["Count"];
        JSONObject danger = info["Risk"];

        PollenInfo pollenInfo = new PollenInfo();
        pollenInfo.coordinates = coordinates;
        pollenInfo.grassCount = (int)count[grassJSONKey].i;
        pollenInfo.treeCount = (int)count[treeJSONKey].i;
        pollenInfo.weedCount = (int)count[weedJSONKey].i;
        pollenInfo.grassDanger = PollenInfo.DangerFromString(danger[grassJSONKey].str);
        pollenInfo.treeDanger = PollenInfo.DangerFromString(danger[treeJSONKey].str);
        pollenInfo.weedDanger = PollenInfo.DangerFromString(danger[weedJSONKey].str);

        pollenInfos.Add(pollenInfo);        
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
            info.grassCount = (int)pollenJSON[grassJSONKey].i;
            info.treeCount = (int)pollenJSON[treeJSONKey].i;
            info.weedCount = (int)pollenJSON[weedJSONKey].i;
            info.grassDanger = PollenInfo.DangerFromString(pollenJSON[grassDangerJSONKey].str);
            info.treeDanger = PollenInfo.DangerFromString(pollenJSON[treeDangerJSONKey].str);
            info.weedDanger = PollenInfo.DangerFromString(pollenJSON[weedDangerJSONKey].str);

            pollenInfos.Add(info);
        });

        return true;
    }    

    void SaveDataToDisc() {
        JSONObject json = new JSONObject();
        /*if(!File.Exists(filePath)) {
            json = new JSONObject();
        } else {
            var text = File.ReadAllText(filePath);
            json = new JSONObject(text);
        }*/

        
        pollenInfos.ForEach(info => {
            JSONObject infoJSON = new JSONObject();
            infoJSON.AddField("day", DateTime.Now.DayOfYear);
            JSONObject pollen = new JSONObject();
            pollen.AddField(grassJSONKey, info.grassCount);
            pollen.AddField(treeJSONKey, info.treeCount);
            pollen.AddField(weedJSONKey, info.weedCount);
            pollen.AddField(grassDangerJSONKey, info.grassDanger.ToString());
            pollen.AddField(treeDangerJSONKey, info.treeDanger.ToString());
            pollen.AddField(weedDangerJSONKey, info.weedDanger.ToString());
            infoJSON.AddField("pollen", pollen);
            infoJSON.AddField("latitude", (float)info.coordinates.x);
            infoJSON.AddField("longitude", (float)info.coordinates.y);

            json.Add(infoJSON);
        });

        File.WriteAllText(filePath, json.ToString());
        
    }
}

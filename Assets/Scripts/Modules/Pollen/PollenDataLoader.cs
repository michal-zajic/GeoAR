////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PollenDataLoader.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.Networking;

//Component loading pollen data from Ambee
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

    //default locations (handpicked), whose pollen infos are stored on disc to optimize requests count
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
    bool firstTime = true;
    Action onDefaultLoadedAction;
    Vector2d tmpLocation = Vector2d.zero;

    bool defaultsLoaded = false;

    //inits loader and tries to load data from disc if available
    public override void Init(AbstractMap map, bool ar = false) {
        base.Init(map, ar);
        if(!defaultsLoaded)
            defaultsLoaded = LoadDataFromDisc();       
    }

    //if there were no data stored on disc, loader needs to download data for default locations in startLocations
    //after downloading, it needs to save them to disc
    //otherwise it checks, if there is nearby stored location, if not, it downloads new data
    public override void GetData(Action onFinish = null) {
        if (firstTime && !defaultsLoaded) {
            tmpLocation = location;
            location = startLocations[0];
            onDefaultLoadedAction = () => {
                SaveDataToDisc();
                defaultsLoaded = true;
                onFinish();
            };
            firstTime = false;
        }
        if (!defaultsLoaded || !IsLocationNearExisting(location)) {            
            LoadJSON((json) => { RequestComplete(json, onFinish); });
            return;
        } else { 
            if (onFinish != null)
                onFinish();
        }        
    }

    //returns nearest pollen info source to requested location
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

    //after the data are downloaded, they are processed and stored to data list
    //if we are in phase of downloading default locations, new downloads are recursively called, until all startLocations are downloaded
    //afterwards, the requested location is also downloaded and processed
    void RequestComplete(JSONObject json, Action onFinish) {             
        ProcessJSON(json, location);

        if (!defaultsLoaded) {
            startLocationsCounter++;
            if (startLocationsCounter < startLocations.Count - 1) {
                location = startLocations[startLocationsCounter];
                print(startLocationsCounter);                
                GetData();
                return;
            } else if (startLocationsCounter == startLocations.Count - 1) {
                location = startLocations[startLocationsCounter];
                print(startLocationsCounter);
                GetData();
                return;
            }
        }
        if (!tmpLocation.Equals(Vector2d.zero)) {
            location = tmpLocation;
            tmpLocation = Vector2d.zero;
            print("get tmp");
            GetData(onDefaultLoadedAction);
            return;
        }

        Stop();
        if (onFinish != null)
            onFinish();
    }

    //processes json and stores to data list
    void ProcessJSON(JSONObject json, Vector2d coordinates) {
        if(json.GetField("message").str.ToLower() != "success")
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

    //checks if there is any location near requested
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

    //loads data from disc
    //returns false if there are no data stored or if they are 5 or more days old
    //returns true otherwise
    bool LoadDataFromDisc() {
        if (!File.Exists(filePath))
            return false;

        var text = File.ReadAllText(filePath);
        JSONObject json = new JSONObject(text);

        if(json.IsArray && Mathf.Abs((int)json[0]["day"].i - DateTime.Now.DayOfYear) > 5) {
            return false;
        }
        pollenInfos = new List<PollenInfo>();
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

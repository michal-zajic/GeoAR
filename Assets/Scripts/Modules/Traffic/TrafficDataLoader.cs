////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: TrafficDataLoader.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.Networking;

public class TrafficDataLoader : ModuleDataLoader {
    string baseAddress = "https://traffic.ls.hereapi.com/traffic/6.2/flow.json";
    string apiKey = "?apiKey=VVbMhxKTMZkABWgvegg3CGisnT4edCrIO_vHiVn_fac";
    string attrAddress = "&responseattributes=sh";

    public List<TrafficSegment> mapSegments = new List<TrafficSegment>();
    public List<TrafficSegment> arSegments = new List<TrafficSegment>();

    string locationAddress => "&prox=" + location.x.ToString() + "," + location.y.ToString() + "," + range.ToString() + attrAddress;
    string finalAddress => baseAddress + apiKey + locationAddress;

    public override void GetData(Action onFinish = null) {
        LoadJSON((json)=> { ProcessJSON(json, onFinish); });
    }

    public override void Init(AbstractMap map, bool ar = false) {
        base.Init(map, ar);
    }

    protected override UnityWebRequest GetRequest() {
        Uri url = new Uri(finalAddress);
        return new UnityWebRequest(url);        
    }

    //pretty complicate processing as there are a lot of single-element lists
    void ProcessJSON(JSONObject json, Action onFinish = null) {
        if (ar)
            arSegments = new List<TrafficSegment>();
        else
            mapSegments = new List<TrafficSegment>();
        if (json.IsNull)
            return;        
        JSONObject flowArray = json["RWS"][0]["RW"];
        foreach(JSONObject rw in flowArray.list) {
            JSONObject segmentArray = rw["FIS"][0]["FI"];
            foreach(JSONObject segmentJSON in segmentArray.list) {
                TrafficSegment segment = new TrafficSegment();                
                JSONObject info = segmentJSON["CF"][0];
                segment.jamFactor = info["JF"].f;

                //during implementation and testing, there were some situations, where SU parameter wasnt available, but SP was
                //SP takes SU values, but limits them to maximum legal speed in this segment as oppose to SU, which allowes higher speeds
                if (info.HasField("SU")) {
                    segment.speed = info["SU"].f;
                } else if (info.HasField("SP")) {
                    segment.speed = info["SP"].f;
                } else {
                    continue;
                }

                //processes segment shape, for some reason, sometimes it is divided into many smaller segments, which are connected anyway
                JSONObject shape = segmentJSON["SHP"];
                foreach(JSONObject shp in shape.list) {
                    string segments = shp["value"][0].str;
                    string[] parsed = segments.Split(' ');
                    foreach(string coord in parsed) {
                        string[] coords = coord.Split(',');
                        if (coords.Length == 2) {
                            Vector2d coordinates = new Vector2d(double.Parse(coords[0]), double.Parse(coords[1]));
                            if (segment.coordinateList.Count == 0 || !segment.coordinateList[segment.coordinateList.Count - 1].Equals(coordinates)) {
                                segment.coordinateList.Add(coordinates);
                            }
                        }
                    }
                }
                                
                if (!CheckIfSegmentCrossesMap(segment.coordinateList)) {
                    continue;
                }

                if (!ar)
                    mapSegments.Add(segment);
                else
                    arSegments.Add(segment);

            }
        }
        Stop();
        if (onFinish != null)
            onFinish();
    }

    //the map is bigger than what user sees, so if some segment doesn't cross the visible part, remove it
    bool CheckIfSegmentCrossesMap(List<Vector2d> coordinates) {
        for(int i = 0; i < coordinates.Count; i++) {
            var c = coordinates[i];
            if(Tools.GetDistanceBetweenPoints(c.x, c.y, location.x, location.y) < range) {
                return true;
            }
        }
        return false;
    }
}

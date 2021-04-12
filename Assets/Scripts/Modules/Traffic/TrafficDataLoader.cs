﻿using System;
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
        string s = finalAddress;
        Stop();
        Finder.instance.uiMgr.AddLoader(this);
        StartCoroutine(LoadJSON(onFinish));
    }

    public override void Init(AbstractMap map, bool ar = false) {
        base.Init(map, ar);
    }

    IEnumerator LoadJSON(Action onFinish = null) {
        Uri address = new Uri(finalAddress);

        UnityWebRequest request = UnityWebRequest.Get(address);
        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            Debug.Log(request.error);
            Finder.instance.uiMgr.ShowNoConnectionAlert(ar);
            Stop();
            yield break;
        } else {
            string s = request.downloadHandler.text;
            JSONObject json = new JSONObject(s);

            ProcessJSON(json, onFinish);
        }
    }

    void ProcessJSON(JSONObject json, Action onFinish = null) {
        if (ar)
            arSegments = new List<TrafficSegment>();
        if (json.IsNull)
            return;
        JSONObject flowArray = json["RWS"][0]["RW"];
        foreach(JSONObject rw in flowArray.list) {
            JSONObject segmentArray = rw["FIS"][0]["FI"];
            foreach(JSONObject segmentJSON in segmentArray.list) {
                TrafficSegment segment = new TrafficSegment();                
                JSONObject info = segmentJSON["CF"][0];
                //IF LOW JAM, SKIP
                segment.jamFactor = info["JF"].f;
                if (info.HasField("SU")) {
                    segment.speed = info["SU"].f;
                } else if (info.HasField("SP")) {
                    segment.speed = info["SP"].f;
                } else {
                    continue;
                }

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
        if (onFinish != null)
            onFinish();
        Stop();
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

using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;
using System;
using UnityEngine.Networking;
using System.Globalization;
using Mapbox.Unity.Map;

public class RecycleDataLoader : ModuleDataLoader
{
    string baseAddress = "https://api.golemio.cz/v2/sortedwastestations/";

    public List<Container> containers { get; private set; }
    public List<Container> arContainers { get; private set; }

    public override void GetData(Action onFinish = null) {
        StartCoroutine(LoadJSON(onFinish));
    }

    public override void Init(AbstractMap map, bool ar = false) {
        base.Init(map, ar);                
    }

    IEnumerator LoadJSON(Action onFinish) {
        string locationString = "";
        string rangeString = "";
        if (range > 0) {
            locationString = "?latlng=" + location.x + "%2C" + location.y + "&";
            rangeString = "range=" + range;
        }
        Uri address = new Uri(baseAddress + locationString + rangeString);

        UnityWebRequest request = UnityWebRequest.Get(address);
        request.SetRequestHeader("x-access-token", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJlbWFpbCI6Im1lZ3JheG9uMjJAZ21haWwuY29tIiwiaWQiOjY5NCwibmFtZSI6bnVsbCwic3VybmFtZSI6bnVsbCwiaWF0IjoxNjE1NjU0NTM3LCJleHAiOjExNjE1NjU0NTM3LCJpc3MiOiJnb2xlbWlvIiwianRpIjoiMmZhYTM1NmItZDMzOC00YTYwLWE2ZWYtNjJkYjQ1ZDVlMWZkIn0.52nQNk3umt8GnOGUABnfEDjxQvpLjOin-l07iV1WJfM");

        yield return request.SendWebRequest();

        if (request.isNetworkError || request.isHttpError) {
            Debug.Log(request.error);
            Finder.instance.uiMgr.ShowNoConnectionAlert(ar);
            yield break;            
        } else {
            string s = request.downloadHandler.text;
            JSONObject json = new JSONObject(s);

            StartCoroutine(ProcessJSON(json, onFinish, ar));
        }
    }

    IEnumerator ProcessJSON(JSONObject json, Action onFinish, bool ar) {
        if (ar)
            arContainers = new List<Container>();
        else
            containers = new List<Container>();
        int i = 0;

        if (json.HasField("features")) {
            JSONObject features = json.GetField("features");
            foreach(JSONObject feature in features.list) {
                Container container = new Container();

                JSONObject coords = feature["geometry"]["coordinates"];
                float lat = coords.list[0].f;
                float lon = coords.list[1].f;
                container.coordinates = new Vector2d(lat, lon);

                JSONObject properties = feature["properties"];
                container.accessibility = (Container.Accessibility)properties["accessibility"]["id"].i;

                container.trashTypes = new List<Container.TrashType>();
                JSONObject bins = properties["containers"];
                foreach(JSONObject bin in bins.list) {
                    container.trashTypes.Add((Container.TrashType)bin["trash_type"]["id"].i);
                }

                (ar ? arContainers : containers).Add(container);

                i++;
                if (i % 10 == 0)
                    yield return null;
            }
        }
        if(onFinish != null)
            onFinish();
    }
    
}

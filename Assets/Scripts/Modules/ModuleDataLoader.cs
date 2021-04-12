using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;
using UnityEngine.Networking;

public abstract class ModuleDataLoader : MonoBehaviour {
    protected Vector2d location;
    protected float range;
    protected bool ar;

    private bool completed = false;
    private int timeout = 7;

    public virtual void Init(AbstractMap map, bool ar = false) {
        this.ar = ar;
        range = ar ? 580 : 1000;
        location = map.CenterLatitudeLongitude;
    }

    public void Stop() {
        Finder.instance.uiMgr.RemoveLoader(this);
        print("STOP");
        StopAllCoroutines();
    }

    public abstract void GetData(Action onFinish = null);
    protected abstract UnityWebRequest GetRequest();

    protected void LoadJSON(Action<JSONObject> onComplete = null) {
        Stop();
        Finder.instance.uiMgr.AddLoader(this);
        completed = false;
        if (Application.internetReachability == NetworkReachability.NotReachable) {
            Debug.Log("Error. Check internet connection!");
        }
        StartCoroutine(TimeoutWrapper(onComplete));
    }

    void OnDownloadFailed() {
        Finder.instance.uiMgr.ShowNoConnectionAlert(ar);
        Stop();
    }

    IEnumerator TimeoutWrapper(Action<JSONObject> onComplete = null) {        
        StartCoroutine(LoadJSONCoroutine(onComplete));
        yield return new WaitForSecondsRealtime(timeout);
        if (!completed) {
            OnDownloadFailed();
        }

    }
        
    IEnumerator LoadJSONCoroutine(Action<JSONObject> onComplete = null) {
        print("START");
        UnityWebRequest request = GetRequest();
        request.timeout = timeout;
        request.downloadHandler = new DownloadHandlerBuffer();
        yield return request.SendWebRequest();
        print("AFTER");
        if (request.isNetworkError || request.isHttpError || request.downloadHandler == null || !request.isDone) {
            Debug.Log(request.error);
            OnDownloadFailed();
            yield break;
        } else {            
            string s = request.downloadHandler.text;
            JSONObject json = new JSONObject(s);
            completed = true;
            if(onComplete != null)
                onComplete(json);
        }
    }
}

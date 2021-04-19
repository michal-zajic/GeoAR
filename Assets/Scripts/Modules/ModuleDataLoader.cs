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
    public bool downloading { get; private set; }

    //initializes properties, which are useful to many data loaders
    public virtual void Init(AbstractMap map, bool ar = false) {
        this.ar = ar;
        range = ar ? 580 : 1000;
        location = map.CenterLatitudeLongitude;
        downloading = false;
    }
    //stops any loading coroutines and stops loading indicator
    public void Stop() {
        Finder.instance.uiMgr.RemoveLoader(this);
        downloading = false;
        StopAllCoroutines();
    }

    //inherited loaders should implement their own data getting function and provide request
    public abstract void GetData(Action onFinish = null);
    protected abstract UnityWebRequest GetRequest();

    //checks internet reachability, starts loading indicator, starts loading coroutine
    protected void LoadJSON(Action<JSONObject> onComplete = null) {
        if (downloading)
            return;
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

    //wraps downloading into timer, as request.timeout doesnt seem to work properly
    //if timeout time passes, the download is stopped and user is notificated
    IEnumerator TimeoutWrapper(Action<JSONObject> onComplete = null) {        
        StartCoroutine(LoadJSONCoroutine(onComplete));
        yield return new WaitForSecondsRealtime(timeout);
        if (!completed) {
            OnDownloadFailed();
        }

    }

    //gets request from inherited loader and sends it
    //after completion, calls onComplete method
    //if it fails, it notifies the user
    IEnumerator LoadJSONCoroutine(Action<JSONObject> onComplete = null) {
        UnityWebRequest request = GetRequest();
        request.timeout = timeout;
        request.downloadHandler = new DownloadHandlerBuffer();
        downloading = true;
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError || request.downloadHandler == null || !request.isDone) {
            Debug.Log(request.error);            
            OnDownloadFailed();
            yield break;
        } else {            
            string s = request.downloadHandler.text;
            JSONObject json = new JSONObject(s);
            completed = true;
            downloading = false;
            if(onComplete != null)
                onComplete(json);
        }
    }
}

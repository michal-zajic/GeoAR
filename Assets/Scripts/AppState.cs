using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;

//contains information about current app state
public class AppState
{
    public static AppState instance {
        get {
            if(_instance == null) {
                _instance = new AppState();
            }
            return _instance;
        }
    }
    private static AppState _instance = null;

    public Vector2d transferLocation { get; private set; }  //when user taps transfer button, the location is stored here and accessed from AR tab

    public bool allowMapConnectionAlert = false;    //we dont want to show no connection popup during certain situations, so individual tabs can 
    public bool allowARConnectionAlert = false;     //set and get the avaiability here

    public int carsSpawned;    //contains current number of spawned cars

    public void UpdateTransferLocation(Vector2d location) {
        transferLocation = location;
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mapbox.Utils;

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

    public Vector2d transferLocation { get; private set; }

    public bool allowMapConnectionAlert = false;
    public bool allowARConnectionAlert = false;

    public int carsSpawned;

    public void UpdateTransferLocation(Vector2d location) {
        transferLocation = location;
    }
}

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

    public Vector2d currentMapCenter;
    public Vector2d markerLocation;
    public bool markerPlaced;

    public bool allowMapConnectionAlert = false;
    public bool allowARConnectionAlert = false;

    public void UpdateMarkerLocation(Vector2d location) {
        markerLocation = location;
    }
    public void UpdateMapCenter(Vector2d center) {
        currentMapCenter = center;
    }
}

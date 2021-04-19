using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

//Traffic data structure
public class TrafficSegment 
{
    public List<Vector2d> coordinateList = new List<Vector2d>();    //segment route points
    public float speed;
    public float jamFactor;

    public static Color GetColorFromJamFactor(float jamFactor) {
        if (jamFactor < 0) {
            return Color.black;
        } else if (jamFactor < 2) {
            return Color.green;
        } else if (jamFactor < 5) {
            return new Color32(255, 190, 0, 255);
        } else return Color.red;
    }
}

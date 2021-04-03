using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

public class PollenInfo
{
    public enum Danger {
        low, medium, high
    }

    public Vector2d coordinates;
    public int grassCount;
    public int treeCount;
    public int weedCount;
    public Danger grassDanger;
    public Danger treeDanger;
    public Danger weedDanger;

    public static Danger DangerFromString(string s) {
        switch (s.ToLower()) {
            case "low":
                return Danger.low;
            case "medium":
                return Danger.medium;
            case "high":
                return Danger.high;
            default:
                return Danger.low;
        }
    }
}



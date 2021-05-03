////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PollenInfo.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

//Pollen data structure
public class PollenInfo
{
    public enum Danger {
        low, medium, high
    }

    public Vector2d coordinates;
    public int grassCount;          //pollen particles per m^3
    public int treeCount;
    public int weedCount;
    public Danger grassDanger;      //pollen danger calculated by Ambee
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



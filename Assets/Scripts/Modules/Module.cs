////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: Module.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

[RequireComponent(typeof(ModuleMapVisualizer), typeof(ModuleDataLoader), typeof(ModuleARVisualizer))]
public abstract class Module: MonoBehaviour {
    [HideInInspector]
    public ModuleMapVisualizer mapVisualizer { get; private set; }
    [HideInInspector]
    public ModuleARVisualizer arVisualizer { get; private set; }
    [HideInInspector]
    public ModuleDataLoader dataLoader { get; private set; }

    public new string name {
        get {
            return GetName();
        }
    }

    public abstract float GetMinZoom();
    public abstract string GetName();
    public abstract string GetDescription();
    public abstract GameObject GetTutorialObject();
    public abstract Sprite GetIcon();

    public void Init() {
        mapVisualizer = GetComponent<ModuleMapVisualizer>();
        arVisualizer = GetComponent<ModuleARVisualizer>();
        dataLoader = GetComponent<ModuleDataLoader>();
    } 
}

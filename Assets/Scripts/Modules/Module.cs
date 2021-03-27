using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

[RequireComponent(typeof(ModuleVisualizer), typeof(ModuleDataLoader), typeof(ModuleARVisualizer))]
public abstract class Module: MonoBehaviour {
    [HideInInspector]
    public ModuleVisualizer visualizer { get; private set; }
    [HideInInspector]
    public ModuleARVisualizer arVisualizer { get; private set; }
    [HideInInspector]
    public ModuleDataLoader dataLoader { get; private set; }

    public new string name {
        get {
            return GetName();
        }
    }

    public abstract string GetName();
    public abstract string GetDescription();
    public abstract GameObject GetTutorialObject();
    public abstract Sprite GetIcon();

    public void Init() {
        visualizer = GetComponent<ModuleVisualizer>();
        arVisualizer = GetComponent<ModuleARVisualizer>();
        dataLoader = GetComponent<ModuleDataLoader>();
    } 
}

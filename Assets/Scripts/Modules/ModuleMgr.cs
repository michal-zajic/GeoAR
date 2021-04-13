using System;
using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public class ModuleMgr : MonoBehaviour
{    
    public List<GameObject> modulesObjects;

    [HideInInspector]
    public List<Module> modules { get; private set; }

    [HideInInspector]
    public Module activeModule = null;

    private Vector2d lastCoord = Vector2d.zero;
    private Vector2d lastCoordAR = Vector2d.zero;
    private AbstractMap map = null;
    private AbstractMap arMap = null;

    // Start is called before the first frame update
    void Start()
    {
        modules = new List<Module>();
        for(int i = modulesObjects.Count - 1; i>=0; i--) {
            GameObject obj = Instantiate(modulesObjects[i], transform);
            if(obj.GetComponent<Module>() == null) {
                modulesObjects.RemoveAt(i);
                Destroy(obj);
            } else {
                modules.Add(obj.GetComponent<Module>());
                modules[modules.Count - 1].Init();
            }
        }

        string defaultModule = (string)Settings.instance.GetValue(Settings.Setting.moduleDefault);
        modules.ForEach(module => {
            if (module.name == defaultModule)
                SetActiveModule(defaultModule);
        });        
    }

    public void SetActiveModule(string moduleName) {
        if(activeModule != null) {
            activeModule.arVisualizer.Disable();
            activeModule.mapVisualizer.Disable();
            activeModule.dataLoader.Stop();
        }

        if(moduleName == null) {
            activeModule = null;            
        }
        modules.ForEach((module) => {
            if(module.name == moduleName) {
                activeModule = module;

                EnableVisualizer(true, () => {
                    EnableVisualizer(false);
                });
            }
        });
        Finder.instance.uiMgr.UpdateHelpButton();
    }

    public void VisualizeOnMap(AbstractMap map) {
        this.map = map;
        lastCoord = map.CenterLatitudeLongitude;
        UpdateAndDrawData(false);
    }

    public void VisualizeAROnMap(AbstractMap map) {
        arMap = map;
        lastCoordAR = map.CenterLatitudeLongitude;
        UpdateAndDrawData(true);
    }

    //If coordinates didnt change, just enable the objects, otherwise reload data
    private void EnableVisualizer(bool ar, Action onFinish = null) {
        ModuleVisualizer vis;
        if (ar)
            vis = activeModule.arVisualizer;
        else
            vis = activeModule.mapVisualizer;
        if (!vis.lastCoord.Equals(Vector2d.zero) && vis.lastCoord.Equals(ar ? lastCoordAR : lastCoord)) {
            vis.Enable();
            if (onFinish != null)
                onFinish();
        } else {
            UpdateAndDrawData(ar, onFinish);
        }
    }

    //Refreshes data on new map coordinates
    private void UpdateAndDrawData(bool ar, Action onFinish = null) {
        AbstractMap currentMap = ar ? arMap : map;
        if (activeModule != null && currentMap != null) {
            ModuleVisualizer vis;
            if (ar)
                vis = activeModule.arVisualizer;
            else
                vis = activeModule.mapVisualizer;
            Vector2d center = (ar ? arMap : map).CenterLatitudeLongitude;
            vis.lastCoord = center;

            activeModule.dataLoader.Init(currentMap, ar);
            activeModule.dataLoader.GetData(() => {
                vis.Draw(activeModule.dataLoader, currentMap);
                if(onFinish != null)
                    onFinish();
            });
        }
    }
}

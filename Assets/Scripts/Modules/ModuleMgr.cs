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

    public Module activeModule = null;

    private Vector2d lastCoord;
    private Vector2d lastCoordAR;
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
            activeModule.visualizer.Disable();
        }

        if(moduleName == null) {
            activeModule = null;
        }
        modules.ForEach((module) => {
            if(module.name == moduleName) {
                activeModule = module;

                EnableVisualizer(true);
                EnableVisualizer(false);
            }
        });
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
    private void EnableVisualizer(bool ar) {
        ModuleVisualizer vis = ar ? activeModule.arVisualizer : activeModule.visualizer;
        if(vis.lastCoord.Equals(ar ? lastCoordAR : lastCoord)) {
            vis.Enable();
        } else {
            UpdateAndDrawData(ar);
        }
    }

    //Refreshes data on new map coordinates
    private void UpdateAndDrawData(bool ar) {
        AbstractMap currentMap = ar ? arMap : map;        
        if (activeModule != null && currentMap != null) {
            ModuleVisualizer vis = ar ? activeModule.arVisualizer : activeModule.visualizer;
            Vector2d center = (ar ? arMap : map).CenterLatitudeLongitude;
            vis.lastCoord = center;

            activeModule.dataLoader.GetDataFor(center, 580, () => {                
                vis.Draw(activeModule.dataLoader, currentMap);
            });
        }
    }
}

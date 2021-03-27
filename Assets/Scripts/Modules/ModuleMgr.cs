using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public class ModuleMgr : MonoBehaviour
{    
    public List<GameObject> modulesObjects;

    [HideInInspector]
    public List<Module> modules { get; private set; }

    public Module activeModule = null;

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
        if(moduleName == null) {
            activeModule = null;
        }
        modules.ForEach((module) => {
            if(module.name == moduleName) {
                activeModule = module;
            }
        });
    }

    public void VisualizeOnMap(AbstractMap map) {

    }

    public void VisualizeAROnMap(AbstractMap map) {
        if (activeModule != null) {
            activeModule.dataLoader.GetDataFor(map.CenterLatitudeLongitude, 580, () => {
                activeModule.arVisualizer.Prepare(activeModule.dataLoader, map);
            });
        }
    }
}

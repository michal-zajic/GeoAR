////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PollenARVisualizer.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public class PollenARVisualizer : ModuleARVisualizer
{
    [SerializeField] GameObject _pollenARObject = null;

    public override void Disable() {
        objects.ForEach(obj => {
            obj.SetActive(false);
        });
    }

    public override void Enable() {
        objects.ForEach(obj => {
            obj.SetActive(true);
        });
    }

    //gets data for nearest pollen source and initializes AR object
    public override void Draw(ModuleDataLoader data, AbstractMap map) {
        DestroyObjects();
        objects = new List<GameObject>();
        PollenDataLoader loader = data as PollenDataLoader;        
        PollenInfo info = loader.NearestInfoTo(map.CenterLatitudeLongitude); 
        GameObject obj = Instantiate(_pollenARObject, map.transform);
        obj.transform.localPosition = new Vector3(0, 0, 0);
        PollenARObject arObj = obj.GetComponent<PollenARObject>();
        arObj.SetParticles(info.grassCount, info.treeCount, info.weedCount);
        objects.Add(obj);
    }
}

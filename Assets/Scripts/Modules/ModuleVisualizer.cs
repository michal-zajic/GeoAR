using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using UnityEngine;

public abstract class ModuleVisualizer : MonoBehaviour
{
    public abstract void Prepare(ModuleDataLoader data, AbstractMap map);
    public abstract void Enable();
    public abstract void Disable();
}

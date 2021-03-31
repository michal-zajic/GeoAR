using System.Collections;
using System.Collections.Generic;
using Mapbox.Unity.Map;
using Mapbox.Utils;
using UnityEngine;

public abstract class ModuleVisualizer : MonoBehaviour
{
    [HideInInspector] public Vector2d lastCoord;
    public abstract void Draw(ModuleDataLoader data, AbstractMap map);
    public abstract void Enable();
    public abstract void Disable();
}

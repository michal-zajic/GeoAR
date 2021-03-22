using System.Collections;
using System.Collections.Generic;
using Mapbox.Utils;
using UnityEngine;

public abstract class ModuleDataLoader : MonoBehaviour
{
    public abstract JSONObject GetDataFor(Vector2d location, float range = 0);
}

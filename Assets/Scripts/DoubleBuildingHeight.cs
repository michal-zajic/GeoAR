using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//mapbox building are unusualy short - this script is added to modifier, which is applied to all spawned buildings and their height is doubled
//result looks more realistic, although there are still some buildings, whose height doesnt correspond to its real life counterpart - nothing can be done about that though
public class DoubleBuildingHeight : MonoBehaviour
{

    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    }

    void Update()
    {
        
    }
}

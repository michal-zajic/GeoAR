////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: PollenMapObject.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pollen object used for 2D map visualization
public class PollenMapObject : MonoBehaviour
{
    [SerializeField] GameObject grassObj = null;
    [SerializeField] GameObject treeObj = null;
    [SerializeField] GameObject weedObj = null;

    public void SetActive(bool grass, bool tree, bool weed) {
        grassObj.SetActive(grass);
        treeObj.SetActive(tree);
        weedObj.SetActive(weed);
    }
}

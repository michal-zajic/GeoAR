using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

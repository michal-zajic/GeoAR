﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleBuildingHeight : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y * 2, transform.localScale.z);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

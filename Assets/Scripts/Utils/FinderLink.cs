////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: FinderLink.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//helper class, objects needed in Finder can be linked in Unity inspector to this class, so there is no need for GameObject.Find method, which is less reliable
public class FinderLink : MonoBehaviour
{
    public ModuleMgr moduleMgr = null;
    public UIMgr uiMgr = null;
    public Camera arCamera = null;
    public MeshRenderer particleEmitPlane = null;
}

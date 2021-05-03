////////////////////////////////////////////////////////////////////////////////////////////////////////
//FileName: Mask3DReceiver.cs
//Author : Michal Zajíc
//Year : 2021
////////////////////////////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

//This component adjusts render queue so these objects will not appear behind mask planes
public class Mask3DReceiver : MonoBehaviour {

	protected void Awake() {
		Renderer renderer = GetComponent<Renderer>();

		Material[] materials = renderer.materials;
		for (int i = 0; i < materials.Length; ++i) {
			materials[i].renderQueue = 3000 + i;
		}
	}
}

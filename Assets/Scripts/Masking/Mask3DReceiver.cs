using UnityEngine;

public class Mask3DReceiver : MonoBehaviour {

	protected void Awake() {
		Renderer renderer = GetComponent<Renderer>();

		Material[] materials = renderer.materials;
		for (int i = 0; i < materials.Length; ++i) {
			materials[i].renderQueue = 3000 + i;
		}
	}
}

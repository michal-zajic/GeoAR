using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Pollen object used for AR visualization
public class PollenARObject : MonoBehaviour
{
    //particle system for each pollen category
    [SerializeField] ParticleSystem _grassPS = null;
    [SerializeField] ParticleSystem _treePS = null;
    [SerializeField] ParticleSystem _weedPS = null;

    public void SetParticles(int grassCount, int treeCount, int weedCount) {
        SetupPS(_grassPS, grassCount);
        SetupPS(_treePS, treeCount);
        SetupPS(_weedPS, weedCount);
    }

    //setups all important particle system parameters, most important is max particles, which limits the number of
    //active particles by pollen particle count
    void SetupPS(ParticleSystem ps, int count) {
        ps.gameObject.SetActive(false);
        var emission = ps.emission;
        emission.rateOverTime = 10;
        var main = ps.main;
        main.maxParticles = Mathf.Max(0, count - 50);
        main.startSpeed = 0.15f;
        main.startSize = 0.1f;
        main.startLifetime = 30.0f;

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.MeshRenderer;
        shape.meshRenderer = Finder.instance.particleEmitPlane;
        shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
        shape.alignToDirection = true;
        shape.useMeshColors = false;

        ps.gameObject.SetActive(true);
        ps.Play();
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class PhysicsSim : MonoBehaviour
{
    public float PAtm;
    public Vector3 WindVelocity = Vector3.zero;
    public float density;
    public float g;
    public GameObject paperPlane;
    PaperSkinnedMeshGenerator meshGenerator;

    // Update is called once per frame
    void Update()
    {
        List<Vector3> cellNorm = meshGenerator.cellNormals;
        List<Vector3> cellCen = meshGenerator.cellCenters;

        Vector3[] ForceField = new Vector3[cellCen.Count];

        // Now you can apply force at each cell center
        for (int i = 0; i < ForceField.Length; i++)
        {
            // Initialize or set the force at each cell center
            ForceField[i] = cellNorm[i] * (float)PressureFinder(WindVelocity, cellCen[i].y); // Example: setting all forces to zero initially
        }
    }

    double PressureFinder(Vector3 velocity, float height){
        return PAtm + 0.5 * density * Mathf.Pow(velocity.magnitude, 2) + density * g * height;
    }
}

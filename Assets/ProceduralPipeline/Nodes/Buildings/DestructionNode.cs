using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using Random = UnityEngine.Random;
using Parabox.CSG;

[CreateNodeMenu("Buildings/Add Building Destruction")]

public class DestructionNode : ExtendedNode
{
    [Input] public GameObject[] inputBuildings;
    [Input] public GameObject comparisonObject;
    [Input] public float quantity;
    [Input] public float scale;
    [Output] public GameObject[] buildingGameObjects;

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {

        if (port.fieldName == "buildingGameObjects")
        {
            return buildingGameObjects;
        }
        Debug.LogWarning("Output not found!");
        return null;
    }

    public override void CalculateOutputs(Action<bool> callback)
    {
        // setup inputs
        GameObject[] buildings = GetInputValue("inputBuildings", inputBuildings);
        Debug.Log("DESTRUCTION RUNNING!");
        GameObject comparison = GetInputValue("comparisonObject", comparisonObject);
        float q = GetInputValue("quantity", quantity);
        float s = GetInputValue("scale", scale);

        // setup outputs
        List<GameObject> gameObjects = new List<GameObject>();
        
        // Initialize two new meshes in the scene
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.localScale = Vector3.one * 1.3f;

        // Perform boolean operation
        Model result = CSG.Subtract(cube, sphere);

        // Create a gameObject to render the result
        var composite = new GameObject();
        composite.AddComponent<MeshFilter>().sharedMesh = result.mesh;
        composite.AddComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
        Debug.Log("Completed Subtraction");

        int count = 0;
        foreach (GameObject building in buildings)
        {
            GameObject buildingGO = AddBuildingDestruction(building, comparison, q, s);
            gameObjects.Add(buildingGO);
            count += 1;
            Debug.Log("Buildings Destroyed " + count);
            // if (count == 2) break;
        }

        // buildingGameObjects = new List<GameObject>().ToArray();
        buildingGameObjects = gameObjects.ToArray();
        callback.Invoke(true);
    }

    private GameObject AddBuildingDestruction(GameObject building, GameObject comparison, float q, float s)
    {
        // Place comparison object at one corner of the mesh and add object as child of building
        Vector3[] vertices = building.GetComponent<MeshFilter>().sharedMesh.vertices;
        // comparison = GameObject.CreatePrimitive(PrimitiveType.Cube);
        int randomNum = 0; // Random.Range(0, vertices.Length);
        Vector3 randomVertex = vertices[randomNum];
        GameObject go = GameObject.Instantiate(
            comparison,
            building.transform.position + randomVertex,
            Random.rotation,
            building.transform
            );

        try
        {
            // Calculate the result of a boolean subtraction operation
            // Perform boolean operation
            Model result = CSG.Subtract(building, go);

            // Create a gameObject to render the result
            building.GetComponent<MeshFilter>().sharedMesh = result.mesh;
            building.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
            
            // Destroy the comparison GO if successful
            Destroy(go);
        }
        catch (StackOverflowException e)
        {
            Debug.Log("CSG Failure: " + e);
        }

        return building;
    }
}

using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using XNode;
using Parabox.CSG;
using Random = UnityEngine.Random;

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
        Debug.Log("This is running");
        Debug.Log(buildings.Length);
        GameObject comparison = GetInputValue("comparisonObject", comparisonObject);
        Debug.Log(comparison);
        float q = GetInputValue("quantity", quantity);
        float s = GetInputValue("scale", scale);

        // setup outputs
        List<GameObject> gameObjects = new List<GameObject>();

        int count = 0;
        foreach (GameObject building in buildings)
        {
            GameObject buildingGO = AddBuildingDestruction(building, comparison, q, s);
            gameObjects.Add(buildingGO);
            count += 1;
            Debug.Log("Buildings Destroyed " + count);
            if (count == 2) break;
        }

        // buildingGameObjects = new List<GameObject>().ToArray();
        buildingGameObjects = gameObjects.ToArray();
        Debug.Log(buildingGameObjects.Length);
        callback.Invoke(true);
    }

    private GameObject AddBuildingDestruction(GameObject building, GameObject comparison, float q, float s)
    {
        // Place comparison object at one corner of the mesh
        // comparison.transform.position = building.GetComponent<MeshFilter>().sharedMesh.vertices[0];
        
        // Add comparison object as child of building
        GameObject go = GameObject.Instantiate(
            comparison,
            building.transform.position + building.GetComponent<MeshFilter>().sharedMesh.vertices[0],
            Random.rotation,
            building.transform
            );
        Debug.Log("The comparison location is: " + go.transform.position);

        // Calculate the result of a boolean subtraction operation
        // Model result = CSG.Subtract(building, comparison);
        
        // Create a gameObject to render the result
        // building.GetComponent<MeshFilter>().sharedMesh = result.mesh;
        // building.GetComponent<MeshRenderer>().sharedMaterials = result.materials.ToArray();
        
        return building;
    }
}

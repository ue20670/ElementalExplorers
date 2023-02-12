using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using XNode;

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

        foreach (GameObject building in buildings)
        {
            GameObject buildingGO = AddBuildingDestruction(building, comparison, q, s);
            gameObjects.Add(buildingGO);
        }

        // buildingGameObjects = new List<GameObject>().ToArray();
        buildingGameObjects = gameObjects.ToArray();
        Debug.Log(buildingGameObjects.Length);
        callback.Invoke(true);
    }

    private GameObject AddBuildingDestruction(GameObject building, GameObject comparison, float q, float s)
    {
        MeshFilter mesh = building.GetComponent<MeshFilter>();
        mesh.sharedMesh = comparison.GetComponent<MeshFilter>().sharedMesh;
        return building;
    }
}

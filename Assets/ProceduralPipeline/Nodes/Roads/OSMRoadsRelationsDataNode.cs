﻿using System;
using System.Collections.Generic;
using UnityEngine;
using XNode;
using UnityEngine.Networking;

[CreateNodeMenu("Roads/OSM Roads Relations Data")]
public class OSMRoadsRelationsDataNode : ExtendedNode
{

    [Input] public GlobeBoundingBox boundingBox;
    [Input] public int timeout;
    [Input] public int maxSize;
    [Input] public bool debug;

    [Output] public OSMRoadRelation[] roadsRelationArray;


    // Use this for initialization
    protected override void Init()
    {
        base.Init();

    }

    // Return the correct value of an output port when requested
    public override object GetValue(NodePort port)
    {
        if (port.fieldName == "roadsRelationArray")
        {
            return roadsRelationArray;
        }
        else
        {
            return null;
        }
    }

    public override void CalculateOutputs(Action<bool> callback)
    {
        GlobeBoundingBox actualBoundingBox = GetInputValue("boundingBox", boundingBox);
        int actualTimeout = GetInputValue("timeout", timeout);
        int actualMaxSize = GetInputValue("maxSize", maxSize);
        sendRequest(actualBoundingBox, actualTimeout, actualMaxSize, callback);
    }

    public void sendRequest(GlobeBoundingBox boundingBox, int timeout, int maxSize, Action<bool> callback)
    {
        string endpoint = "https://overpass.kumi.systems/api/interpreter/?";
        string query = "data=[out:json][timeout:" + timeout + "][maxsize:" + maxSize + "];relation[highway](" + boundingBox.south + "," + boundingBox.west + "," +
            boundingBox.north + "," + boundingBox.east + ");out body;>;out skel qt;";
        string sendURL = endpoint + query;


        UnityWebRequest request = UnityWebRequest.Get(sendURL);
        UnityWebRequestAsyncOperation operation = request.SendWebRequest();
        operation.completed += (AsyncOperation operation) =>
        {
            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(request.error);
                callback.Invoke(false);
            }
            else
            {

                OSMRelationsContainer result = JsonUtility.FromJson<OSMRelationsContainer>(request.downloadHandler.text.Replace("ref", "reference"));
                List<RelationUninitialised> relations = new List<RelationUninitialised>();
                Dictionary<ulong, OSMWay> wayDictionary = new Dictionary<ulong, OSMWay>();
                if (debug)
                {
                    Debug.Log(result);
                }
                foreach (RelationOrWay element in result.elements)
                {
                    if (element.type == "relation")
                    {
                        relations.Add(new RelationUninitialised() { tags = element.tags, ways = element.members });
                    }
                    else if (element.type == "way")
                    {
                        wayDictionary.Add(element.id, new OSMWay() { id = element.id, nodes = element.nodes });
                    }
                }

                roadsRelationArray = new OSMRoadRelation[relations.Count];
                List<OSMWay> innerWays = new List<OSMWay>();
                List<OSMWay> outerWays = new List<OSMWay>();
                for (int i = 0; i < relations.Count; i++)
                {
                    innerWays.Clear();
                    outerWays.Clear();
                    foreach (RelationWay way in relations[i].ways)
                    {
                        if (way.role == "outer")
                        {
                            outerWays.Add(wayDictionary[way.reference]);
                        }
                        else
                        {
                            innerWays.Add(wayDictionary[way.reference]);
                        }
                    }
                    roadsRelationArray[i] = new OSMRoadRelation() { tags = relations[i].tags, innerWays = innerWays.ToArray(), outerWays = outerWays.ToArray() };
                }
                callback.Invoke(true);
            }
            request.Dispose();
        };
    }

    public override void Release()
    {
        base.Release();
        roadsRelationArray = null;
    }

    [System.Serializable]
    public class RelationOrWay
    {
        public string type;
        public ulong id;
        public ulong[] nodes;
        public RelationWay[] members;
        public OSMTags tags;
    }

    [System.Serializable]
    public class RelationUninitialised
    {
        public OSMTags tags;
        public RelationWay[] ways;
    }

    [System.Serializable]
    public class OSMRelationsContainer
    {
        public RelationOrWay[] elements;
    }
}


[System.Serializable]
public struct OSMRoadRelation
{
    public OSMTags tags;
    public OSMWay[] innerWays;
    public OSMWay[] outerWays;
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class VRMap
{
    public Transform vrTarget;
    public Transform rigTarget;

    public Vector3 trackingPositionOffset;
    public Vector3 trackingRotationOffset;

    public void Map()
    {
        rigTarget.position = vrTarget.TransformPoint((trackingPositionOffset));
        rigTarget.rotation = vrTarget.rotation * Quaternion.Euler(trackingRotationOffset);
    }
}

public class VRRig : MonoBehaviour
{

    [SerializeField] private Transform headConstriant;

    [SerializeField] private Vector3 headBodyOffset;

    [SerializeField] private VRMap head;
    [SerializeField] private VRMap leftHand;
    [SerializeField] private VRMap rightHand;
    
    // Start is called before the first frame update
    void Start()
    {
        headBodyOffset = transform.position - headConstriant.position;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = headConstriant.position + headBodyOffset;
        transform.forward = Vector3.Lerp(transform.forward, Vector3.ProjectOnPlane(headConstriant.up, Vector3.up).normalized, Time.deltaTime);
        head.Map();
        leftHand.Map();
        rightHand.Map();
        transform.position = new Vector3(51, 4, 18);
    }
}

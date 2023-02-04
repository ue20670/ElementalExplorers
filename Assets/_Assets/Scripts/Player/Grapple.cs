using System;
using UnityEngine;
using Valve.VR;

public class Grapple : MonoBehaviour
{

    [Tooltip("The SteamVR boolean action that starts grappling")]
    [SerializeField] private SteamVR_Action_Boolean triggerPull;
    [Tooltip("The SteamVR boolean action that grapples in")]
    [SerializeField] private SteamVR_Action_Boolean aPressed;

    [SerializeField] private SteamVR_Input_Sources[] handControllers;
    [SerializeField] private GameObject[] handObjects = new GameObject[2];

    [SerializeField] private float spring;
    [SerializeField] private float damppening;
    [SerializeField] private float thrust;
    [SerializeField] float grappleMaxDistance;
    [SerializeField] float grappleMinDistance;
    [SerializeField] float castRadius;

    private readonly SpringJoint[] sjs = new SpringJoint[2];
    private LineRenderer[] lrs;
    private SteamVR_Behaviour_Pose[] handPoses;
    private readonly Vector3[] attachmentPoints = new Vector3[2] { Vector3.zero, Vector3.zero };
    private Rigidbody rb;
    private bool[] grappleBroke = new bool[2] {false, false};
    private LayerMask lm;

    public SteamVR_Action_Boolean.StateHandler[] callBacksTriggerPullState = new SteamVR_Action_Boolean.StateHandler[2];
    public SteamVR_Action_Boolean.StateUpHandler[] callBacksTriggerPullStateUp = new SteamVR_Action_Boolean.StateUpHandler[2];
    public SteamVR_Action_Boolean.StateHandler[] callBacksAPressedState = new SteamVR_Action_Boolean.StateHandler[2];

    private void Start()
    {
        if (triggerPull == null || aPressed == null)
        {
            Debug.LogError("[SteamVR] Boolean action not set.", this);
            return;
        }
        if (handControllers.Length != 2)
        {
            Debug.LogError("[SteamVR] hands not added", this);
        }
        lrs = new LineRenderer[2] { handObjects[0].GetComponent<LineRenderer>(), handObjects[1].GetComponent<LineRenderer>() };
        handPoses = new SteamVR_Behaviour_Pose[2] { handObjects[0].GetComponent<SteamVR_Behaviour_Pose>(), handObjects[1].GetComponent<SteamVR_Behaviour_Pose>() };
        rb = gameObject.GetComponent<Rigidbody>();
        for (int i = 0; i < 2; i++)
        {
            callBacksTriggerPullState[i] = StartGrappleHand(i);
            triggerPull[handControllers[i]].onState += callBacksTriggerPullState[i];
            callBacksTriggerPullStateUp[i] = EndGrappleHand(i);
            triggerPull[handControllers[i]].onStateUp += callBacksTriggerPullStateUp[i];
            callBacksAPressedState[i] = GrappleInHand(i);
            aPressed[handControllers[i]].onState += callBacksAPressedState[i];
        }
        lm = ~gameObject.layer; // not player layer
    }

    private void OnDestroy() // probs won't work correctly
    {
        for (int i = 0; i < 2; i++)
        {
            triggerPull[handControllers[i]].onState -= callBacksTriggerPullState[i];
            triggerPull[handControllers[i]].onStateUp -= callBacksTriggerPullStateUp[i];
            aPressed[handControllers[i]].onState -= callBacksAPressedState[i];
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < 2; i++) { // updates each line renderer and shortens the spring joints
            if (sjs[i]) // if connected with spring joint (grappling)
            {
                sjs[i].minDistance = Mathf.Min((handPoses[i].transform.position - attachmentPoints[i]).magnitude, sjs[i].minDistance);
                sjs[i].anchor = handPoses[i].transform.localPosition - Vector3.up; // connected point
                lrs[i].SetPositions(new Vector3[] { attachmentPoints[i], handPoses[i].transform.position });
                if (sjs[i].minDistance < grappleMinDistance)
                {
                    grappleBroke[i] = true;
                    Destroy(sjs[i]);
                    sjs[i] = null;
                    lrs[i].enabled = false;
                }
            }
        }
    }

    private SteamVR_Action_Boolean.StateHandler StartGrappleHand(int i)
    {
        return delegate (SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (sjs[i] != null || grappleBroke[i]) // if already grappling or the grapple broke don't try again
            {
                return;
            }

            Ray ray = new(handPoses[i].transform.position, handPoses[i].transform.forward);
            if (!Physics.Raycast(ray, out RaycastHit hit, grappleMaxDistance, lm))
            {
                if (!Physics.SphereCast(ray, castRadius, out hit, grappleMaxDistance, lm))
                {
                    return;
                }
            }

            if (hit.transform.gameObject.layer == 5) return; // 5 if object is in UI layer

            attachmentPoints[i] = hit.point;
            sjs[i] = gameObject.AddComponent<SpringJoint>();
            sjs[i].anchor = handPoses[i].transform.position; // connected point
            sjs[i].autoConfigureConnectedAnchor = false;
            sjs[i].connectedAnchor = attachmentPoints[i]; // grapple point

            sjs[i].spring = spring;
            sjs[i].damper = damppening;

            sjs[i].minDistance = Mathf.Min((gameObject.transform.position - attachmentPoints[i]).magnitude, sjs[i].minDistance);
            sjs[i].minDistance = float.MaxValue;

            lrs[i].SetPositions(new Vector3[] { attachmentPoints[i], handPoses[i].transform.position });
            lrs[i].enabled = true;
        };
    }

    private SteamVR_Action_Boolean.StateUpHandler EndGrappleHand(int i)
    {
        return delegate (SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            grappleBroke[i] = false;
            if (sjs[i] != null)
            {
                Destroy(sjs[i]);
                sjs[i] = null;
                lrs[i].enabled = false;
            }
        };
    }

    public SteamVR_Action_Boolean.StateHandler GrappleInHand(int i)
    {
        return delegate (SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource)
        {
            if (sjs[i] != null)
            {
                sjs[i].minDistance = Mathf.Max(sjs[i].minDistance - thrust * Time.deltaTime, 0);
                rb.AddForce((attachmentPoints[i] - handPoses[i].transform.position).normalized * Time.deltaTime, ForceMode.VelocityChange);
            }
        };
    }
}

using UnityEngine;
using UnityEngine.Rendering;

public class CheckpointController : MonoBehaviour
{
    [SerializeField] public int checkpoint;
    [SerializeField] private bool finish;
    [SerializeField] private Shader shader;
    public RaceController raceController;
    public bool passed;
    // reference to shader property

    private static readonly int h = Shader.PropertyToID("_Y_height");
    private static readonly int holoHeight = Shader.PropertyToID("_Hologrm_Height");

    public void Start()
    {
        float yOffset = gameObject.transform.position.y - gameObject.GetComponent<MeshCollider>().bounds.size[0];
        Material instanceOfCheckpointShader = new Material(shader);
        instanceOfCheckpointShader.SetFloat(h, yOffset);
        instanceOfCheckpointShader.SetFloat(holoHeight, gameObject.transform.localScale.y * 2);
        var render = gameObject.GetComponent<MeshRenderer>();
        render.material = instanceOfCheckpointShader;
        render.shadowCastingMode = ShadowCastingMode.Off;
    
    }

    public void PassCheckpoint(float time)
    {
        if (passed) return;
        if (raceController != null)
        {
            raceController.PassCheckpoint(checkpoint, time, finish); 
        }
        else
        {
            Debug.LogWarning("Checkpoint not connected to race controller");
            gameObject.SetActive(false);
        }
    }
}

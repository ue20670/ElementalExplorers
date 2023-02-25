using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

/**
 * determines which method to use for static batching
 */
public enum StaticBatchingMethod
{
    MeshCombineMeshes,
    StaticBatchingUtility
}

/**
 * A struct that contains a set of Transforms, a Mesh and a Material to be used for GPU instancing
 */
public struct GPUInstancedMeshSet
{
    public List<Matrix4x4> Transforms;
    public Mesh Mesh;
    public Material Material;
    public bool FrustumCulling;
    public bool OcclusionCulling;
    public bool IsIndirect;

    public GPUInstancedMeshSet(List<Matrix4x4> transforms, Mesh mesh, Material material, bool frustumCulling, bool occlusionCulling, bool isIndirect)
    {
        Transforms = transforms;
        Mesh = mesh;
        Material = material;
        FrustumCulling = frustumCulling;
        OcclusionCulling = occlusionCulling;
        IsIndirect = isIndirect;
    }

    public int GetPopulation()
    {
        return Transforms.Count;
    }
}

/**
 * A struct containing a list of meshes and transforms to be statically batched
 */
public struct StaticBatchingMeshSet
{
    private List<Mesh> _meshes;
    private List<Transform> _transforms;

    public StaticBatchingMeshSet(List<Mesh> meshes, List<Transform> transforms)
    {
        _meshes = meshes;
        _transforms = transforms;
    }

    public int GetPopulation()
    {
        return _meshes.Count;
    }
}


/**
 * Manager class that has methods for instancing and batching meshes, instancing operations are stored in this class,
 * batching operations return meshes that are handled by the caller
 */
public class OptimisationManager : MonoBehaviour
{
    // hashset of meshes to gpu instance each frame
    private HashSet<GPUInstancedMeshSet> _gpuInstancedMeshSets;
    
    /**
     * Main method called each frame responsible for actually instancing the meshes
     */
    private void Update()
    {
        // in each update instance the mesh sets in the hashset
        foreach (GPUInstancedMeshSet set in _gpuInstancedMeshSets)
        {
            // perform culling
            List<Matrix4x4> culledMatricies = new List<Matrix4x4>();
            if (set.FrustumCulling)
            {
                // TODO: frustum culling
            }

            if (set.OcclusionCulling)
            {
                // TODO: occlusion culling
            }

            // perform instancing
            if (set.IsIndirect)
            {
                // TODO: indirect instancing
            }
            else
            {
                // TODO: direct instancing
            }
        }
    }

    /**
     * Adds a new set of meshes to be GPU instanced each frame
     * Returns true or false depending on the success of the operation
     */
    public bool AddNewGPUInstancedMeshSet(GPUInstancedMeshSet set)
    {
        return _gpuInstancedMeshSets.Add(set);
    }

    /**
     * Removes a GPU instanced mesh set from the manager, returns true or false dependant on the
     * success of the operation
     */
    public bool RemoveGPUInstancedMeshSet(GPUInstancedMeshSet set)
    {
        return _gpuInstancedMeshSets.Remove(set);
    }

    /**
     * Takes a StaticBatchingMeshSet and returns a mesh that has been combined using the specified method
     */
    public Mesh StaticallyBatchSetOfMeshes(StaticBatchingMeshSet set, StaticBatchingMethod method)
    {
        switch (method)
        {
            case StaticBatchingMethod.MeshCombineMeshes:
                // TODO: implement this
                break;
            case StaticBatchingMethod.StaticBatchingUtility:
                // TODO: implement this
                break;
        }
        throw new NotImplementedException();
    }

    
}

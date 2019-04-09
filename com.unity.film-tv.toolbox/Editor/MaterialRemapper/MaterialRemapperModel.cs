using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityEditor.Formats.Alembic
{ 
    [Serializable]
    public class MaterialRemapperModel :  ScriptableObject
    {
        [SerializeField]
        public List<Material> materialList = new List<Material>();
        [SerializeField]
        public List<Transform> objectsToRemap = new List<Transform>();
        [SerializeField]
        List<MeshRenderer> meshToRemap = new List<MeshRenderer>();
        [SerializeField]
        public List<RemapList> remapList = new List<RemapList>();                // the remapper - stores mesh, material and selected index (popup index for the remapper)

        /// <summary>
        /// from our scene selection, prep the remapper, finding all of the mesh & materials that we'll want to remap
        /// </summary>
        /// <param name="objectList"></param>
        public void FindMeshesAndMaterials(List<Transform> objectList)
        {
            objectsToRemap.Clear();
            meshToRemap.Clear();
            objectsToRemap = objectList;
            foreach (var entry in objectList)
            {
                var meshList = entry.GetComponentsInChildren<MeshRenderer>();
                meshToRemap.AddRange(meshList.ToList());
            }

            var uniqueMeshList = new List<MeshRenderer>();
            remapList.Clear();

            materialList = meshToRemap.SelectMany(m => m.sharedMaterials).Distinct().ToList();
            
            // build our list of unique meshes
            foreach (var mesh in meshToRemap)
            {
                // if we don't already have the name in our list, then add it
                if (!uniqueMeshList.Contains(mesh))
                {
                    var newRemap = new RemapList()
                    {
                        mesh = mesh,
                        selectedIndex = new List<int>()
                    };
                    // prep our remapper
                    newRemap.selectedIndex.Clear();
                    foreach (var mat in mesh.sharedMaterials)
                    {
                        var idx = materialList.IndexOf(mat);
                        newRemap.selectedIndex.Add(idx);
                    }

                    // build the list of things that we are going to remap
                    remapList.Add(newRemap);
                    uniqueMeshList.Add(mesh);
                }
            }
            Debug.Log("MaterialRemapper:FindMeshesAndMaterials() - object count: " + objectList.Count + " Found : " + meshToRemap.Count + " meshes, " + uniqueMeshList.Count + " unique meshes");
        }

        
        /// <summary>
        /// reset everything
        /// </summary>
        public void ResetData()
        {
            materialList.Clear();
            objectsToRemap.Clear();
            meshToRemap.Clear();
            remapList.Clear();
        }
        
        
        public static Material GetDefaultMaterial()
        {
            Material defaultMat;
            var pipelineAsset = GraphicsSettings.renderPipelineAsset;
            if( pipelineAsset != null)
            {
#if UNITY_2019_1_OR_NEWER
                defaultMat = pipelineAsset.defaultMaterial;
#else
                defaultMat = pipelineAsset.GetDefaultMaterial();
#endif
            }
            else
            {
                defaultMat = new Material(Shader.Find("Standard"));
            }
            return defaultMat;
        }

        /// <summary>
        /// Do the remapping!
        /// </summary>
        public void ApplyMaterialChanges()
        {
            Debug.Log("MaterialRemapper:ApplyMaterialChanges() - " + meshToRemap.Count + " total meshes - " + remapList.Count + " unique meshes to remap to " + materialList.Count + " materials");
            // iterate through all of the meshes
            foreach (var mesh in meshToRemap)
            {
                var sourceMesh = mesh;

                // for each mesh, find the remap equiv
                var remap = remapList.FirstOrDefault(u => u.mesh.name == mesh.name);

                Debug.Log("mesh: " + sourceMesh.name + " found remap mesh: " + remap.mesh.name);

                // build our materials struct
                Debug.Log("preparing to remap materials for mesh: " + sourceMesh.name);
                var matList = new List<Material>();
                foreach( var matIdx in remap.selectedIndex)
                {
                    if( matIdx != -1)
                    {
                        var newMat = materialList[matIdx];
                        matList.Add(newMat);
                        Debug.Log("  - material: " + newMat.name);
                    }
                    else
                    {
                        Debug.Log("  - no material");
                    }   
                }

                if( matList.Count > 0)
                {
                    Undo.RegisterCompleteObjectUndo(sourceMesh.sharedMaterials.Cast<UnityEngine.Object>().ToArray(), "Assign Materials");
                    Debug.Log("Applying materials...");
                    sourceMesh.sharedMaterials = matList.ToArray();
                }
            }
        }

        /// <summary>
        /// the remapping list of mesh to materials
        /// </summary>
        [Serializable]
        public class RemapList
        {
            public MeshRenderer mesh;
            public List<int> selectedIndex = new List<int>();
        }
        
        /********************************
         *  utility functions
        ********************************/

        /// <summary>
        /// get count of mesh in our selection
        /// </summary>
        /// <returns></returns>
        public int GetMeshCount()
        {
            var meshCount = 0;
            foreach (var entry in objectsToRemap)
            {
                var count = entry.GetComponentsInChildren<MeshRenderer>().Length;
                meshCount += count;
            }
            return meshCount;
        }
        

        /// <summary>
        /// figure out the count of unique mesh in our list
        /// </summary>
        public List<MeshRenderer> GetUniqueMeshList()
        {
            var ret = new List<MeshRenderer>();

            foreach (var entry in objectsToRemap)
            {
                var meshList = entry.GetComponentsInChildren<MeshRenderer>();
                foreach (var mesh in meshList)
                {
                    if (!ret.FirstOrDefault(u => u.name == mesh.name))
                    {
                        ret.Add(mesh);
                    }
                }
            }
            return ret;
        }
    }
}


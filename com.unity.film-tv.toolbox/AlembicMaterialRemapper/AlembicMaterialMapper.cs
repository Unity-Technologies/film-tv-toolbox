using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Unity.FilmTV.Toolbox
{
    public class AlembicMaterialMapper : MonoBehaviour
    {
        public GameObject fbxLookDev;           // our master fbx that we look to for our source materials
      
        private List<Renderer> sourceMeshList = new List<Renderer>();
        private List<Renderer> destMeshList = new List<Renderer>();

        [ContextMenu("Sync Materials")]
        public void SyncMaterials()
        {
            if (!fbxLookDev)
            {
                Debug.Log("AlembicMaterialMapper - connect source fbx to your alembic to automap materials");
                return;
            }

            sourceMeshList = fbxLookDev.GetComponentsInChildren<Renderer>().ToList();
            destMeshList = gameObject.GetComponentsInChildren<Renderer>().ToList();
            
            // sync them
            foreach( var destMesh in destMeshList)
            {
                foreach( var sourceMesh in sourceMeshList )
                {
                    // alembic adds an empty parent node with the actual name we want, the mesh is contained underneath
                    if (sourceMesh.name == destMesh.transform.parent.name)
                    {
                        destMesh.sharedMaterials = sourceMesh.sharedMaterials;
                    }
                }
            }
        }
    }
}
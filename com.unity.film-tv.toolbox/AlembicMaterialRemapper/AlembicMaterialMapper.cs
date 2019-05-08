using System.Collections.Generic;
using UnityEngine;

namespace Unity.FilmTV.Toolbox
{
    public class AlembicMaterialMapper : MonoBehaviour
    {
        public GameObject fbxLookDev;           // our master fbx that we look to for our source materials

        private Renderer[] sourceMeshList;
        private Renderer[] destMeshList;

        [ContextMenu("Sync Materials")]
        public void SyncMaterials()
        {
            if (!fbxLookDev)
            {
                Debug.Log("AlembicMaterialMapper - connect source fbx to your alembic to automap materials");
                return;
            }

            sourceMeshList = fbxLookDev.GetComponentsInChildren<Renderer>();
            destMeshList = gameObject.GetComponentsInChildren<Renderer>();
            
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
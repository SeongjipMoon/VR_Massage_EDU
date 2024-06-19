using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.IO;

namespace LylekGames.Tools
{
    [CustomEditor(typeof(CombineMeshes))]
	public class CombineMeshesEditor : Editor
    {
		CombineMeshes myCombine;

        public void OnEnable()
        {
            myCombine = (CombineMeshes)target;
            myCombine.Start();
        }
        public override void OnInspectorGUI()
        {
			myCombine = (CombineMeshes)target;
			DrawDefaultInspector();
            if (myCombine.myMeshFilter.sharedMesh == null)
            {
                if (GUILayout.Button("NEW Combine Meshes"))
                    myCombine.CombineMultiMaterialMesh();
            }
            if (myCombine.myMeshFilter.sharedMesh != null)
            {
                if (GUILayout.Button("Save As Prefab (DESTRUCTIVE)"))
                    SaveMeshData(true);
                if (GUILayout.Button("Save As Prefab (Non-Destructive)"))
                    SaveMeshData(false);
                if (GUILayout.Button("Disable Meshes"))
                {
                    myCombine.DisableMesh();
                    EditorGUIUtility.ExitGUI();
                }
            }
        }
        public void SaveMeshData(bool destructive = false)
        {
            if(PrefabUtility.GetPrefabInstanceStatus(myCombine.gameObject) == PrefabInstanceStatus.Connected)
                PrefabUtility.UnpackPrefabInstance(myCombine.gameObject, PrefabUnpackMode.Completely, InteractionMode.AutomatedAction);
            if (!Directory.Exists(Application.dataPath + "/MeshCombiner/Prefabs/"))
                Directory.CreateDirectory(Application.dataPath + "/MeshCombiner/Prefabs/");
            if (!Directory.Exists(Application.dataPath + "/MeshCombiner/MeshData/"))
                Directory.CreateDirectory(Application.dataPath + "/MeshCombiner/MeshData/");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ///SAVE MESH AND CREATE PREFAB
            SaveMesh(myCombine.myMeshFilter, destructive);
        }
        public void SaveMesh(MeshFilter meshFilter, bool destructive = false)
        {
            string meshPath = "Assets/MeshCombiner/MeshData/" + meshFilter.gameObject.name + ".asset";

            string colliderPath = "Assets/MeshCombiner/MeshData/" + meshFilter.gameObject.name + "-Collider.asset";

            if (AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh))) { Debug.LogError("MeshData with this name already exists. Change the name of the active gameObject or delete the existing data (" + meshPath + ")"); return; }

            if (AssetDatabase.LoadAssetAtPath(colliderPath, typeof(Mesh))) { Debug.LogError("MeshData(Collider) with this name already exists. Change the name of the active gameObject or delete the existing data (" + colliderPath + ")"); return; }

            ///SAVE MESH DATA
            AssetDatabase.CreateAsset(meshFilter.sharedMesh, meshPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();

            ///LOAD REFERENCE TO OUR NEWLY SAVED MESH DATA
            Mesh meshData = (Mesh)AssetDatabase.LoadAssetAtPath(meshPath, typeof(Mesh));
            ///TEMP COLLIDER DATA REFERENCE
            Mesh colliderData = meshData;

            MeshCollider meshCollider = meshFilter.gameObject.GetComponent<MeshCollider>();
            if (meshCollider)
            {
                if (meshCollider.sharedMesh != meshFilter.sharedMesh)
                {
                    ///SAVE COLLIDER DATA
                    AssetDatabase.CreateAsset(meshCollider.sharedMesh, colliderPath);
                    AssetDatabase.SaveAssets();
                    AssetDatabase.Refresh();
                    ///LOAD REFERENCE TO OUR NEWLY SAVED COLLIDER DATA
                    colliderData = (Mesh)AssetDatabase.LoadAssetAtPath(colliderPath, typeof(Mesh));
                }
                ///Assign collider data
                meshCollider.sharedMesh = colliderData;
            }
            ///Assign mesh data
            meshFilter.sharedMesh = meshData;

            ///REMOVE OUR CHILD OBJECTS
            if (destructive)
            {
                int childCount = meshFilter.gameObject.transform.childCount;
                for (int i = 0; i < childCount; i++)
                    DestroyImmediate(meshFilter.transform.GetChild(0).gameObject);
            }
            ///------------------CREATE A PREFAB OF THIS GAMEOBJECT------------------///
            Vector3 tempPos = meshFilter.transform.localPosition;
            Quaternion tempRot = meshFilter.transform.localRotation;

            PrefabUtility.SaveAsPrefabAssetAndConnect(myCombine.gameObject, "Assets/MeshCombiner/Prefabs/" + meshFilter.gameObject.name + ".prefab", InteractionMode.AutomatedAction);

            ///REMOVE THE COMBINE SCRIPT FROM OUR PREFAB, WE WON'T BE NEEDING IT ANYMORE
            DestroyImmediate(myCombine);
            GameObject prefab = (GameObject)AssetDatabase.LoadAssetAtPath("Assets/MeshCombiner/Prefabs/" + meshFilter.gameObject.name + ".prefab", typeof(GameObject));
            if (prefab != null)
            {
                ///nullify prefab's position and rotation
                prefab.transform.position = Vector3.zero;
                prefab.transform.rotation = Quaternion.identity;

                if (prefab.GetComponent<CombineMeshes>())
                {
                    DestroyImmediate(prefab.GetComponent<CombineMeshes>(), true);
                }
            }

            ///reorient our active gameObject
            meshFilter.transform.localPosition = tempPos;
            meshFilter.transform.localRotation = tempRot;

            Debug.Log("Data saved successfully.");
        }
    }
}

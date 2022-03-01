using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[ExecuteAlways]
public class MeshPrinter : MonoBehaviour
{
    public string targetDir;
    public string specificMesh;
    [Header("Optional")]
    public Transform instantiatedParent;

    [ContextMenu("Print all meshes")]
    void PrintAllMeshes() {
        PrintAllMeshes(targetDir);
    }

    [ContextMenu("Find and print specific mesh")]
    void PrintSpecificMesh() {
        Object obj = AssetFinder.GetObjectFromAll(targetDir, specificMesh, typeof(Mesh));
        string result = obj ? obj.name : "NOT FOUND";
        print($"Specific mesh: {result}");
    }

    [ContextMenu("Find and instantiate specific mesh")]
    void InstantiateSpecificMesh() {
        Object obj = AssetFinder.GetObjectFromAll(targetDir, specificMesh, typeof(Mesh));

        if(!obj) {
            print($"Instantiate mesh: MESH NOT FOUND");
            return;
        }

        GameObject inst = new GameObject(obj.name, typeof(MeshRenderer));
        inst.AddComponent<MeshFilter>().mesh = (Mesh)obj;
        inst.transform.parent = instantiatedParent ? instantiatedParent : null;
    }

    void PrintAllMeshes(string path) {
        Object[] meshes = Resources.LoadAll(path, typeof(GameObject));

        foreach (var m in meshes) print(m);
    }
}

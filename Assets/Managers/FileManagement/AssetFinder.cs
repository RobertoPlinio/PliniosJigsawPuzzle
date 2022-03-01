using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public static class AssetFinder
{
    public static Object GetObject(string path, System.Type type) {
        return Resources.Load(path, type);
    }

    public static Object GetObjectFromAll(string path, string compareKey, System.Type type) {
        return Resources.LoadAll(path, type).First(x => x.name.Contains(compareKey));
    }
}

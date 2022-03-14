using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshUVOffsetFinder : MonoBehaviour
{
    [Range(0f,1f)]
    public float percentage = 0;
    public Color mainColor = Color.cyan;
    public Color offsetColor = Color.black;

    [ContextMenu("Apply texture on model")]
    void ApplyTexture() {
        MeshRenderer mr = GetComponent<MeshRenderer>();

        int defaultSize = 500;
        int size = (int)(defaultSize + defaultSize * percentage);

        Color[] offsetColors = new Color[size * size];
        Color[] mainColors = new Color[defaultSize * defaultSize];
        for (int i = 0; i < offsetColors.Length; i++) offsetColors[i] = offsetColor;
        for (int i = 0; i < mainColors.Length; i++) mainColors[i] = mainColor;

        Texture2D tex = new Texture2D(size, size); 

        tex.SetPixels(offsetColors);
        tex.SetPixels((size - defaultSize)/2, (size - defaultSize)/2, defaultSize, defaultSize, mainColors);
        tex.Apply();

        mr.sharedMaterial.mainTexture = tex;
    }
}

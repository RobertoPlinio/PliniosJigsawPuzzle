using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshTexture2DDebug : MonoBehaviour
{
    public Texture2D texture;
    public int x,y;

    public int xCoord, yCoord;

    MeshRenderer mr;
    private void Update() {
        if (!mr) mr = GetComponent<MeshRenderer>();

        mr.material.mainTexture = GetTextureBlock();
    }

    Texture2D GetTextureBlock() {
        xCoord = Mathf.Clamp(xCoord, 0, texture.width - x);
        yCoord = Mathf.Clamp(yCoord, 0, texture.height - y);

        Texture2D temp = new Texture2D(x, y);
        temp.SetPixels(texture.GetPixels(xCoord, yCoord, x, y));
        temp.Apply();
        return temp;
    }
}

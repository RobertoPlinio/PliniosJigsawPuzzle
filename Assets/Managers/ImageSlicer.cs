using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    public class ImageSlicer : MonoBehaviour
    {
        [Header("Debug options")]
        public bool showDebug = false;
        public Texture2D debug_image;

        int xAmount, yAmount;

        private void Start() {
            SliceOriginal();
        }

        public void Init(int xPieces, int yPieces) {
            xAmount = xPieces;
            yAmount = yPieces;
        }

        [ContextMenu("Slice")]
        public void SliceOriginal() {
            float xSize = debug_image.width / xAmount;
            float ySize = debug_image.height / yAmount;

            int pDiv = GetGCD(xAmount, yAmount);
            float pRatio = (float)xAmount / yAmount;
            int iDiv = GetGCD(debug_image.width, debug_image.height);
            float iRatio = (float)debug_image.width / debug_image.height;

            print($"Ratio of pieces is {xAmount / pDiv}:{yAmount / pDiv} or {pRatio}");
            print($"Ration of image is {debug_image.width / iDiv}:{debug_image.height / iDiv} or {iRatio}");

            bool equalAxis = Mathf.Approximately(pRatio, iRatio);
            bool xIsBigger = xAmount > yAmount;
            int fillW = debug_image.width;
            int fillH = debug_image.height;

            if (!equalAxis) {
                if (xIsBigger) {
                    fillW = (int)(debug_image.width * pRatio);
                    print($"Image needs filling on horizontal by {(fillW - debug_image.width) / 2} each side");
                } else {
                    pRatio = (float)yAmount / xAmount;
                    fillH = (int)(debug_image.height * pRatio);
                    print($"Image needs filling on vertical by {(fillH - debug_image.height) / 2} each side");
                }

                print($"Image size will be: {fillW},{fillH}");

                Color[] fillDefault = new Color[fillW * fillH];
                for (int i = 0; i < fillDefault.Length; i++) fillDefault[i] = Color.black;

                Texture2D tempImg = new Texture2D(fillW, fillH);
                tempImg.SetPixels(fillDefault);

                if (xIsBigger) {
                    print($"Filling at x: {(fillW - debug_image.width) / 2}");
                    tempImg.SetPixels((fillW - debug_image.width) / 2, 0, debug_image.width, debug_image.height, debug_image.GetPixels());
                } else {
                    print($"Filling at y: {(fillH - debug_image.height) / 2}");
                    tempImg.SetPixels(0, (fillH - debug_image.height) / 2, debug_image.width, debug_image.height, debug_image.GetPixels());
                }

                tempImg.Apply();
                AssetDatabase.CreateAsset(tempImg, "Assets/Piece/Result.asset");

            } else print("Image fits perfectly");

            print($"Piece size: {xSize}x{ySize}");

            //Texture2D tempImg = new Texture2D(width, height);
            //tempImg.SetPixels(original.GetPixels(0, 0, width, height));
            //tempImg.Apply();
        }

        int GetGCD(int a, int b) {
            while (a != b) {
                if (a < b) b = b - a;
                else a = a - b;
            }

            return a;
        }
    }
}
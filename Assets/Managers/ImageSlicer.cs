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
        public bool fillImage = true;

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
            Texture2D finalImage;

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
                    if (fillImage) {
                        fillW = (int)(debug_image.width * pRatio);
                        print($"Image needs filling on horizontal by {(fillW - debug_image.width) / 2} each side");
                    } else {
                        pRatio = (float)yAmount / xAmount;
                        fillH = (int)(debug_image.height * pRatio);
                        print($"Image will be cropped on vertical by {(debug_image.width - fillH) / 2} each side");
                    }
                } else {
                    if (fillImage) {
                        pRatio = (float)yAmount / xAmount;
                        fillH = (int)(debug_image.height * pRatio);
                        print($"Image needs filling on vertical by {(fillH - debug_image.height) / 2} each side");
                    } else {
                        fillW = (int)(debug_image.width * pRatio);
                        print($"Image will be cropped on horizontal by {(debug_image.width - fillW) / 2} each side");
                    }
                }

                print($"Image size will be: {fillW},{fillH}");      
                finalImage = fillImage? FillImage(debug_image, fillW, fillH, xIsBigger) : CropImage(debug_image, fillW, fillH, !xIsBigger);

            } else {
                print("Image fits perfectly");
                finalImage = debug_image;
            }

            float xSize = finalImage.width / xAmount;
            float ySize = finalImage.height / yAmount;
            print($"Piece size: {xSize}x{ySize}");
        }

        Texture2D FillImage(Texture2D original, int newWidth, int newHeight, bool fillOnX) {
            Color[] fillDefault = new Color[newWidth * newHeight];
            for (int i = 0; i < fillDefault.Length; i++) fillDefault[i] = Color.black;

            Texture2D tempImg = new Texture2D(newWidth, newHeight);
            tempImg.SetPixels(fillDefault);

            if (fillOnX) {
                print($"Filling at x: {(newWidth - original.width) / 2}");
                tempImg.SetPixels((newWidth - original.width) / 2, 0, original.width, original.height, original.GetPixels());
            } else {
                print($"Filling at y: {(newHeight - original.height) / 2}");
                tempImg.SetPixels(0, (newHeight - original.height) / 2, original.width, original.height, original.GetPixels());
            }

            tempImg.Apply();
            AssetDatabase.CreateAsset(tempImg, "Assets/Piece/Result.asset");
            return tempImg;
        }

        Texture2D CropImage(Texture2D original, int newWidth, int newHeight, bool cropOnX) {
            Texture2D tempImg = new Texture2D(newWidth, newHeight);
            
            if(cropOnX) {
                int wDiff = original.width - newWidth;
                tempImg.SetPixels(0, 0, newWidth, newHeight, original.GetPixels(wDiff/2, 0, newWidth, newHeight));
                print($"Cropping image at x: {wDiff/2}");
            }else {
                int hDiff = original.height - newHeight;
                tempImg.SetPixels(0, 0, newWidth, newHeight, original.GetPixels(0, hDiff / 2, newWidth, newHeight));
                print($"Cropping image at y: {hDiff / 2}");
            }

            tempImg.Apply();
            AssetDatabase.CreateAsset(tempImg, "Assets/Piece/Result.asset");
            return tempImg;
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
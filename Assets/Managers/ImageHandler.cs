using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    public class ImageHandler : MonoBehaviour
    {
        [Header("Debug options")]
        public bool debug = false;
        public Texture2D debug_image;
        public bool debug_fillImage = true;
        public bool debug_createNewImgAsset = false;

        int xAmount, yAmount;
        int pieceXsize, pieceYsize;

        public void Init(int xPieces, int yPieces) {
            xAmount = xPieces;
            yAmount = yPieces;
        }

        [ContextMenu("Prepare Image")]
        public void Debug_PrepareImg() => AdjustImageToBounds(debug_image, debug_fillImage);

        public Texture2D AdjustImageToBounds(Texture2D image, bool fillImg = false) {
            Texture2D finalImage;

            int pDiv = GetGCD(xAmount, yAmount);
            float pRatio = (float)xAmount / yAmount;
            int iDiv = GetGCD(image.width, image.height);
            float iRatio = (float)image.width / image.height;

            print($"Ratio of pieces is {xAmount / pDiv}:{yAmount / pDiv} or {pRatio}");
            print($"Ratio of image is {image.width / iDiv}:{image.height / iDiv} or {iRatio}");

            bool equalAxis = Mathf.Approximately(pRatio, iRatio);
            bool xIsBigger = xAmount > yAmount;
            int fillW = image.width;
            int fillH = image.height;

            if (!equalAxis) {
                if (xIsBigger) {
                    if (debug_fillImage) {
                        fillW = (int)(image.width * pRatio);
                        print($"[x] Image needs filling on horizontal by {(fillW - image.width) / 2} each side");
                    } else {
                        pRatio = (float)yAmount / xAmount;
                        fillH = (int)(image.height * pRatio);
                        print($"[x] Image will be cropped on vertical by {(image.width - fillH) / 2} each side");
                    }
                } else {
                    if (debug_fillImage) {
                        pRatio = (float)yAmount / xAmount;
                        fillH = (int)(image.height * pRatio);
                        print($"[y] Image needs filling on vertical by {(fillH - image.height) / 2} each side");
                    } else {
                        fillW = (int)(image.width * pRatio);
                        print($"[y] Image will be cropped on horizontal by {(image.width - fillW) / 2} each side");
                    }
                }

                int fiDiv = GetGCD(fillW, fillH);
                float fiRatio = (float)fillW / fillH;
                print($"Image size will be: {fillW},{fillH} with ratio {fillW / fiDiv}:{fillH / fiDiv} or {fiRatio}");
                finalImage = fillImg ? FillImage(image, fillW, fillH, xIsBigger) : CropImage(image, fillW, fillH, !xIsBigger);

                if (debug_createNewImgAsset) {
                    AssetDatabase.CreateAsset(finalImage, "Assets/Piece/Result.asset");
                }

            } else {
                print("Image fits perfectly");
                finalImage = image;
            }

            pieceXsize = finalImage.width / xAmount;
            pieceYsize = finalImage.height / yAmount;
            print($"Piece size: {pieceXsize}x{pieceYsize}");

            return finalImage;
        }

        public Texture2D[] SliceImage(Texture2D img, float uvOffset = 0f) => SliceImage(img, pieceXsize, pieceYsize, uvOffset);

        public Texture2D[] SliceImage(Texture2D img, int blockXSize, int blockYSize, float uvOffset = 0f) {
            int bxOffset = (int)(blockXSize * uvOffset);
            int byOffset = (int)(blockYSize * uvOffset);

            Texture2D tempImg = new Texture2D(img.width + bxOffset, img.height + byOffset);

            Color[] fillDefault = new Color[tempImg.width * tempImg.height];
            Color[] sliceDefault = new Color[(blockXSize + bxOffset) * (blockYSize + byOffset)];

            for (int i = 0; i < fillDefault.Length; i++) {
                fillDefault[i] = Color.black;

                if (i < sliceDefault.Length) sliceDefault[i] = Color.black;
            }

            tempImg.SetPixels(fillDefault);
            tempImg.SetPixels(bxOffset / 2, byOffset / 2, img.width, img.height, img.GetPixels());
            tempImg.Apply();
            AssetDatabase.CreateAsset(tempImg, "Assets/Piece/Offset.asset");
            Texture2D[] slices = new Texture2D[xAmount * yAmount];

            int index = 0;

            for (int i = 0; i < yAmount; i++) {
                for (int j = 0; j < xAmount; j++) {
                    Texture2D temp = new Texture2D(blockXSize + bxOffset, blockYSize + byOffset);

                    temp.SetPixels(tempImg.GetPixels(j * blockXSize, img.height -  (i+1) * blockYSize, blockXSize + bxOffset, blockYSize + byOffset));
                    temp.Apply();

                    slices[index] = temp;
                    index++;
                }
            }

            return slices;
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
            return tempImg;
        }

        Texture2D CropImage(Texture2D original, int newWidth, int newHeight, bool cropOnX) {
            Texture2D tempImg = new Texture2D(newWidth, newHeight);

            if (cropOnX) {
                int wDiff = original.width - newWidth;
                tempImg.SetPixels(0, 0, newWidth, newHeight, original.GetPixels(wDiff / 2, 0, newWidth, newHeight));
                print($"Cropping image at x: {wDiff / 2}");
            } else {
                int hDiff = original.height - newHeight;
                tempImg.SetPixels(0, 0, newWidth, newHeight, original.GetPixels(0, hDiff / 2, newWidth, newHeight));
                print($"Cropping image at y: {hDiff / 2}");
            }

            tempImg.Apply();
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
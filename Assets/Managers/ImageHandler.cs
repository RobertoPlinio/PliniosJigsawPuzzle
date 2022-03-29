using MyBox;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace JigsawPuzzle
{
    public class ImageHandler : MonoBehaviour
    {
        public enum AdjustmentMode { Stretch, Crop, Fill };

        [Header("Debug options")]
        public bool debug = false;
        [ConditionalField(nameof(debug))]  public Texture2D debug_image;
        [ConditionalField(nameof(debug))] public AdjustmentMode debug_adjustmentMode = AdjustmentMode.Stretch;
        [ConditionalField(nameof(debug))] public bool debug_createNewImgAsset = false;

        int xAmount, yAmount;
        int pieceXsize, pieceYsize;

        public void Init(int xPieces, int yPieces) {
            xAmount = xPieces;
            yAmount = yPieces;
        }

        [ContextMenu("Prepare Image")]
        public void Debug_PrepareImg() => AdjustImageToBounds(debug_image, debug_adjustmentMode);

        public Texture2D AdjustImageToBounds(Texture2D image, AdjustmentMode adjustMode = AdjustmentMode.Stretch) {
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

            if (equalAxis) {
                print("Image fits perfectly");
                finalImage = image;
            } else {
                switch (adjustMode) {
                    default:
                        print("Stretching image");
                        finalImage = new Texture2D(image.width, image.height);
                        finalImage.SetPixels(image.GetPixels());
                        finalImage.Apply();
                        break;
                    case AdjustmentMode.Crop:
                        print("Cropping image");
                        finalImage = CropImage(image, iRatio, pRatio);
                        break;
                    case AdjustmentMode.Fill:
                        print("Filling image");
                        finalImage = FillImage(image, iRatio, pRatio);
                        break;
                }
            }

            pieceXsize = finalImage.width / xAmount;
            pieceYsize = finalImage.height / yAmount;
            print($"Piece size: {pieceXsize}x{pieceYsize}");

            if (debug_createNewImgAsset && !equalAxis) {
                AssetDatabase.CreateAsset(finalImage, "Assets/Piece/Result.asset");
            }

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
            
            //AssetDatabase.CreateAsset(tempImg, "Assets/Piece/Offset.asset");
            
            Texture2D[] slices = new Texture2D[xAmount * yAmount];

            int index = 0;

            for (int i = 0; i < yAmount; i++) {
                for (int j = 0; j < xAmount; j++) {
                    Texture2D temp = new Texture2D(blockXSize + bxOffset, blockYSize + byOffset);

                    temp.SetPixels(tempImg.GetPixels(j * blockXSize, img.height - (i + 1) * blockYSize, blockXSize + bxOffset, blockYSize + byOffset));
                    temp.Apply();

                    slices[index] = temp;
                    index++;
                }
            }

            return slices;
        }

        Texture2D CropImage(Texture2D originalImg, float originalRatio, float targetRatio) {
            int newSize;
            Texture2D returnImg;

            if (originalRatio > targetRatio) {
                newSize = (int)(originalImg.height * targetRatio);
                returnImg = new Texture2D(newSize, originalImg.height);
                returnImg.SetPixels(originalImg.GetPixels((originalImg.width - newSize) / 2, 0, newSize, originalImg.height));
            } else {
                newSize = (int)(originalImg.width / targetRatio);
                returnImg = new Texture2D(originalImg.width, newSize);
                returnImg.SetPixels(originalImg.GetPixels(0, (originalImg.height - newSize) / 2, originalImg.width, newSize));
            }

            returnImg.Apply();
            return returnImg;
        }

        Texture2D FillImage(Texture2D originalImg, float originalRatio, float targetRatio) {
            int newSize, fillOffset;
            Texture2D returnImg;

            if (originalRatio > targetRatio) {
                newSize = (int)(originalImg.width / targetRatio);
                fillOffset = newSize - originalImg.height;
                returnImg = new Texture2D(originalImg.width, newSize);
                SetDefaultFill(ref returnImg);
                returnImg.SetPixels(0, fillOffset / 2, originalImg.width, originalImg.height, originalImg.GetPixels());
            } else {
                newSize = (int)(originalImg.height * targetRatio);
                fillOffset = newSize - originalImg.width;
                returnImg = new Texture2D(newSize, originalImg.height);
                SetDefaultFill(ref returnImg);
                returnImg.SetPixels(fillOffset / 2, 0, originalImg.width, originalImg.height, originalImg.GetPixels());
            }

            returnImg.Apply();
            return returnImg;

            void SetDefaultFill(ref Texture2D img) {
                Color[] colorArray = new Color[img.width * img.height];

                for (int i = 0; i < colorArray.Length; i++) {
                    colorArray[i] = Color.black;
                }

                img.SetPixels(colorArray);
                img.Apply();
            }
        }

        int GetGCD(int a, int b) {
            while (a != b) {
                if (a < b) b -= a;
                else a -= b;
            }

            return a;
        }
    }
}
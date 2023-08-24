using System.Collections;
using System.Collections.Generic;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.UI;

public class Piece : MonoBehaviour
{
    //[SerializeField] private PieceBase[] _pieceBases;
    [SerializeField] public database_piece data;
    [SerializeField] private piece_imagedata _pieceImage;

    [SerializeField] Image piece;
    [SerializeField] SVGImage top;
    [SerializeField] SVGImage topRight;
    [SerializeField] SVGImage right;
    [SerializeField] SVGImage bottomRight;
    [SerializeField] SVGImage bottom;
    [SerializeField] SVGImage bottomLeft;
    [SerializeField] SVGImage left;
    [SerializeField] SVGImage topLeft;

    [SerializeField] Sprite[] _icons;

    //public void Set(PieceBase pieceBase)
    //{
    //    piece.sprite = pieceBase.PieceIcon;
    //    top.sprite = pieceBase.TopIcon;
    //    topRight.sprite = pieceBase.TopRightIcon;
    //    right.sprite = pieceBase.RightIcon;
    //    bottomRight.sprite = pieceBase.BottomRightIcon;
    //    bottom.sprite = pieceBase.BottomIcon;
    //    bottomLeft.sprite = pieceBase.BottomLeftIcon;
    //    left.sprite = pieceBase.LeftIcon;
    //    topLeft.sprite = pieceBase.TopLeftIcon;

    //    if (pieceBase.Id == 7)
    //    {
    //        GameObject canvas = transform.Find("Canvas").gameObject;
    //        GameObject tr = canvas.transform.Find("TopRight").gameObject;
    //        tr.transform.localRotation = Quaternion.Euler(0, 0, 0);

    //        GameObject tl = canvas.transform.Find("TopLeft").gameObject;
    //        tl.transform.localRotation = Quaternion.Euler(0, 180, 0);
    //    }

    //    if (top.sprite == null)
    //    {
    //        top.color = new Color32(0, 0, 0, 0);
    //    }
    //    if (topRight.sprite == null)
    //    {
    //        topRight.color = new Color32(0, 0, 0, 0);
    //    }
    //    if (right.sprite == null)
    //    {
    //        right.color = new Color32(0, 0, 0, 0);
    //    }
    //    if (bottomRight.sprite == null)
    //    {
    //        bottomRight.color = new Color32(0, 0, 0, 0);
    //    }
    //    if (bottom.sprite == null)
    //    {
    //        bottom.color = new Color32(0, 0, 0, 0);
    //    }
    //    if (bottomLeft.sprite == null)
    //    {
    //        bottomLeft.color = new Color32(0, 0, 0, 0);
    //    }
    //    if (left.sprite == null)
    //    {
    //        left.color = new Color32(0, 0, 0, 0);
    //    }
    //    if (topLeft.sprite == null)
    //    {
    //        topLeft.color = new Color32(0, 0, 0, 0);
    //    }
    //}

    public void Set(int number)
    {
        top.color = new Color32(0, 0, 0, 255);
        topRight.color = new Color32(0, 0, 0, 255);
        right.color = new Color32(0, 0, 0, 255);
        bottomRight.color = new Color32(0, 0, 0, 255);
        bottom.color = new Color32(0, 0, 0, 255);
        bottomLeft.color = new Color32(0, 0, 0, 255);
        left.color = new Color32(0, 0, 0, 255);
        topLeft.color = new Color32(0, 0, 0, 255);

        piece.sprite = _pieceImage.piece_imageList[number];

        int pos0 = (int)data.list[number].Pos0;
        int pos1 = (int)data.list[number].Pos1;
        int pos2 = (int)data.list[number].Pos2;
        int pos3 = (int)data.list[number].Pos3;
        int pos4 = (int)data.list[number].Pos4;
        int pos5 = (int)data.list[number].Pos5;
        int pos6 = (int)data.list[number].Pos6;
        int pos7 = (int)data.list[number].Pos7;
        top.sprite = _icons[pos0];
        topRight.sprite = _icons[pos1];
        right.sprite = _icons[pos2];
        bottomRight.sprite = _icons[pos3];
        bottom.sprite = _icons[pos4];
        bottomLeft.sprite = _icons[pos5];
        left.sprite = _icons[pos6];
        topLeft.sprite = _icons[pos7];

        if (number == 7)
        {
            GameObject canvas = transform.Find("Canvas").gameObject;
            GameObject tr = canvas.transform.Find("TopRight").gameObject;
            tr.transform.localRotation = Quaternion.Euler(0, 0, 0);

            GameObject tl = canvas.transform.Find("TopLeft").gameObject;
            tl.transform.localRotation = Quaternion.Euler(0, 180, 0);
        }

        if (top.sprite == null)
        {
            top.color = new Color32(0, 0, 0, 0);
        }
        if (topRight.sprite == null)
        {
            topRight.color = new Color32(0, 0, 0, 0);
        }
        if (right.sprite == null)
        {
            right.color = new Color32(0, 0, 0, 0);
        }
        if (bottomRight.sprite == null)
        {
            bottomRight.color = new Color32(0, 0, 0, 0);
        }
        if (bottom.sprite == null)
        {
            bottom.color = new Color32(0, 0, 0, 0);
        }
        if (bottomLeft.sprite == null)
        {
            bottomLeft.color = new Color32(0, 0, 0, 0);
        }
        if (left.sprite == null)
        {
            left.color = new Color32(0, 0, 0, 0);
        }
        if (topLeft.sprite == null)
        {
            topLeft.color = new Color32(0, 0, 0, 0);
        }
    }

    //public void SetEvolution(PieceBase.PieceType pieceType)
    //{
    //    foreach (PieceBase pieceBase in _pieceBases)
    //    {
    //        if (pieceBase.Type == pieceType)
    //        {
    //            piece.sprite = pieceBase.PieceIcon;
    //            top.sprite = pieceBase.TopIcon;
    //            topRight.sprite = pieceBase.TopRightIcon;
    //            right.sprite = pieceBase.RightIcon;
    //            bottomRight.sprite = pieceBase.BottomRightIcon;
    //            bottom.sprite = pieceBase.BottomIcon;
    //            bottomLeft.sprite = pieceBase.BottomLeftIcon;
    //            left.sprite = pieceBase.LeftIcon;
    //            topLeft.sprite = pieceBase.TopLeftIcon;

    //            if (pieceBase.Id == 7)
    //            {
    //                GameObject canvas = transform.Find("Canvas").gameObject;
    //                GameObject tr = canvas.transform.Find("TopRight").gameObject;
    //                tr.transform.localRotation = Quaternion.Euler(0, 0, 0);

    //                GameObject tl = canvas.transform.Find("TopLeft").gameObject;
    //                tl.transform.localRotation = Quaternion.Euler(0, 180, 0);
    //            }

    //            if (top.sprite == null)
    //            {
    //                top.color = new Color32(0, 0, 0, 0);
    //            }
    //            if (topRight.sprite == null)
    //            {
    //                topRight.color = new Color32(0, 0, 0, 0);
    //            }
    //            if (right.sprite == null)
    //            {
    //                right.color = new Color32(0, 0, 0, 0);
    //            }
    //            if (bottomRight.sprite == null)
    //            {
    //                bottomRight.color = new Color32(0, 0, 0, 0);
    //            }
    //            if (bottom.sprite == null)
    //            {
    //                bottom.color = new Color32(0, 0, 0, 0);
    //            }
    //            if (bottomLeft.sprite == null)
    //            {
    //                bottomLeft.color = new Color32(0, 0, 0, 0);
    //            }
    //            if (left.sprite == null)
    //            {
    //                left.color = new Color32(0, 0, 0, 0);
    //            }
    //            if (topLeft.sprite == null)
    //            {
    //                topLeft.color = new Color32(0, 0, 0, 0);
    //            }

    //            if (top.sprite != null)
    //            {
    //                top.color = new Color32(0, 0, 0, 255);
    //            }
    //            if (topRight.sprite != null)
    //            {
    //                topRight.color = new Color32(0, 0, 0, 255);
    //            }
    //            if (right.sprite != null)
    //            {
    //                right.color = new Color32(0, 0, 0, 255);
    //            }
    //            if (bottomRight.sprite != null)
    //            {
    //                bottomRight.color = new Color32(0, 0, 0, 255);
    //            }
    //            if (bottom.sprite != null)
    //            {
    //                bottom.color = new Color32(0, 0, 0, 255);
    //            }
    //            if (bottomLeft.sprite != null)
    //            {
    //                bottomLeft.color = new Color32(0, 0, 0, 255);
    //            }
    //            if (left.sprite != null)
    //            {
    //                left.color = new Color32(0, 0, 0, 255);
    //            }
    //            if (topLeft.sprite != null)
    //            {
    //                topLeft.color = new Color32(0, 0, 0, 255);
    //            }
    //        }
    //    }
    //}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Effect3 : MonoBehaviour
{
    ///<summary>
    ///自分の持ちゴマを１つひみつゾーン１へ送る
    ///</summary>
    private Mouse _mouse;
    private FieldManager fieldManager;
    private CardManager _card;
    private GameObject _myPieceUI;
    private MyPieceListCard _myPieceListCard;

    // Start is called before the first frame update
    void Start()
    {
        _mouse = Mouse.current;
        fieldManager = FieldManager.instance;
        _card = CardManager.instance;
        fieldManager.OpenAnnounce(_card.explainData.list[4]);
        _myPieceUI = _card.myPieceUI;
    }

    // Update is called once per frame
    void Update()
    {
        if (_mouse.leftButton.wasPressedThisFrame)
        {
            //������O�Ȃ�I��
            if (fieldManager.capturePieces.Count == 0)
            {
                this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                fieldManager.trashPosition += 0.01f;
                Destroy(GetComponent<Effect3>());
                fieldManager.nextMode = FieldManager.Mode.Select;
            }
            else
            {
                int pieceCount = 0;
                foreach (PieceController obj in fieldManager.capturePieces)
                {
                    string tagToCheck = null;
                    if (fieldManager.nowPlayer == 0)
                    {
                        tagToCheck = "1P";
                    }
                    else if (fieldManager.nowPlayer == 1)
                    {
                        tagToCheck = "2P";
                    }

                    if (obj.gameObject.CompareTag(tagToCheck) == true)
                    {
                        pieceCount++;
                    }
                }

                if (pieceCount == 0)
                {
                    this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                    fieldManager.trashPosition += 0.01f;
                    Destroy(GetComponent<Effect3>());
                    fieldManager.nextMode = FieldManager.Mode.Select;
                }
                else
                {
                    _myPieceUI.SetActive(true);
                }
            }

            //if (fieldManager.nowPlayer == 0)
            //{
                //if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.transform.parent != null)
                //{
                //    MyPiece myPiece = hit.collider.gameObject.GetComponent<MyPiece>();
                //    if (myPiece != null)
                //    {
                //        _myPieceUI.SetActive(false);
                //        piece = fieldManager.capturePieces.Find(p => p.typeID == myPiece.typeID && p.playerNumber == fieldManager.nowPlayer);
                //        piece.SendToSecret(piece, 4);
                //        Destroy(GetComponent<Effect3>());
                //        fieldManager.nextMode = FieldManager.Mode.Select;
                //    }

                //    //if (hit.collider.gameObject.tag == "1P" || hit.collider.transform.parent.gameObject.tag == "1P")
                //    //{
                //    //    if (hit.collider.transform.parent.gameObject.tag == "1P")
                //    //    {
                //    //        piece = hit.collider.transform.parent.gameObject.GetComponent<PieceController>();
                //    //    }
                //    //    else
                //    //    {
                //    //        piece = hit.collider.gameObject.GetComponent<PieceController>();
                //    //    }

                //    //    if (piece.fieldStatus == PieceController.FieldStatus.Captured)
                //    //    {
                //    //        piece.SendToSecret(piece, 4);
                //    //        this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                //    //        fieldManager.trashPosition += 0.01f;
                //    //        Destroy(GetComponent<Effect3>());
                //    //        fieldManager.nextMode = FieldManager.Mode.Select;
                //    //    }
                //    //}
                //}
            //}
            //if (fieldManager.nowPlayer == 1)
            //{
                //if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.transform.parent != null)
                //{
                //    MyPiece myPiece = hit.collider.gameObject.GetComponent<MyPiece>();
                //    if (myPiece != null)
                //    {
                //        _myPieceUI.SetActive(false);
                //        piece = fieldManager.capturePieces.Find(p => p.typeID == myPiece.typeID && p.playerNumber == fieldManager.nowPlayer);
                //        piece.SendToSecret(piece, 5);
                //        Destroy(GetComponent<Effect3>());
                //        fieldManager.nextMode = FieldManager.Mode.Select;
                //    }

                //    //if (hit.collider.gameObject.tag == "2P" || hit.collider.transform.parent.gameObject.tag == "2P")
                //    //{
                //    //    if (hit.collider.transform.parent.gameObject.tag == "2P")
                //    //    {
                //    //        piece = hit.collider.transform.parent.gameObject.GetComponent<PieceController>();
                //    //    }
                //    //    else
                //    //    {
                //    //        piece = hit.collider.gameObject.GetComponent<PieceController>();
                //    //    }

                //    //    if (piece.fieldStatus == PieceController.FieldStatus.Captured)
                //    //    {
                //    //        piece.SendToSecret(piece, 5);
                //    //        this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                //    //        fieldManager.trashPosition += 0.01f;
                //    //        Destroy(GetComponent<Effect3>());
                //    //        fieldManager.nextMode = FieldManager.Mode.Select;
                //    //    }
                //    //}
                //}
            //}
        }
    }

    public void DoneEffect3(int myPiece)
    {
        PieceController piece = null;
        _myPieceListCard = MyPieceListCard.instance;
        _myPieceListCard.SetPieceNum();

        if (fieldManager.nowPlayer == 0)
        {
            _myPieceUI.SetActive(false);
            piece = fieldManager.capturePieces.Find(p => p.typeID == myPiece && p.playerNumber == fieldManager.nowPlayer);
            if (piece == null)
            {
                return;
            }
            piece.SendToSecret(piece, 4);
            Destroy(GetComponent<Effect3>());
            fieldManager.nextMode = FieldManager.Mode.Select;
        }
        if (fieldManager.nowPlayer == 1)
        {
            _myPieceUI.SetActive(false);
            piece = fieldManager.capturePieces.Find(p => p.typeID == myPiece && p.playerNumber == fieldManager.nowPlayer);
            if (piece == null)
            {
                return;
            }
            piece.SendToSecret(piece, 5);
            Destroy(GetComponent<Effect3>());
            fieldManager.nextMode = FieldManager.Mode.Select;
        }
    }
}

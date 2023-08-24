using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Effect5 : MonoBehaviour
{
    ///<summary>
    ///自分の持ちゴマを２つ、ひみつゾーン１と２に１つずつ送る
    ///</summary>
    
    private PieceController _piece;
    private Mouse _mouse;
    private CameraController _cameraController;
    private FieldManager fieldManager;
    private CardManager _card;
    //��I�����Ă邩�ǂ���(�Ⴄ�ꏊ�N���b�N����ƃ��Z�b�g�����)
    private bool _isSelectPiece;
    //1��2�̑I���t���O(�Е��I�񂾂�Е��I�ׂȂ�)
    private bool _isSelect1;
    private bool _isSelect2;

    private GameObject _myPieceUI;
    private MyPieceListCard _myPieceListCard;

    private string[] _announce = {"ひみつゾーン1（仮）に送る持ちゴマを　選択してください", "ひみつゾーン2（仮）に送る持ちゴマを　選択してください", "送り先の　ひみつゾーン（仮）を　選択してください"};

    // Start is called before the first frame update
    void Start()
    {
        _mouse = Mouse.current;
        _cameraController = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        _isSelectPiece = false;
        _isSelect1 = false;
        _isSelect2 = false;
        fieldManager = FieldManager.instance;
        _card = CardManager.instance;
        _myPieceUI = _card.myPieceUI;
        _myPieceListCard = MyPieceListCard.instance;
    }

    // Update is called once per frame
    void Update()
    {
        //2����������I��
        if (_isSelect1 == true && _isSelect2 == true)
        {
            _myPieceUI.SetActive(false);
            this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
            fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect5>());
            fieldManager.nextMode = FieldManager.Mode.Select;
        }

        if (_mouse.leftButton.wasPressedThisFrame)
        {
            //������O�Ȃ�I��
            if (fieldManager.capturePieces.Count == 0)
            {
                _myPieceUI.SetActive(false);
                this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                fieldManager.trashPosition += 0.01f;
                Destroy(GetComponent<Effect5>());
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
                    _myPieceUI.SetActive(false);
                    this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                    fieldManager.trashPosition += 0.01f;
                    //_cameraController.ResetZoom();
                    Destroy(GetComponent<Effect5>());
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
                //    if (hit.collider.gameObject.tag == "1P" || hit.collider.transform.parent.gameObject.tag == "1P")
                //    {
                //        if (hit.collider.transform.parent.gameObject.tag == "1P")
                //        {
                //            _piece = hit.collider.transform.parent.gameObject.GetComponent<PieceController>();
                //            if (_piece.fieldStatus == PieceController.FieldStatus.Captured)
                //            {
                //                _cameraController.ZoomInSecretZone(0);
                //                _isSelectPiece = true;
                //            }
                //            else
                //            {
                //                _cameraController.ResetZoom();
                //                _isSelectPiece = false;
                //            }
                //        }
                //        else
                //        {
                //            _piece = hit.collider.gameObject.GetComponent<PieceController>();
                //            if (_piece.fieldStatus == PieceController.FieldStatus.Captured)
                //            {
                //                _cameraController.ZoomInSecretZone(0);
                //                _isSelectPiece = true;
                //            }
                //            else
                //            {
                //                _cameraController.ResetZoom();
                //                _isSelectPiece = false;
                //            }
                //        }
                //    }
                //}
                
                //if (_isSelectPiece == true)
                //{
                //    if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "Secret1_1P" || hit.collider.transform.parent.gameObject.tag == "Secret1_1P") && _isSelect1 == false)
                //    {
                //        _piece.SendToSecret(_piece, 4);
                //        _isSelectPiece = false;
                //        _isSelect1 = true;
                //        _cameraController.ResetZoom();
                //    }
                //    if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "Secret2_1P" || hit.collider.transform.parent.gameObject.tag == "Secret2_1P") && _isSelect2 == false)
                //    {
                //        _piece.SendToSecret(_piece, 6);
                //        _isSelectPiece = false;
                //        _isSelect2 = true;
                //        _cameraController.ResetZoom();
                //    }
                //}
            //}
            //if (fieldManager.nowPlayer == 1)
            //{
                //if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.transform.parent != null)
                //{
                //    if (hit.collider.gameObject.tag == "2P" || hit.collider.transform.parent.gameObject.tag == "2P")
                //    {
                //        if (hit.collider.transform.parent.gameObject.tag == "2P")
                //        {
                //            _piece = hit.collider.transform.parent.gameObject.GetComponent<PieceController>();
                //            if (_piece.fieldStatus == PieceController.FieldStatus.Captured)
                //            {
                //                _cameraController.ZoomInSecretZone(1);
                //                _isSelectPiece = true;
                //            }
                //            else
                //            {
                //                _cameraController.ResetZoom();
                //                _isSelectPiece = false;
                //            }
                //        }
                //        else
                //        {
                //            _piece = hit.collider.gameObject.GetComponent<PieceController>();
                //            if (_piece.fieldStatus == PieceController.FieldStatus.Captured)
                //            {
                //                _cameraController.ZoomInSecretZone(1);
                //                _isSelectPiece = true;
                //            }
                //            else
                //            {
                //                _cameraController.ResetZoom();
                //                _isSelectPiece = false;
                //            }
                //        }
                //    }
                //}

                //if (_isSelectPiece == true)
                //{
                //    if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "Secret1_2P" || hit.collider.transform.parent.gameObject.tag == "Secret1_2P") && _isSelect1 == false)
                //    {
                //        _piece.SendToSecret(_piece, 5);
                //        _isSelectPiece = false;
                //        _isSelect1 = true;
                //        _cameraController.ResetZoom();
                //    }
                //    if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "Secret2_2P" || hit.collider.transform.parent.gameObject.tag == "Secret2_2P") && _isSelect2 == false)
                //    {
                //        _piece.SendToSecret(_piece, 7);
                //        _isSelectPiece = false;
                //        _isSelect2 = true;
                //        _cameraController.ResetZoom();
                //    }
                //}
            //}
        }
    }

    public void DoneEffect5(int myPiece)
    {
        PieceController piece = null;
        _myPieceListCard = MyPieceListCard.instance;
        _myPieceListCard.SetPieceNum();

        if (fieldManager.nowPlayer == 0)
        {
            if (_isSelect1 == false)
            {
                piece = fieldManager.capturePieces.Find(p => p.typeID == myPiece && p.playerNumber == fieldManager.nowPlayer);
                if (piece == null)
                {
                    return;
                }
                piece.SendToSecret(piece, 4);
                _isSelect1 = true;
            }
            else if (_isSelect1 == true)
            {
                piece = fieldManager.capturePieces.Find(p => p.typeID == myPiece && p.playerNumber == fieldManager.nowPlayer);
                if (piece == null)
                {
                    return;
                }
                piece.SendToSecret(piece, 6);
                _isSelect2 = true;
            }
        }
        if (fieldManager.nowPlayer == 1)
        {
            if (_isSelect1 == false)
            {
                piece = fieldManager.capturePieces.Find(p => p.typeID == myPiece && p.playerNumber == fieldManager.nowPlayer);
                if (piece == null)
                {
                    return;
                }
                piece.SendToSecret(piece, 5);
                _isSelect1 = true;
            }
            else if (_isSelect1 == true)
            {
                piece = fieldManager.capturePieces.Find(p => p.typeID == myPiece && p.playerNumber == fieldManager.nowPlayer);
                if (piece == null)
                {
                    return;
                }
                piece.SendToSecret(piece, 7);
                _isSelect2 = true;
            }
        }
    }
}

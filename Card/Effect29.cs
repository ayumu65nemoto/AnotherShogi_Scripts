using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Effect29 : MonoBehaviour
{
    ///<summary>
    ///ひみつゾーンにある自分のコマ１つを持ちゴマにする
    ///</summary>

    private string _announce = "持ちゴマにするコマを　選択してください";
    
    private FieldManager _fieldManager;
    private Mouse _mouse;
    //�閧�]�[���̋����̂��m�F
    private int _count = 0;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;
        _mouse = Mouse.current;
        _fieldManager.OpenAnnounce(_announce);

        if (_fieldManager.nowPlayer == 0)
        {
            GameObject[] pieceObject = GameObject.FindGameObjectsWithTag("1P");
            foreach (GameObject obj in pieceObject)
            {
                PieceController piece = obj.GetComponent<PieceController>();
                if (piece != null)
                {
                    if (piece.fieldStatus == PieceController.FieldStatus.Secret)
                    {
                        _count++;
                    }
                }
            }
        }

        if (_fieldManager.nowPlayer == 1)
        {
            GameObject[] pieceObject = GameObject.FindGameObjectsWithTag("2P");
            foreach (GameObject obj in pieceObject)
            {
                PieceController piece = obj.GetComponent<PieceController>();
                if (piece != null)
                {
                    if (piece.fieldStatus == PieceController.FieldStatus.Secret)
                    {
                        _count++;
                    }
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //�閧�]�[���̋���Ȃ���΃X���[
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay && _count == 0)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect29>());
            _fieldManager.nextMode = FieldManager.Mode.Select;
        }

        if (_mouse.leftButton.wasPressedThisFrame)
        {
            PieceController piece = null;
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit = new RaycastHit();

            if (_fieldManager.nowPlayer == 0)
            {
                if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.transform.parent != null)
                {
                    if (hit.collider.gameObject.tag == "1P" || hit.collider.transform.parent.gameObject.tag == "1P")
                    {
                        if (hit.collider.transform.parent.gameObject.tag == "1P")
                        {
                            piece = hit.collider.transform.parent.gameObject.GetComponent<PieceController>();
                        }
                        else
                        {
                            piece = hit.collider.gameObject.GetComponent<PieceController>();
                        }

                        if (piece.fieldStatus == PieceController.FieldStatus.Secret)
                        {
                            piece.Capture(0);
                            _fieldManager.capturePieces.Add(piece);
                            _fieldManager.AlignCapturePieces(0);
                            piece.DeleteSecret1Piece1(piece.gameObject);
                            piece.DeleteSecret2Piece1(piece.gameObject);

                            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
                            _fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect29>());
                            _fieldManager.nextMode = FieldManager.Mode.Select;
                        }
                    }
                }
            }
            if (_fieldManager.nowPlayer == 1)
            {
                if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.transform.parent != null)
                {
                    if (hit.collider.gameObject.tag == "2P" || hit.collider.transform.parent.gameObject.tag == "2P")
                    {
                        if (hit.collider.transform.parent.gameObject.tag == "2P")
                        {
                            piece = hit.collider.transform.parent.gameObject.GetComponent<PieceController>();
                        }
                        else
                        {
                            piece = hit.collider.gameObject.GetComponent<PieceController>();
                        }

                        if (piece.fieldStatus == PieceController.FieldStatus.Secret)
                        {
                            piece.Capture(1);
                            _fieldManager.capturePieces.Add(piece);
                            _fieldManager.AlignCapturePieces(1);
                            piece.DeleteSecret1Piece2(piece.gameObject);
                            piece.DeleteSecret2Piece2(piece.gameObject);

                            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
                            _fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect29>());
                            _fieldManager.nextMode = FieldManager.Mode.Select;
                        }
                    }
                }
            }
        }
    }
}

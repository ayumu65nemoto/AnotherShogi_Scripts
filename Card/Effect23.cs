using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Effect23 : MonoBehaviour
{
    ///<summary>
    ///自分のコマ１つの状態異常を回復させる
    ///</summary>

    private string _announce = "異常状態から回復させるコマを　選択してください";
    
    private FieldManager _fieldManager;
    private Mouse _mouse;
    //��Ԉُ�̋����̂��m�F
    private int _count = 0;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;
        _mouse = Mouse.current;
        _fieldManager.OpenAnnounce(_announce);


        if (_fieldManager.nowPlayer == 0)
        {
            List<PieceController> pieces = _fieldManager.GetPieces(0);
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].pieceAbnormal == PieceController.PieceAbnormal.Sleep)
                {
                    _count++;
                }
            }
        }

        if (_fieldManager.nowPlayer == 1)
        {
            List<PieceController> pieces = _fieldManager.GetPieces(1);
            for (int i = 0; i < pieces.Count; i++)
            {
                if (pieces[i].pieceAbnormal == PieceController.PieceAbnormal.Sleep)
                {
                    _count++;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        //��Ԉُ�̋���Ȃ���΃X���[
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay && _count == 0)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect23>());
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

                        if (piece.fieldStatus == PieceController.FieldStatus.OnBoard && piece.pieceAbnormal == PieceController.PieceAbnormal.Sleep)
                        {
                            //��Ԉُ��
                            piece.pieceAbnormal = PieceController.PieceAbnormal.None;
                            piece.gameObject.GetComponent<Renderer>().material = _fieldManager.originMaterial;
                            piece.sleepCount = 0;

                            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
                            _fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect23>());
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

                        if (piece.fieldStatus == PieceController.FieldStatus.OnBoard && piece.pieceAbnormal == PieceController.PieceAbnormal.Sleep)
                        {
                            piece.pieceAbnormal = PieceController.PieceAbnormal.None;
                            piece.gameObject.GetComponent<Renderer>().material = _fieldManager.originMaterial;
                            piece.sleepCount = 0;

                            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
                            _fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect23>());
                            _fieldManager.nextMode = FieldManager.Mode.Select;
                        }
                    }
                }
            }
        }
    }
}

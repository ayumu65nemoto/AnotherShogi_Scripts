using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Effect21 : MonoBehaviour
{
    ///<summary>
    ///相手のコマ１つを２ターンの間　眠らす
    ///</summary>

    private string _announce = "2ターンの間眠らす　相手のコマを　選択してください";
    
    private FieldManager _fieldManager;
    private Mouse _mouse;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;
        _mouse = Mouse.current;
        _fieldManager.OpenAnnounce(_announce);
    }

    // Update is called once per frame
    void Update()
    {
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

                        if (piece.fieldStatus == PieceController.FieldStatus.OnBoard)
                        {
                            //2�^�[�����炷
                            piece.pieceAbnormal = PieceController.PieceAbnormal.Sleep;
                            piece.gameObject.GetComponent<Renderer>().material = _fieldManager.sleepMaterial;
                            piece.sleepCount = 3;

                            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
                            _fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect21>());
                            _fieldManager.nextMode = FieldManager.Mode.Select;
                        }
                    }
                }
            }
            if (_fieldManager.nowPlayer == 1)
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

                        if (piece.fieldStatus == PieceController.FieldStatus.OnBoard)
                        {
                            piece.pieceAbnormal = PieceController.PieceAbnormal.Sleep;
                            piece.gameObject.GetComponent<Renderer>().material = _fieldManager.sleepMaterial;
                            piece.sleepCount = 3;

                            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
                            _fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect21>());
                            _fieldManager.nextMode = FieldManager.Mode.Select;
                        }
                    }
                }
            }
        }
    }
}

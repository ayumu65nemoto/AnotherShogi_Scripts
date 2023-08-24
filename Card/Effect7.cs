using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Effect7 : MonoBehaviour
{
    ///<summary>
    ///相手の持ちゴマを1つ、ひみつゾーン１か２に送る
    ///</summary>

    private PieceController _piece;
    private Mouse _mouse;
    private CameraController _cameraController;
    private FieldManager fieldManager;
    //��I�����Ă邩�ǂ���
    private bool _isSelectPiece;

    private string _announce_0 = "ひみつゾーン（仮）に送る持ちゴマを　選択してください";
    private string _announce_1 = "送り先の　ひみつゾーン（仮）を　選択してください";

    // Start is called before the first frame update
    void Start()
    {
        _mouse = Mouse.current;
        _cameraController = GameObject.FindWithTag("MainCamera").GetComponent<CameraController>();
        _isSelectPiece = false;
        fieldManager = FieldManager.instance;
        fieldManager.OpenAnnounce(_announce_0);
    }

    // Update is called once per frame
    void Update()
    {
        if (_mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit = new RaycastHit();

            if (fieldManager.nowPlayer == 1)
            {
                if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.transform.parent != null)
                {
                    if (hit.collider.gameObject.tag == "1P" || hit.collider.transform.parent.gameObject.tag == "1P")
                    {
                        if (hit.collider.transform.parent.gameObject.tag == "1P")
                        {
                            _piece = hit.collider.transform.parent.gameObject.GetComponent<PieceController>();
                            if (_piece.fieldStatus == PieceController.FieldStatus.OnBoard)
                            {
                                _cameraController.ZoomInSecretZone(0);
                                _isSelectPiece = true;
                                fieldManager.OpenAnnounce(_announce_1);
                            }
                            else
                            {
                                _cameraController.ResetZoom();
                                _isSelectPiece = false;
                                fieldManager.OpenAnnounce(_announce_0);
                            }
                        }
                        else
                        {
                            _piece = hit.collider.gameObject.GetComponent<PieceController>();
                            if (_piece.fieldStatus == PieceController.FieldStatus.OnBoard)
                            {
                                _cameraController.ZoomInSecretZone(0);
                                _isSelectPiece = true;
                                fieldManager.OpenAnnounce(_announce_1);
                            }
                            else
                            {
                                _cameraController.ResetZoom();
                                _isSelectPiece = false;
                                fieldManager.OpenAnnounce(_announce_0);
                            }
                        }
                    }
                }

                if (_isSelectPiece == true)
                {
                    if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "Secret1_1P" || hit.collider.transform.parent.gameObject.tag == "Secret1_1P"))
                    {
                        _piece.SendToSecret(_piece, 0);
                        _isSelectPiece = false;
                        _cameraController.ResetZoom();
                        this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                        fieldManager.trashPosition += 0.01f;
                        Destroy(GetComponent<Effect7>());
                        fieldManager.nextMode = FieldManager.Mode.Select;
                    }
                    else if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "Secret2_1P" || hit.collider.transform.parent.gameObject.tag == "Secret2_1P"))
                    {
                        _piece.SendToSecret(_piece, 2);
                        _isSelectPiece = false;
                        _cameraController.ResetZoom();
                        this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                        fieldManager.trashPosition += 0.01f;
                        Destroy(GetComponent<Effect7>());
                        fieldManager.nextMode = FieldManager.Mode.Select;
                    }
                }
            }
            if (fieldManager.nowPlayer == 0)
            {
                if (Physics.Raycast(ray, out hit) && hit.collider != null && hit.collider.transform.parent != null)
                {
                    if (hit.collider.gameObject.tag == "2P" || hit.collider.transform.parent.gameObject.tag == "2P")
                    {
                        if (hit.collider.transform.parent.gameObject.tag == "2P")
                        {
                            _piece = hit.collider.transform.parent.gameObject.GetComponent<PieceController>();
                            if (_piece.fieldStatus == PieceController.FieldStatus.OnBoard)
                            {
                                _cameraController.ZoomInSecretZone(1);
                                _isSelectPiece = true;
                            }
                            else
                            {
                                _cameraController.ResetZoom();
                                _isSelectPiece = false;
                            }
                        }
                        else
                        {
                            _piece = hit.collider.gameObject.GetComponent<PieceController>();
                            if (_piece.fieldStatus == PieceController.FieldStatus.OnBoard)
                            {
                                _cameraController.ZoomInSecretZone(1);
                                _isSelectPiece = true;
                            }
                            else
                            {
                                _cameraController.ResetZoom();
                                _isSelectPiece = false;
                            }
                        }
                    }
                }

                if (_isSelectPiece == true)
                {
                    if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "Secret1_2P" || hit.collider.transform.parent.gameObject.tag == "Secret1_2P"))
                    {
                        _piece.SendToSecret(_piece, 1);
                        _isSelectPiece = false;
                        _cameraController.ResetZoom();
                        this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                        fieldManager.trashPosition += 0.01f;
                        Destroy(GetComponent<Effect7>());
                        fieldManager.nextMode = FieldManager.Mode.Select;
                    }
                    else if (Physics.Raycast(ray, out hit) && (hit.collider.gameObject.tag == "Secret2_2P" || hit.collider.transform.parent.gameObject.tag == "Secret2_2P"))
                    {
                        _piece.SendToSecret(_piece, 3);
                        _isSelectPiece = false;
                        _cameraController.ResetZoom();
                        this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                        fieldManager.trashPosition += 0.01f;
                        Destroy(GetComponent<Effect7>());
                        fieldManager.nextMode = FieldManager.Mode.Select;
                    }
                }
            }
        }
    }
}

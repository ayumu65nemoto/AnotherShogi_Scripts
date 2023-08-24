using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Effect8 : MonoBehaviour
{
    ///<summary>
    ///ひみつゾーンにある自分のコマ１つをゾーンの出口に移動させる
    ///</summary>

    private Mouse _mouse;
    private FieldManager fieldManager;
    private string _announce = "脱出させるコマを　選択してください";

    // Start is called before the first frame update
    void Start()
    {
        _mouse = Mouse.current;
        fieldManager = FieldManager.instance;
        fieldManager.OpenAnnounce(_announce);
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

            if (fieldManager.nowPlayer == 0)
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
                            piece.SendToSecret(piece, 8);
                            this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                            fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect8>());
                            fieldManager.nextMode = FieldManager.Mode.Select;
                        }
                    }
                }
            }
            if (fieldManager.nowPlayer == 1)
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
                            piece.SendToSecret(piece, 9);
                            this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                            fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect8>());
                            fieldManager.nextMode = FieldManager.Mode.Select;
                        }
                    }
                }
            }
        }
    }
}

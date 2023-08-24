using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Effect2 : MonoBehaviour
{
    ///<summary>
    ///自分のコマ１つをひみつゾーン２へ移動させる
    ///</summary>
    private Mouse _mouse;
    private FieldManager fieldManager;
    private CardManager _card;

    private void Start()
    {
        _mouse = Mouse.current;
        fieldManager = FieldManager.instance;
        _card = CardManager.instance;
        fieldManager.OpenAnnounce(_card.explainData.list[3]);
    }

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

                        if (piece.fieldStatus == PieceController.FieldStatus.OnBoard)
                        {
                            piece.SendToSecret(piece, 2);
                            this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                            fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect2>());
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

                        if (piece.fieldStatus == PieceController.FieldStatus.OnBoard)
                        {
                            piece.SendToSecret(piece, 3);
                            this.gameObject.transform.position = new Vector3(7f, (0f + fieldManager.trashPosition), 0);
                            fieldManager.trashPosition += 0.01f;
                            Destroy(GetComponent<Effect2>());
                            fieldManager.nextMode = FieldManager.Mode.Select;
                        }
                    }
                }
            }
        }
    }
}

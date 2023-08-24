using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecretZone : MonoBehaviour
{
    [SerializeField] private GameObject _nextTile;
    public List<GameObject> secretPieceObject = new List<GameObject>();
    private FieldManager _fieldManager;

    void Start(){
        _fieldManager = FieldManager.instance;
    }

    private void Update()
    {
        //�����o��
        if (secretPieceObject.Count > 1)
        {
            if (_nextTile == null)
            {
                if (this.gameObject.tag == "Exit1_1P")
                {
                    Vector2Int tileIndex = new Vector2Int(7, 5);
                    secretPieceObject[0].GetComponent<PieceController>().fieldStatus = PieceController.FieldStatus.OnBoard;
                    _fieldManager.CapturePiece(_fieldManager.nowPlayer, tileIndex);
                    secretPieceObject[0].transform.position = new Vector3(3, 1, 2);
                    secretPieceObject[0].GetComponent<PieceController>().posIndex = tileIndex;
                    //����C���f�b�N�X�ɖ߂�
                    _fieldManager.AddPiece(secretPieceObject[0].GetComponent<PieceController>(), tileIndex);
                    secretPieceObject.RemoveAt(0);
                    //������̕\�����X�V
                    _fieldManager.AlignCapturePieces(_fieldManager.nowPlayer);
                }
                if (this.gameObject.tag == "Exit2_1P")
                {
                    Vector2Int tileIndex = new Vector2Int(7, 6);
                    secretPieceObject[0].GetComponent<PieceController>().fieldStatus = PieceController.FieldStatus.OnBoard;
                    _fieldManager.CapturePiece(_fieldManager.nowPlayer, tileIndex);
                    secretPieceObject[0].transform.position = new Vector3(3, 1, 3);
                    secretPieceObject[0].GetComponent<PieceController>().posIndex = tileIndex;
                    //����C���f�b�N�X�ɖ߂�
                    _fieldManager.AddPiece(secretPieceObject[0].GetComponent<PieceController>(), tileIndex);
                    secretPieceObject.RemoveAt(0);
                    //������̕\�����X�V
                    _fieldManager.AlignCapturePieces(_fieldManager.nowPlayer);
                }
                if (this.gameObject.tag == "Exit1_2P")
                {
                    Vector2Int tileIndex = new Vector2Int(0, 1);
                    secretPieceObject[0].GetComponent<PieceController>().fieldStatus = PieceController.FieldStatus.OnBoard;
                    _fieldManager.CapturePiece(_fieldManager.nowPlayer, tileIndex);
                    secretPieceObject[0].transform.position = new Vector3(-4, 1, -2);
                    secretPieceObject[0].GetComponent<PieceController>().posIndex = tileIndex;
                    //����C���f�b�N�X�ɖ߂�
                    _fieldManager.AddPiece(secretPieceObject[0].GetComponent<PieceController>(), tileIndex);
                    secretPieceObject.RemoveAt(0);
                    //������̕\�����X�V
                    _fieldManager.AlignCapturePieces(_fieldManager.nowPlayer);
                }
                if (this.gameObject.tag == "Exit1_2P")
                {
                    Vector2Int tileIndex = new Vector2Int(7, 5);
                    secretPieceObject[0].GetComponent<PieceController>().fieldStatus = PieceController.FieldStatus.OnBoard;
                    _fieldManager.CapturePiece(_fieldManager.nowPlayer, tileIndex);
                    secretPieceObject[0].transform.position = new Vector3(-4, 1, -3);
                    secretPieceObject[0].GetComponent<PieceController>().posIndex = tileIndex;
                    //����C���f�b�N�X�ɖ߂�
                    _fieldManager.AddPiece(secretPieceObject[0].GetComponent<PieceController>(), tileIndex);
                    secretPieceObject.RemoveAt(0);
                    //������̕\�����X�V
                    _fieldManager.AlignCapturePieces(_fieldManager.nowPlayer);
                }
            }
            else
            {
                secretPieceObject[0].transform.position = _nextTile.transform.position + new Vector3(0, 1, 0);
                _nextTile.GetComponent<SecretZone>().secretPieceObject.Add(secretPieceObject[0]);
                secretPieceObject.RemoveAt(0);
            }
        }

        if (this.gameObject.tag == "EvolutionZone")
        {
            if (secretPieceObject != null && secretPieceObject.Count > 0 && secretPieceObject[0].GetComponent<PieceController>().isEvolution == false)
            {
                secretPieceObject[0].GetComponent<PieceController>().Evolution(true);
            }
        }
    }
    public void SecretMove(int player)
    {
        if (secretPieceObject != null && secretPieceObject.Count > 0)
        {
            if (player == 0)
            {
                if (secretPieceObject[0].tag == "1P")
                {
                    if (_nextTile == null)
                    {
                        if (this.gameObject.tag == "Exit1_1P")
                        {
                            Vector2Int tileIndex = new Vector2Int(7, 5);
                            secretPieceObject[0].GetComponent<PieceController>().fieldStatus = PieceController.FieldStatus.OnBoard;
                            _fieldManager.CapturePiece(_fieldManager.nowPlayer, tileIndex);
                            secretPieceObject[0].transform.position = new Vector3(3, 1, 2);
                            secretPieceObject[0].GetComponent<PieceController>().posIndex = tileIndex;
                            //����C���f�b�N�X�ɖ߂�
                            _fieldManager.AddPiece(secretPieceObject[0].GetComponent<PieceController>(), tileIndex);
                            secretPieceObject.RemoveAt(0);
                            //������̕\�����X�V
                            _fieldManager.AlignCapturePieces(_fieldManager.nowPlayer);
                        }
                        if (this.gameObject.tag == "Exit2_1P")
                        {
                            Vector2Int tileIndex = new Vector2Int(7, 6);
                            secretPieceObject[0].GetComponent<PieceController>().fieldStatus = PieceController.FieldStatus.OnBoard;
                            _fieldManager.CapturePiece(_fieldManager.nowPlayer, tileIndex);
                            secretPieceObject[0].transform.position = new Vector3(3, 1, 3);
                            secretPieceObject[0].GetComponent<PieceController>().posIndex = tileIndex;
                            //����C���f�b�N�X�ɖ߂�
                            _fieldManager.AddPiece(secretPieceObject[0].GetComponent<PieceController>(), tileIndex);
                            secretPieceObject.RemoveAt(0);
                            //������̕\�����X�V
                            _fieldManager.AlignCapturePieces(_fieldManager.nowPlayer);
                        }
                    }
                    else
                    {
                        secretPieceObject[0].transform.position = _nextTile.transform.position + new Vector3(0, 1, 0);
                        _nextTile.GetComponent<SecretZone>().secretPieceObject.Add(secretPieceObject[0]);
                        secretPieceObject.RemoveAt(0);
                    }
                }
            }
            if (player == 1)
            {
                if (secretPieceObject[0].tag == "2P")
                {
                    if (_nextTile == null)
                    {
                        if (this.gameObject.tag == "Exit1_2P")
                        {
                            Vector2Int tileIndex = new Vector2Int(0, 1);
                            secretPieceObject[0].GetComponent<PieceController>().fieldStatus = PieceController.FieldStatus.OnBoard;
                            _fieldManager.CapturePiece(_fieldManager.nowPlayer, tileIndex);
                            secretPieceObject[0].transform.position = new Vector3(-4, 1, -2);
                            secretPieceObject[0].GetComponent<PieceController>().posIndex = tileIndex;
                            //����C���f�b�N�X�ɖ߂�
                            _fieldManager.AddPiece(secretPieceObject[0].GetComponent<PieceController>(), tileIndex);
                            secretPieceObject.RemoveAt(0);
                            //������̕\�����X�V
                            _fieldManager.AlignCapturePieces(_fieldManager.nowPlayer);
                        }
                        if (this.gameObject.tag == "Exit2_2P")
                        {
                            Vector2Int tileIndex = new Vector2Int(0, 0);
                            secretPieceObject[0].GetComponent<PieceController>().fieldStatus = PieceController.FieldStatus.OnBoard;
                            _fieldManager.CapturePiece(_fieldManager.nowPlayer, tileIndex);
                            secretPieceObject[0].transform.position = new Vector3(-4, 1, -3);
                            secretPieceObject[0].GetComponent<PieceController>().posIndex = tileIndex;
                            //����C���f�b�N�X�ɖ߂�
                            _fieldManager.AddPiece(secretPieceObject[0].GetComponent<PieceController>(), tileIndex);
                            secretPieceObject.RemoveAt(0);
                            //������̕\�����X�V
                            _fieldManager.AlignCapturePieces(_fieldManager.nowPlayer);
                        }
                    }
                    else
                    {
                        secretPieceObject[0].transform.position = _nextTile.transform.position + new Vector3(0, 1, 0);
                        _nextTile.GetComponent<SecretZone>().secretPieceObject.Add(secretPieceObject[0]);
                        secretPieceObject.RemoveAt(0);
                    }
                }
            }
        }
    }
}

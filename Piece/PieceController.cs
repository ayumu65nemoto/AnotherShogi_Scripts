using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PieceController : MonoBehaviour
{
    public enum FieldStatus
    {
        None = -1,
        OnBoard,
        Captured,
        Secret,
    }

    public enum SecretStatus
    {
        Secret1,
        Secret2,
    }

    public enum PieceAbnormal
    {
        None,
        Sleep,
        Invincible,
    }

    //駒のプレイヤー番号
    public int playerNumber{get; set;}
    //ユニットの状態
    public FieldStatus fieldStatus{get; set;}
    public SecretStatus secretStatus{get; set;}
    public PieceAbnormal pieceAbnormal{get; set;}
    //駒のデータベース
    private Piece _pieceScript;

    //駒の情報
    public int typeID, oldTypeID;

    //成ったかどうか
    public bool isEvolution{get; set;}

    //ユニット選択・非選択のy座標
    public const float selectPieceY = 1.5f;
    public const float unSelectPieceY = 0.7f;

    //置いてる場所のインデックス
    public Vector2Int posIndex{get; set;}

    //選択される前のy座標
    private float _oldPosY;

    //今現在選択されているかどうか
    public bool isSelected{get; set;}

    //眠るターンカウント
    public int sleepCount{get; set;} = 0;

    //FieldManager
    private FieldManager _fieldManager;

    private AudioManager _audio;

    //駒の元の色
    private Material _originMaterial;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;
        _audio = AudioManager.instance;
        _originMaterial = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        //状態異常リセット
        if (_fieldManager.nowMode == FieldManager.Mode.TurnChange)
        {
            if (sleepCount > 0)
            {
                sleepCount--;
                if (sleepCount == 0 && pieceAbnormal == PieceAbnormal.Sleep)
                {
                    pieceAbnormal = PieceAbnormal.None;
                    gameObject.GetComponent<Renderer>().material = _originMaterial;
                }
            }
        }
    }

    //初期設定
    public void Init(int player, int pieceType, GameObject tile, Vector2Int pos)
    {
        playerNumber = player;
        typeID = pieceType;
        //取られたとき基に戻る
        oldTypeID = pieceType;
        //駒の状態
        fieldStatus = FieldStatus.OnBoard;
        transform.eulerAngles = GetAngles(player);
        Move(tile, pos);

        _pieceScript = GetComponent<Piece>();
    }

    //指定されたプレイヤー番号の向きを返す
    Vector3 GetAngles(int player)
    {
        return new Vector3(0, player * 180, 0);
    }

    //移動処理
    public void Move(GameObject tile, Vector2Int tileIndex)
    {
        //少し浮かせて新しい場所
        Vector3 pos = tile.transform.position;
        pos.y = unSelectPieceY;
        transform.position = pos;

        //インデックス更新
        posIndex = tileIndex;
    }

    //選択
    public void Select(bool select = true)
    {
        Vector3 pos = transform.position;
        bool kinematic = true;

        if (select == true)
        {
            _oldPosY = pos.y;
            pos.y = selectPieceY;
        }
        else
        {
            pos.y = unSelectPieceY;

            //持ち駒の位置
            if (FieldStatus.Captured == fieldStatus)
            {
                pos.y = _oldPosY;
                kinematic = true;
            }
        }

        GetComponent<Rigidbody>().isKinematic = kinematic;
        transform.position = pos;
    }

    //移動可能範囲取得
    public List<Vector2Int> GetMoveTiles(PieceController[,] pieces, bool checkOtherPiece = true)
    {
        List<Vector2Int> ret = new List<Vector2Int>();
        //持ち駒状態
        if (fieldStatus == FieldStatus.Captured)
        {
            //持ち駒は空いてる場所にしか置けない
            foreach (var pos in GetEmptyCaptureTiles(pieces, playerNumber))
            {
                //移動可能
                bool isMove = true;

                //一時的に移動した状態をつくる
                posIndex = pos;
                fieldStatus = FieldStatus.OnBoard;

                //自分以外いないフィールドを作成し、置いた後に移動できないなら移動不可
                PieceController[,] exPieces = new PieceController[pieces.GetLength(0), pieces.GetLength(1)];
                exPieces[pos.x, pos.y] = this;

                if (GetMoveTiles(exPieces, typeID - 1).Count < 1)
                {
                    //isMove = false;
                }

                //歩
                //if (typeID == 2)
                //{
                //    //二歩のチェック
                //    for (int i = 0; i < pieces.GetLength(1); i++)
                //    {
                //        if (pieces[pos.x, i] && pieces[pos.x, i].typeID == 2 && playerNumber == pieces[pos.x, i].playerNumber)
                //        {
                //            isMove = false;
                //            break;
                //        }
                //    }
                //}

                //打ち歩詰め
                int nextPlayer = FieldManager.GetNextPlayer(playerNumber);

                //今回打ったことにして、王手になる場合
                PieceController[,] copyPieces = FieldManager.GetCopyArray(pieces);
                copyPieces[pos.x, pos.y] = this;
                int outeCount = FieldManager.GetOutePieces(copyPieces, nextPlayer, false).Count;
                if (outeCount > 0)
                {
                    //相手の王が歩を取った状態を作る
                    copyPieces[pos.x, pos.y] = FieldManager.GetPiece(pieces, nextPlayer, 1);
                    outeCount = FieldManager.GetOutePieces(copyPieces, nextPlayer, false).Count;
                    //打ち歩詰めの状態
                    if (outeCount > 0)
                    {
                        //isMove = false;
                    }
                }

                if (!isMove)
                {
                    continue;
                }

                ret.Add(pos);
            }
            //移動状態を元に戻す
            posIndex = new Vector2Int(-1, -1);
            fieldStatus = FieldStatus.Captured;
        }
        else if (typeID == 2)
        {
            ret = GetMoveTiles(pieces, 1);

            //相手の移動範囲を考慮しない
            if (!checkOtherPiece)
            {
                return ret;
            }

            //削除対象のタイル(敵の移動範囲)
            List<Vector2Int> removeTiles = new List<Vector2Int>();

            foreach (var item in ret)
            {
                //移動した状態を作って王手されているなら、削除対象
                PieceController[,] copyPieces = FieldManager.GetCopyArray(pieces);
                //今いる場所から移動した状態を作る
                copyPieces[posIndex.x, posIndex.y] = null;
                copyPieces[item.x, item.y] = null;

                int outeCount = FieldManager.GetOutePieces(copyPieces, playerNumber, false).Count;
                if (outeCount > 0)
                {
                    removeTiles.Add(item);
                }
            }

            //↑で取得したタイルを除外する
            foreach (var item in removeTiles)
            {
                //ret.Remove(item);
            }
        }
        else
        {
            ret = GetMoveTiles(pieces, typeID - 1);
        }

        return ret;
    }

    public List<Vector2Int> GetMoveTiles(PieceController[,] pieces, int pieceType)
    {
        List<Vector2Int> ret = new List<Vector2Int>();

        if (_pieceScript.data.list[pieceType].Pos0 == database_piece_data.Pos_Way.Single)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //前方１マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(0, 1 * dir),
            };

            //前方
            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }
        if (_pieceScript.data.list[pieceType].Pos0 == database_piece_data.Pos_Way.Multiple)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //まっすぐ駒とぶつかるまで
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(0, 1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                while (isCheck(pieces, checkPos))
                {
                    //駒があったら終了
                    if (pieces[checkPos.x, checkPos.y])
                    {
                        //相手の駒
                        if (playerNumber != pieces[checkPos.x, checkPos.y].playerNumber)
                        {
                            ret.Add(checkPos);
                        }
                        break;
                    }

                    ret.Add(checkPos);
                    checkPos += item;
                }
            }
        }

        if (_pieceScript.data.list[pieceType].Pos1 == database_piece_data.Pos_Way.Single)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //右前方１マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(1 * dir, 1 * dir),
            };

            //前方
            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }
        if (_pieceScript.data.list[pieceType].Pos1 == database_piece_data.Pos_Way.Multiple)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //まっすぐ駒とぶつかるまで
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(1 * dir, 1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                while (isCheck(pieces, checkPos))
                {
                    //駒があったら終了
                    if (pieces[checkPos.x, checkPos.y])
                    {
                        //相手の駒
                        if (playerNumber != pieces[checkPos.x, checkPos.y].playerNumber)
                        {
                            ret.Add(checkPos);
                        }
                        break;
                    }

                    ret.Add(checkPos);
                    checkPos += item;
                }
            }
        }
        if (_pieceScript.data.list[pieceType].Pos1 == database_piece_data.Pos_Way.Keima)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(1 * dir, 2 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }

        if (_pieceScript.data.list[pieceType].Pos2 == database_piece_data.Pos_Way.Single)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //右１マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(1 * dir, 0),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }
        if (_pieceScript.data.list[pieceType].Pos2 == database_piece_data.Pos_Way.Multiple)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //まっすぐ駒とぶつかるまで
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(1 * dir, 0),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                while (isCheck(pieces, checkPos))
                {
                    //駒があったら終了
                    if (pieces[checkPos.x, checkPos.y])
                    {
                        //相手の駒
                        if (playerNumber != pieces[checkPos.x, checkPos.y].playerNumber)
                        {
                            ret.Add(checkPos);
                        }
                        break;
                    }

                    ret.Add(checkPos);
                    checkPos += item;
                }
            }
        }

        if (_pieceScript.data.list[pieceType].Pos3 == database_piece_data.Pos_Way.Single)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //右１マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(1 * dir, -1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }
        if (_pieceScript.data.list[pieceType].Pos3 == database_piece_data.Pos_Way.Multiple)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //まっすぐ駒とぶつかるまで
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(1 * dir, -1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                while (isCheck(pieces, checkPos))
                {
                    //駒があったら終了
                    if (pieces[checkPos.x, checkPos.y])
                    {
                        //相手の駒
                        if (playerNumber != pieces[checkPos.x, checkPos.y].playerNumber)
                        {
                            ret.Add(checkPos);
                        }
                        break;
                    }

                    ret.Add(checkPos);
                    checkPos += item;
                }
            }
        }

        if (_pieceScript.data.list[pieceType].Pos4 == database_piece_data.Pos_Way.Single)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //右１マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(0, -1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }
        if (_pieceScript.data.list[pieceType].Pos4 == database_piece_data.Pos_Way.Multiple)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //まっすぐ駒とぶつかるまで
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(0, -1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                while (isCheck(pieces, checkPos))
                {
                    //駒があったら終了
                    if (pieces[checkPos.x, checkPos.y])
                    {
                        //相手の駒
                        if (playerNumber != pieces[checkPos.x, checkPos.y].playerNumber)
                        {
                            ret.Add(checkPos);
                        }
                        break;
                    }

                    ret.Add(checkPos);
                    checkPos += item;
                }
            }
        }

        if (_pieceScript.data.list[pieceType].Pos5 == database_piece_data.Pos_Way.Single)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //右１マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1 * dir, -1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }
        if (_pieceScript.data.list[pieceType].Pos5 == database_piece_data.Pos_Way.Multiple)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //まっすぐ駒とぶつかるまで
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1 * dir, -1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                while (isCheck(pieces, checkPos))
                {
                    //駒があったら終了
                    if (pieces[checkPos.x, checkPos.y])
                    {
                        //相手の駒
                        if (playerNumber != pieces[checkPos.x, checkPos.y].playerNumber)
                        {
                            ret.Add(checkPos);
                        }
                        break;
                    }

                    ret.Add(checkPos);
                    checkPos += item;
                }
            }
        }

        if (_pieceScript.data.list[pieceType].Pos6 == database_piece_data.Pos_Way.Single)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //右１マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1 * dir, 0),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }
        if (_pieceScript.data.list[pieceType].Pos6 == database_piece_data.Pos_Way.Multiple)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //まっすぐ駒とぶつかるまで
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1 * dir, 0),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                while (isCheck(pieces, checkPos))
                {
                    //駒があったら終了
                    if (pieces[checkPos.x, checkPos.y])
                    {
                        //相手の駒
                        if (playerNumber != pieces[checkPos.x, checkPos.y].playerNumber)
                        {
                            ret.Add(checkPos);
                        }
                        break;
                    }

                    ret.Add(checkPos);
                    checkPos += item;
                }
            }
        }

        if (_pieceScript.data.list[pieceType].Pos7 == database_piece_data.Pos_Way.Single)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //右１マス
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1 * dir, 1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }
        if (_pieceScript.data.list[pieceType].Pos7 == database_piece_data.Pos_Way.Multiple)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            //まっすぐ駒とぶつかるまで
            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1 * dir, 1 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                while (isCheck(pieces, checkPos))
                {
                    //駒があったら終了
                    if (pieces[checkPos.x, checkPos.y])
                    {
                        //相手の駒
                        if (playerNumber != pieces[checkPos.x, checkPos.y].playerNumber)
                        {
                            ret.Add(checkPos);
                        }
                        break;
                    }

                    ret.Add(checkPos);
                    checkPos += item;
                }
            }
        }
        if (_pieceScript.data.list[pieceType].Pos7 == database_piece_data.Pos_Way.Keima)
        {
            //向き
            int dir = (playerNumber == 0) ? 1 : -1;

            List<Vector2Int> vec = new List<Vector2Int>()
            {
                new Vector2Int(-1 * dir, 2 * dir),
            };

            foreach (var item in vec)
            {
                Vector2Int checkPos = posIndex + item;
                if (!isCheck(pieces, checkPos) || IsMyPiece(pieces[checkPos.x, checkPos.y]))
                {
                    continue;
                }
                ret.Add(checkPos);
            }
        }

        return ret;
    }

    //配列内かどうか
    bool isCheck(PieceController[,] ary, Vector2Int idx)
    {
        if (idx.x < 0 || ary.GetLength(0) <= idx.x || idx.y < 0 || ary.GetLength(1) <= idx.y)
        {
            return false;
        }

        return true;
    }

    //キャプチャされたとき
    public void Capture(int player)
    {
        playerNumber = player;
        fieldStatus = FieldStatus.Captured;
        pieceAbnormal = PieceAbnormal.None;
        Evolution(false);
        GetComponent<Rigidbody>().isKinematic = true;
    }

    //成り
    public void Evolution(bool evolution = true)
    {
        //成り
        if (evolution == true)
        {
            int tempID = typeID;
            typeID = _pieceScript.data.list[tempID - 1].EvolveTo;

            if (typeID == 0)
            {
                typeID = oldTypeID;
            }
            else if(typeID != 5){
                //エンゼルじゃなかったら進化SE
                //_audio.SE_Play(_fieldManager.evolveSE);
                //ここで進化エフェクト
                ParticleOperator po = Instantiate(_fieldManager.effectList.list[2], this.gameObject.transform).GetComponent<ParticleOperator>();
                po.PlayParticle(_fieldManager.evolveSE);
            }

            Piece piece = this.gameObject.GetComponent<Piece>();
            piece.Set(typeID - 1);
        }
        else
        {
            typeID = oldTypeID;
            transform.eulerAngles = GetAngles(playerNumber);

            Piece piece = this.gameObject.GetComponent<Piece>();
            piece.Set(typeID - 1);

            //退化SE
            //_audio.SE_Play(_fieldManager.degenerateSE);
            //ここで退化エフェクト
            ParticleOperator po = Instantiate(_fieldManager.effectList.list[3], this.gameObject.transform).GetComponent<ParticleOperator>();
            po.PlayParticle(_fieldManager.degenerateSE);
        }

        isEvolution = evolution;
    }

    //自分の駒かどうか
    bool IsMyPiece(PieceController piece)
    {
        if (piece && playerNumber == piece.playerNumber)
        {
            return true;
        }
        return false;
    }

    //空いているマスを返す
    List<Vector2Int> GetEmptyTiles(PieceController[,] pieces)
    {
        List<Vector2Int> ret = new List<Vector2Int>();
        for (int i = 0; i < pieces.GetLength(0); i++)
        {
            for (int j = 0; j < pieces.GetLength(1); j++)
            {
                if (pieces[i, j])
                {
                    continue;
                }
                ret.Add(new Vector2Int(i, j));
            }
        }
        return ret;
    }

    //持ち駒置くとき用の　空いてるマスを返す
    List<Vector2Int> GetEmptyCaptureTiles(PieceController[,] pieces, int player)
    {
        List<Vector2Int> ret = new List<Vector2Int>();
        for (int i = 0; i < 8; i++)
        {
            if (player == 0)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (pieces[i, j])
                    {
                        continue;
                    }
                    ret.Add(new Vector2Int(i, j));
                    //Debug.Log(ret);
                }
            }
            else if (player == 1)
            {
                for (int j = 4; j < 7; j++)
                {
                    if (pieces[i, j])
                    {
                        continue;
                    }
                    ret.Add(new Vector2Int(i, j));
                    //Debug.Log(ret);
                }
            }
        }
        return ret;
    }

    //カード効果
    public void SendToSecret(PieceController piece, int num)
    {
        //1Pの駒を秘密ゾーン１に送る
        if (num == 0)
        {
            if (piece.fieldStatus == FieldStatus.OnBoard && playerNumber == 0)
            {
                //インデックスから削除(じゃないと駒が盤面にある判定になる)
                _fieldManager.RemovePiece(piece);
                piece.transform.position = _fieldManager.mySecrets1[0].transform.position + new Vector3(0, 1, 0);
                _fieldManager.mySecrets1[0].GetComponent<SecretZone>().secretPieceObject.Add(this.gameObject);
                piece.fieldStatus = FieldStatus.Secret;
                piece.secretStatus = SecretStatus.Secret1;
            }
        }
        //2Pの駒１つを秘密ゾーン１に送る
        else if (num == 1)
        {
            if (piece.fieldStatus == FieldStatus.OnBoard && playerNumber == 1)
            {
                _fieldManager.RemovePiece(piece);
                piece.transform.position = _fieldManager.otherSecrets1[0].transform.position + new Vector3(0, 1, 0);
                _fieldManager.otherSecrets1[0].GetComponent<SecretZone>().secretPieceObject.Add(this.gameObject);
                piece.fieldStatus = FieldStatus.Secret;
                piece.secretStatus = SecretStatus.Secret1;
            }
        }
        //1Pの駒１つを秘密ゾーン２に送る
        else if (num == 2)
        {
            if (piece.fieldStatus == FieldStatus.OnBoard && playerNumber == 0)
            {
                _fieldManager.RemovePiece(piece);
                piece.transform.position = _fieldManager.mySecrets2[0].transform.position + new Vector3(0, 1, 0);
                _fieldManager.mySecrets2[0].GetComponent<SecretZone>().secretPieceObject.Add(this.gameObject);
                piece.fieldStatus = FieldStatus.Secret;
                piece.secretStatus = SecretStatus.Secret2;
            }
        }
        //2Pの駒１つを秘密ゾーン２に送る
        else if (num == 3)
        {
            if (piece.fieldStatus == FieldStatus.OnBoard && playerNumber == 1)
            {
                _fieldManager.RemovePiece(piece);
                piece.transform.position = _fieldManager.otherSecrets2[0].transform.position + new Vector3(0, 1, 0);
                _fieldManager.otherSecrets2[0].GetComponent<SecretZone>().secretPieceObject.Add(this.gameObject);
                piece.fieldStatus = FieldStatus.Secret;
                piece.secretStatus = SecretStatus.Secret2;
            }
        }

        //1Pの持ち駒を秘密ゾーン１に送る
        else if (num == 4)
        {
            if (piece.fieldStatus == FieldStatus.Captured && playerNumber == 0)
            {
                piece.transform.position = _fieldManager.mySecrets1[0].transform.position + new Vector3(0, 1, 0);
                _fieldManager.mySecrets1[0].GetComponent<SecretZone>().secretPieceObject.Add(this.gameObject);
                piece.fieldStatus = FieldStatus.Secret;
                piece.secretStatus = SecretStatus.Secret1;
                _fieldManager.capturePieces.Remove(piece);
                piece.GetComponent<Rigidbody>().isKinematic = false;
                piece.gameObject.SetActive(true);
            }
        }
        //2Pの持ち駒を秘密ゾーン１に送る
        else if (num == 5)
        {
            if (piece.fieldStatus == FieldStatus.Captured && playerNumber == 1)
            {
                piece.transform.position = _fieldManager.otherSecrets1[0].transform.position + new Vector3(0, 1, 0);
                _fieldManager.otherSecrets1[0].GetComponent<SecretZone>().secretPieceObject.Add(this.gameObject);
                piece.fieldStatus = FieldStatus.Secret;
                piece.secretStatus = SecretStatus.Secret1;
                _fieldManager.capturePieces.Remove(piece);
                piece.GetComponent<Rigidbody>().isKinematic = false;
                piece.gameObject.SetActive(true);
            }
        }
        //1Pの持ち駒１つを秘密ゾーン２に送る
        else if (num == 6)
        {
            if (piece.fieldStatus == FieldStatus.Captured && playerNumber == 0)
            {
                piece.transform.position = _fieldManager.mySecrets2[0].transform.position + new Vector3(0, 1, 0);
                _fieldManager.mySecrets2[0].GetComponent<SecretZone>().secretPieceObject.Add(this.gameObject);
                piece.fieldStatus = FieldStatus.Secret;
                piece.secretStatus = SecretStatus.Secret2;
                _fieldManager.capturePieces.Remove(piece);
                piece.GetComponent<Rigidbody>().isKinematic = false;
                piece.gameObject.SetActive(true);
            }
        }
        //2Pの持ち駒１つを秘密ゾーン２に送る
        else if (num == 7)
        {
            if (piece.fieldStatus == FieldStatus.Captured && playerNumber == 1)
            {
                piece.transform.position = _fieldManager.otherSecrets2[0].transform.position + new Vector3(0, 1, 0);
                _fieldManager.otherSecrets2[0].GetComponent<SecretZone>().secretPieceObject.Add(this.gameObject);
                piece.fieldStatus = FieldStatus.Secret;
                piece.secretStatus = SecretStatus.Secret2;
                _fieldManager.capturePieces.Remove(piece);
                piece.GetComponent<Rigidbody>().isKinematic = false;
                piece.gameObject.SetActive(true);
            }
        }

        //1Pの秘密ゾーンの駒1つを出口に送る
        else if (num == 8)
        {
            if (piece.fieldStatus == FieldStatus.Secret && piece.secretStatus == SecretStatus.Secret1 && playerNumber == 0)
            {
                piece.transform.position = _fieldManager.mySecrets1[2].transform.position + new Vector3(0, 1, 0);
                DeleteSecret1Piece1(gameObject);
                _fieldManager.mySecrets1[2].GetComponent<SecretZone>().secretPieceObject.Add(gameObject);
                _fieldManager.mySecrets1[2].GetComponent<SecretZone>().SecretMove(0);
            }
            if (piece.fieldStatus == FieldStatus.Secret && piece.secretStatus == SecretStatus.Secret2 && playerNumber == 0)
            {
                piece.transform.position = _fieldManager.mySecrets2[2].transform.position + new Vector3(0, 1, 0);
                DeleteSecret2Piece1(gameObject);
                _fieldManager.mySecrets2[4].GetComponent<SecretZone>().secretPieceObject.Add(gameObject);
                _fieldManager.mySecrets2[4].GetComponent<SecretZone>().SecretMove(0);
            }
        }
        //2Pの秘密ゾーンの駒1つを出口に送る
        else if (num == 9)
        {

            if (piece.fieldStatus == FieldStatus.Secret && piece.secretStatus == SecretStatus.Secret1 && playerNumber == 1)
            {
                piece.transform.position = _fieldManager.otherSecrets1[2].transform.position + new Vector3(0, 1, 0);
                DeleteSecret2Piece1(gameObject);
                _fieldManager.otherSecrets1[2].GetComponent<SecretZone>().secretPieceObject.Add(gameObject);
                _fieldManager.otherSecrets1[2].GetComponent<SecretZone>().SecretMove(1);
            }
            if (piece.fieldStatus == FieldStatus.Secret && piece.secretStatus == SecretStatus.Secret2 && playerNumber == 1)
            {
                piece.transform.position = _fieldManager.mySecrets2[2].transform.position + new Vector3(0, 1, 0);
                DeleteSecret2Piece2(gameObject);
                _fieldManager.otherSecrets2[4].GetComponent<SecretZone>().secretPieceObject.Add(gameObject);
                _fieldManager.otherSecrets2[4].GetComponent<SecretZone>().SecretMove(1);
            }
        }
    }

    //1Pの特定のオブジェクトを秘密ゾーン1のリストから削除する
    public void DeleteSecret1Piece1(GameObject gameObject)
    {
        if (_fieldManager.mySecrets1[0].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.mySecrets1[0].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.mySecrets1[0].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.mySecrets1[1].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.mySecrets1[1].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.mySecrets1[1].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.mySecrets1[2].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.mySecrets1[2].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.mySecrets1[2].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
    }

    //1Pの特定のオブジェクトを秘密ゾーン2のリストから削除する
    public void DeleteSecret2Piece1(GameObject gameObject)
    {
        if (_fieldManager.mySecrets2[0].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.mySecrets2[0].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.mySecrets2[0].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.mySecrets2[1].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.mySecrets2[1].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.mySecrets2[1].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.mySecrets2[2].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.mySecrets2[2].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.mySecrets2[2].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.mySecrets2[3].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.mySecrets2[2].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.mySecrets2[3].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.mySecrets2[4].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.mySecrets2[2].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.mySecrets2[4].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
    }

    //2Pの特定のオブジェクトを秘密ゾーン1のリストから削除する
    public void DeleteSecret1Piece2(GameObject gameObject)
    {
        if (_fieldManager.otherSecrets1[0].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.otherSecrets1[0].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.otherSecrets1[0].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.otherSecrets1[1].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.otherSecrets1[1].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.otherSecrets1[1].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.otherSecrets1[2].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.otherSecrets1[2].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.otherSecrets1[2].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
    }

    //2Pの特定のオブジェクトを秘密ゾーン2のリストから削除する
    public void DeleteSecret2Piece2(GameObject gameObject)
    {
        if (_fieldManager.otherSecrets2[0].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.otherSecrets2[0].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.otherSecrets2[0].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.otherSecrets2[1].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.otherSecrets2[1].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.otherSecrets2[1].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.otherSecrets2[2].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.otherSecrets2[2].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.otherSecrets2[2].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.otherSecrets2[3].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.otherSecrets2[2].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.otherSecrets2[3].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
        if (_fieldManager.otherSecrets2[4].GetComponent<SecretZone>().secretPieceObject.Count > 0 && _fieldManager.otherSecrets2[2].GetComponent<SecretZone>().secretPieceObject.Contains(gameObject))
        {
            _fieldManager.otherSecrets2[4].GetComponent<SecretZone>().secretPieceObject.Remove(gameObject);
        }
    }
}

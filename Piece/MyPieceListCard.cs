using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;
using Unity.VectorGraphics;

public class MyPieceListCard : MonoBehaviour
{
    [SerializeField] private RectTransform _cursorRect;
    [SerializeField] private PieceInfo _pieceInfo;
    [SerializeField] private RectTransform _piecesParent;
    [SerializeField] private piece_imagedata _imageData;
    [SerializeField] private RectTransform _numTextParent;
    [SerializeField] private Animator _thisAnim;
    [SerializeField] private StringData _explainData;

    private AnnounceBar _instAnnounce;
    private Image[] _pieceImage = new Image[9];
    private Button[] _pieceButton = new Button[9];
    private TextMeshProUGUI[] _numText = new TextMeshProUGUI[9];

    public static MyPieceListCard instance;

    public int[] pieceNum { get; set; } = new int[9];//各コマの所持数を一時保存する変数
    public bool settingPiece { get; set; }//コマの配置場所を決定するフェーズか

    private AudioManager _audio;
    private FieldManager _field;
    private float _dead;
    private SaveDataManager _saveDataManager;//セーブデータを管理するスクリプト
    private CheckDeckNum _checkDeck;
    private sbyte _selecting = 0;//選択中のオブジェクトの番号
    private byte _pieceMax;//持ちゴマ所持数
    private byte _rowNum;//コマの総行数

    private Button _thisButton;//BGのキャンセルボタン
    private SVGImage _thisImage;//BGの画像

    private float gap = 75.0f;//コマからのずれ・左方向

    private CardManager _cardManager;

    void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _audio = AudioManager.instance;
        _field = FieldManager.instance;
        _saveDataManager = SaveDataManager.instance;//設定内容一覧をセーブデータから取得
        _dead = _saveDataManager.dead;
        _checkDeck = CheckDeckNum.instance;

        _thisButton = GetComponent<Button>();
        _thisImage = GetComponent<SVGImage>();

        _cardManager = CardManager.instance;

        //持ちゴマの配列を使用してコマ生成
        for (int i = 0; i < _pieceImage.Length; i++)
        {
            RectTransform temp = (RectTransform)_piecesParent.GetChild(i);

            //コマ画像取得
            _pieceImage[i] = temp.GetChild(0).GetComponent<Image>();
            _pieceImage[i].sprite = _imageData.piece_imageList[i];

            _pieceButton[i] = temp.GetComponent<Button>();

            _numText[i] = _numTextParent.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
        SetPieceNum();

        _pieceMax = (byte)_piecesParent.childCount;
        _rowNum = (byte)Mathf.CeilToInt(_pieceMax / 3);

        //カーソル位置を一番左上にする
        StartCoroutine(WaitInfoReady());
    }

    //public void Pressed_Done(InputAction.CallbackContext context)
    //{//決定ボタンが押された時の処理
    //    if (context.performed)
    //    {
    //        //SelectPiece(_selecting);
    //    }
    //}

    public void Pressed_Cancel(InputAction.CallbackContext context)
    {//キャンセルボタンが押された時の処理
        if (context.performed)
        {
            Cancel();
        }
    }

    public void Pressed_Cross(InputAction.CallbackContext context)
    {//上下ボタンが押された時の処理。スティック操作・連続入力に未対応
        if (context.performed)
        {
            Vector2 value = context.ReadValue<Vector2>();

            if (value.y >= _dead)
            {//上方向
                if (Mathf.FloorToInt(_selecting / 3) == 0)
                {
                    _selecting = (sbyte)(((_rowNum * 3) - 3) + (_selecting % 3));//末尾行の左側へ移動後、選んでいた列に移動
                }
                else
                {
                    _selecting -= 3;
                }
            }
            else if (value.y <= -_dead)
            {//下方向
                _selecting += 3;
                if (_selecting >= _pieceMax)
                {
                    _selecting %= 3;
                }
            }
            else if (value.x >= _dead)
            {//右方向
                if (_selecting % 3 == 2)
                {//一番右か
                    _selecting -= 2;//同じ行の一番左へ
                }
                else
                {
                    _selecting++;
                }
                //CheckMinMax();
            }
            else if (value.x <= -_dead)
            {//左方向
                if (_selecting % 3 == 0)
                {//一番左か
                    _selecting += 2;//同じ行の一番右へ
                }
                else
                {
                    _selecting--;
                }
                //CheckMinMax();
            }
            MoveCursor((RectTransform)_piecesParent.GetChild(_selecting));
        }
    }

    public void MoveCursor(RectTransform rect)
    {//カーソルを動かす
        float newX = rect.position.x - gap;
        _cursorRect.position = new Vector2(newX, rect.position.y);
        _selecting = (sbyte)rect.GetSiblingIndex();
        _pieceInfo.WriteInfo(_selecting + 1);//コマ情報更新
        _audio.SE_UI_Play(AudioManager.WhichSE.CursorMove);
        //持ちゴマの配列からID取得？
        //_pieceInfo.WriteInfo(ID);//PieceInfoに、選択中のコマ情報を表示させる
    }

    public void SelectPieceCard(int type)
    {   //コマを選択した時の挙動・選択したコマのIDを返す
        _audio.SE_UI_Play(AudioManager.WhichSE.Done);
        if (_cardManager.GetComponent<Effect3>() != null)
        {
            _cardManager.GetComponent<Effect3>().DoneEffect3(type);
        }
        if (_cardManager.GetComponent<Effect4>() != null)
        {
            _cardManager.GetComponent<Effect4>().DoneEffect4(type);
        }
        if (_cardManager.GetComponent<Effect5>() != null)
        {
            _cardManager.GetComponent<Effect5>().DoneEffect5(type);
        }
    }

    public void Cancel()
    {
        //ここで持ちゴマ開いてる判定falseにする
        if (!settingPiece)
        {//コマ決定前の場合
            _checkDeck.PieceListCancel();
        }
        else
        {//コマ決定後の場合
            //ここでコマ配置待機解除
            settingPiece = false;
            _thisAnim.SetFloat("Speed", -1.0f);
            _thisAnim.SetTrigger("Do");
            _field.CloseAnnounce();
            _field.SelectCursors();
            _audio.SE_UI_Play(AudioManager.WhichSE.Cancel);
            _thisButton.interactable = true;
            _thisImage.raycastTarget = true;
        }
    }

    private void CheckMinMax()
    {
        if (_selecting >= _pieceMax)
        {
            _selecting = 0;
        }
        else if (_selecting < 0)
        {
            _selecting = (sbyte)(_pieceMax - 1);//一番最後のところへ移動・_selectingは0始まりなので-1
        }
    }

    IEnumerator WaitInfoReady()
    {
        while (true)
        {
            if (_pieceInfo.ready)
            {//PieceInfoの初期化が終わったか
                RectTransform rect = (RectTransform)_piecesParent.GetChild(0);//一番左上のコマを取得
                float newX = rect.position.x - gap;//位置を調整
                _cursorRect.position = new Vector2(newX, rect.position.y);//カーソル位置をコマの左側へ移動
                _pieceInfo.WriteInfo(_selecting + 1);//コマ情報更新・キングをさすようにする
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }

    public void SetPieceNum()
    {   //各持ち駒の所持数を更新します
        pieceNum = new int[9];
        for (int i = 0; i < _field.capturePieces.Count; i++)
        {
            if (_field.capturePieces[i].playerNumber == _field.nowPlayer)
            {//そのコマが操作プレイヤーのものであった場合
                pieceNum[(_field.capturePieces[i].typeID - 1)]++;//コマIDに該当するカウント変数に加算・IDは1始まりなので配列に合わせて-1
            }
        }
        for (int i = 0; i < 9; i++)
        {
            _numText[i].text = "×" + pieceNum[i].ToString("00");//各コマの所持数をpieceNumから引っ張る
        }
    }
}

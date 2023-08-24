using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class FieldManager : MonoBehaviour
{
    public static FieldManager instance;

    [SerializeField]private AudioClip[] _clip;

    //縦、横何列にするか
    [SerializeField] private int _boardWidth;
    [SerializeField] private int _boardHeight;

    [SerializeField] private GameObject _tile;

    //カウント変数
    private int _counter;

    //駒のプレハブ
    [SerializeField] private database_piece _pieceBases;
    [SerializeField] private Piece _piece;

    //初期配置
    private int[,] _startPieces =
    {
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
        { 0, 0, 0, 0, 0, 0, 0 },
    };

    //フィールドデータ
    private Dictionary<Vector2Int, GameObject> _tiles;
    private PieceController[,] _pieces;

    //選択中の駒
    private PieceController _selectPiece;

    //移動可能タイル
    private Dictionary<GameObject, Vector2Int> _moveTiles;

    //カーソルのプレハブ
    [SerializeField] private GameObject _prefabCursor;

    //カーソルのオブジェクト
    private List<GameObject> _cursors;

    //プレイヤーとターン数
    public int nowPlayer{get; set;}
    int turnCount;
    bool isCpu;

    //モード
    public enum Mode
    {
        None,
        Start,
        Draw,
        CardSelect,
        CardOpen,
        CardPlay,
        Select,
        WaitEvolution,
        TurnChange,
        Result
    }
    public Mode nowMode, nextMode;

    //持ち駒のプレハブ
    [SerializeField] public GameObject prefabPieceTile;

    //持ち駒を置く場所
    public List<GameObject>[] pieceTiles{get; set;}

    //キャプチャされたユニット
    public List<PieceController> capturePieces{get; set;}

    //進化ゾーン
    //const int _evolutionLine = 1;
    private List<int> _lineLists;

    //CardManager
    private CardManager _cardManager;

    //ターン交代時のPrefab
    [SerializeField]private GameObject _changeTurnObj;
    //Prefab生成時の親(Range_UI)
    [SerializeField]private RectTransform _instParent;

    //秘密ゾーン
    [SerializeField] public List<GameObject> mySecrets1 = new List<GameObject>();
    [SerializeField] public List<GameObject> mySecrets2 = new List<GameObject>();
    [SerializeField] public List<GameObject> otherSecrets1 = new List<GameObject>();
    [SerializeField] public List<GameObject> otherSecrets2 = new List<GameObject>();

    //ゲームセットのオブジェクト
    [SerializeField]private GameObject _gameSetObj;

    //操作アナウンスを表示するObj
    [SerializeField]private GameObject _announceBarObj;
    private AnnounceBar _instAnnounce;
    private bool _isAnnounced;//既にアナウンスバー生成されているか
    [SerializeField]private StringData _explainData;//よくつかう説明文のデータ(OperateExplain)

    //バトル開始を示すObj・先攻抽せんもやってくれる
    [SerializeField]private GameObject _battleStartObj;

    [SerializeField]private GameObject _panel;//決着時のFlash演出用
    public AudioClip[] moveSE;//コマを移動させるときのSE
    [SerializeField]private AudioClip[] _finishSE;//王を取った瞬間のSE・パシーンッ！！！
    [SerializeField]private AudioClip _finishSE2;//パシーンと同時に流すSE・スローモーション
    [SerializeField]private AudioClip[] _getSE;//コマを取った瞬間のSE・パシン！
    public AudioClip evolveSE;//進化時のSE・シュピーン？
    public AudioClip degenerateSE;//退化時のSE・しゅーん？
    public AudioClip summonSE;//召喚するときのSE
    [SerializeField]private AudioClip _gamesetSE;//ゲームセット表示時のSE

    [SerializeField]private Animator _buttonsAnim;//ボタン群UIのアニメーター・表示非表示

    public GameObjectList effectList;

    //選択されたコマを格納
    public PieceController piece{get; set;}

    //廃棄カード位置
    public float trashPosition{get; set;} = 0;

    //マウス
    private Mouse _mouse;

    //InputSystem
    private PlayerInput _playerInput;

    //ドロースキップフラグ
    public bool isSkipDraw1{get; set;} = false;
    public bool isSkipDraw2{get; set;} = false;
    //カード身代わり開始・終了フラグ
    public bool isProxyCard{get; set;} = false;
    public bool isProxyCardReset{get; set;} = false;

    //仮レベルアップ
    public bool isTempEvol{get; set;} = false;
    public PieceController tempEvolPiece{get; set;}

    //キングスライムが取られないフラグ・カウント
    public bool isInvincibleKing1{get; set;} = false;
    public bool isInvincibleKing2{get; set;} = false;
    public int invincibleCount1{get; set;} = 0;
    public int invincibleCount2{get; set;} = 0;

    //ターンスキップフラグ・カウント
    public bool isSkipTurn1{get; set;} = false;
    public bool isSkipTurn2{get; set;} = false;
    public int skipTurnCount1{get; set;} = 0;
    public int skipTurnCount2{get; set;} = 0;

    //自分の駒が相手に取られないフラグ・カウント
    public bool isInvincibleAll1{get; set;} = false;
    public bool isInvincibleAll2{get; set;} = false;
    public int invincibleAllCount1{get; set;} = 0;
    public int invincibleAllCount2{get; set;} = 0;

    private SaveDataManager _saveDataManager;
    private CountTime _count;//持ち時間のカウントスクリプト
    private CheckDeckNum _checkDeck;//持ちゴマとかのタブを消すように取得
    private AudioManager _audio;
    private CameraController _camera;

    private bool _onceInMethod;//Mode中で1回だけ行いたい処理用

    private bool _instedGameOver;//既にゲームセットObj生成したか
    public bool winner{get; set;}//勝者・false,true = 1P,2P

    //マテリアル
    [SerializeField] public Material originMaterial;
    [SerializeField] public Material sleepMaterial;

    void Awake(){
        instance = this;

        //キャプチャされたユニット
        capturePieces = new List<PieceController>();

        //ここで先攻後攻を決める
        if (UnityEngine.Random.Range(0, 2) == 0)
        {
            //1P
            nowPlayer = 0;
        }
        else
        {
            //2P
            nowPlayer = 1;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //マウス取得
        _mouse = Mouse.current;

        //InputSystem
        _playerInput = GetComponent<PlayerInput>();

        //駒の初期位置を２次元配列に格納
        for (int i = 0; i < 9; i++)
        {
            _startPieces[_pieceBases.list[i].StartX, _pieceBases.list[i].StartY] = i + 1;
            _startPieces[(_boardWidth - 1) - _pieceBases.list[i].StartX, (_boardHeight - 1) - _pieceBases.list[i].StartY] = i + 11;
        }

        //カウンター初期化
        _counter = 0;
        //フィールド初期化
        _tiles = new Dictionary<Vector2Int, GameObject>();
        _pieces = new PieceController[_boardWidth, _boardHeight];
        //移動可能範囲初期化
        _moveTiles = new Dictionary<GameObject, Vector2Int>();
        //カーソルの初期化
        _cursors = new List<GameObject>();
        //持ち駒を置く場所
        pieceTiles = new List<GameObject>[2];

        _count = CountTime.instance;
        _cardManager = CardManager.instance;
        _audio = AudioManager.instance;
        _checkDeck = CheckDeckNum.instance;
        _camera = CameraController.instance;

        //持ち駒を置く場所作成
        Vector3 startPos = new Vector3(7.5f, 0.5f, -2);
        for (int i = 0; i < 2; i++)
        {
            pieceTiles[i] = new List<GameObject>();
            int dir = (0 == i) ? 1 : -1;

            for (int j = 0; j < 10; j++)
            {
                Vector3 pos = startPos;
                pos.x = (pos.x + j % 3) * dir;
                pos.z = (pos.z - j / 3) * dir;

                GameObject obj = Instantiate(prefabPieceTile, pos, Quaternion.identity);
                pieceTiles[i].Add(obj);

                obj.SetActive(false);
            }
        }

        //進化ゾーン作成
        _lineLists = new List<int>();
        int range = 3;
        _lineLists.Add(range);

        //デッキ作成
        _cardManager.GenerateDeck();

        //TurnChangeから始める場合ー１
        //nowPlayer = -1;

        //nowMode = Mode.None;
        //nextMode = Mode.TurnChange;

        //セーブデータ管理取得
        _saveDataManager = SaveDataManager.instance;
        _saveDataManager.SetBGMStartTime = true;

        _count.StartPlayCount();//試合時間のカウント開始
        
        _audio.BGM_Play(_clip[UnityEngine.Random.Range(0, _clip.Length)]);

        StartCoroutine(WaitAnimEnd());//バトル開始表示が消えるのを待つ・消えたらターンチェンジ表示を行う

        MakeField();
    }

    IEnumerator WaitAnimEnd(){//バトル開始表示が消えるのを待つ・消えたらターンチェンジ表示を行う
        GameObject startObj = Instantiate(_battleStartObj, _instParent);//バトル開始表示
        //このままだと、スタート表示とターン交代の間にコマクリックされそう
        //yield return new WaitForSeconds((140f/60f)-0.5f);//アニメが終わるより少し早めにターン交代
        while(startObj != null){
            yield return null;
        }
        //初回モード
        nowMode = Mode.None;
        nextMode = Mode.TurnChange;
        yield break;
    }

    void Update()
    {
        if (Mode.Start == nowMode)
        {
            StartMode();
        }
        else if (Mode.Draw == nowMode)
        {
            DrawMode();
        }
        else if (Mode.CardSelect == nowMode)
        {
            //CardSelectMode();
        }
        else if (Mode.CardOpen == nowMode)
        {
            CardOpenMode();
        }
        else if (Mode.CardPlay == nowMode)
        {
            CardPlayMode();
        }
        else if (Mode.Select == nowMode)
        {
            SelectMode();
        }
        else if (Mode.TurnChange == nowMode)
        {
            TurnChangeMode();
        }
        else if (Mode.Result == nowMode)
        {
            //StartCoroutine(ResultMode());
        }

        //モード変更
        if ((Mode.None != nextMode) && (nowMode != Mode.Result))
        {
            nowMode = nextMode;
            nextMode = Mode.None;
            _onceInMethod = false;
        }
    }

    private void MakeField()
    {
        for (int i = 0; i < _boardWidth; i++)
        {
            //カウンター初期化
            _counter = 0;
            for (int j = 0; j < _boardHeight; j++)
            {
                //タイルと駒のポジション
                float x = i - _boardWidth / 2;
                float y = j - _boardHeight / 2;

                //タイルのインデックス
                Vector2Int idx = new Vector2Int(i, j);

                //ポジション
                Vector3 pos = new Vector3(x, 0, y);

                //フィールド生成
                GameObject gridField = Instantiate(_tile, pos, Quaternion.identity);
                _tiles.Add(idx, gridField);

                //しんかゾーン
                if (_counter == 3 )
                {
                    GameObject InTile = gridField.transform.Find("Cube").gameObject;
                    InTile.GetComponent<Renderer>().material.color = new Color32(0, 235, 255, 255);
                }
                _counter++;

                //設定からエンゼルの除外処理
                if (_saveDataManager.saveData.SpecialPiece == false)
                {
                    _startPieces[_pieceBases.list[4].StartX, _pieceBases.list[4].StartY] = 0;
                    _startPieces[(_boardWidth - 1) - _pieceBases.list[4].StartX, (_boardHeight - 1) - _pieceBases.list[4].StartY] = 0;
                }

                //先手のエンゼル除外処理
                if (nowPlayer == 0)
                {
                    _startPieces[_pieceBases.list[4].StartX, _pieceBases.list[4].StartY] = 0;
                }
                if (nowPlayer == 1)
                {
                    _startPieces[(_boardWidth - 1) - _pieceBases.list[4].StartX, (_boardHeight - 1) - _pieceBases.list[4].StartY] = 0;
                }

                //駒の作成
                int type = _startPieces[i, j] % 10;
                int player = _startPieces[i, j] / 10;

                if (0 == type)
                {
                    continue;
                }

                //初期化
                pos.y = 0.7f;

                Piece piece = Instantiate(_piece, pos, Quaternion.Euler(0, player * 180, 0));
                piece.Set(type - 1);
                piece.gameObject.AddComponent<Rigidbody>();
                piece.gameObject.AddComponent<SelectPieceEvent>();
                
                if (player == 0)
                {
                    piece.tag = "1P";
                }
                else if (player == 1)
                {
                    piece.tag = "2P";
                }

                PieceController pieceController = piece.gameObject.AddComponent<PieceController>();

                pieceController.Init(player, type, gridField, idx);

                //駒のデータをセット
                _pieces[i, j] = pieceController;
            }
        }
    }

    //選択時
    public void SelectCursors(PieceController piece = null, bool playerPiece = true)
    {
        //Debug.Log("Did SelectCursors");
        
        //カーソル解除
        foreach (var item in _cursors)
        {
            Destroy(item);
        }
        _cursors.Clear();

        //選択駒の非選択状態
        if (null != _selectPiece)
        {
            _selectPiece.Select(false);
            _selectPiece = null;
            OpenAnnounce(_explainData.list[0]);
        }

        if (piece == null)
        {
            return;
        }

        if (piece.isSelected == true)
        {
            piece.isSelected = false;
            return;
        }

        //移動可能範囲取得
        List<Vector2Int> moveTiles = getMoveTiles(piece);
        this._moveTiles.Clear();
        foreach (var item in moveTiles)
        {
            _moveTiles.Add(_tiles[item], item);
            //カーソル
            Vector3 pos = _tiles[item].transform.position;
            pos.y += 0.51f;
            GameObject cursor = Instantiate(_prefabCursor, pos, Quaternion.identity);
            _cursors.Add(cursor);
            piece.isSelected = true;
            OpenAnnounce(_explainData.list[1]);
        }

        //選択状態
        if (playerPiece == true)
        {
            piece.Select();
            _selectPiece = piece;
        }
    }

    //駒移動
    Mode MovePiece(PieceController piece, Vector2Int tileIndex)
    {
        //移動し終わった後のモード
        Mode ret = Mode.TurnChange;

        //現在値
        Vector2Int oldPos = piece.posIndex;

        //移動先に駒があったら取る
        CapturePiece(nowPlayer, tileIndex);

        //駒の移動
        piece.Move(_tiles[tileIndex], tileIndex);

        //エンゼルだったら1回休み
        if (nowPlayer == 0)
        {
            if (piece.typeID == 5)
            {
                isSkipTurn1 = true;
                skipTurnCount1 = 2;
            }
        }
        if (nowPlayer == 1)
        {
            if (piece.typeID == 5)
            {
                isSkipTurn2 = true;
                skipTurnCount2 = 2;
            }
        }

        //内部データ更新(新しい場所)
        _pieces[tileIndex.x, tileIndex.y] = piece;

        //ボード上の駒を更新
        if ( piece.fieldStatus == PieceController.FieldStatus.OnBoard)
        {
            //内部データ更新(古い場所)
            _pieces[oldPos.x, oldPos.y] = null;

            //成
            if (!piece.isEvolution && _lineLists.Contains(tileIndex.y))
            {
                //次のターンに移動可能かどうか
                PieceController[,] copyPieces = new PieceController[_boardWidth, _boardHeight];
                //自分以外いないフィールドを作る
                copyPieces[piece.posIndex.x, piece.posIndex.y] = piece;

                //CPUもしくは次回移動できないなら強制に成る
                if (/*isCpu || */ piece.GetMoveTiles(copyPieces, true).Count > 1)
                {
                    piece.Evolution();
                }
                //成るか確認
                else
                {
                    //成った状態を表示
                    piece.Evolution();
                    SelectCursors(piece);

                    //ナビゲーション
                    //texitResultImfo.text = "成りますか？";
                    //buttonEvolutionApply.gameObject.SetActive(true);
                    //buttonEvolutionCansel.gameObject.SetActive(true);

                    //ret = Mode.WaitEvolution;

                    //とりあえず強制的に成るようにしておく(選択できるようにしたければボタンにでも付ける)
                    OnClickEvolutionApply();
                }
            }
        }
        //持ち駒の更新
        else
        {
            capturePieces.Remove(piece);
        }

        //駒の状態を更新
        piece.fieldStatus = PieceController.FieldStatus.OnBoard;
        piece.gameObject.SetActive(true);
        //持ち駒の表示を更新
        AlignCapturePieces(nowPlayer);

        return ret;
    }

    //移動可能な範囲取得
    List<Vector2Int> getMoveTiles(PieceController piece)
    {
        if (piece.fieldStatus == PieceController.FieldStatus.Secret)
        {
            return null;
        }

        //通常範囲外
        List<Vector2Int> ret = piece.GetMoveTiles(_pieces);

        //1Pのキングは取れない
        if (isInvincibleKing1 == true)
        {
            InvincibleKing(ret, 0);
        }

        //2Pのキングは取れない
        if (isInvincibleKing2 == true)
        {
            InvincibleKing(ret, 1);
        }

        //1Pの駒は取れない
        if (isInvincibleAll1 == true)
        {
            InvincibleAll(ret, 0);
        }
        //2Pの駒は取れない
        if (isInvincibleAll2 == true)
        {
            InvincibleAll(ret, 1);
        }

        //そこをどいたら王手されるかチェック
        //PieceController[,] copyPieces = GetCopyArray(_pieces);
        //if (piece.fieldStatus == PieceController.FieldStatus.OnBoard)
        //{
        //    copyPieces[piece.posIndex.x, piece.posIndex.y] = null;
        //}
        //int outeCount = GetOutePieces(copyPieces, piece.playerNumber).Count;

        //王手を回避できているタイルを返す
        //if (outeCount > 0)
        //{
        //    ret = new List<Vector2Int>();
        //    //移動可能範囲
        //    List<Vector2Int> moveTiles = piece.GetMoveTiles(_pieces);
        //    foreach (var item in moveTiles)
        //    {
        //        //移動した状態を作る
        //        PieceController[,] copyPieces2 = GetCopyArray(copyPieces);
        //        copyPieces2[item.x, item.y] = piece;

        //        outeCount = GetOutePieces(copyPieces2, piece.playerNumber, false).Count;

        //        if (outeCount < 1)
        //        {
        //            ret.Add(item);
        //        }
        //    }
        //}
        return ret;
    }

    //ターン開始
    void StartMode()
    {
        if(!_onceInMethod){
            StartCoroutine(_checkDeck.OpenCheck());//持ちゴマ・使用済タブ全消し
            _checkDeck.CheckNum_Piece();//持ちゴマ個数更新

            //各種ボタンを非表示にする
            _buttonsAnim.SetFloat("Speed", -1.0f);
            _buttonsAnim.SetTrigger("Do");

            _onceInMethod = true;//1回だけの処理をしたフラグ
        }
        CloseAnnounce();

        //勝敗チェック
        foreach (PieceController piece in capturePieces)
        {
            if (piece.typeID == 1)
            {
                nextMode = Mode.Result;
            }
        }

        if (Mode.Result == nextMode)
        {
            //textTurnInfo.text = "";
            //buttonRematch.gameObject.SetActive(true);
            //buttonTitle.gameObject.SetActive(true);

            //現時点では、決着時の操作プレイヤーを勝者としているが、カード効果による決着時には？
            winner = Convert.ToBoolean(nowPlayer);
            Debug.Log("決着");
            StartCoroutine(ResultMode());
        }
        else{
            //何もなければ通常モード
            nextMode = Mode.Draw;

            //ChangeTurn生成
            GameObject changeTurnObject = Instantiate(_changeTurnObj, _instParent);

            //秘密ゾーン移動
            MoveSecretPiece();

            //ターンスキップ
            if (isSkipTurn1 == true)
            {
                if (nowPlayer == 0)
                {
                    nextMode = Mode.TurnChange;
                    Destroy(changeTurnObject);
                }
            }
            if (isSkipTurn2 == true)
            {
                if (nowPlayer == 1)
                {
                    nextMode = Mode.TurnChange;
                    Destroy(changeTurnObject);
                }
            }

            //自駒無敵リセット
            if (nowPlayer == 0)
            {
                //自駒取られない状態リセット
                if (invincibleAllCount1 > 0)
                {
                    invincibleAllCount1--;
                    if (invincibleAllCount1 == 0)
                    {
                        isInvincibleAll1 = false;
                    }
                }
            }
            if (nowPlayer == 1)
            {
                //自駒取られない状態リセット
                if (invincibleAllCount2 > 0)
                {
                    invincibleAllCount2--;
                    if (invincibleAllCount2 == 0)
                    {
                        isInvincibleAll1 = false;
                    }
                }
            }
        }
    }

    //カードを引くモード
    void DrawMode()
    {
        if (isSkipDraw1 == true)
        {
            if (nowPlayer == 0)
            {
                nextMode = Mode.Select;
                isSkipDraw1 = false;
                GameObject changeTurnObj = GameObject.Find("ChangeTurn(Clone)");
                Destroy(changeTurnObj);
            }
            else
            {
                nextMode = Mode.CardSelect;
                _cardManager.DrawCard();
            }
        }
        else if (isSkipDraw2 == true)
        {
            if (nowPlayer == 1)
            {
                nextMode = Mode.Select;
                isSkipDraw2 = false;
                GameObject changeTurnObj = GameObject.Find("ChangeTurn(Clone)");
                Destroy(changeTurnObj);
            }
            else
            {
                nextMode = Mode.CardSelect;
                _cardManager.DrawCard();
            }
        }
        else
        {
            nextMode = Mode.CardSelect;
            _cardManager.DrawCard();
        }

        if (isProxyCard == true)
        {
            ExchangePlayerNumber();
            isProxyCard = false;
            isProxyCardReset = true;
        }
    }

    //カードを選択するモード
    /*
    void CardSelectMode()
    {
        _skipCardButton.SetActive(true);

        GameObject card = null;

        if (_mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            RaycastHit hit = new RaycastHit();

            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Card")
            {
                card = hit.collider.gameObject;
                _cardManager.SelectCard(card);
                nextMode = Mode.CardOpen;
            }
        }
    }
    */

    //カードを公開するモード
    void CardOpenMode()
    {
        if (_mouse.leftButton.wasPressedThisFrame)
        {
            _cardManager.ReturnCard();
            nextMode = Mode.CardPlay;
        }
    }

    //カードをプレイするモード
    void CardPlayMode()
    {
        //仮レベルアップ
        if (isTempEvol == true)
        {
            tempEvolPiece.Evolution(true);
        }
    }

    //ユニットとタイルを選択するモード
    void SelectMode()
    {
        if(!_onceInMethod){
            OpenAnnounce(_explainData.list[0]);//操作案内表示
            _checkDeck.CheckNum_Trush();//使用済カード枚数更新
            AllSleepCheck(nowPlayer);

            //各種ボタンを表示する
            _buttonsAnim.SetFloat("Speed", 1.0f);
            _buttonsAnim.SetTrigger("Do");

            _onceInMethod = true;//1回だけの処理をしたフラグ
        }

        if (isProxyCardReset == true)
        {
            ExchangePlayerNumber();
            isProxyCardReset = false;
        }

        GameObject tile = null;

        //プレイヤー処理
        if (_mouse.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = Camera.main.ScreenPointToRay(mousePosition);
            //駒にも当たり判定があるのでヒットした全てのオブジェクト情報を取得(駒を貫通して盤面をレイで取得、その上にある駒を取得するというやり方)
            foreach (RaycastHit hit in Physics.RaycastAll(ray))
            {
                PieceController hitPiece = hit.transform.GetComponent<PieceController>();
                //持ち駒
                if (hitPiece != null && PieceController.FieldStatus.Captured == hitPiece.fieldStatus)
                {
                    piece = hitPiece;
                }
                //タイル選択と上に乗ってる駒
                else if (_tiles.ContainsValue(hit.transform.gameObject))
                {
                    tile = hit.transform.gameObject;
                    //タイルから駒を探す
                    foreach (var item in _tiles)
                    {
                        if (item.Value == tile)
                        {
                            piece = _pieces[item.Key.x, item.Key.y];
                        }
                    }
                    break;
                }
            }
        }

        //なにも選択されていなければ処理しない
        if (tile == null && piece == null)
        {
            return;
        }

        //移動先選択
        if (tile && _selectPiece && _moveTiles.ContainsKey(tile))
        {
            _audio.SE_UI_Play(AudioManager.WhichSE.Done);
            if(_selectPiece.fieldStatus == PieceController.FieldStatus.Captured){
                //召喚エフェクト
                ParticleOperator po = Instantiate(effectList.list[4], tile.transform).GetComponent<ParticleOperator>();
                po.PlayParticle(summonSE);
            }
            else{
                //移動エフェクト
                //_audio.SE_Play(moveSE[UnityEngine.Random.Range(0, moveSE.Length)]);//コマ移動音
                ParticleOperator po = Instantiate(effectList.list[0], tile.transform).GetComponent<ParticleOperator>();
                po.PlayParticle(moveSE[UnityEngine.Random.Range(0, moveSE.Length)]);
            }
            nextMode = MovePiece(_selectPiece, _moveTiles[tile]);
        }

        //ユニット選択
        else if (piece != null)
        {
            bool isPlayer = nowPlayer == piece.playerNumber;
            if (isPlayer == true && piece.pieceAbnormal != PieceController.PieceAbnormal.Sleep)
            {
                _audio.SE_UI_Play(AudioManager.WhichSE.Done);
                SelectCursors(piece, isPlayer);
            }
        }
        piece = null;
    }

    //ターン変更
    void TurnChangeMode()
    {
        //仮レベルアップ終了
        if (isTempEvol == true)
        {
            tempEvolPiece.Evolution(false);
            isTempEvol = false;
            tempEvolPiece = null;
        }

        //無敵やスキップのリセット
        if (nowPlayer == 0)
        {
            //キング無敵リセット
            if (invincibleCount2 > 0)
            {
                invincibleCount2--;
                if (invincibleCount2 == 0)
                {
                    isInvincibleKing2 = false;
                }
            }
            //スキップリセット
            if (skipTurnCount2 > 0)
            {
                skipTurnCount2--;
                if (skipTurnCount2 == 0)
                {
                    isSkipTurn2 = false;
                }
            }
        }
        if (nowPlayer == 1)
        {
            //キング無敵リセット
            if (invincibleCount1 > 0)
            {
                invincibleCount1--;
                if (invincibleCount1 == 0)
                {
                    isInvincibleKing1 = false;
                }
            }
            //スキップリセット
            if (skipTurnCount1 > 0)
            {
                skipTurnCount1--;
                if (skipTurnCount1 == 0)
                {
                    isSkipTurn1 = false;
                }
            }
        }

        //ボタンとカーソルのリセット
        SelectCursors();
        //buttonEvolutionApply.gameObject.SetActive(false);
        //buttonEvolutionCansel.gameObject.SetActive(false);

        isCpu = false;

        //次のプレイヤーへ
        nowPlayer = GetNextPlayer(nowPlayer);

        //経過ターンをカウント
        if (nowPlayer == 0)
        {
            turnCount++;
        }

        nextMode = Mode.Start;
    }

    //次のプレイヤー番号を返す
    public static int GetNextPlayer(int player)
    {
        int next = player + 1;
        if (next >= 2)
        {
            next = 0;
        }
        return next;
    }

    //駒を持ち駒にする
    public void CapturePiece(int player, Vector2Int tileIndex)
    {
        PieceController piece = _pieces[tileIndex.x, tileIndex.y];
        if (piece == null)
        {
            return;
        }
        
        //_audio.SE_Play(_getSE[UnityEngine.Random.Range(0, _getSE.Length)]);//パシーン！！！
        //攻撃エフェクト
        ParticleOperator po = Instantiate(effectList.list[1], piece.transform).GetComponent<ParticleOperator>();
        po.PlayParticle(_getSE[UnityEngine.Random.Range(0, _getSE.Length)]);

        piece.Capture(player);
        capturePieces.Add(piece);
        _pieces[tileIndex.x, tileIndex.y] = null;

        //駒のタグを変更
        if (player == 0)
        {
            piece.tag = "1P";
        }
        if (player == 1)
        {
            piece.tag = "2P";
        }
        _checkDeck.CheckNum_Piece();//持ちゴマ個数更新
    }

    //持ち駒を並べる
    public void AlignCapturePieces(int player)
    {
        //タイプとユニットごとに分ける
        Dictionary<int, List<PieceController>> typePiece = new Dictionary<int, List<PieceController>>();

        foreach (var item in capturePieces)
        {
            if (item.playerNumber != player)
            {
                continue;
            }
            typePiece.TryAdd(item.typeID, new List<PieceController>());
            typePiece[item.typeID].Add(item);
        }

        //タイプごとに並べて一番上だけ表示する
        int poscnt = 0;
        foreach (var item in typePiece)
        {
            if (1 > item.Value.Count)
            {
                continue;
            }
            //置く場所の目印
            GameObject tile = pieceTiles[player][poscnt++];
            tile.SetActive(true);

            //同じ種類の持ち駒を並べる
            for (int i = 0; i < item.Value.Count; i++)
            {
                //リスト内のユニットを取得
                GameObject piece = item.Value[i].gameObject;

                Vector3 pos = tile.transform.position;
                piece.SetActive(false);
                piece.transform.position = pos;
                //１個目以降は非表示
                if (0 < i)
                {
                    piece.SetActive(false);
                }
            }
        }
    }

    //指定された配列をコピーして返す
    public static PieceController[,] GetCopyArray(PieceController[,] ary)
    {
        PieceController[,] ret = new PieceController[ary.GetLength(0), ary.GetLength(1)];
        Array.Copy(ary, ret, ary.Length);
        return ret;
    }

    //指定された配置で王手している駒を返す
    public static List<PieceController> GetOutePieces(PieceController[,] pieces, int player, bool checkOtherPiece = true)
    {
        List<PieceController> ret = new List<PieceController>();

        foreach (var item in pieces)
        {
            if (!item || player == item.playerNumber)
            {
                continue;
            }

            //敵駒1枚
            List<Vector2Int> moveTiles = item.GetMoveTiles(pieces, checkOtherPiece);

            foreach (var tile in moveTiles)
            {
                if (!pieces[tile.x, tile.y])
                {
                    continue;
                }

                if (pieces[tile.x, tile.y].typeID == 1)
                {
                    ret.Add(item);
                }
            }
        }

        return ret;
    }

    //盤上の指定の駒を取得する
    public static PieceController GetPiece(PieceController[,] pieces, int player, int type)
    {
        foreach (var item in pieces)
        {
            if (!item || player != item.playerNumber || type != item.typeID)
            {
                continue;
            }
            return item;
        }
        return null;
    }

    //成るボタン
    public void OnClickEvolutionApply()
    {
        nextMode = Mode.TurnChange;
    }

    //成らないボタン
    public void OnClickEvolutionCansel()
    {
        _selectPiece.Evolution(false);
        OnClickEvolutionApply();
    }

    //指定されたプレイヤー番号の全駒を取得する
    public List<PieceController> GetPieces(int player)
    {
        List<PieceController> ret = new List<PieceController>();

        //全駒のリストを作成する
        List<PieceController> copyPieces = new List<PieceController>(capturePieces);
        
        for (int i = 0; i < _boardWidth; i++)
        {
            for (int j = 0; j < _boardHeight; j++)
            {
                if (_pieces[i, j] != null)
                {
                    copyPieces.Add(_pieces[i, j]);
                }
            }
        }

        foreach (var item in copyPieces)
        {
            if (!item || player != item.playerNumber)
            {
                continue;
            }
            ret.Add(item);
        }

        return ret;
    }

    //選択した駒をインデックスから削除
    public void RemovePiece(PieceController piece)
    {
        for (int i = 0; i < _pieces.GetLength(0); i++)
        {
            for (int j = 0; j < _pieces.GetLength(1); j++)
            {
                if (_pieces[i, j] == piece)
                {
                    _pieces[i, j] = null;
                }
            }
        }
    }

    //選択した駒をインデックスに追加
    public void AddPiece(PieceController piece, Vector2Int tileIndex)
    {
        _pieces[tileIndex.x, tileIndex.y] = piece;
    }

    //秘密ゾーン移動処理
    public void MoveSecretPiece()
    {
        if (nowPlayer == 0)
        {
            for (int i = (mySecrets1.Count - 1); -1 < i; i--)
            {
                SecretZone secretZone = mySecrets1[i].GetComponent<SecretZone>();
                secretZone.SecretMove(0);
            }
            for (int i = (mySecrets2.Count - 1); -1 < i; i--)
            {
                SecretZone secretZone = mySecrets2[i].GetComponent<SecretZone>();
                secretZone.SecretMove(0);
            }
        }
        if (nowPlayer == 1)
        {
            for (int i = (otherSecrets1.Count - 1); -1 < i; i--)
            {
                SecretZone secretZone = otherSecrets1[i].GetComponent<SecretZone>();
                secretZone.SecretMove(1);
            }
            for (int i = (otherSecrets2.Count - 1); -1 < i; i--)
            {
                SecretZone secretZone = otherSecrets2[i].GetComponent<SecretZone>();
                secretZone.SecretMove(1);
            }
        }
    }

    //プレイヤー番号の交換
    public void ExchangePlayerNumber()
    {
        if (nowPlayer == 0)
        {
            nowPlayer = 1;
        }
        else
        {
            nowPlayer = 0;
        }
    }

    //自分の場の駒すべてを進化させる
    public void AllEvolution(int player)
    {
        List<PieceController> piecesList = GetPieces(player);

        for (int i = 0; i < piecesList.Count; i++)
        {
            if (piecesList[i].fieldStatus == PieceController.FieldStatus.OnBoard && piecesList[i].isEvolution == false)
            {
                piecesList[i].Evolution(true);
            }
        }
    }

    //進化している自分の駒全てを退化させる
    public void AllDegeneration(int player)
    {
        List<PieceController> piecesList = GetPieces(player);

        for (int i = 0; i < piecesList.Count; i++)
        {
            if (piecesList[i].fieldStatus == PieceController.FieldStatus.OnBoard && piecesList[i].isEvolution == true)
            {
                piecesList[i].Evolution(false);
            }
        }
    }

    //自分の駒全てを眠らせる
    public void AllSleep(int player, int sleepTurn)
    {
        List<PieceController> piecesList = GetPieces(player);

        for (int i = 0; i < piecesList.Count; i++)
        {
            if (piecesList[i].fieldStatus == PieceController.FieldStatus.OnBoard)
            {
                piecesList[i].pieceAbnormal = PieceController.PieceAbnormal.Sleep;
                //Material newMaterial = new Material(piecesList[i].gameObject.GetComponent<Renderer>().material);
                piecesList[i].gameObject.GetComponent<Renderer>().material = sleepMaterial;
                //newMaterial.color = Color.blue;
                piecesList[i].sleepCount = sleepTurn + 1;
            }
        }
    }

    //自分の場の駒が全て眠っているか確認
    public void AllSleepCheck(int player)
    {
        List<PieceController> piecesList = GetPieces(player);
        int count = 0;

        for (int i = 0; i < piecesList.Count; i++)
        {
            if (piecesList[i].fieldStatus != PieceController.FieldStatus.OnBoard)
            {
                piecesList.RemoveAt(i);
            }
        }

        for (int j = 0; j < piecesList.Count; j++)
        {
            if (piecesList[j].pieceAbnormal == PieceController.PieceAbnormal.None)
            {
                count++;
            }
        }

        if (count == 0)
        {
            nextMode = Mode.TurnChange;
        }
    }

    //キングを取れなくする
    public void InvincibleKing(List<Vector2Int> ret, int player)
    {
        for (int i = 0; i < _pieces.GetLength(0); i++)
        {
            for (int j = 0; j < _pieces.GetLength(1); j++)
            {
                if (_pieces[i, j] != null)
                {
                    if (_pieces[i, j].playerNumber == player && _pieces[i, j].typeID == 1)
                    {
                        Vector2Int outPiece = new Vector2Int(i, j);
                        if (ret.Contains(outPiece))
                        {
                            ret.Remove(outPiece);
                        }
                    }
                }
            }
        }
    }

    //自分の駒を取れなくする
    public void InvincibleAll(List<Vector2Int> ret, int player)
    {
        for (int i = 0; i < _pieces.GetLength(0); i++)
        {
            for (int j = 0; j < _pieces.GetLength(1); j++)
            {
                if (_pieces[i, j] != null)
                {
                    if (_pieces[i, j].playerNumber == player)
                    {
                        Vector2Int outPiece = new Vector2Int(i, j);
                        if (ret.Contains(outPiece))
                        {
                            ret.Remove(outPiece);
                        }
                    }
                }
            }
        }
    }

    //行動できなくなる代わりにキング無敵
    public void BindAndInvincibleKing(int player, int count)
    {
        if (player == 0)
        {
            isInvincibleKing1 = true;
            invincibleCount1 = count;
            isSkipTurn1 = true;
            skipTurnCount1 = count;
        }
        if (player == 1)
        {
            isInvincibleKing2 = true;
            invincibleCount2 = count;
            isSkipTurn2 = true;
            skipTurnCount2 = count;
        }
    }

    //行動できなくなる代わりに無敵
    public void BindAndInvincibleAll(int player, int count)
    {
        if (player == 0)
        {
            isInvincibleAll1 = true;
            invincibleAllCount1 = count;
        }
        if (player == 1)
        {
            isInvincibleAll2 = true;
            invincibleAllCount2 = count;
        }
    }

    //お互いの全ての駒を持ち駒にする
    public void AllCapture()
    {
        for (int x = 0; x < _pieces.GetLength(0); x++)
        {
            for (int y = 0; y < _pieces.GetLength(1); y++)
            {
                PieceController piece = _pieces[x, y];
                if (piece != null)
                {
                    if (piece.fieldStatus == PieceController.FieldStatus.OnBoard && piece.typeID != 1)
                    {
                        Vector2Int index = new Vector2Int(x, y);
                        CapturePiece(piece.playerNumber, index);
                    }
                }
            }
        }

        AlignCapturePieces(0);
        AlignCapturePieces(1);
    }

    //リザルト再戦
    public void OnClickRematch()
    {
        SceneManager.LoadScene("NewBattleScene");
    }

    //リザルトタイトルへ
    public void OnClickTitle()
    {
        SceneManager.LoadScene("TitleScene");
    }

    public void OpenAnnounce(string explain){//操作アナウンスをするObjを生成する
        if(!_isAnnounced){//既にアナウンスバー表示しているか
            _isAnnounced = true;
            _instAnnounce = Instantiate(_announceBarObj, _instParent).GetComponent<AnnounceBar>();
            _instAnnounce.OpenAnnounce(explain);
        }
        else{
            //既に開いていた場合、一度閉じて再度展開
            _instAnnounce.CloseAnnounce();
            _instAnnounce = Instantiate(_announceBarObj, _instParent).GetComponent<AnnounceBar>();
            _instAnnounce.OpenAnnounce(explain);
        }
    }
    public void CloseAnnounce(){//操作アナウンスをするObjを削除する
        if(_isAnnounced){
            _isAnnounced = false;
            _instAnnounce.CloseAnnounce();
        }
    }

    IEnumerator ResultMode(){
        //決着時
        if(!_instedGameOver){
            _instedGameOver = true;
            _checkDeck.getOperate = false;//ポーズメニューとか開けなくする
            _audio.SE_Play(_finishSE[UnityEngine.Random.Range(0, _finishSE.Length)]);//パシーン！！！
            _audio.SE_Play(_finishSE2);
            _count.StopCount();//持ち時間のカウント終了
            _count.StopPlayCount();//試合時間のカウント終了
            //ボタンを受け付けなくする
            Image _image = Instantiate(_panel, _instParent).GetComponent<Image>();
            _camera.Mode_GameSet();
            _image.CrossFadeAlpha(0, 2.0f, false);//Flash演出
            //_audio.ChangePitch(AudioManager.WhichAudio.BGM, 0.5f);
            //yield return new WaitForSeconds(2.0f);
            _audio.BGM_Stop(true);
            yield return new WaitForSeconds(3.0f);
            _audio.SE_Play(_gamesetSE);
            Instantiate(_gameSetObj, _instParent);
            //勝者は1Pなのか2Pなのか取得できるようにしてほしい
        }
    }
}

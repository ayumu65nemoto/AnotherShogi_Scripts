using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController instance;

    [SerializeField]private float _time_TurnChange = 0.5f;//移動時間

    //ズーム対象オブジェクト
    [SerializeField] private Transform _target1;
    [SerializeField] private Transform _target2;
    //ズーム速度
    private float _zoomSpeed = 1f;
    //目標のズーム値
    private float _targetZoom = 30f;
    //元のズーム値
    private float _originZoom = 60f;
    //メインカメラ
    private Camera _mainCamera;
    //メインカメラ初期位置
    private Vector3 _originPos;

    //バトル開始時の位置と回転
    [SerializeField]public Vector3[] _start;
    [SerializeField]private Vector3[] _startMove;

    //各プレイヤーコマ移動時の位置と回転
    [SerializeField]private Vector3[] _playerPos_Move;
    [SerializeField]private Vector3[] _playerRot_Move;

    //各プレイヤーカード選択時の位置と回転
    [SerializeField]private Vector3[] _playerPos_Card;
    [SerializeField]private Vector3[] _playerRot_Card;

    //各プレイヤーひみつゾーンカメラの位置と回転
    [SerializeField]private Vector3[] _playerPos_Sec;
    [SerializeField]private Vector3[] _playerRot_Sec;

    private FieldManager _field;
    private Vector3 _v_Pos;
    private Vector3 _v_Rot;
    private float _t;
    private byte _targetPlayer;
    private bool _move;

    //https://kyoro-s.com/unity-9/#toc4 より
    //コルーチンを止める
    public Coroutine _coroutine;

    void Awake(){
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GetComponent<Camera>();
        _field = FieldManager.instance;
        Mode_StartBattle();
    }

    public void StartCameraMove(Vector3 TargetVec_Pos, Vector3 TargetVec_Rot, float MoveTime){
        Initialize();
        StartCoroutine(MoveCamera(TargetVec_Pos, TargetVec_Rot, MoveTime));
    }

    public void ZoomInSecretZone(int num)
    {
        _originPos = transform.position;
        
        if (num == 0)
        {
            //カメラ位置をターゲットに合わせる
            transform.position = _target1.position + new Vector3(0, 5, 0);
        }
        if (num == 1)
        {
            //カメラ位置をターゲットに合わせる
            transform.position = _target2.position + new Vector3(0, 5, 0);
        }

        //現在のズーム値と目標のズーム値を補完
        float currentZoom = _mainCamera.fieldOfView;
        float newZoom = Mathf.Lerp(currentZoom, _targetZoom, Time.deltaTime * _zoomSpeed);

        //カメラのズーム値を設定する
        _mainCamera.fieldOfView = newZoom;
    }

    public void ResetZoom()
    {
        //カメラ位置を元に戻す
        transform.position = _originPos;

        //現在のズーム値と目標のズーム値を補完
        float currentZoom = _mainCamera.fieldOfView;
        float newZoom = Mathf.Lerp(currentZoom, _originZoom, Time.deltaTime * _zoomSpeed);

        //カメラのズーム値を設定する
        _mainCamera.fieldOfView = newZoom;
    }

    private void Mode_StartBattle(){//バトルスタート時のカメラ
        Initialize();
        transform.position = _start[0];
        transform.rotation = Quaternion.Euler(_start[1]);
        _coroutine = StartCoroutine(MoveCamera(_startMove[0], _startMove[1], 0.5f));
    }

    public void Mode_SelectCard(int player){//カード選択時（ターン交代時）のカメラ
        Initialize();
        _coroutine = StartCoroutine(MoveCamera(_playerPos_Card[player], _playerRot_Card[player], _time_TurnChange));
    }

    public void Mode_MovePiece(int player){//コマ選択と移動時のカメラ
        Initialize();
        _coroutine = StartCoroutine(MoveCamera(_playerPos_Move[player], _playerRot_Move[player], (_time_TurnChange / 2f)));
    }

    public void Mode_SecretZone(int player){
        Initialize();
        _coroutine = StartCoroutine(MoveCamera(_playerPos_Sec[player], _playerRot_Sec[player], (_time_TurnChange / 2f)));
    }

    public void Mode_GameSet(){
        //取られた王のところにズームしてほしい
    }

    IEnumerator MoveCamera(Vector3 TargetVec_Pos, Vector3 TargetVec_Rot, float MoveTime){//カメラ移動を行う
        _move = true;
        while(true){
            if(_move){
                transform.position = Vector3.SmoothDamp(transform.position, TargetVec_Pos, ref _v_Pos, MoveTime);
                transform.rotation = Quaternion.Euler(Vector3.SmoothDamp(transform.rotation.eulerAngles, TargetVec_Rot, ref _v_Rot, MoveTime));
                _t += Time.deltaTime;
                if((_t > MoveTime * 5) && _move){//指定位置へ到着したかを確認したい
                    Initialize();
                    SetGoal(TargetVec_Pos, TargetVec_Rot);
                    yield break;
                }
            }
            else{
                Initialize();
                SetGoal(TargetVec_Pos, TargetVec_Rot);
                yield break;
            }
            yield return null;
        }
    }

    private void Initialize(){
        //https://kyoro-s.com/unity-9/#toc4 より
        if(_coroutine != null){
            StopCoroutine(_coroutine);
            _coroutine = null;
        }
        _move = false;
        _v_Pos = Vector3.zero;
        _v_Rot = Vector3.zero;
        _t = 0;
    }

    private Vector3 SetGoal(Vector3 TargetVec_Pos, Vector3 TargetVec_Rot){//カメラ移動スキップ時、ゴール位置へ瞬間移動させる
        transform.position = TargetVec_Pos;
        transform.rotation = Quaternion.Euler(TargetVec_Rot);
        Debug.Log("カメラ移動完了 Pos: " + TargetVec_Pos + " , Rot: " + TargetVec_Rot);
        return transform.position;
    }
}
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

    public int[] pieceNum { get; set; } = new int[9];//�e�R�}�̏��������ꎞ�ۑ�����ϐ�
    public bool settingPiece { get; set; }//�R�}�̔z�u�ꏊ�����肷��t�F�[�Y��

    private AudioManager _audio;
    private FieldManager _field;
    private float _dead;
    private SaveDataManager _saveDataManager;//�Z�[�u�f�[�^���Ǘ�����X�N���v�g
    private CheckDeckNum _checkDeck;
    private sbyte _selecting = 0;//�I�𒆂̃I�u�W�F�N�g�̔ԍ�
    private byte _pieceMax;//�����S�}������
    private byte _rowNum;//�R�}�̑��s��

    private Button _thisButton;//BG�̃L�����Z���{�^��
    private SVGImage _thisImage;//BG�̉摜

    private float gap = 75.0f;//�R�}����̂���E������

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
        _saveDataManager = SaveDataManager.instance;//�ݒ���e�ꗗ���Z�[�u�f�[�^����擾
        _dead = _saveDataManager.dead;
        _checkDeck = CheckDeckNum.instance;

        _thisButton = GetComponent<Button>();
        _thisImage = GetComponent<SVGImage>();

        _cardManager = CardManager.instance;

        //�����S�}�̔z����g�p���ăR�}����
        for (int i = 0; i < _pieceImage.Length; i++)
        {
            RectTransform temp = (RectTransform)_piecesParent.GetChild(i);

            //�R�}�摜�擾
            _pieceImage[i] = temp.GetChild(0).GetComponent<Image>();
            _pieceImage[i].sprite = _imageData.piece_imageList[i];

            _pieceButton[i] = temp.GetComponent<Button>();

            _numText[i] = _numTextParent.GetChild(i).GetComponent<TextMeshProUGUI>();
        }
        SetPieceNum();

        _pieceMax = (byte)_piecesParent.childCount;
        _rowNum = (byte)Mathf.CeilToInt(_pieceMax / 3);

        //�J�[�\���ʒu����ԍ���ɂ���
        StartCoroutine(WaitInfoReady());
    }

    //public void Pressed_Done(InputAction.CallbackContext context)
    //{//����{�^���������ꂽ���̏���
    //    if (context.performed)
    //    {
    //        //SelectPiece(_selecting);
    //    }
    //}

    public void Pressed_Cancel(InputAction.CallbackContext context)
    {//�L�����Z���{�^���������ꂽ���̏���
        if (context.performed)
        {
            Cancel();
        }
    }

    public void Pressed_Cross(InputAction.CallbackContext context)
    {//�㉺�{�^���������ꂽ���̏����B�X�e�B�b�N����E�A�����͂ɖ��Ή�
        if (context.performed)
        {
            Vector2 value = context.ReadValue<Vector2>();

            if (value.y >= _dead)
            {//�����
                if (Mathf.FloorToInt(_selecting / 3) == 0)
                {
                    _selecting = (sbyte)(((_rowNum * 3) - 3) + (_selecting % 3));//�����s�̍����ֈړ���A�I��ł�����Ɉړ�
                }
                else
                {
                    _selecting -= 3;
                }
            }
            else if (value.y <= -_dead)
            {//������
                _selecting += 3;
                if (_selecting >= _pieceMax)
                {
                    _selecting %= 3;
                }
            }
            else if (value.x >= _dead)
            {//�E����
                if (_selecting % 3 == 2)
                {//��ԉE��
                    _selecting -= 2;//�����s�̈�ԍ���
                }
                else
                {
                    _selecting++;
                }
                //CheckMinMax();
            }
            else if (value.x <= -_dead)
            {//������
                if (_selecting % 3 == 0)
                {//��ԍ���
                    _selecting += 2;//�����s�̈�ԉE��
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
    {//�J�[�\���𓮂���
        float newX = rect.position.x - gap;
        _cursorRect.position = new Vector2(newX, rect.position.y);
        _selecting = (sbyte)rect.GetSiblingIndex();
        _pieceInfo.WriteInfo(_selecting + 1);//�R�}���X�V
        _audio.SE_UI_Play(AudioManager.WhichSE.CursorMove);
        //�����S�}�̔z�񂩂�ID�擾�H
        //_pieceInfo.WriteInfo(ID);//PieceInfo�ɁA�I�𒆂̃R�}����\��������
    }

    public void SelectPieceCard(int type)
    {   //�R�}��I���������̋����E�I�������R�}��ID��Ԃ�
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
        //�����Ŏ����S�}�J���Ă锻��false�ɂ���
        if (!settingPiece)
        {//�R�}����O�̏ꍇ
            _checkDeck.PieceListCancel();
        }
        else
        {//�R�}�����̏ꍇ
            //�����ŃR�}�z�u�ҋ@����
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
            _selecting = (sbyte)(_pieceMax - 1);//��ԍŌ�̂Ƃ���ֈړ��E_selecting��0�n�܂�Ȃ̂�-1
        }
    }

    IEnumerator WaitInfoReady()
    {
        while (true)
        {
            if (_pieceInfo.ready)
            {//PieceInfo�̏��������I�������
                RectTransform rect = (RectTransform)_piecesParent.GetChild(0);//��ԍ���̃R�}���擾
                float newX = rect.position.x - gap;//�ʒu�𒲐�
                _cursorRect.position = new Vector2(newX, rect.position.y);//�J�[�\���ʒu���R�}�̍����ֈړ�
                _pieceInfo.WriteInfo(_selecting + 1);//�R�}���X�V�E�L���O�������悤�ɂ���
                yield break;
            }
            else
            {
                yield return null;
            }
        }
    }

    public void SetPieceNum()
    {   //�e������̏��������X�V���܂�
        pieceNum = new int[9];
        for (int i = 0; i < _field.capturePieces.Count; i++)
        {
            if (_field.capturePieces[i].playerNumber == _field.nowPlayer)
            {//���̃R�}������v���C���[�̂��̂ł������ꍇ
                pieceNum[(_field.capturePieces[i].typeID - 1)]++;//�R�}ID�ɊY������J�E���g�ϐ��ɉ��Z�EID��1�n�܂�Ȃ̂Ŕz��ɍ��킹��-1
            }
        }
        for (int i = 0; i < 9; i++)
        {
            _numText[i].text = "�~" + pieceNum[i].ToString("00");//�e�R�}�̏�������pieceNum�����������
        }
    }
}

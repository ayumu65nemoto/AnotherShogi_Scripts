using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyPieceCard : MonoBehaviour
{
    private MyPieceListCard _piecelistScript;
    private RectTransform _rect;
    private AudioManager _audio;
    //この駒のID
    [SerializeField]
    public int typeID;

    // Start is called before the first frame update
    void Start()
    {
        _piecelistScript = MyPieceListCard.instance;
        _rect = GetComponent<RectTransform>();
        //_audio = AudioManager.instance;
    }

    public void ButtonClicked()
    {   //コマが押された時
        _piecelistScript.SelectPieceCard(typeID);//自分の番号をわたす
    }

    public void ButtonEnter()
    {
        //_audio.SE_UI_Play(AudioManager.WhichSE.CursorMove);
        _piecelistScript.MoveCursor(_rect);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Effect13 : MonoBehaviour
{
    private CardManager _cardManager;
    private FieldManager _fieldManager;

    private Mouse _mouse;
    private string _explain = "カードを　確認してください";

    ///<summary>
    ///山札の上から5枚を見る
    ///</summary>

    // Start is called before the first frame update
    void Start()
    {
        _cardManager = CardManager.instance;
        _fieldManager = FieldManager.instance;
        _mouse = Mouse.current;

        _cardManager.OpenFiveCard();
        this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
        _fieldManager.trashPosition += 0.01f;
        _fieldManager.OpenAnnounce(_explain);
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            if (_mouse.rightButton.wasPressedThisFrame || _mouse.leftButton.wasPressedThisFrame)
            {
                _cardManager.ReturnFiveCard();
                Destroy(GetComponent<Effect13>());
            }
        }
    }
}

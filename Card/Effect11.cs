using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect11 : MonoBehaviour
{
    private CardManager _cardManager;
    private FieldManager _fieldManager;

    ///<summary>
    ///使用済みカード全てを山札の一番下に戻す
    ///</summary>

    // Start is called before the first frame update
    void Start()
    {
        _cardManager = CardManager.instance;
        _fieldManager = FieldManager.instance;

        _cardManager.ResetDeck();
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect11>());
            _fieldManager.nextMode = FieldManager.Mode.Select;
        }
    }
}

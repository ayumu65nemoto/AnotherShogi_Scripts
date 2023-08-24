using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect12 : MonoBehaviour
{
    private CardManager _cardManager;
    private FieldManager _fieldManager;

    ///<summary>
    ///最後に使われたカードの効果をもう一度発動させる
    ///</summary>

    // Start is called before the first frame update
    void Start()
    {
        _cardManager = CardManager.instance;
        _fieldManager = FieldManager.instance;

        if (_cardManager.usedCardIndexHistory != null)
        {
            if (_cardManager.usedCardIndexHistory.Count > 1)
            {
                _cardManager.ReverseUsedCard();
            }
            else
            {
                _fieldManager.nowMode = FieldManager.Mode.CardPlay;
                _fieldManager.nextMode = FieldManager.Mode.Select;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect12>());
        }
    }
}

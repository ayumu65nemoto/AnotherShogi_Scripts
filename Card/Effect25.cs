using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect25 : MonoBehaviour
{
    ///<summary>
    ///相手は２ターン　行動できないただしその間相手の王を取ることはできない
    ///</summary>
    
    private FieldManager _fieldManager;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;

        if (_fieldManager.nowPlayer == 0)
        {
            _fieldManager.BindAndInvincibleKing(1, 3);
        }
        else if (_fieldManager.nowPlayer == 1)
        {
            _fieldManager.BindAndInvincibleKing(0, 3);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect25>());
            _fieldManager.nextMode = FieldManager.Mode.Select;
        }
    }
}

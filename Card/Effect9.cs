using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect9 : MonoBehaviour
{
    private FieldManager _fieldManager;

    ///<summary>
    ///相手は次のターン カードを引けない
    ///</summary>

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;
        if (_fieldManager.nowPlayer == 0)
        {
            _fieldManager.isSkipDraw2 = true;
        }
        else
        {
            _fieldManager.isSkipDraw1 = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect9>());
            _fieldManager.nextMode = FieldManager.Mode.Select;
        }
    }
}

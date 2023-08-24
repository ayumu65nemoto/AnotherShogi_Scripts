using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect26 : MonoBehaviour
{
    ///<summary>
    ///このターン何もできないが次の自分のターンまで自分のコマが相手に取られることは無い
    ///</summary>
    
    private FieldManager _fieldManager;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;

        if (_fieldManager.nowPlayer == 0)
        {
            _fieldManager.BindAndInvincibleAll(0, 1);
        }
        else if (_fieldManager.nowPlayer == 1)
        {
            _fieldManager.BindAndInvincibleAll(1, 1);
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
            _fieldManager.nextMode = FieldManager.Mode.TurnChange;
        }
    }
}

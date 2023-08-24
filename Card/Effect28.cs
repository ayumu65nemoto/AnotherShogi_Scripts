using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect28 : MonoBehaviour
{
    ///<summary>
    ///自分と相手はキングスライム以外の全てのコマをそれぞれの持ちゴマにする
    ///</summary>
    
    private FieldManager _fieldManager;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;
        _fieldManager.AllCapture();
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect28>());
            _fieldManager.nextMode = FieldManager.Mode.Select;
        }
    }
}

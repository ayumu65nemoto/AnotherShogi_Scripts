using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect18 : MonoBehaviour
{
    ///<summary>
    ///自分のコマ全てを進化させる
    ///</summary>
    
    private FieldManager _fieldManager;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;

        if (_fieldManager.nowPlayer == 0)
        {
            _fieldManager.AllEvolution(0);
        }
        else if (_fieldManager.nowPlayer == 1)
        {
            _fieldManager.AllEvolution(1);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect18>());
            _fieldManager.nextMode = FieldManager.Mode.Select;
        }
    }
}

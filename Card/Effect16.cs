using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect16 : MonoBehaviour
{
    ///<summary>
    ///相手の進化しているコマ全てを退化させる
    ///</summary>
    
    private FieldManager _fieldManager;

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;

        if (_fieldManager.nowPlayer == 0)
        {
            _fieldManager.AllDegeneration(1);
        }
        else if (_fieldManager.nowPlayer == 1)
        {
            _fieldManager.AllDegeneration(0);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect16>());
            _fieldManager.nextMode = FieldManager.Mode.Select;
        }
    }
}

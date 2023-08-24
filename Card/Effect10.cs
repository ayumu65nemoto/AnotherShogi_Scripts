using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effect10 : MonoBehaviour
{
    private FieldManager _fieldManager;
    
    ///<summary>
    ///次に自分が引いたカードを　相手が引いたことにすることができる
    ///</summary>

    // Start is called before the first frame update
    void Start()
    {
        _fieldManager = FieldManager.instance;
        //_fieldManager.isProxyCard = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.CardPlay)
        {
            this.gameObject.transform.position = new Vector3(7f, (0f + _fieldManager.trashPosition), 0);
            _fieldManager.trashPosition += 0.01f;
            Destroy(GetComponent<Effect10>());
            _fieldManager.nextMode = FieldManager.Mode.Select;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class SelectPieceEvent : MonoBehaviour
{
    Color originalColor;

    void Start()
    {
        // オブジェクトの元の色を保存する
        originalColor = GetComponent<Renderer>().material.color;
    }

    void OnMouseEnter()
    {
        // オブジェクトの色を変更する
        GetComponent<Renderer>().material.color = Color.red;
    }

    void OnMouseExit()
    {
        // オブジェクトの元の色に戻す
        GetComponent<Renderer>().material.color = originalColor;
    }
}

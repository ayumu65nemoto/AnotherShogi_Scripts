using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    [SerializeField] private database_Card _cardData;
    [SerializeField] private GameObject _backCanvas;
    [SerializeField] private GameObject _cardCanvas;
    [SerializeField] private Image _cardColor;
    [SerializeField] private Text _cardName;
    [SerializeField] private Image _cardIcon;
    [SerializeField] private TextMeshProUGUI _cardEffect;
    public int cardNumber;

    public void CardSet(int number)
    {
        cardNumber = number;
        _cardName.text = _cardData.list[number].Name;
        _cardEffect.text = _cardData.list[number].Explain;
        //SetTextEllipsis(_cardEffect, _cardEffect.text);
    }

    //マウスを載せた時
    private void OnMouseEnter()
    {
        _cardColor.color = Color.red;
    }

    //マウスを離したとき
    private void OnMouseExit()
    {
        _cardColor.color = Color.white;
    }

    //テキスト省略 TMPにすると使えない
    //void SetTextEllipsis(TextMeshProUGUI text, string value)
    //{
    //    TextGenerator generator = new TextGenerator();
    //    RectTransform rectTransform = text.GetComponent<RectTransform>();
    //    TextGenerationSettings settings = text.GetGenerationSettings(rectTransform.rect.size);
    //    generator.Populate(value, settings);

    //    int characterCountVisible = generator.characterCountVisible;
    //    string updatedText = value;
    //    if (value.Length > characterCountVisible && characterCountVisible - 3 > 0)
    //    {
    //        updatedText = value.Substring(0, characterCountVisible - 3);
    //        updatedText += "...";
    //    }

    //    text.text = updatedText;
    //}

    public void SwitchCanvas()
    {
        _backCanvas.SetActive(false);
        _cardCanvas.SetActive(true);
    }

    public void ReturnCanvas()
    {
        _backCanvas.SetActive(true);
        _cardCanvas.SetActive(false);
    }
}

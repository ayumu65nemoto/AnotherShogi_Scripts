using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public static CardManager instance;
    //カードプレハブ
    //[SerializeField] private Card _cardPrefab;
    [SerializeField] private List<Card_2D> _cardsUI = new List<Card_2D>();
    //デッキ枚数
    [SerializeField]public int deckSize = 30;
    //デッキのリスト
    //public List<GameObject> deckList = new List<GameObject>();
    public List<int> deckIndex{get; set;} = new List<int>();
    //引いたカードのリスト
    //public List<GameObject> drawnCards = new List<GameObject>();
    public List<int> drawnIndex{get; set;} = new List<int>();
    //使用済みカード
    //public List<GameObject> usedCardList = new List<GameObject>();
    public List<int> usedCardIndex{get; set;} = new List<int>();
    //使用済みカード履歴
    public List<int> usedCardIndexHistory { get; set; } = new List<int>();
    //カードを戻す位置(y座標)
    private float _posY = 1.5f;
    //FieldManager
    private FieldManager _fieldManager;
    
    private CheckDeckNum _checkDeck;

    //カード開示用リスト
    //public List<GameObject> openCards = new List<GameObject>();
    public List<int> openCardIndex{get; set;} = new List<int>();
    //ChangeTurn

    //カードドローした回数（ChangeTurnにて加算）
    public int drawNum{get; set;}

    //共通する説明文を収めたリスト
    public StringData explainData;

    //カード5枚開示用のPrefab
    [SerializeField] private GameObject _findCardsObj;
    //生成したPrefabをいれる
    private GameObject _instObj;
    //カード5枚開示用のリスト
    [SerializeField] private List<Card_2D> _fiveCardsUI = new List<Card_2D>();
    //Prefab生成時の親(Range_UI)
    [SerializeField] private RectTransform _instParent;

    //Effect３～５に渡す持ち駒UI
    [SerializeField] public GameObject myPieceUI;

    void Awake(){
        instance = this;
    }

    void Start(){
        _fieldManager = FieldManager.instance;
        _checkDeck = CheckDeckNum.instance;
    }

    private void Update()
    {
        if (_fieldManager.nowMode == FieldManager.Mode.Draw)
        {
            GameObject[] cardObjects = GameObject.FindGameObjectsWithTag("Card");

            foreach (GameObject cardObject in cardObjects)
            {
                Card_2D cardScript = cardObject.GetComponent<Card_2D>();
                if (cardScript != null)
                {
                    _cardsUI.Add(cardScript);
                }
            }
        }

        if (_fieldManager.nowMode == FieldManager.Mode.TurnChange)
        {
            _cardsUI.Clear();
        }
    }

    //デッキを生成する
    public void GenerateDeck()
    {
        //カードデータのListから、ランダムにカードを選択してデッキに追加する
        for (int i = 27; i < 30; i++)
        {
            deckIndex.Add(i);
        }

        ShuffleDeck();
    }

    private void ShuffleDeck()
    {
        for (int i = 0; i < deckIndex.Count; i++)
        {
            int temp = deckIndex[i];
            int randomIndex = UnityEngine.Random.Range(i, deckIndex.Count);
            deckIndex[i] = deckIndex[randomIndex];
            deckIndex[randomIndex] = temp;
        }
    }

    //デッキからカードを引く
    public void DrawCard()
    {
        for (int i = 0; i < 3; i++)
        {
            if (deckIndex.Count > 0)
            {
                _cardsUI[i].CardSet(deckIndex[0]);
                deckIndex.RemoveAt(0);
                drawnIndex.Add(_cardsUI[i].cardNumber);
            }
            else
            {
                ResetDeck();
                ShuffleDeck();
                _cardsUI[i].CardSet(deckIndex[0]);
                deckIndex.RemoveAt(0);
                drawnIndex.Add(_cardsUI[i].cardNumber);
            }
        }
    }

    //カードを選ぶ
    public void SelectCard(int card)
    {
        drawnIndex.Remove(card);
        usedCardIndex.Add(card);
        usedCardIndexHistory.Add(card);
        string scriptName = "Effect" + card.ToString();
        Type scriptType = Type.GetType(scriptName);
        gameObject.AddComponent(scriptType);
    }

    //選ばなかったカードを戻す
    public void ReturnCard()
    {
        for (int i = 0; i < 2; i++)
        {
            if (drawnIndex == null || drawnIndex.Count == 0)
            {
                break;
            }
            deckIndex.Add(drawnIndex[0]);
            drawnIndex.RemoveAt(0);
        }
    }

    //カードをプレイしない場合
    public void SkipCardPlay()
    {
        for (int i = 0; i < 3; i++)
        {
            if (drawnIndex == null || drawnIndex.Count == 0)
            {
                break;
            }
            deckIndex.Add(drawnIndex[0]);
            drawnIndex.RemoveAt(0);
        }

        _fieldManager.nextMode = FieldManager.Mode.Select;
    }

    //使用済みカードをデッキに戻す
    public void ResetDeck()
    {
        for (int i = 0; i < usedCardIndex.Count; i++)
        {
            deckIndex.Add(usedCardIndex[0]);
            usedCardIndex.RemoveAt(0);
        }
    }

    //最後に使われたカードを再使用
    public void ReverseUsedCard()
    {
        int cardNum = usedCardIndexHistory[usedCardIndexHistory.Count - 2];
        //効果スクリプトをアタッチ
        string scriptName = "Effect" + cardNum.ToString();
        Type scriptType = Type.GetType(scriptName);
        gameObject.AddComponent(scriptType);
    }

    //カードを上から５枚開示
    public void OpenFiveCard()
    {
        _instObj = Instantiate(_findCardsObj, _instParent);
        Transform cardsTransform = _instObj.transform.Find("Cards");
        for (int i = 0; i < cardsTransform.childCount; i++)
        {
            Card_2D card = cardsTransform.GetChild(i).GetComponent<Card_2D>();
            _fiveCardsUI.Add(card);
        }

        for (int i = 0; i < 5; i++)
        {
            if (deckIndex.Count > 0)
            {
                _fiveCardsUI[i].CardSet(deckIndex[0]);
                deckIndex.RemoveAt(0);
                openCardIndex.Add(_cardsUI[i].cardNumber);
            }
            else
            {
                ResetDeck();
                ShuffleDeck();
                _fiveCardsUI[i].CardSet(deckIndex[0]);
                deckIndex.RemoveAt(0);
                openCardIndex.Add(_cardsUI[i].cardNumber);
            }
        }
    }

    //開示したカードを元に戻す
    public void ReturnFiveCard()
    {
        //開示したカードを全てデッキに戻す
        for (int i = 0; i < 5; i++)
        {
            if (openCardIndex == null || openCardIndex.Count == 0)
            {
                break;
            }
            deckIndex.Insert(0, openCardIndex[0]);
            openCardIndex.RemoveAt(0);
        }

        Destroy(_instObj);
        _fiveCardsUI.Clear();
        _fieldManager.nextMode = FieldManager.Mode.Select;
    }
}

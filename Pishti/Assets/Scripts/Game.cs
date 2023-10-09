using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading;

public class Game : MonoBehaviour
{
    public static Game instance;

    public GameObject CardPlacePreFab;
    private GameObject cardPlaceObj;
    private CardPlace cardPlace;

    public GameObject InHandRow;
    public GameObject CollectedCardsRow;
    public GameObject MiddleRow;

    public TextMeshProUGUI botScoreText;
    public TextMeshProUGUI playerScoreText;
    public TextMeshProUGUI freeCardsText;

    public List<Card> freeCards;
    public List<Card> botCards;
    public List<Card> playerCards;
    public List<Card> middleDeck;

    private int playerScore = 0;
    private int botScore = 0;
    private int playerCollectedCardCount = 0;
    private int botCollectedCardCount = 0;
    private int riskThreshold = 3;
  
    private bool playerTurn = true;
    private Card openCard;

    private void Awake()
    {
        instance = this;
    }
    public void generateBoard()
    {
        freeCards = GenerateAllCards();
        give4toMiddle();
        newTurn();
    }
    public void newTurn()
    {
        if(freeCards.Count == 0)
        {   
            if(playerCollectedCardCount > botCollectedCardCount) 
            {
                Debug.Log($"Game ends! Player Score: {playerScore+3}, Bot Score: {botScore}");
            }
            else if (playerCollectedCardCount == botCollectedCardCount)
            {
                Debug.Log($"Game ends! Player Score: {playerScore}, Bot Score: {botScore}");
            }
            else
            {
                Debug.Log($"Game ends! Player Score: {playerScore}, Bot Score: {botScore+3}");
            }          
        }
        else
        {
            Debug.Log("New Turn!");
            give4toBot();
            give4toPlayer();
        }
        freeCardsText.SetText($"{freeCards.Count}");
    }

    private void give4toMiddle() 
    {
        middleDeck = DrawRandomCards(freeCards, 4);
        for(int i=0; i<4; i++)
        {
            cardPlaceObj = Instantiate(CardPlacePreFab, MiddleRow.transform);
            cardPlace = cardPlaceObj.GetComponent<CardPlace>();
            
            cardPlace.card = middleDeck[i];
            cardPlace.icon.sprite = Resources.Load<Sprite>("Cards/card back red");
            cardPlace.button.interactable = false;

            if (i == 3)
            {           
                cardPlace.icon.sprite = GetCardSprite(cardPlace.card);
            } 
        }
    }
    private void give4toBot() 
    {
        botCards = DrawRandomCards(freeCards, 4);
    }
    private void give4toPlayer() 
    {
        playerCards = DrawRandomCards(freeCards, 4);
        for (int i = 0; i < 4; i++)
        {
            cardPlaceObj = Instantiate(CardPlacePreFab, InHandRow.transform);
            cardPlace = cardPlaceObj.GetComponent<CardPlace>();

            cardPlace.card = playerCards[i];
            cardPlace.icon.sprite = GetCardSprite(cardPlace.card);
            cardPlace.button.interactable = true;
        }
    }
    
    public void playCard(CardPlace cardPlace)
    {      
        cardPlace.button.interactable = false; 
        if(playerTurn == true) { Debug.Log($"Played Card: {cardPlace.card.ToString()} by player"); }
        else                   { Debug.Log($"Played Card: {cardPlace.card.ToString()} by bot"); }

        if (middleDeck.Count != 0 && cardPlace.card.rank == Card.Rank.Jack) //JACK
        {
            if (playerTurn == true)
            {
                playerCollectedCardCount += middleDeck.Count;
                playerScore += getScore(middleDeck);
                playerScoreText.SetText($"Score: {playerScore}");
                playerCards.Remove(cardPlace.card);

                playerTurn = false;
                
                Debug.Log("Jack takes all for player");

                clearMiddle(playerTurn);

                Transform parentTransform = CollectedCardsRow.transform;
                Transform childTransform = cardPlace.transform;

                childTransform.SetParent(parentTransform);
            }
            else
            {
                botCollectedCardCount += middleDeck.Count;
                botScore += getScore(middleDeck);
                botScoreText.SetText($"Score: {botScore}");
                botCards.Remove(cardPlace.card);
                
                playerTurn = true;
         
                Debug.Log("Jack takes all for bot");
                
                clearMiddle(playerTurn);
                if (botCards.Count == 0) { newTurn(); }
            }
        }
        else if (middleDeck.Count != 0 && cardPlace.card.rank == middleDeck[middleDeck.Count-1].rank) //MATCH
        {
            if (middleDeck.Count == 1) // PISHTI
            {
                if (playerTurn == true)
                {
                    playerCollectedCardCount += 2;
                    playerScore += 10;
                    playerScoreText.SetText($"Score: {playerScore}");
                    playerCards.Remove(cardPlace.card);
                    
                    playerTurn = false;
                    
                    Debug.Log("Pishti for player");

                    clearMiddle(playerTurn);

                    Transform parentTransform = CollectedCardsRow.transform;
                    Transform childTransform = cardPlace.transform;

                    childTransform.SetParent(parentTransform);
                }
                else
                {
                    botCollectedCardCount += 2;
                    botScore += 10;
                    botScoreText.SetText($"Score: {botScore}");
                    botCards.Remove(cardPlace.card);

                    playerTurn = true;

                    Debug.Log("Pishti for bot");

                    clearMiddle(playerTurn);
                    if (botCards.Count == 0) { newTurn(); }
                }
            }        
            else //REGULAR MATCH
            {
                if(playerTurn == true)
                {
                    playerCollectedCardCount += middleDeck.Count;
                    playerScore += getScore(middleDeck);
                    playerScoreText.SetText($"Score: {playerScore}");
                    playerCards.Remove(cardPlace.card);

                    playerTurn = false;

                    Debug.Log("Match for player");

                    clearMiddle(playerTurn);

                    Transform parentTransform = CollectedCardsRow.transform;
                    Transform childTransform = cardPlace.transform;

                    childTransform.SetParent(parentTransform);
                }
                else
                {
                    botCollectedCardCount += middleDeck.Count;
                    botScore += getScore(middleDeck);
                    botScoreText.SetText($"Score: {botScore}");
                    botCards.Remove(cardPlace.card);

                    playerTurn = true;

                    Debug.Log("Match for bot");

                    clearMiddle(playerTurn);
                    if (botCards.Count == 0) { newTurn(); }
                }
            }
        }
        else //NO MATCH, JUST ADD CARD
        {
            middleDeck.Add(cardPlace.card);
            openCard = cardPlace.card;

            Transform parentTransform = MiddleRow.transform;
            Transform childTransform = cardPlace.transform;

            childTransform.SetParent(parentTransform);

            if(playerTurn == true)
            {
                playerCards.Remove(cardPlace.card);
                playerTurn = false;
            }
            else
            {
                botCards.Remove(cardPlace.card);
                playerTurn = true;

                if (botCards.Count == 0) { newTurn(); }
            }
        }
             
        if (!playerTurn && botCards.Count > 0) //bot's play
        {
            int randomIndex = Random.Range(0, botCards.Count);
            
            cardPlaceObj = Instantiate(CardPlacePreFab, MiddleRow.transform);
            cardPlace = cardPlaceObj.GetComponent<CardPlace>();
            
            if(botCards.Find(card => card.rank == Card.Rank.Jack) != null &&
               middleDeck.Count > riskThreshold)
            {
                cardPlace.card = botCards.Find(card => card.rank == Card.Rank.Jack);
                botCards.Remove(cardPlace.card);
            }
            else if(botCards.Find(card => card.rank == openCard.rank) != null)
            {
                cardPlace.card = botCards.Find(card => card.rank == openCard.rank);
                botCards.Remove(cardPlace.card);
            }
            else
            {
                cardPlace.card = botCards[randomIndex];
                botCards.RemoveAt(randomIndex);
            }
       
            cardPlace.button.interactable = false;
            cardPlace.icon.sprite = GetCardSprite(cardPlace.card);
           
            playCard(cardPlace); // Recursive call to play the bot's card.
        }
    }

    public void clearMiddle(bool playerTurn)
    {   
        middleDeck = new List<Card>();
        int childCount = MiddleRow.transform.childCount;

        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = MiddleRow.transform.GetChild(i);
            CardPlace _cardPlace = child.GetComponent<CardPlace>();
            _cardPlace.icon.sprite = GetCardSprite(_cardPlace.card);
          
            if (playerTurn == false)
            {
                child.SetParent(CollectedCardsRow.transform);
            }
            else
            {
                Destroy(child.gameObject);
            }       
        }
        Debug.Log("Middle deck/row cleared!");
    }

    private int getScore(List<Card> hand)
    {
        int score = 0;
        foreach (Card card in hand)
        {
            if(card.suit == Card.Suit.Spades && card.rank == Card.Rank.Two)
            {
                score+=2;
            }
            else if(card.suit == Card.Suit.Diamonds && card.rank == Card.Rank.Ten)
            {
                score+=3;
            }
            else if (card.rank == Card.Rank.Jack || card.rank == Card.Rank.Ace )
            {
                score+= 1;
            }
        }
        return score;
    }

    public Sprite GetCardSprite(Card card)
    {
        string fileName = GetCardName(card);
        Sprite cardSprite = Resources.Load<Sprite>("Cards/" + fileName);

        if (cardSprite == null)
        {
            Debug.Log("Card image not found for: " + fileName);
        }

        return cardSprite;
    }

    public static string GetCardName(Card card)
    {
        string rankString = GetRankString(card.rank);
        if (rankString == "jack" ||
            rankString == "queen" ||
            rankString == "king") { return rankString + "_of_" + card.suit.ToString().ToLower() + "2"; }
        else                      { return rankString + "_of_" + card.suit.ToString().ToLower(); }
        
    }

    private static string GetRankString(Card.Rank rank)
    {
        if (rank >= Card.Rank.Two && rank <= Card.Rank.Ten)
        {
            return ((int)rank+2).ToString();
        }
        else
        {
            return rank.ToString().ToLower();
        }
    }

    List<Card> GenerateAllCards()
    {
        List<Card> cards = new List<Card>();

        foreach (Card.Suit suit in System.Enum.GetValues(typeof(Card.Suit)))
        {
            foreach (Card.Rank rank in System.Enum.GetValues(typeof(Card.Rank)))
            {
                Card newCard = new Card(suit, rank);
                cards.Add(newCard);
            }
        }
        return cards;
    }

    List<Card> DrawRandomCards(List<Card> cardList, int numCards)
    {
        List<Card> drawnCards = new List<Card>();

        if (numCards <= 0 || numCards > cardList.Count)
        {
            Debug.LogError("Invalid number of cards to draw!");
            return drawnCards;
        }

        for (int i = 0; i < numCards; i++)
        {
            int randomIndex = Random.Range(0, cardList.Count);

            drawnCards.Add(cardList[randomIndex]);
            cardList.RemoveAt(randomIndex);
        }

        return drawnCards;
    }
    
}


[System.Serializable]
public class Card
{
    public enum Suit
    {
        Hearts,
        Diamonds,
        Clubs,
        Spades
    }

    public enum Rank
    {
        Two, Three, Four, Five, Six, Seven, Eight, Nine, Ten, Jack, Queen, King, Ace
    }

    public Suit suit;
    public Rank rank;

    public Card(Suit suit, Rank rank)
    {
        this.suit = suit;
        this.rank = rank;
    }

    public override string ToString()
    {
        return rank.ToString() + " of " + suit.ToString();
    }
}
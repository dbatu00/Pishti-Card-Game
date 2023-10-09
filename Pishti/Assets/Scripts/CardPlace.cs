using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CardPlace : MonoBehaviour
{
    public Image icon;
    public Button button;
    public Card card;

    private void Start() => button.onClick.AddListener(() => Game.instance.playCard(this));
}

   

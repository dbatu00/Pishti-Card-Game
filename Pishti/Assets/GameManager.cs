using UnityEngine;
public class GameManager : MonoBehaviour
{
    private void Start()
    {
        Game.instance.generateBoard();
    }
}
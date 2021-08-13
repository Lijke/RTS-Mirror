using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using TMPro;
public class GameOverDisplay : MonoBehaviour
{
    [SerializeField] private GameObject gameOverDispalyParent;
    [SerializeField] private TMP_Text winnerNameText;
    private void Start()
    {
        GameOverHandlerer.ClientOnGameOver += ClientHandlerGameOver;
    }

    private void OnDestroy()
    {
        GameOverHandlerer.ClientOnGameOver -= ClientHandlerGameOver;
    }
    public void LeaveGame()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
        }
    }
    private void ClientHandlerGameOver(string winner)
    {
        gameOverDispalyParent.SetActive(true);
        winnerNameText.text = $"{winner} has won!";

    }
}

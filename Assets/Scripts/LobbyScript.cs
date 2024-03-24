using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyScript : MonoBehaviour
{
    [SerializeField]
    private GameObject _choosePanel;
    public void BackToMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void JoinGameKillCount()
    {
        _choosePanel.SetActive(true);
    }

    public void JoinGameTeamBattle()
    {
        _choosePanel.SetActive(true);
    }

    public void JoinGameNoRespawn()
    {
        _choosePanel.SetActive(true);
    }
}

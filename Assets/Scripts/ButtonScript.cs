using System.Collections;
using System.Collections.Generic;
using FishNet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonScript : MonoBehaviour
{
    public void SelectButton(int buttonNumber)
    {
        InstanceFinder.NetworkManager.GetComponent<GameManager>().PlayerColor = buttonNumber;
        InstanceFinder.ClientManager.StartConnection();
        SceneManager.LoadScene(2);
    }
}

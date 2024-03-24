using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using FishNet.Object;
using FishNet;
using FishNet.Managing.Client;
using TMPro;

public class FirstLoader : MonoBehaviour
{
    private string _setName = "";
    public TMP_InputField PlayerNickName;
    
    public void EnterButton()
    {
        if (PlayerNickName.text == "") return;
        var component = InstanceFinder.NetworkManager.GetComponent<GameManager>();
        component.PlayerNickName = _setName;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }
    
    // Update is called once per frame
    public void UpdateText()
    {
        _setName = PlayerNickName.text;
    }
    
    public void ExitButton()
    {
        Application.Quit();
    }
}

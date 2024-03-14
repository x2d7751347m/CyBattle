using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public TMP_InputField playerNickName;

    private string setName = "";

    public GameObject connecting;
    // Start is called before the first frame update
    void Start()
    {
        connecting.SetActive(false);
    }

    // Update is called once per frame
    public void UpdateText()
    {
        setName = playerNickName.text;
        // name to server
    }

    public void EnterButton()
    {
        if (setName != "")
        {
            connecting.SetActive(true);
        }
    }

    public void ExitButton()
    {
        Application.Quit();
    }
}

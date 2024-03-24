using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;

public class KillCount : MonoBehaviour
{
    public List<Kills> HighestKills = new();

    public TMP_Text[] Names;

    public TMP_Text[] KillAmts;

    private GameObject _killCountPanel;

    private GameObject _namesObject;

    private bool _killCountOn = false;
    // Start is called before the first frame update
    void Start()
    {
        _killCountPanel = GameObject.Find("KillCountPanel");
        _namesObject = GameObject.Find("namesBG");
        _killCountPanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            if (!_killCountOn)
            {
                _killCountPanel.SetActive(true);
                _killCountOn = true;
                HighestKills.Clear();
                for (var i = 0; i < Names.Length; i++)
                {
                    HighestKills.Add(new Kills(_namesObject.GetComponent<NickNamesScript>().Names[i].text, Random.Range(1, 2900)));
                }
                HighestKills.Sort();
                for (var i = 0; i < Names.Length; i++)
                {
                    Names[i].text = HighestKills[i].PlayerName;
                    KillAmts[i].text = HighestKills[i].PlayerKills.ToString();
                }
                for (var i = 0; i < Names.Length; i++)
                {
                    if (Names[i].text == "")
                    {
                        Names[i].text = "";
                        KillAmts[i].text = "";
                    }
                }
            }
            else
            {
                _killCountPanel.SetActive(false);
                _killCountOn = false;
            }
        }
    }
}

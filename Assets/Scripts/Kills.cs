using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Kills : IComparable<Kills>
{
    public string PlayerName;
    public int PlayerKills;

    public Kills(string newPlayerName, int newPlayerScore)
    {
        PlayerName = newPlayerName;
        PlayerKills = newPlayerScore;
    }
    public int CompareTo(Kills other)
    {
        return other.PlayerKills - PlayerKills;
    }
}

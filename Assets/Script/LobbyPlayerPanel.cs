using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class LobbyPlayerPanel : MonoBehaviour
{
    [SerializeField] public  Text playerName;
    [SerializeField] public Image playerIcon;
    [SerializeField] public Toggle readyIcon;
    [SerializeField] private Text waitingText;

    internal void UpdatePlayerName(Text playerNameIn)
    {
        playerName = playerNameIn;
    }

    internal void UpdatePlayerIcon(Image playerIconIn)
    {
        playerIcon = playerIconIn;
    }

    internal void UpdateReadyIcon(Toggle readyIconIn)
    {
        readyIcon = readyIconIn;
    }
}

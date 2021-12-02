using MLAPI.Messaging;
using MLAPI.NetworkVariable;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAPI;
using MLAPI.NetworkVariable.Collections;

public class MP_ChatUIScript : NetworkBehaviour
{
    public Text chatText = null;
    public InputField chatInput = null;

    NetworkVariableString messages = new NetworkVariableString("Temp");

    public NetworkList<MP_PlayerInfo> chatPlayers;
    private string playerName = "NA";

    private bool showScore = false;
    public GameObject scoreCardPanel;
    public Text scorePlayerName;
    public Text scoreKills;
    public Text scoreDeaths;

    // Start is called before the first frame update
    void Start()
    {
        messages.OnValueChanged += updateUIClientRpc;
        foreach(MP_PlayerInfo player in chatPlayers)
        {
            if(NetworkManager.LocalClientId == player.networkClientID)
            {
                playerName = player.networkPlayerName;
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.U))
        {
            //show score ui
            showScore = true;
            
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            //hide score ui
            showScore = false;
            
        }
        
        if (showScore)
        {
            scoreCardPanel.SetActive(showScore);
            if (IsOwner)
            {
                UpdateUIScoreServerRPC();
            }
        }
        else
        {
            scoreCardPanel.SetActive(showScore);
        }
    }

   

    public void handleSend()
    {
        if (!IsServer)
        {
            snedMessageServerRpc(chatInput.text);
        }
        else
        {
            messages.Value += "\n" + playerName + " says:  " + chatInput.text;
        }
       
    }

    [ClientRpc]

    private void updateUIClientRpc(string previousValue, string newValue)
    {
        chatText.text += newValue.Substring(previousValue.Length, newValue.Length - previousValue.Length);
    }

    [ServerRpc]

    private void snedMessageServerRpc(string text, ServerRpcParams svrParam = default)
    {
        foreach (MP_PlayerInfo player in chatPlayers)
        {
            if (svrParam.Receive.SenderClientId == player.networkClientID)
            {
                playerName = player.networkPlayerName;
            }
        }
        messages.Value += "\n" + playerName + " says:  " + text;
    }

    [ServerRpc]

    private void UpdateUIScoreServerRPC(ServerRpcParams svrParam = default)
    {
        //clear out old scores

        clearUIScoreClientRPC();
        //get each player's info
        GameObject[] currentPlayers = GameObject.FindGameObjectsWithTag("Player");
        foreach(GameObject playerObj in currentPlayers)
        {
            foreach(MP_PlayerInfo playerInfo in chatPlayers)
            {
                if (playerObj.GetComponent<NetworkObject>().OwnerClientId == playerInfo.networkClientID)
                {
                    updateUIScoreClientRPC(playerInfo.networkPlayerName,playerObj.GetComponent<MP_PlayerAttribs>().kills.Value, playerObj.GetComponent<MP_PlayerAttribs>().deaths.Value);
                }
            }
        }
    }

    [ClientRpc]
    private void updateUIScoreClientRPC(string networkPlayerName, int kills, int deaths)
    {
        if (IsOwner)
        {
            scorePlayerName.text += networkPlayerName + "\n";
            scoreKills.text += kills + "\n";
            scoreDeaths.text += deaths + "\n";

        }
    }

    [ClientRpc]
    private void clearUIScoreClientRPC()
    {
        if (IsOwner)
        {
            scorePlayerName.text = "";
            scoreKills.text = "";
            scoreDeaths.text = "";

        }
    }
}

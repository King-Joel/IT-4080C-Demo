using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;
using UnityEngine.UI;
using MLAPI.NetworkVariable.Collections;
using System;
using MLAPI.Connection;
using MLAPI.SceneManagement;
using MLAPI.Messaging;

public class MP_Lobby : NetworkBehaviour
{
    [SerializeField] private LobbyPlayerPanel[] lobbyPlayers;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Button startGameButton;
    private NetworkList<MP_PlayerInfo> nwPlayers = new NetworkList<MP_PlayerInfo>();

    [SerializeField] private GameObject chatPrefab;

    void Start()
    {
        if (IsOwner)
        {
            UpdateConnListServerRpc(NetworkManager.LocalClientId);
            
        }
        NetworkManager.Singleton.OnClientConnectedCallback += HandleClientConnected;

    }

    public override void NetworkStart()
    {

        nwPlayers.OnListChanged += HandlePlayersListChanged;

        if (IsServer)
        {
            NetworkManager.Singleton.OnClientDisconnectCallback += HandleClientDisconnected;
        }
    }

    

    [ServerRpc]

    private void UpdateConnListServerRpc(ulong localClientId)
    {
        nwPlayers.Add(new MP_PlayerInfo(localClientId, PlayerPrefs.GetString("PName"), false));
    }

    public void ReadyButtonToggled()
    {
        ReadyUpServerRPC();
    }

    public void StartGameButtonPressed()
    {
        if (IsServer)
        {
            NetworkSceneManager.OnSceneSwitched += SceneSwitched;
            NetworkSceneManager.SwitchScene("S_MainGame");

        }
        else
        {
            Debug.Log("You are not the host");
        }

    }

    private void SceneSwitched()
    {
        GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");

        foreach (MP_PlayerInfo tmpClient in nwPlayers)
        {
            UnityEngine.Random.InitState((int)System.DateTime.Now.Ticks);
            int index = UnityEngine.Random.Range(0, spawnPoints.Length);
            GameObject currentPoint = spawnPoints[index];

            GameObject playerSpawn = Instantiate(playerPrefab, currentPoint.transform.position, Quaternion.identity);
            playerSpawn.GetComponent<NetworkObject>().SpawnWithOwnership(tmpClient.networkClientID);
            Debug.Log("Player spawned for: " + tmpClient.networkPlayerName);

            //add chat ui
            GameObject chatUISpawn = Instantiate(chatPrefab);
            chatUISpawn.GetComponent<NetworkObject>().SpawnWithOwnership(tmpClient.networkClientID);
            chatUISpawn.GetComponent<MP_ChatUIScript>().chatPlayers = nwPlayers;
        }
    }


    [ServerRpc(RequireOwnership = false)]

    private void ReadyUpServerRPC(ServerRpcParams serverRpcParams = default)
    {
        for(int index = 0; index < nwPlayers.Count; index++)
        {
            if(nwPlayers[index].networkClientID == serverRpcParams.Receive.SenderClientId)
            {
                nwPlayers[index] = new MP_PlayerInfo(nwPlayers[index].networkClientID, nwPlayers[index].networkPlayerName, !nwPlayers[index].networkPlayerReady);
            }
        }
    }

    private void HandleClientConnected(ulong clientId)
    {
        if (IsOwner)
        {
            UpdateConnListServerRpc(clientId);
        }
        Debug.Log("A player has connected ID: " + clientId);
    }

    private void HandleClientDisconnected(ulong clientID)
    {
        for(int index = 0; index < nwPlayers.Count; index++)
        {
            if(clientID == nwPlayers[index].networkClientID)
            {
                nwPlayers.RemoveAt(index);
                break;
            }
        }
    }

    private void HandlePlayersListChanged(NetworkListEvent<MP_PlayerInfo> changeEvent)
    {
        int index = 0;
          foreach(MP_PlayerInfo connectedPlayer in nwPlayers)
          { 
            lobbyPlayers[index].playerName.text = connectedPlayer.networkPlayerName;
            lobbyPlayers[index].readyIcon.SetIsOnWithoutNotify(connectedPlayer.networkPlayerReady);
            index++;
          }

        //Update ui on player disconnected

        for (; index < 3; index++)
        {
            lobbyPlayers[index].playerName.text = "Player Name";
            lobbyPlayers[index].readyIcon.SetIsOnWithoutNotify(false);
        }

        if (IsHost)
        {
            startGameButton.gameObject.SetActive(true);
            startGameButton.interactable = CheckIsEveryOneReady();
        }
    }

    private bool CheckIsEveryOneReady()
    {
        foreach(MP_PlayerInfo player in nwPlayers)
        {
            if (!player.networkPlayerReady)
            {
                return false;

            }
        }
        return true;
    }
}

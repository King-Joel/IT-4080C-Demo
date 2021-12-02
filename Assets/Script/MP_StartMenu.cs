using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAPI;

public class MP_StartMenu : MonoBehaviour
{
    public GameObject startMenu;

    public void HostButtonClicked()
    {
        NetworkManager.Singleton.StartHost();
        startMenu.SetActive(false);
    }

    public void ClientButtonClicked()
    {
        NetworkManager.Singleton.StartClient();
        startMenu.SetActive(false);
    }
}

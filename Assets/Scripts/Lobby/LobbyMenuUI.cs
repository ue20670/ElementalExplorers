using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LobbyMenuUI : NetworkBehaviour
{
    [SerializeField] private GameObject startGameBtn;
    [SerializeField] private GameObject leaveLobbyBtn;
    [SerializeField] private GameObject lobbyText;
    [SerializeField] private GameObject mainMenuUI;
    [SerializeField] private GameObject player1ConnectedBtn;
    [SerializeField] private GameObject player2ConnectedBtn;
    [SerializeField] private GameObject player1ReadyBtn;
    [SerializeField] private GameObject player2ReadyBtn;
    [SerializeField] private Material activatedMat;
    [SerializeField] private Material disabledMat;

    private bool player1Ready = false;
    private bool player2Ready = false;
    private bool player2Connected = false;

    private void Awake()
    {

        NetworkManager.Singleton.OnClientDisconnectCallback += (ulong id) =>
        {
            NetworkManager.Singleton.Shutdown();
            mainMenuUI.SetActive(true);
            gameObject.SetActive(false);
        };

        startGameBtn.GetComponent<UIInteraction>().AddCallback(() =>
        {
            // startGameServerRpc();
            // return;
            if (player1Ready && player2Ready && player2Connected && (NetworkManager.Singleton.IsConnectedClient || IsHost))
            {
                Debug.Log("Sending Start Game Server RPC");
                StartGameServerRpc();
            }
        });

        leaveLobbyBtn.GetComponent<UIInteraction>().AddCallback(() =>
        {
            NetworkManager.Singleton.Shutdown();
            mainMenuUI.SetActive(true);
            gameObject.SetActive(false);
        });

        if (IsHost)
        {
            player1ReadyBtn.GetComponent<UIInteraction>().AddCallback(() =>
            {
                // Flip the ready button and tell the other player that it has happened
                player1Ready = !player1Ready;
                ReadyStatusClientRpc(player1Ready, true);
            });
        }
        else
        {
            player2ReadyBtn.GetComponent<UIInteraction>().AddCallback(() =>
            {
                player2Ready = !player2Ready;
                ReadyStatusClientRpc(player2Ready, false);
            });
        }
    }

    private void OnDisable()
    {
        // When disabled reset all UI elements
        SwitchButtonStyle(player1ReadyBtn, "NOT READY", "READY", false);
        SwitchButtonStyle(player2ReadyBtn, "NOT READY", "READY", false);
        SwitchButtonStyle(player1ConnectedBtn, "DISCONNECTED", "CONNECTED", false);
        SwitchButtonStyle(player2ConnectedBtn, "DISCONNECTED", "CONNECTED", false);
        lobbyText.GetComponentInChildren<TMP_Text>().text = "";
        player1Ready = false;
        player2Ready = false;
        player2Connected = false;
    }

    private void SwitchButtonStyle(GameObject button, string falseText, string trueText, bool on) 
    {
        if (on)
        {
            button.GetComponentInChildren<TMP_Text>().text = trueText;
            button.GetComponent<MeshRenderer>().material = activatedMat;
        }
        else
        {
            button.GetComponentInChildren<TMP_Text>().text = falseText;
            button.GetComponent<MeshRenderer>().material = disabledMat;
        }
    }

    private void Update()
    {
        if (IsHost)
        {
            if (!player2Connected && NetworkManager.Singleton.ConnectedClientsList.Count == 2)
            {
                // Player 2 has connected
                player2Connected = true;
                SwitchButtonStyle(player2ConnectedBtn, "NOT READY", "READY", true);
                HandshakeClientRpc(player1Ready);
            }
            else if (player2Connected && NetworkManager.Singleton.ConnectedClientsList.Count <= 1)
            {
                // Player 2 has disconnected
                player2Connected = false;
                SwitchButtonStyle(player2ConnectedBtn, "NOT READY", "READY", false);
                SwitchButtonStyle(player2ReadyBtn, "NOT READY", "READY", false);
            }
        }
    }


    public void SetLobbyJoinCode(string joinCode)
    {
        if (IsHost)
        {
            SwitchButtonStyle(player1ConnectedBtn, "NOT READY", "READY", true);
        }
        lobbyText.GetComponentInChildren<TMP_Text>().text = joinCode;
        SwitchButtonStyle(player1ReadyBtn, "NOT READY", "READY", player1Ready);
        SwitchButtonStyle(player2ReadyBtn, "NOT READY", "READY", player2Ready);
    }
    
        
    [ServerRpc(RequireOwnership = false)]
    private void StartGameServerRpc()
    {
        NetworkManager.SceneManager.LoadScene("Precompute", LoadSceneMode.Single);
    }

    // Tell player 2 that it has connected and send button status
    [ClientRpc]
    private void HandshakeClientRpc(bool player1Rdy)
    {
        if (!IsHost)
        {
            player2Connected = true;
            player1Ready = player1Rdy;
            SwitchButtonStyle(player1ReadyBtn, "NOT READY", "READY", player1Ready);
            ReadyStatusClientRpc(player2Ready, false);
        }
    }

    // Updates the status of the other player
    [ClientRpc]
    private void ReadyStatusClientRpc(bool newReadyStatus, bool isPlayer1)
    {
        if (isPlayer1)
        {
            player2Ready = newReadyStatus;
        }
        else
        {
            player1Ready = newReadyStatus;
        }
    }
    
    public void DisconnectedFromServer()
    {
        mainMenuUI.SetActive(true);
        gameObject.SetActive(false);
    }
}

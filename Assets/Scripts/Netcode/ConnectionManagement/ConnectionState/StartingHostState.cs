using System;
using Netcode.SessionManagement;
using Unity.Netcode;
using UnityEngine;

namespace Netcode.ConnectionManagement.ConnectionState
{
    /// <summary>
    /// Connection state corresponding to a host starting up. Starts the host when entering the state. If successful,
    /// transitions to the Hosting state, if not, transitions back to the Offline state.
    /// </summary>
    class StartingHostState : OnlineState
    {
        ConnectionMethodBase m_ConnectionMethod;

        public StartingHostState Configure(ConnectionMethodBase baseConnectionMethod)
        {
            m_ConnectionMethod = baseConnectionMethod;
            return this;
        }

        public override void Enter()
        {
            StartHost();
        }

        public override void Exit() { }

        public override void OnClientDisconnect(ulong clientId)
        {
            if (clientId == m_ConnectionManager.NetworkManager.LocalClientId)
            {
                StartHostFailed();
            }
        }

        void StartHostFailed()
        {
            m_ConnectStatusPublisher.Publish(ConnectStatus.StartHostFailed);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Offline);
        }

        public override void OnServerStarted()
        {
            m_ConnectStatusPublisher.Publish(ConnectStatus.Success);
            m_ConnectionManager.ChangeState(m_ConnectionManager.m_Hosting);
        }

        public override void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            var connectionData = request.Payload;
            var clientId = request.ClientNetworkId;
            // This happens when starting as a host, before the end of the StartHost call. In that case, we simply approve ourselves.
            if (clientId == m_ConnectionManager.NetworkManager.LocalClientId)
            {
                var payload = System.Text.Encoding.UTF8.GetString(connectionData);
                var connectionPayload = JsonUtility.FromJson<ConnectionPayload>(payload); // https://docs.unity3d.com/2020.2/Documentation/Manual/JSONSerialization.html

                SessionManager<SessionPlayerData>.Instance.SetupConnectingPlayerSessionData(clientId, connectionPayload.playerId,
                    new SessionPlayerData(clientId, true));

                // connection approval will create a player object for you
                response.Approved = true;
                response.CreatePlayerObject = true;
            }
        }

        async void StartHost()
        {
            try
            {
                await m_ConnectionMethod.SetupHostConnectionAsync();
                Debug.Log($"Created relay allocation");

                // NGO's StartHost launches everything
                if (!m_ConnectionManager.NetworkManager.StartHost())
                {
                    OnClientDisconnect(m_ConnectionManager.NetworkManager.LocalClientId);
                }
            }
            catch (Exception)
            {
                StartHostFailed();
                throw;
            }
        }
    }
}

using System;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.UIElements;

public class HostJoinUI : MonoBehaviour
{
    [SerializeField]
    int maxConnections = 5;
    [SerializeField]
    bool usingUnityRelay = false;
    [SerializeField]
    string joinCode = null;

    [SerializeField]
    UIDocument m_MainMenuUIDocument;

    [SerializeField]
    UIDocument m_InGameUIDocument;

    VisualElement m_MainMenuRootVisualElement;

    VisualElement m_InGameRootVisualElement;

    Button m_HostButton;

    Button m_ServerButton;

    Button m_ClientButton;

    TextField m_IPAddressTextField;

    TextField m_PortTextField;

    void Awake()
    {
        m_MainMenuRootVisualElement = m_MainMenuUIDocument.rootVisualElement;
        m_InGameRootVisualElement = m_InGameUIDocument.rootVisualElement;

        m_HostButton = m_MainMenuRootVisualElement.Query<Button>("HostButton");
        m_ClientButton = m_MainMenuRootVisualElement.Query<Button>("ClientButton");
        m_ServerButton = m_MainMenuRootVisualElement.Query<Button>("ServerButton");
        m_IPAddressTextField = m_MainMenuRootVisualElement.Query<TextField>("IPAddressField");
        m_PortTextField = m_MainMenuRootVisualElement.Query<TextField>("PortField");

        m_HostButton.clickable.clickedWithEventInfo += StartHost;
        m_ServerButton.clickable.clickedWithEventInfo += StartServer;
        m_ClientButton.clickable.clickedWithEventInfo += StartClient;
    }

    void OnDestroy()
    {
        m_HostButton.clickable.clickedWithEventInfo -= StartHost;
        m_ServerButton.clickable.clickedWithEventInfo -= StartServer;
        m_ClientButton.clickable.clickedWithEventInfo -= StartClient;
    }

    void Start()
    {
        ToggleMainMenuUI(true);
        ToggleInGameUI(false);

        // !!!!!!!!
        // Modified for the Relay service usage
        // Checking the used UnityTransport Protocol type
        var transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport;

        if (transport is UnityTransport unityTransport && unityTransport.Protocol.Equals(UnityTransport.ProtocolType.RelayUnityTransport))
        {
            usingUnityRelay = true;
        }

    }

    // !!!!!!!!
    // Modified to accept Relay service
    void StartHost(EventBase obj)
    {
        var result = false;
        if (usingUnityRelay)
        {
            var joinCodeTask = StartHostWithRelay(maxConnections);
            if (joinCodeTask != null)
            {
                print("THE JOIN CODE: " + joinCode);
                result = true;
            }
        }
        else
        {
            SetUtpConnectionData();
            result = NetworkManager.Singleton.StartHost();

        }
        if (result)
        {
            ToggleInGameUI(true);
            ToggleMainMenuUI(false);
        }
    }

    // !!!!!!!!
    // Added method for starting a host while using Relay service, max connections defined in this script (would be better to have closer to NetworkManager)
    public async Task<string> StartHostWithRelay(int maxConnections = 5)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
        Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxConnections);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(allocation, "udp"));// "dtls"));
        var receivedJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);

        joinCode = receivedJoinCode.ToString();
        var scoreUI = FindObjectOfType<ScoreUI>();
        scoreUI.OnJoinCodeReceived(joinCode);

        return NetworkManager.Singleton.StartHost() ? receivedJoinCode : null;
    }

    // !!!!!!!!
    // Modified to accept Relay service
    void StartClient(EventBase obj)
    {
        var result = false;
        if (usingUnityRelay)
        {
            // Join code typed to the IP field if using Unity Relay as the UnityTransport Protocol Type
            joinCode = Sanitize(m_IPAddressTextField.text);
            var joined = StartClientWithRelay(joinCode);
            if (joined != null)
            {
                print("Joined in!: " + joinCode);
                result = true;
            }
        }
        else
        {
            SetUtpConnectionData();
            result = NetworkManager.Singleton.StartClient();
        }

        if (result)
        {
            ToggleInGameUI(true);
            ToggleMainMenuUI(false);
        }
    }

    // !!!!!!!!
    // Added method for starting a client when using Relay service
    public async Task<bool> StartClientWithRelay(string joinCode)
    {
        await UnityServices.InitializeAsync();
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }

        var joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode: joinCode);
        NetworkManager.Singleton.GetComponent<UnityTransport>().SetRelayServerData(new RelayServerData(joinAllocation, "dtls"));
        return !string.IsNullOrEmpty(joinCode) && NetworkManager.Singleton.StartClient();
    }

    void StartServer(EventBase obj)
    {
        SetUtpConnectionData();
        var result = NetworkManager.Singleton.StartServer();
        if (result)
        {
            ToggleInGameUI(true);
            ToggleMainMenuUI(false);
        }
    }

    void ToggleMainMenuUI(bool isVisible)
    {
        m_MainMenuRootVisualElement.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void ToggleInGameUI(bool isVisible)
    {
        m_InGameRootVisualElement.style.display = isVisible ? DisplayStyle.Flex : DisplayStyle.None;
    }

    void SetUtpConnectionData()
    {
        var sanitizedIPText = Sanitize(m_IPAddressTextField.text);
        var sanitizedPortText = Sanitize(m_PortTextField.text);

        ushort.TryParse(sanitizedPortText, out var port);

        var utp = (UnityTransport)NetworkManager.Singleton.NetworkConfig.NetworkTransport;
        utp.SetConnectionData(sanitizedIPText, port);
    }

    /// <summary>
    /// Sanitize user port InputField box allowing only alphanumerics and '.'
    /// </summary>
    /// <param name="dirtyString"> string to sanitize. </param>
    /// <returns> Sanitized text string. </returns>
    static string Sanitize(string dirtyString)
    {
        return Regex.Replace(dirtyString, "[^A-Za-z0-9.]", "");
    }
}

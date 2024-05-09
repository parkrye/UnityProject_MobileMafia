using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using System;
using UnityEngine;

public class MorningSession : Session, IChatClientListener
{
    private string[] _channels = Enum.GetNames(typeof(GameData.PlayerState));
    [SerializeField] private int _channel = 0;
    [SerializeField] private int _historyLengthToFetch;
    private ChatClient _chatClient;
    private ChatAppSettings _chatAppSettings;

    protected override void AwakeSelf()
    {
        if (GetInputField("ChatInputField", out var cInputField))
            cInputField.onSubmit.AddListener(InputChat);
        if (GetButton("EveryoneButton", out var eButton))
            eButton.onClick.AddListener(() => ChangeChatServer(0));
        if (GetButton("DeadmanButton", out var dButton))
            dButton.onClick.AddListener(() => ChangeChatServer(1));
        if (GetButton("SpyButton", out var sButton))
            sButton.onClick.AddListener(() => ChangeChatServer(2));

        _chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();

        Time = 120f;
    }

    void Start()
    {
        _chatClient = new ChatClient(this);
        _chatClient.UseBackgroundWorkerForSending = true;
        _chatClient.AuthValues = new AuthenticationValues(GameManager.Data._playerName);
        _chatClient.ConnectUsingSettings(_chatAppSettings);
        
    }

    void OnDestroy()
    {
        _chatClient.Disconnect();
    }

    void OnApplicationQuit()
    {
        _chatClient.Disconnect();
    }

    public void AddLine(string lineString)
    {
        if (string.IsNullOrEmpty(lineString) || _channel < 0 || _channel > 2)
            return;

        if (GetText("ChatText", out var cText))
            cText.text += lineString + "\n";

        if (_chatClient.TryGetChannel(_channels[_channel], out var chatChannel))
        {
            _chatClient.PublishMessage(chatChannel.Name, lineString);
            ShowChatMessages();
            ShowChannel(_channels[_channel]);
        }
    }

    public void DebugReturn(DebugLevel level, string message)
    {
        if (level == DebugLevel.ERROR)
        {
            Debug.LogError(message);
        }
        else if (level == DebugLevel.WARNING)
        {
            Debug.LogWarning(message);
        }
        else
        {
            Debug.Log(message);
        }
    }

    public void OnChatStateChange(ChatState state)
    {
        Debug.Log($"OnChatStateChange: {state}");
    }

    public void OnConnected()
    {
        if (_channels != null && _channels.Length > 0)
        {
            _chatClient.Subscribe(_channels, _historyLengthToFetch);
        }

        Debug.Log($"[{GameManager.Data._playerName}] 채팅 서버에 연결되었습니다");

        _chatClient.SetOnlineStatus(ChatUserStatus.Online);
    }

    public void OnDisconnected()
    {
        Debug.Log($"[{GameManager.Data._playerName}] 채팅 서버에 연결이 끊겼습니다");
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //if (_chatClient.TryGetChannel(_channels[_channel], out var chatChannel))
        //{
        //    if (channelName.Equals(chatChannel.Name))
        //    {
        //        ShowChannel(_channels[_channel]);
        //    }
        //}
        ShowChatMessages();
        ShowChannel(_channels[_channel]);
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        AddLine($"[비밀] {message}");
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        Debug.Log($"OnStatusUpdate: user:{user}, status:{status}, msg:{message}");
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        foreach (string channel in channels)
        {
            Debug.Log($"[{GameManager.Data._playerName}] {channel} 채널에 입장되었습니다");
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        Debug.Log($"[{GameManager.Data._playerName}] 채팅 서버에 퇴장되었습니다");
    }

    public void OnUserSubscribed(string channel, string user)
    {
        Debug.Log($"[{GameManager.Data._playerName}] 채팅 서버에 입장되었습니다");
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        Debug.Log($"[{GameManager.Data._playerName}] 채팅 서버에 퇴장되었습니다");
    }

    void Update()
    {
        if (_chatClient == null)
            return;

        _chatClient.Service();
    }

    public void InputChat(string text)
    {
        if (GetInputField("ChatInputField", out var cInputField))
            cInputField.text = string.Empty;

        if (_chatClient.State != ChatState.ConnectedToFrontEnd)
            return;

        if (GameManager.Data._playerState == GameData.PlayerState.Deadman)
        {
            if (_chatClient.TryGetChannel(_channels[_channel], out var chatChannel))
            {
                if(chatChannel.Name.Equals(GameData.PlayerState.Deadman.ToString()))
                    AddLine(text);
            }
            return;
        }
        AddLine(text);
    }

    public void EnableChatServer()
    {
        if (GameManager.Data._playerState == GameData.PlayerState.Deadman)
        {
            if (GetButton("DeadmanButton", out var dButton))
                dButton.interactable = true;
            if (GetButton("SpyButton", out var sButton))
                sButton.interactable = true;
        }
        if (GameManager.Data._playerState == GameData.PlayerState.Spy)
        {
            if (GetButton("SpyButton", out var sButton))
                sButton.interactable = true;
        }
    }

    private void ChangeChatServer(int serverNum)
    {
        _channel = serverNum;

        ShowChatMessages();
        ShowChannel(_channels[_channel]);
    }

    private void ShowChatMessages()
    {
        if (_chatClient.TryGetChannel(_channels[_channel], out var chatChannel))
        {
            if (GetText("ChatText", out var cText))
                cText.text = chatChannel.ToStringMessages();
        }
    }

    public void ShowChannel(string channelName)
    {
        if (string.IsNullOrEmpty(channelName))
            return;

        if (_chatClient.TryGetChannel(channelName, out var channel) == false)
        {
            Debug.Log("ShowChannel failed to find channels: " + channelName);
            return;
        }
    }
}
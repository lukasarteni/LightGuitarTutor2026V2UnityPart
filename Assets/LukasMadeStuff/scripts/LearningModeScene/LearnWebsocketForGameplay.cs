using System.Text;
using NativeWebSocket;
using UnityEngine;

public class LearnWebsocketForGameplay : MonoBehaviour
{
    WebSocket websocket;
    public LearnHitManager hitManager;

    async void Start()
    {
        Application.runInBackground = true;

        websocket = new WebSocket("ws://127.0.0.1:8000/ws");

        websocket.OnOpen += () =>
        {
            Debug.Log("Connection open!");
        };

        websocket.OnError += (e) =>
        {
            Debug.LogError("Error! " + e);
        };

        websocket.OnClose += (code) =>
        {
            Debug.Log("Connection closed!");
        };

        websocket.OnMessage += (bytes) =>
        {
            string rawMessage = Encoding.UTF8.GetString(bytes).Trim();

            Debug.Log("Received from Python: " + rawMessage);

            // Normalize input (important!)
            string chord = rawMessage;

            // Validate before sending
            if (chord!= null) 
                hitManager.TryHit(chord);
            else
            {
                Debug.LogWarning("Invalid chord received: " + rawMessage);
            }
        };

        await websocket.Connect();
        Debug.Log("Connect() finished");

        if (websocket.State == WebSocketState.Open)
        {
            Debug.Log("Sending hello from Unity...");
            await websocket.SendText("hello from Unity");
            Debug.Log("Sent hello from Unity");
        }
        else
        {
            Debug.LogWarning("WebSocket was not open after Connect()");
        }
    }

    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket?.DispatchMessageQueue();
#endif
    }

    // methods

    public async void SendData(string message)
    {
        if (websocket != null && websocket.State == WebSocketState.Open)
        {
            Debug.Log("Sending: " + message);
            await websocket.SendText(message);
        }
        else
        {
            Debug.LogWarning("WebSocket is not open.");
        }
    }

    private async void OnApplicationQuit()
    {
        if (websocket != null)
        {
            await websocket.Close();
        }
    }
}

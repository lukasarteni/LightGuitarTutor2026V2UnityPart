using System.Text;
using NativeWebSocket;
using UnityEngine;

public class WebsocketForGameplay : MonoBehaviour
{
    WebSocket websocket;
    public HitManager hitManager;

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

            // Convert incoming message into normalized chord
            string chord = ConvertToChord(rawMessage);

            if (chord != null)
            {
                hitManager.TryHit(chord);
            }
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

    // -------------------------
    // Chord Conversion Logic
    // -------------------------
    private string ConvertToChord(string rawMessage)
    {
        if (string.IsNullOrWhiteSpace(rawMessage))
            return null;

        string[] parts = rawMessage.Split(':');

        // Expect format like "A:min" or "C:maj"
        if (parts.Length != 2)
            return null;

        string root = parts[0].Trim();
        string quality = parts[1].Trim().ToLower();

        switch (quality)
        {
            case "min":
                return root + "m";

            case "maj":
                return root;

            default:
                return null; // reject unknown types
        }
    }
}
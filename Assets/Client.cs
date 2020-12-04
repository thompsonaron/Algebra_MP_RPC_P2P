using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;
using EventArgs = System.EventArgs;

public class Client
{
    WebSocket client;

    public void init()
    {
        client = new WebSocket("ws://localhost:8080/lobby");
        client.OnOpen += onOpen;
        client.OnMessage += onMessage;
        client.OnError += onError;
        client.Connect();
    }

    public void send(NetData data)
    {
        client.Send(Serializator.serialize(data));
    }

    private void onOpen(object sender, EventArgs e)
    {
    }

    private void onMessage(object sender, MessageEventArgs e)
    {
        NetData packet = Serializator.deserialize<NetData>(e.RawData);
        Session.instance.receiving.Add(packet);
    }

    private void onError(object sender, ErrorEventArgs e)
    {
        Debug.Log("onError.." + e.Exception.Message);
    }
}
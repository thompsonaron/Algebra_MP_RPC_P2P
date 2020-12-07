using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using WebSocketSharp;
using WebSocketSharp.Server;

public class Host
{
	WebSocketServer server;
	public static string opponentId;
	public static bool hostCanPlay;
	public static bool hostTurn;

	public void init()
	{
		server = new WebSocketServer(8080);
		server.AddWebSocketService<LobbyBehaviour>("/lobby");
		server.Start();
	}
	
	public void send(NetData packet)
	{
		LobbyBehaviour instance = null;
		foreach (var behaviour in server.WebSocketServices["/lobby"].Sessions.Sessions)
		{
			instance = (LobbyBehaviour)behaviour;
			break;
		}
		instance.send(packet);
	}

	public class LobbyBehaviour : WebSocketBehavior
	{
		protected override void OnOpen()
		{
			Host.opponentId = ID;
			NetData packet = new NetData() { dataType = NetType.StartTheGame };
			send(packet);

			base.OnOpen();
		}

		protected override void OnMessage(MessageEventArgs e)
		{
			var packet = Serializator.deserialize<NetData>(e.RawData);
			Session.instance.receiving.Add(packet);
			base.OnMessage(e);
		}

		public void send(NetData packet)
		{
			Send(Serializator.serialize(packet));
			Session.instance.receiving.Add(packet);
		}
	}
}
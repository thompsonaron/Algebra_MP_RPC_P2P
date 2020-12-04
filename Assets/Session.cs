using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Session : MonoBehaviour
{
	public List<NetData> receiving = new List<NetData>();
	public List<NetData> sending = new List<NetData>();

	public bool isKing;
	public Client client;
	public Host host;
		
	public static Session instance;

	public void Awake()
	{
		if(instance != null)
		{
			Destroy(gameObject);
		}
		else
		{
			instance = this;
			DontDestroyOnLoad(this);
		}
	}

	public void Update()
	{
		lock (receiving)
		{
			foreach (var packet in receiving)
			{
				Debug.Log("got: " + packet.dataType);
				if (packet.dataType == NetType.StartTheGame)
				{
					startTheGame();
				}
				else if(packet.dataType == NetType.Vector3)
				{
					var vec3 = Serializator.deserialize<NetVector3>(packet.data);
					Debug.Log(vec3.x + " " + vec3.y + " " + vec3.z);
				}
				else if(packet.dataType == NetType.RandomNumber)
				{
					Game.instance.numberToGuess = packet.data[0];
					Debug.Log("sessions: " + Game.instance.numberToGuess);
				}
				else if(packet.dataType == NetType.NumberGuess)
				{
					Game.instance.clientNumberGuess(packet.data[0]);
				}
				else if(packet.dataType == NetType.GoBigger || packet.dataType == NetType.GoLower || packet.dataType == NetType.CorrectGuess)
				{
					Game.instance.hostGuessResponse(packet);
				}
			}
			receiving.Clear();
		}
		foreach (var packet in sending)
		{
			if(isKing)
			{
				host.send(packet);
			}
			else
			{
				client.send(packet);
			}
		}
		sending.Clear();
	}

	public void startHost()
	{
		isKing = true;
		host = new Host();
		host.init();
	}

	public void startClient()
	{
		isKing = false;
		client = new Client();
		client.init();
	}


	void startTheGame()
	{
		Debug.Log("start the game");
		SceneManager.LoadScene("Game");
	}
}
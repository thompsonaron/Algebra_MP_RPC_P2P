using System;
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

	public int[,] field;

	public bool canPlay;

	public void Awake()
	{
		// "singleton"
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

    public void Start()
    {
        field = new int[9, 9];
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
				field[i, j] = 0;
            }
        }
    }

    public void Update()
	{
		// processing logic
		lock (receiving)
		{
			foreach (var packet in receiving)
			{
				Debug.Log("got: " + packet.dataType);
				if (packet.dataType == NetType.StartTheGame)
				{
					startTheGame();
				}
				if (packet.dataType == NetType.ReadyToPlay)
				{
					Debug.Log("READY TO PLAY");
				}
                else if (packet.dataType == NetType.ClientMove)
                {
					Game.instance.ClientMove(packet.data[0]);
					Game.instance.UpdateField(packet.data[0], 2);
					//UpdateField((int)packet.data[0]);
					canPlay = true;
                }
				else if (packet.dataType == NetType.HostMove)
				{
					Game.instance.HostMove(packet.data[0]);
					Game.instance.UpdateField(packet.data[0], 1);
					canPlay = true;
				}
                else if (packet.dataType == NetType.HostMove)
				{
					// TODO activate you lose UI and disable input
                }

			}
			receiving.Clear();
		}
        lock (sending)
        {
			// sending packets "online"
			foreach (var packet in sending)
			{
                if (packet.dataType == NetType.ClientMove || packet.dataType == NetType.HostMove)
                {
					canPlay = false;
                }
				if (isKing)
				{
					host.send(packet);
				}
				else
				{
					client.send(packet);
				}
			}
		}
		
		sending.Clear();
	}


    // initializing host
    public void startHost()
	{
		isKing = true;
		host = new Host();
		host.init();
	}

	// initializing client
	public void startClient()
	{
		isKing = false;
		client = new Client();
		client.init();
	}

	// loading game scene
	void startTheGame()
	{
		Debug.Log("start the game");
		SceneManager.LoadScene("Game");
	}
}
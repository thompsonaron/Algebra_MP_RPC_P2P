using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static Game instance;
    public GameObject player1Ball;
    public GameObject player2Ball;

    public int numberToGuess;

    void Start()
    {
        instance = this;
        if (!Session.instance.isKing)
        {
            //PEASANT CODE
            var packet = new NetData();
            packet.dataType = NetType.Vector3;
            packet.data = Serializator.serialize(new NetVector3() { x = 1, y = 2, z = 3 });
            Session.instance.sending.Add(packet);
        }
        else
        {
            //KING CODE
            //Generate the number
            //Sends the number to peasant

            byte generatedNumber = (byte)Random.Range(1, 10); //1....9
            var packet = new NetData() { dataType = NetType.RandomNumber, data = new byte[] { generatedNumber } };
            Session.instance.sending.Add(packet);
        }
    }

    void Update()
    {
        // TODO add can play switching to both host and client
        
        // client logic
        if (!Session.instance.isKing)
        {
            for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha9; i++)
            {
                var keycode = (KeyCode)i;
                if (Input.GetKeyDown(keycode))
                {
                    var realNumber = i - KeyCode.Alpha0;
                    //Check the number;
                    var packet = new NetData() { dataType = NetType.ClientMove, data = new byte[] { (byte)realNumber } };
                    Session.instance.sending.Add(packet);
                    Debug.Log("ClientMove");

                   Instantiate(player2Ball, new Vector3((int)realNumber - 1f, 10f, 0f), Quaternion.identity);
           // Debug.Log("INSTANTIATING blue");

                }
            }
        }
        // host logic
        else
        {
            for (int i = (int)KeyCode.Alpha1; i <= (int)KeyCode.Alpha9; i++)
            {
                var keycode = (KeyCode)i;
                if (Input.GetKeyDown(keycode))
                {
                    var realNumber = i - KeyCode.Alpha0;
                    //Check the number;
                    var packet = new NetData() { dataType = NetType.HostMove, data = new byte[] { (byte)realNumber } };
                    Session.instance.sending.Add(packet);
                    Debug.Log("HostMove");
                   // Instantiate(player1Ball, new Vector3((int)realNumber - 1f, 10f, 0f), Quaternion.identity);

                }
            }
        }
    }

    //public void clientNumberGuess(int number)
    //{
    //    //if (Session.instance.isKing)
    //    //{
    //    //    if (number == numberToGuess)
    //    //    {
    //    //        var packet = new NetData() { dataType = NetType.CorrectGuess };
    //    //        Session.instance.sending.Add(packet);
    //    //    }
    //    //    else if (number > numberToGuess)
    //    //    {
    //    //        var packet = new NetData() { dataType = NetType.GoLower };
    //    //        Session.instance.sending.Add(packet);
    //    //    }
    //    //    else
    //    //    {
    //    //        var packet = new NetData() { dataType = NetType.GoBigger };
    //    //        Session.instance.sending.Add(packet);
    //    //    }
    //    //}

        
    //    //if (Session.instance.isKing)
    //    //{
    //    //    Debug.Log("INSTANTIATING as client");
    //    //    Instantiate(player2Ball, new Vector3(number - 1f, 10f, 0f), Quaternion.identity);
    //    //    //var packet = new NetData() { dataType = NetType.NumberGuess };
    //    //    //Session.instance.sending.Add(packet);
            
    //    //}
    //    //else
    //    //{
    //    //    Instantiate(player1Ball, new Vector3(number - 1f, 10f, 0f), Quaternion.identity);
    //    //    //var packet = new NetData() { dataType = NetType.NumberGuess };
    //    //    //Session.instance.sending.Add(packet);
    //    //}
    //}

    public void HostMove(int number)
    {
        Instantiate(player1Ball, new Vector3(number - 1f, 10f, 0f), Quaternion.identity);
    }

    public void ClientMove(int number)
    {
        Instantiate(player2Ball, new Vector3(number - 1f, 10f, 0f), Quaternion.identity);
    }

    public void hostGuessResponse(NetData packet)
    {
        if (!Session.instance.isKing)
        {
            if (packet.dataType == NetType.GoBigger)
            {
                Debug.Log("I have to guess a bigger number!");
            }
            else if (packet.dataType == NetType.GoLower)
            {
                Debug.Log("I have to guess a lower number!");
            }
        }
        if (packet.dataType == NetType.CorrectGuess)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
    }
}
using System;
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
        //if (!Session.instance.isKing)
        //{
        //    //PEASANT CODE
        //    //var packet = new NetData();
        //    //packet.dataType = NetType.Vector3;
        //    //packet.data = Serializator.serialize(new NetVector3() { x = 1, y = 2, z = 3 });
        //    //Session.instance.sending.Add(packet);
        //}
        //else
        //{
        //    //KING CODE
        //    //Generate the number
        //    //Sends the number to peasant

        //    //byte generatedNumber = (byte)Random.Range(1, 10); //1....9
        //    //var packet = new NetData() { dataType = NetType.RandomNumber, data = new byte[] { generatedNumber } };
        //    //Session.instance.sending.Add(packet);
        //}
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
                    // -1 is added since array and positions start at 0
                    var realNumber = i - KeyCode.Alpha0 - 1;
                    //if (TryMove((int)realNumber))
                    //{

                    //}
                    //Check the number;
                    var packet = new NetData() { dataType = NetType.ClientMove, data = new byte[] { (byte)realNumber } };
                    Session.instance.sending.Add(packet);
                    Debug.Log("ClientMove");
                    Instantiate(player2Ball, new Vector3((int)realNumber, 10f, 0f), Quaternion.identity);
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
                    // -1 is added since array and positions start at 0
                    var realNumber = i - KeyCode.Alpha0 - 1;

                    //if (TryMove((int)realNumber))
                    //{

                    //}

                    //Check the number;
                    var packet = new NetData() { dataType = NetType.HostMove, data = new byte[] { (byte)realNumber } };
                    Session.instance.sending.Add(packet);
                    Debug.Log("HostMove");
                    Instantiate(player1Ball, new Vector3((int)realNumber, 10f, 0f), Quaternion.identity);
                }
            }
        }
    }

    private bool TryMove(int realNumber)
    {
        if (Session.instance.field[8, realNumber] == 0)
        {
            return true;
        }
        return false;
    }

    public void HostMove(int position)
    {
        Instantiate(player1Ball, new Vector3(position, 10f, 0f), Quaternion.identity);
        //UpdateField(position, 1);
    }

    public void ClientMove(int position)
    {
        Instantiate(player2Ball, new Vector3(position, 10f, 0f), Quaternion.identity);
        //UpdateField(position, 2);
    }

    private void UpdateField(int number, int codeNum)
    {
        int row = 0;
        int column = number;

        for (int i = row; i < 9; row++)
        {
            if (Session.instance.field[row, column] == 0)
            {
                Session.instance.field[row, column] = codeNum;
                CheckForWin(row, column, codeNum);
            }
        }

    }

    private void CheckForWin(int startingRowPos, int startingColumnPos, int codeNum)
    {
        // check UP
        if (startingRowPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos + 1, startingColumnPos] == codeNum && Session.instance.field[startingRowPos + 2, startingColumnPos] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check DOWN
        if (startingRowPos - 2 >= 0)
        {
            if (Session.instance.field[startingRowPos - 1, startingColumnPos] == codeNum && Session.instance.field[startingRowPos - 2, startingColumnPos] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check LEFT
        if (startingColumnPos -2 >= 0)
        {
            if (Session.instance.field[startingRowPos, startingColumnPos-1] == codeNum && Session.instance.field[startingRowPos, startingColumnPos-2] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check RIGHT
        if (startingColumnPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos, startingColumnPos+1] == codeNum && Session.instance.field[startingRowPos, startingColumnPos+2] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check LEFT UPPPER 
        if (startingColumnPos - 2 >= 0 && startingRowPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos +1, startingColumnPos-1] == codeNum && Session.instance.field[startingRowPos+2, startingColumnPos-2] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check RIGHT LOWER
        if (startingColumnPos + 2 <= 8 && startingRowPos - 2 >= 0)
        {
            if (Session.instance.field[startingRowPos - 1, startingColumnPos + 1] == codeNum && Session.instance.field[startingRowPos - 2, startingColumnPos + 2] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check LEFT LOWER
        if (startingColumnPos -2 >= 0 && startingRowPos -2 >= 0)
        {
            if (Session.instance.field[startingRowPos - 1, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos - 2, startingColumnPos - 2] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check RIGHT UPPER
        if (startingColumnPos + 2 <= 8 && startingRowPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos + 1, startingColumnPos + 1] == codeNum && Session.instance.field[startingRowPos + 2, startingColumnPos + 2] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
    }

    public void hostGuessResponse(NetData packet)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif

    }
}
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

                    if (TryMove((int)realNumber))
                    {
                        //Check the number;
                        var packet = new NetData() { dataType = NetType.ClientMove, data = new byte[] { (byte)realNumber } };
                        Session.instance.sending.Add(packet);
                        Debug.Log("ClientMove");
                        Instantiate(player2Ball, new Vector3((int)realNumber, 10f, 0f), Quaternion.identity);
                        UpdateField((int)realNumber, 2);
                    }
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

                    if (TryMove((int)realNumber))
                    {
                        //Check the number;
                        var packet = new NetData() { dataType = NetType.HostMove, data = new byte[] { (byte)realNumber } };
                        Session.instance.sending.Add(packet);
                        Debug.Log("HostMove");
                        Instantiate(player1Ball, new Vector3((int)realNumber, 10f, 0f), Quaternion.identity);

                        UpdateField((int)realNumber, 1);
                    }
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

    public void UpdateField(int column, int codeNum)
    {
        //int row = 0;


        for (int row = 0; row < 9; row++)
        {
            if (Session.instance.field[row, column] == 0)
            {
                Session.instance.field[row, column] = codeNum;
                Debug.Log(Session.instance.field[4, 0] + "" + Session.instance.field[4, 1] + "" + Session.instance.field[4, 2] + "" + Session.instance.field[4, 3] + "" + Session.instance.field[4, 4] + "" + Session.instance.field[4, 5] + "" + Session.instance.field[4, 6] + "" + Session.instance.field[4, 7] + "" + Session.instance.field[4, 8]); 
                Debug.Log(Session.instance.field[3, 0] + "" + Session.instance.field[3, 1] + "" + Session.instance.field[3, 2] + "" + Session.instance.field[3, 3] + "" + Session.instance.field[3, 4] + "" + Session.instance.field[3, 5] + "" + Session.instance.field[3, 6] + "" + Session.instance.field[3, 7] + "" + Session.instance.field[3, 8]);
                Debug.Log(Session.instance.field[2, 0] + "" + Session.instance.field[2, 1] + "" + Session.instance.field[2, 2] + "" + Session.instance.field[2, 3] + "" + Session.instance.field[2, 4] + "" + Session.instance.field[2, 5] + "" + Session.instance.field[2, 6] + "" + Session.instance.field[2, 7] + "" + Session.instance.field[2, 8]); 
                Debug.Log(Session.instance.field[1, 0] + "" + Session.instance.field[0, 1] + "" + Session.instance.field[1, 2] + "" + Session.instance.field[1, 3] + "" + Session.instance.field[1, 4] + "" + Session.instance.field[1, 5] + "" + Session.instance.field[1, 6] + "" + Session.instance.field[1, 7] + "" + Session.instance.field[1, 8]);
                Debug.Log(Session.instance.field[0, 0] + "" + Session.instance.field[0, 1] + "" + Session.instance.field[0, 2] + "" + Session.instance.field[0, 3] + "" + Session.instance.field[0, 4] + "" + Session.instance.field[0, 5] + "" + Session.instance.field[0, 6] + "" + Session.instance.field[0, 7] + "" + Session.instance.field[0, 8]);
                return;
                //CheckForWin(row, column, codeNum);
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
        if (startingColumnPos - 2 >= 0)
        {
            if (Session.instance.field[startingRowPos, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos, startingColumnPos - 2] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check RIGHT
        if (startingColumnPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos, startingColumnPos + 1] == codeNum && Session.instance.field[startingRowPos, startingColumnPos + 2] == codeNum)
            {
                Debug.Log("Victory");
            }
        }
        // check LEFT UPPPER 
        if (startingColumnPos - 2 >= 0 && startingRowPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos + 1, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos + 2, startingColumnPos - 2] == codeNum)
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
        if (startingColumnPos - 2 >= 0 && startingRowPos - 2 >= 0)
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
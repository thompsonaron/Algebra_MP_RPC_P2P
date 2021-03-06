﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public static Game instance;
    public GameObject player1Ball;
    public GameObject player2Ball;
    public Text winLoseText;
    private int internalID;

    void Start()
    {
        instance = this;
        if (Session.instance.isKing)
        {
            internalID = 1;
        }
        else
        {
            internalID = 2;
        }
    }

    void Update()
    {
        if (Session.instance.canPlay)
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
                        if (Session.instance.isKing) { packet.dataType = NetType.HostMove; }
                        Session.instance.sending.Add(packet);

                        if (Session.instance.isKing) { Instantiate(player1Ball, new Vector3((int)realNumber, 10f, 0f), Quaternion.identity); }
                        else                         { Instantiate(player2Ball, new Vector3((int)realNumber, 10f, 0f), Quaternion.identity); }
                       
                        int updatedRowHeight = UpdateField((int)realNumber, internalID);
                        if (CheckForWin(updatedRowHeight, (int)realNumber, internalID))
                        {
                            var packetWin = new NetData() { dataType = NetType.YouLose };
                            Session.instance.sending.Add(packetWin);
                            YouWin();
                        }
                    }
                }
            }
        }
    }

    public void YouWin()
    {
        // activate some canvas and disable input
        Debug.Log("You win");
        winLoseText.text = "You Win!";
        Session.instance.canPlay = false;
    }

    public void YouLose()
    {
        // activate some canvas and disable input
        Debug.Log("You lose");
        winLoseText.text = "You Lose";
        Session.instance.canPlay = false;
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
    }

    public void ClientMove(int position)
    {
        Instantiate(player2Ball, new Vector3(position, 10f, 0f), Quaternion.identity);
    }

    public int UpdateField(int column, int codeNum)
    {
        for (int row = 0; row < 9; row++)
        {
            if (Session.instance.field[row, column] == 0)
            {
                Session.instance.field[row, column] = codeNum;
                //Debug.Log(Session.instance.field[4, 0] + "" + Session.instance.field[4, 1] + "" + Session.instance.field[4, 2] + "" + Session.instance.field[4, 3] + "" + Session.instance.field[4, 4] + "" + Session.instance.field[4, 5] + "" + Session.instance.field[4, 6] + "" + Session.instance.field[4, 7] + "" + Session.instance.field[4, 8]); 
                //Debug.Log(Session.instance.field[3, 0] + "" + Session.instance.field[3, 1] + "" + Session.instance.field[3, 2] + "" + Session.instance.field[3, 3] + "" + Session.instance.field[3, 4] + "" + Session.instance.field[3, 5] + "" + Session.instance.field[3, 6] + "" + Session.instance.field[3, 7] + "" + Session.instance.field[3, 8]);
                //Debug.Log(Session.instance.field[2, 0] + "" + Session.instance.field[2, 1] + "" + Session.instance.field[2, 2] + "" + Session.instance.field[2, 3] + "" + Session.instance.field[2, 4] + "" + Session.instance.field[2, 5] + "" + Session.instance.field[2, 6] + "" + Session.instance.field[2, 7] + "" + Session.instance.field[2, 8]); 
                //Debug.Log(Session.instance.field[1, 0] + "" + Session.instance.field[0, 1] + "" + Session.instance.field[1, 2] + "" + Session.instance.field[1, 3] + "" + Session.instance.field[1, 4] + "" + Session.instance.field[1, 5] + "" + Session.instance.field[1, 6] + "" + Session.instance.field[1, 7] + "" + Session.instance.field[1, 8]);
                //Debug.Log(Session.instance.field[0, 0] + "" + Session.instance.field[0, 1] + "" + Session.instance.field[0, 2] + "" + Session.instance.field[0, 3] + "" + Session.instance.field[0, 4] + "" + Session.instance.field[0, 5] + "" + Session.instance.field[0, 6] + "" + Session.instance.field[0, 7] + "" + Session.instance.field[0, 8]);
                return row;
            }
        }

        return 0;

    }

    private bool CheckForWin(int startingRowPos, int startingColumnPos, int codeNum)
    {
        Debug.Log("Checking for victory");
        // check UP // TODO probably pointless
        if (startingRowPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos + 1, startingColumnPos] == codeNum && Session.instance.field[startingRowPos + 2, startingColumnPos] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check DOWN
        if (startingRowPos - 2 >= 0)
        {
            if (Session.instance.field[startingRowPos - 1, startingColumnPos] == codeNum && Session.instance.field[startingRowPos - 2, startingColumnPos] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check LEFT
        if (startingColumnPos - 2 >= 0)
        {
            if (Session.instance.field[startingRowPos, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos, startingColumnPos - 2] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check RIGHT
        if (startingColumnPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos, startingColumnPos + 1] == codeNum && Session.instance.field[startingRowPos, startingColumnPos + 2] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check LEFT UPPPER 
        if (startingColumnPos - 2 >= 0 && startingRowPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos + 1, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos + 2, startingColumnPos - 2] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check RIGHT LOWER
        if (startingColumnPos + 2 <= 8 && startingRowPos - 2 >= 0)
        {
            if (Session.instance.field[startingRowPos - 1, startingColumnPos + 1] == codeNum && Session.instance.field[startingRowPos - 2, startingColumnPos + 2] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check LEFT LOWER
        if (startingColumnPos - 2 >= 0 && startingRowPos - 2 >= 0)
        {
            if (Session.instance.field[startingRowPos - 1, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos - 2, startingColumnPos - 2] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check RIGHT UPPER
        if (startingColumnPos + 2 <= 8 && startingRowPos + 2 <= 8)
        {
            if (Session.instance.field[startingRowPos + 1, startingColumnPos + 1] == codeNum && Session.instance.field[startingRowPos + 2, startingColumnPos + 2] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }


        // SINGLE CHECKS
        // check LEFT AND RIGHT
        if (startingColumnPos - 1 >= 0 && startingColumnPos + 1 <= 8)
        {
            if (Session.instance.field[startingRowPos, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos, startingColumnPos + 1] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check UP AND DOWN // TODO probably pointless
        if (startingRowPos - 1 >= 0 && startingRowPos + 1 <= 8)
        {
            if (Session.instance.field[startingRowPos - 1, startingColumnPos] == codeNum && Session.instance.field[startingRowPos + 1, startingColumnPos] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check LEFT SLASH - \
        if (startingColumnPos >= 1 && startingColumnPos <= 7 && startingRowPos >= 1 && startingRowPos <= 7)
        {
            if (Session.instance.field[startingRowPos + 1, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos - 1, startingColumnPos + 1] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }
        // check RIGHT SLASH - /
        if (startingColumnPos >= 1 && startingColumnPos <= 7 && startingRowPos >= 1 && startingRowPos <= 7)
        {
            if (Session.instance.field[startingRowPos - 1, startingColumnPos - 1] == codeNum && Session.instance.field[startingRowPos + 1, startingColumnPos + 1] == codeNum)
            {
                Debug.Log("Victory");
                return true;
            }
        }

        return false;
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
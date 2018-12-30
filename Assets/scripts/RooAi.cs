using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RooAi {
    private Board board;
    private Room source;
    private Room dest;

    public RooAi(Board board)
    {
        this.board = board;
    }

    public void Decide()
    {
        if (!WinningMove()) MakeMonkeyMove();
    }

    public bool WinningMove()
    {
        int[] arr = board.GetBeadPositions(Room.Bead.ROO);
        Debug.Log(MyArray.ToString(arr));
        int i, j = 0, dest = -1;
        Board.LabelText = "Searching for moves: \n";
        for(i = 0; i < arr.Length; i++)
        {
            Board.LabelText += "\nValid moves for: " + arr[i] +": ";
            for(j = 0; j < Room.graph.GetLength(0); j++)
            {
                if(board.rooms[arr[i]].IsValidMove(board.rooms[j]))
                {
                    Board.LabelText += j + " ";
                    if (IsWin(arr[i], j))
                    {
                        dest = j;
                        break;
                    }
                }
            }
            if (dest != -1) break;
            Board.LabelText += "\n" + arr[i] + " has no winning move.";
        }
        if(dest == -1)
        {
            Debug.LogWarning("No winning move found.");
            return false;
        } else
        {
            Debug.Log("Make a move from : " + arr[i] + " to " + j);
            board.MakeAMove(board.rooms[arr[i]], board.rooms[j]);
            return true;
        }
    }


    internal Boolean IsWin(int prevIndex, int currentIndex)
    {
        // TODO refresh positions here
        int[] arr = new int[3];
        RefreshPositions(prevIndex, currentIndex, arr);
        // TODO is there any way to set who has a WiningMove in next move?
        if (board.rooms[prevIndex].getMember() == Room.Bead.ROO)
        {
            if (arr[0] == 0 && arr[1] == 1)
                return false; // Initial Position of Roo
        }
        else if(board.rooms[prevIndex].getMember() == Room.Bead.VEE)
        {
            if (arr[0] == 6 && arr[1] == 7)
                return false; // Initial Position of Vee
        } else
        {
            throw new UnexpectedEmptyException();
        }
        if (arr[1] - arr[0] == 2)
        {
            if (arr[0] != 2) // Matches 2 4 6
            {
                return false;
            }
        }
        else if (arr[1] - arr[0] == 1)
        {
            return arr[0] % 3 == 0 ? arr[2] - arr[1] == 1 : false;
        }
        return arr[1] - arr[0] == arr[2] - arr[1];
    }

    private int[] RefreshPositions(int prevIndex, int currentIndex, int[] arr)
    {
        Debug.Log("prevIndex: " + prevIndex + " : " + board.rooms[prevIndex].getMember());
        int[] tmp = board.rooms[prevIndex].getMember() != Room.Bead.EMPTY ?
            board.GetBeadPositions(board.rooms[prevIndex].getMember()) : null;
        Array.Copy(tmp,
            arr, 3);
        arr[Array.IndexOf(arr, prevIndex)] = currentIndex;
        Array.Sort(arr);
        return arr;
    }

    public void MakeMonkeyMove()
    {
        source = PickMonkeyBead();
        // Picking a destination room
        int[] validDsts = source.GetValidMoves();
        if(validDsts.Length == 0) // No possible move
        {
            MakeMonkeyMove(); // Start again with a new source
            return;
        }
        int rand = UnityEngine.Random.Range(0, validDsts.Length);
        dest = board.rooms[validDsts[rand]];
        board.MakeAMove(source, dest);
    }

    public Room PickMonkeyBead() => board.rooms[board.GetBeadPositions(Room.Bead.ROO)[(int)UnityEngine.Random.Range(0, 3)]];

    public Room getSource() => this.source;

    public Room getDest() => this.dest;
}

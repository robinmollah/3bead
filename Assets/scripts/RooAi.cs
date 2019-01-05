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
        if (!HasWinningMove() && !PreventWin() && !DontLetWin()) // FIX does this condition okay?
        {
            MakeMonkeyMove();
        }
    }

    /*
     * Donot make a move what will let the player win.
     */
    private bool DontLetWin()
    {
        // 
        // if I make this move, will the player win?
        return false;
    }

    /*
     * If the player has an winmove. Prevent him to win.
     */
    public bool PreventWin()
    {
        // if this move give him a chance to win don't move
        // if this move prevent him from win make this move
        int[] arr = board.GetBeadPositions(Room.Bead.VEE);
        int i, j = 0;
        for(i = 0; i < arr.Length; i++)
        {
            for(j = 0; j < Room.graph.GetLength(0); j++)
            {
                if (board.rooms[arr[i]].IsValidMove(board.rooms[j]))
                {
                    if(IsWin(arr[i], j))
                    {
                        return MakeAMoveTo(j);
                    }
                }
            }
        }
        return false;
    }

    private bool MakeAMoveTo(int dest)
    {
        int[] arr = board.GetBeadPositions(Room.Bead.ROO);
        for(int i = 0; i < arr.Length; i++)
        {
            if (i == dest) continue;
            if(Room.HasEdge(arr[i], dest) && board.rooms[dest].IsEmpty())
            {
                board.MakeAMove(board.rooms[arr[i]], board.rooms[dest]);
                return true;
            }
        }
        return false;
    }

    public bool HasWinningMove(Room.Bead bead = Room.Bead.ROO)
    {
        int[] arr = board.GetBeadPositions(bead);
        int i, j = 0, dest = -1;
        // TODO is there anyway to know if a move is exposing a win to vee;
        for(i = 0; i < arr.Length; i++)
        {
            for(j = 0; j < 9; j++)
            {
                if(board.rooms[arr[i]].IsValidMove(board.rooms[j]))
                {
                    if (IsWin(arr[i], j))
                    {
                        dest = j;
                        break;
                    }
                }
            }
            if (dest != -1) break;
        }
        if(dest == -1)
        {
            return false;
        } else
        {
            Debug.Log("Make a move from : " + arr[i] + " to " + j);
            if(bead == Room.Bead.ROO)
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
            Debug.LogWarning("This bead has no valid destination.");
        }
        int rand = UnityEngine.Random.Range(0, validDsts.Length);
        dest = board.rooms[validDsts[rand]];
        board.MakeAMove(source, dest);
    }

    public Room PickMonkeyBead()
    {
        // TODO co-operate this method with the main monkey.
        int[] roos = board.GetBeadPositions(Room.Bead.ROO);
        int[] vees = board.GetBeadPositions(Room.Bead.VEE);
        int[] black = new int[3];
        int blackedCount = 0;
        int rand;
        int tryCount = 5;
        while (true)
        {
            Board.LabelText = "Attempt Remaining: " + tryCount;
            rand = UnityEngine.Random.Range(0, 3);
            // Checking if already blacked
            if (Array.IndexOf(black, rand) != -1)
            {
                continue;
            }
            if (!board.rooms[roos[rand]].HasValidMove())
            {
                black[blackedCount++] = rand;
                continue;
            }
            if(blackedCount == 2)
            {
                return board.rooms[roos[rand]];
            }
            bool shouldContinue = false;
            for (int i = 0; i < 3; i++)
            {
                Board.LabelText += "\nTesting: " + vees[i];
                if (Room.HasEdge(vees[i], roos[rand]) &&  IsWin(vees[i], roos[rand]))
                {
                    Debug.Log(vees[i] + " can access this position to win");
                    shouldContinue = true;
                }
            }
            if(tryCount <= 0)
            {
                break;
            }
            if (shouldContinue)
            {
                tryCount--;
                continue;
            }
            // infinite loop: because randomly picked bead might not have any destination. so before picking 
            // make sure it has at least one destination destination
            break;
        }
        Board.LabelText += "\nPicked: " + roos[rand];

        // if moving this bead exposes a win = if (the source bead is accessible by any player's bead and if it is replaced
        // by players bead then player wins) then it cannot be a source bead.

        return board.rooms[roos[rand]];
    }

    public Room GetSource()
    {
        return this.source;
    }

    public Room GetDest()
    {
        return this.dest;
    }
}

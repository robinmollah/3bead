﻿using System;
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
        // Next Move Win VEE
        // Prevent Next Move of Person
        // Don't move if PERSON will win have next move win state
        MakeMonkeyMove();
    }

    public void WinningMove()
    {

    }

    public void MakeMonkeyMove()
    {
        source = PickMonkeyBead();
        // Picking a destination room
        int[] validDsts = source.GetValidMoves();
        if(validDsts.Length == 0) // No possible move
        {
            MakeMonkeyMove(); // Start again with a new source
        }
        dest = board.rooms[validDsts[UnityEngine.Random.Range(0, validDsts.Length)]];
    }

    public Room PickMonkeyBead()
    {
        return board.rooms[board.GetBeadPositions(Room.Bead.ROO)[(int)UnityEngine.Random.Range(0, 3)]];
    }
    
    public Room getSource()
    {
        return this.source;
    }

    public Room getDest()
    {
        return this.dest;
    }

}

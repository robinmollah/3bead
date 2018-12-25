﻿using System;
using UnityEngine;


public class Room : MonoBehaviour
{
    private static readonly int[,] graph = new int[9, 9]{
        {0, 1, 0, 1, 1, 0, 0, 0, 0 },
        {1, 0, 1, 0, 1, 0, 0, 0, 0 },
        {0, 1, 0, 0, 1, 1, 0, 0, 0 },
        {1, 0, 0, 0, 1, 0, 1, 0, 0 },
        {1, 1, 1, 1, 0, 1, 1, 1, 1 },
        {0, 0, 1, 0, 1, 0, 0, 0, 1 },
        {0, 0, 0, 1, 1, 0, 0, 1, 0 },
        {0, 0, 0, 0, 1, 0, 1, 0, 1 },
        {0, 0, 0, 0, 1, 1, 0, 1, 0 }
    };
    public Bead member = Bead.EMPTY;
    public Vector2 Position { private get; set; }
    [NonSerialized]
    private int index;

    public Room(int index)
    {
        this.index = index;
    }

    public void setIndex(int index)
    {
        this.index = index;
    }

    public int GetIndex()
    {
        return index;
    }

    public Vector3 GetPosition()
    {
        return new Vector3(this.Position.x, this.Position.y, -0.1f);
    }

    public void setMember(Bead type)
    {
        this.member = type;
        
    }

    /*
     * Makes a move from this to type
     */ 
    public void SetMove(Bead type)
    {
        setMember(type);
        // Visual effect
        Board board = transform.parent.gameObject.GetComponent<Board>();
        Texture2D tex = this.member == Bead.ROO ? board.rooTex : board.veeTex;
        if(gameObject.GetComponent<SpriteRenderer>() as SpriteRenderer != null)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height)
           , new Vector2(0.5f, 0.5f));
        } else
        {
            SpriteRenderer sp = gameObject.AddComponent<SpriteRenderer>();
            sp.sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height),
                new Vector2(0.5f, 0.5f));
        }
    }

    /*
     * return
     * if <i>room</i> is a valid destination of this room?
     */ 
    public bool IsValidMove(Room room)
    {
        if (!room.isEmpty())
        {
            Debug.Log("This room is already occupied.");
            return false;
        }
        return graph[this.index, room.index] == 1;
    }

    public int[] GetValidMoves()
    {
        int[] arr = new int[9];
        Room[] rooms = transform.parent.GetComponent<Board>().rooms;
        int j = 0;
        for(int i = 0; i < Room.graph.GetLength(0); i++)
        {
            if (Room.graph[index, i] == 1)
            {
                if(rooms[index].IsValidMove(rooms[i]))
                    arr[j++] = i;
            }
        }
        Array.Resize<int>(ref arr, j);
        Debug.Log("Number of valid moves: " + j);
        return arr;
    }

    private int GetNumberOfPossibleMoves()
    {
        return index == 4 ? 8 : (index % 2 == 0) ? 3 : 5;
    }

    public Bead getMember()
    {
        return this.member;
    }

    public Boolean isEmpty()
    {
        return this.member == Bead.EMPTY;
    }

    public void RemoveMove()
    {
        this.SetEmpty();
        Destroy(gameObject.GetComponent<SpriteRenderer>());
    }

    private void SetEmpty()
    {
        setMember(Bead.EMPTY);
    }

    public enum Bead
    {
        EMPTY, ROO, VEE
    }
}

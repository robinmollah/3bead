using System;
using UnityEngine;


public class Room : MonoBehaviour
{
    private static int[,] graph = new int[9, 9]{
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
    public Vector2 position { private get; set; }
    [NonSerialized]
    public int index;

    public Room(int index)
    {
        this.index = index;
    }

    public void setIndex(int index)
    {
        this.index = index;
    }

    public int getIndex()
    {
        return index;
    }

    public Vector3 getPosition()
    {
        return new Vector3(this.position.x, this.position.y, -0.1f);
    }

    public void setMember(Bead type)
    {
        this.member = type;
        
    }

    /*
     * Makes a move from this to type
     */ 
    public void setMove(Bead type)
    {
        setMember(type);
        // Visual effect
        Board board = transform.parent.gameObject.GetComponent<Board>();
        Texture2D tex = board.getConjugateTexture(this.member);
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
    public bool ValidMove(Room room)
    {
        if (!room.isEmpty())
        {
            Debug.Log("This room is already occupied.");
            return false;
        }
        if (graph[this.index, room.index] == 1)
        {
            return true;
        } 
        return false;
    }

    public Bead getMember()
    {
        return this.member;
    }

    public Boolean isEmpty()
    {
        return this.member == Bead.EMPTY;
    }

    public void removeMove()
    {
        this.setEmpty();
        Destroy(gameObject.GetComponent<SpriteRenderer>());
    }

    private void setEmpty()
    {
        setMember(Bead.EMPTY);
    }

    public enum Bead
    {
        EMPTY, ROO, VEE
    }
}

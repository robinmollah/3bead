using System;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Bead member = Bead.EMPTY;
    public Vector2 position { private get; set; }
    private int index;

    public Room(int index)
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

    public void setMove(Bead type)
    {
        setMember(type);
        // Visual effect
        Board board = transform.parent.gameObject.GetComponent<Board>();
        Texture2D tex = board.getConjugateTexture(this.member);
        gameObject.GetComponent<SpriteRenderer>().sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height)
            , new Vector2(0.5f, 0.5f));
    }

    public Bead getMember()
    {
        return this.member;
    }

    public Boolean isEmpty()
    {
        return this.member == Bead.EMPTY;
    }

    public enum Bead
    {
        EMPTY, ROO, VEE
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public Room[] rooms = new Room[9];
    public Texture2D rooTex, veeTex;
    private Room takenRoom;
    private Boolean taken;
    private Room.Bead currentTurn = Room.Bead.VEE;
    
	void Start () {
        InitiateRooms();
	}

    void Update () {
    #if UNITY_ANDROID
        for(int i = 0; i < Input.touchCount; i++)
        {
            if(Input.GetTouch(i).phase == TouchPhase.Began)
            {
                Raycast(Input.GetTouch(i).position);
            }
        }
    #else
        if (Input.GetMouseButtonDown(0))
        {
            Raycast(Input.mousePosition);
        }
    #endif
    }

    private void Raycast(Vector2 screenTouch)
    {
        // Test line
        RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenTouch), Vector2.zero);
        if (hitInfo)
        {
            /*
             * Bugs:
             *  4. Move animation
             */
            Room room = hitInfo.transform.gameObject.GetComponent<Room>();
            if (taken)
            {
                if(room.index == takenRoom.index)
                {
                    Debug.Log("Back this bead.");
                    Color tmpColor = room.GetComponent<SpriteRenderer>().color;
                    tmpColor.a = 1f;
                    room.GetComponent<SpriteRenderer>().color = tmpColor;
                    taken = false;
                    takenRoom = null;
                    return;
                }
                if(!takenRoom.ValidMove(room))
                {
                    return;
                }
                room.setMove(takenRoom.getMember());
                takenRoom.removeMove();
                taken = false;
                currentTurn = currentTurn == Room.Bead.VEE ? Room.Bead.ROO : Room.Bead.VEE;
                return;
            } else
            {
                if (!room.isEmpty() && room.getMember() == currentTurn)
                {
                    Color tmpColor = room.GetComponent<SpriteRenderer>().color;
                    tmpColor.a = 0.5f;
                    
                    room.GetComponent<SpriteRenderer>().color = tmpColor;
                    takenRoom = room;
                    taken = true;
                }
            }
        }
    }

    private void InitiateRooms()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int rIndex = 3 * (i + 1) + j + 1;
                Debug.Log("Initiating room: " + rIndex);
                GameObject obj = new GameObject("RoomIndex : " + rIndex);
                rooms[rIndex] = obj.AddComponent<Room>();
                rooms[rIndex].setIndex(rIndex);
                obj.transform.SetParent(transform);
                rooms[rIndex].position = new Vector2(j, i * -1);
                obj.transform.localPosition = rooms[rIndex].getPosition();
                // Create SpriteRenderer

                if (i == -1 || i == 1)
                {
                    Texture2D tex = rooTex;
                    rooms[rIndex].setMember(Room.Bead.ROO);
                    if (i == 1)
                    {
                        rooms[rIndex].setMember(Room.Bead.VEE);
                        tex = veeTex;
                    }
                    Sprite sprite = Sprite.Create(tex, new Rect(0f, 0f, tex.width, tex.height), new Vector2(0.5f, 0.5f));
                    SpriteRenderer renderer = obj.AddComponent<SpriteRenderer>() as SpriteRenderer;
                    renderer.sprite = sprite;
                }
                // Create BoxCollider
                BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();
                collider.size = new Vector2(0.8f, 0.8f);
                
                obj.transform.localScale = Vector3.one;
            }
        }
    }

    public Texture2D getConjugateTexture(Room.Bead beadType)
    {
        if (beadType == Room.Bead.ROO) return rooTex;
        else if (beadType == Room.Bead.VEE) return veeTex;
        else return null;
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public Room[] rooms = new Room[9];
    public Texture2D rooTex, veeTex;
    private Room takenRoom;
    
	void Start () {
        InitiateRooms();
	}

    void Update () {
    #if UNITY_ANDROID
        for(int i = 0; i < Input.touchCount; i++)
        {
            if(Input.GetTouch(i).phase == TouchPhase.Began)
            {
                raycast(Input.GetTouch(i).position);
            }
        }
#endif
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 v = Input.mousePosition;
            Debug.Log(v);
            raycast(v);
        }
    #endif
    }

    private void raycast(Vector2 screenTouch)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenTouch), Vector2.zero);
        if (hitInfo)
        {
            GameObject obj = hitInfo.transform.gameObject;
            Room room = obj.GetComponent<Room>();
            if(takenRoom != null && takenRoom.member != Room.Bead.EMPTY)
            {
                room.setMove(takenRoom.getMember());
                // remove from previous room

                takenRoom = null;
            }
            if (!room.isEmpty())
            {
                Color tmpColor = obj.GetComponent<SpriteRenderer>().color;
                tmpColor.a = 0.5f;
                obj.GetComponent<SpriteRenderer>().color = tmpColor;
                takenRoom = room;
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

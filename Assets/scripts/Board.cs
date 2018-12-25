using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public Room[] rooms = new Room[9];
    public Texture2D rooTex, veeTex;
    private Room takenRoom;
    private Boolean taken;
    private Room.Bead currentTurn;
    private readonly int[] vees = new int[3] { 6, 7, 8 };
    private readonly int[] roos = new int[3] { 0, 1, 2 };
    
	void Start () {
        currentTurn = UnityEngine.Random.value > 0.5f ? Room.Bead.VEE : Room.Bead.ROO;
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
#endif
#if UNITY_EDITOR || UNITY_FACEBOOK
        if (Input.GetMouseButtonDown(0))
        {
            Raycast(Input.mousePosition);
        }
    #endif
    }

    private void Raycast(Vector2 screenTouch)
    {
        RaycastHit2D hitInfo = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(screenTouch), Vector2.zero);
        if (hitInfo)
        {
           
            Room room = hitInfo.transform.gameObject.GetComponent<Room>();
            if (taken)
            {
                MakeAMove(takenRoom, room);
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

    private void MakeAMove(Room takenRoom, Room destRoom)
    {
        /*
        * TODO:
        *  4. Move animation
        */
        if (destRoom.index == takenRoom.index)
        {
            Debug.Log("Back this bead.");
            Color tmpColor = destRoom.GetComponent<SpriteRenderer>().color;
            tmpColor.a = 1f;
            destRoom.GetComponent<SpriteRenderer>().color = tmpColor;
            taken = false;
            takenRoom = null;
            return;
        }
        if (!takenRoom.ValidMove(destRoom))
        {
            return;
        }
        destRoom.SetMove(takenRoom.getMember());
        takenRoom.RemoveMove();
        if (isWin(takenRoom.index, destRoom.index))
        {
            Debug.Log(destRoom.getMember() + " won!");
        }
        taken = false;
        currentTurn = currentTurn == Room.Bead.VEE ? Room.Bead.ROO : Room.Bead.VEE;
    }

    private Boolean isWin(int prevIndex, int currentIndex)
    {
        // Refresh positions
        int[] arr = rooms[currentIndex].getMember() == Room.Bead.ROO ? roos : vees;
        arr[Array.IndexOf(arr, prevIndex)] = currentIndex;
        // Check positions
        Array.Sort(arr);
        if (arr[1] - arr[0] == 2) return false; // Cases increments by 2, like 024, 135 are not valid win position

        if(rooms[currentIndex].getMember() == Room.Bead.ROO)
        {
            if(arr[0] == 0 && arr[1] == 1 && arr[2] == 2)
                return false; // Initial Position of Roo
        } else
        {
            if (arr[0] == 6 && arr[1] == 7 && arr[2] == 8)
                return false; // Initial Position of Vee
        }
        return arr[1] - arr[0] == arr[2] - arr[1];
    }

    private void InitiateRooms()
    {
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                int rIndex = 3 * (i + 1) + j + 1;
                GameObject obj = new GameObject("RoomIndex : " + rIndex);
                rooms[rIndex] = obj.AddComponent<Room>();
                rooms[rIndex].setIndex(rIndex);
                obj.transform.SetParent(transform);
                rooms[rIndex].Position = new Vector2(j, i * -1);
                obj.transform.localPosition = rooms[rIndex].GetPosition();
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
}

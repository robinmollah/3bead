using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Board : MonoBehaviour {
    public Room[] rooms = new Room[9];
    public static string LabelText = "No text";
    public Texture2D rooTex, veeTex;
    private Room takenRoom;
    private Boolean taken;
    private Room.Bead currentTurn = Room.Bead.VEE;
    private readonly int[] vees = new int[3] { 6, 7, 8 };
    private readonly int[] roos = new int[3] { 0, 1, 2 };
    private RooAi ai;


	void Start () {
        ai = new RooAi(this);
        InitiateRooms();
	}

    private void OnGUI()
    {
        Vector2 labelSize = new Vector2(200, Screen.height * 0.75f);
        Vector2 labelPosition = new Vector2(Screen.width - labelSize.x - 20, 10);
        GUI.Label(new Rect(labelPosition, labelSize), LabelText);
    }

    void Update () {
        if(currentTurn == Room.Bead.ROO)
        {
            Debug.Log("Roo's turn.");
            ai.Decide();
            // ToggleTurn();
            currentTurn = Room.Bead.VEE;
            return;
        }
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

    private void ToggleTurn()
    {
        currentTurn = currentTurn == Room.Bead.ROO ? Room.Bead.VEE : Room.Bead.ROO;
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
                if (!room.IsEmpty() && room.getMember() == currentTurn)
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

    public void MakeAMove(Room takenRoom, Room destRoom)
    {
        /*
        * TODO:
        *  4. Move animation
        */
        if (destRoom.GetIndex() == takenRoom.GetIndex())
        {
            Debug.Log("Back this bead.");
            Color tmpColor = destRoom.GetComponent<SpriteRenderer>().color;
            tmpColor.a = 1f;
            destRoom.GetComponent<SpriteRenderer>().color = tmpColor;
            taken = false;
            takenRoom = null;
            return;
        }
        if (!takenRoom.IsValidMove(destRoom))
        {
            return;
        }
        destRoom.SetMove(takenRoom.getMember());
        if (IsWin(takenRoom.GetIndex(), destRoom.GetIndex()))
        {
            Debug.Log(destRoom.getMember() + " won! " + MyArray.ToString(GetBeadPositions(destRoom.getMember())));
            Restart();
        }
        takenRoom.RemoveMove();
        taken = false;
        currentTurn = currentTurn == Room.Bead.VEE ? Room.Bead.ROO : Room.Bead.VEE;
    }

    private void Restart()
    {
        Board.LabelText += "Victory!!!";
        // Wait for some second before making the next
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    internal Boolean IsWin(int prevIndex, int currentIndex)
    {
        int[] arr = RefreshPositions(prevIndex, currentIndex);
        // TODO is there any way to set who has a WiningMove in next move?
        if (rooms[currentIndex].getMember() == Room.Bead.ROO)
        {
            if(arr[0] == 0 && arr[1] == 1)
                return false; // Initial Position of Roo
        } else
        {
            if (arr[0] == 6 && arr[1] == 7)
                return false; // Initial Position of Vee
        }
        if (arr[1] - arr[0] == 2)
        {
            if (arr[0] != 2) // Matches 2 4 6
            {
                return false;
            }
        } else if(arr[1] - arr[0] == 1)
        {
            return arr[0] % 3 == 0 ? arr[2] - arr[1] == 1 : false;
        }
        return arr[1] - arr[0] == arr[2] - arr[1];
    }

    internal int[] RefreshPositions(int prevIndex, int currentIndex)
    {
        int[] arr = rooms[prevIndex].getMember() == Room.Bead.ROO ? roos : vees;
        arr[Array.IndexOf(arr, prevIndex)] = currentIndex;
        // Check positions
        Array.Sort(arr);
        return arr;
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

    public int[] GetBeadPositions(Room.Bead bead)
    {
        return bead == Room.Bead.ROO ? roos : vees;
    }
}

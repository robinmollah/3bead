using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CanvasEvents : MonoBehaviour {
    public void OnStart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}

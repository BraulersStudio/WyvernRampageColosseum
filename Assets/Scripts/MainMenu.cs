using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using Unity.VisualScripting;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject pausePanel;
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);

    }

    // Update is called once per frame
    void Update()
    {
        ResetOnPause();
        ContinueGame();
        if (Input.GetKey(KeyCode.Space) && SceneManager.GetActiveScene().buildIndex == 0)
        {
            PlayGame();
        }

        if (SceneManager.GetActiveScene().buildIndex != 1)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

    }

    public void ContinueGame()
    {
        if (Input.GetKey(KeyCode.C))
        {
            pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }

    public void ResetOnPause()
    {
        if (Input.GetKey(KeyCode.R))
        {
            SceneManager.LoadSceneAsync(0);
        }


    }

}

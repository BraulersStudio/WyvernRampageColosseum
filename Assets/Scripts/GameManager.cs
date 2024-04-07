using JetBrains.Annotations;
using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public TMP_Text colleNumTxt, totalCollTxt, wavCount, msnTxt, healtmsn;
    private GameObject panel, tuto;    
    public SpawnManager spwnScript;
    public PlayerController playCtrl;  
    
    void Start()
    {
        panel = GameObject.FindGameObjectWithTag("PausePnl");
        tuto = GameObject.FindGameObjectWithTag("TutoPnl");
        tuto.SetActive(true);
        Time.timeScale = 0f;
        panel.SetActive(false);
       
    }


    // Update is called once per frame
    void Update()
    {
        wavCount.text = spwnScript.waveCount.ToString();
        totalCollTxt.text = spwnScript.enemyCount.ToString();
        msnTxt.text = playCtrl.msn.ToString();
        healtmsn.text = playCtrl.health.ToString();
       

        if (Input.GetKey(KeyCode.P))
        {
            panel.SetActive(true);
            Time.timeScale = 0;
        }

        GOScene();

        if (Input.GetKey(KeyCode.T))
        {
            tuto.SetActive(false);
            Time.timeScale = 1.0f;
        }
    }   

    public void LoadNextScene()
    {
        if (panel != null)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    public void Restart()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            panel.SetActive(true);

            if (panel != null)
            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }

        }
    }

    public void GOScene() {

        if (playCtrl.health <= 0) {
            SceneManager.LoadScene("GameOver");
        }
    }

    public void VictoryScene()
    {
        if (spwnScript.waveCount == 11)
        {
            SceneManager.LoadScene("Win");
        }
      
    }

    
}

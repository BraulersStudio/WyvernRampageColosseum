using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class GameManager : MonoBehaviour
{
    public TMP_Text colleNumTxt, totalCollTxt;
    public GameObject panel;
    private int colleNum, totalCollNum;
    public GameObject swpn;
    void Start()
    {
       colleNum = swpn.transform.childCount;
        panel = panel.gameObject;
        
    }

    public void AddCollect()
    {
        totalCollTxt.text = totalCollNum.ToString();
        colleNum++;
        colleNumTxt.text = colleNum.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void PauseScene(Scene scene)
    {
        if(Input.GetKey(KeyCode.P))
        {
           panel.SetActive(true);
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
        if (SceneManager.GetActiveScene().buildIndex == 1 && transform.childCount <= 0)
        {
            panel.SetActive(true);

            if (panel != null)
            {

                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
            }

        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuNavigation : MonoBehaviour
{
    public Transform mainMenu;
    public Transform controlMenu;

    // Start is called before the first frame update
    void Start()
    {
        mainMenu.gameObject.SetActive(true);
        controlMenu.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnPlayButton()
    {
        Debug.Log("BUTTON PRESSED");
        SceneManager.LoadScene("MainScene");
    }
    public void OnControlsButton()
    {
        controlMenu.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
    }
    public void OnControlsBackButton()
    {
        mainMenu.gameObject.SetActive(true);
        controlMenu.gameObject.SetActive(false);
    }
    public void OnQuitButton()
    {
        Application.Quit();
    }


}

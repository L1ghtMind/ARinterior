using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonHide : MonoBehaviour
{
    public GameObject[] button;
    CanvasGroup canvas;
    
    // Start is called before the first frame update
    void Start()
    {
        //button = GetComponent<Button>();
        canvas = GetComponent<CanvasGroup>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideCanvas()
    {
        canvas.alpha = 0;
        canvas.blocksRaycasts = false;

        StartCoroutine("ShowCanvas");
    }

    IEnumerator ShowCanvas()
    {
        yield return new WaitForSeconds(1);
        canvas.alpha = 1;
        canvas.blocksRaycasts = true;
    }
    /*
    public void hideCanvas()
    {
        button = GameObject.FindGameObjectsWithTag("Button");
        for(int i = 0; i < button.Length; i++)
        {
            button[i].SetActive(false);
        }
        StartCoroutine("showButton");
    }

   
    IEnumerator showButton()
    {
        yield return new WaitForSeconds(1);
        for(int i = 0; i<button.Length;i++)
        {
            button[i].SetActive(true);
        }
    }
    */
}

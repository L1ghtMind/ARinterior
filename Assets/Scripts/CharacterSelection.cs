using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class CharacterSelection : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    public GameObject[] characters;
    public int selectedCharacter = 0;
    public int temp = 0;
    public int index = 0;
    /*
        public void NextCharacter()
        {
            characters[selectedCharacter].SetActive(false);
            selectedCharacter = (selectedCharacter + 1) % characters.Length;
            characters[selectedCharacter].SetActive(true);
        }
    */
    public void OnBeginDrag(PointerEventData eventData)
    {
        if ((Mathf.Abs(eventData.delta.x)) > (Mathf.Abs(eventData.delta.y)))
        {
            if (eventData.delta.x > 0)
            {
                characters[selectedCharacter].SetActive(false);
                selectedCharacter = (selectedCharacter + 1) % characters.Length;
                characters[selectedCharacter].SetActive(true);
            }
            else
            {
                characters[selectedCharacter].SetActive(false);
                selectedCharacter--;
                if (selectedCharacter < 0)
                {
                    selectedCharacter += characters.Length;
                }
                characters[selectedCharacter].SetActive(true);
            }
           
        }
       // temp = selectedCharacter;
        //Value(temp);
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        // throw new System.NotImplementedException();
    }


    

   

    /*
           public void PreviousCharacter()
           {
               characters[selectedCharacter].SetActive(false);
               selectedCharacter--;
               if(selectedCharacter<0)
               {
                   selectedCharacter += characters.Length;
               }
               characters[selectedCharacter].SetActive(true);
           }
   */
    public void StartGame(int selectedCharacter)
    {
        PlayerPrefs.SetInt("selectedCharacter", selectedCharacter);
        SceneManager.LoadScene(1, LoadSceneMode.Single);

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class NonPlayerCharacter : MonoBehaviour
{
    public float displayTime = 4.0f;
    public GameObject dialogBox;
    float timerDisplay;
    public Text fixedrobots;

    AudioSource audioSource;
    public AudioClip frogsound;

    void Start()
    {
        dialogBox.SetActive(false);
        timerDisplay = -1.0f;
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (timerDisplay >= 0)
        {
            timerDisplay -= Time.deltaTime;
            if (timerDisplay < 0)
            {
                dialogBox.SetActive(false);
            }
        }
    }

    public void DisplayDialog()
    {
        if (int.Parse(fixedrobots.text)>=5)
        {
            SceneManager.LoadScene("Level 2");
            return;
        }
        timerDisplay = displayTime;
        dialogBox.SetActive(true);
        audioSource.PlayOneShot(frogsound,1.0f);
    }
    
    



}

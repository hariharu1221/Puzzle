using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class Imsi : MonoBehaviour
{
   public void ChangeFirstScene()
    {
        SceneManager.LoadScene("Interface");
    }
    public void ChangeSecondScene()
    {
        SceneManager.LoadScene("ExInGame");
    }
    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            SceneManager.LoadScene("ExInGame");
        }*/
    }
}

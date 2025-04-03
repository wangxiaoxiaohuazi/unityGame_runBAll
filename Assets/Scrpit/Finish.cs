using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour
{
    private void Start()
    {
        GameObject canvas = GameObject.Find("Canvas");
        canvas.transform.Find("Panel").gameObject.SetActive(false); // 'false' is a character, not a boolean
    }
    void OnTriggerEnter(Collider other)
    {
        GameObject canvas = GameObject.Find("Canvas");
        canvas.transform.Find("Panel").gameObject.SetActive(true); // 'true' is a character, not a boolean
    }
}

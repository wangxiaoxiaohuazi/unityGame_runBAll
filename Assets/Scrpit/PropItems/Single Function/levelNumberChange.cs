using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class levelNumberChange : MonoBehaviour
{
    public float levelNumber;
    public bool IsDestroy = false;//是否销毁
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerEnter(Collider other)
    {
        ChangeStart(other);
    }
    public void ChangeStart(Collider other, float _number = 0, float _time = 0, bool _isDestroy = false)
    {
        GameObject _player = GameObject.FindGameObjectWithTag("Player");
        if (_player)
        {
            _player.GetComponent<player>().setLevelNode((int)_number, other);

        }
    }
}

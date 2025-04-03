using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    // Start is called before the first frame update
    //左右移动速度
    public float HorizontalSpeed = 5;
    //前进速度
    public float ForwardSpeed = 5;
    public float turnSpeed = 4;
    public bool AutoMoveForward = false;//自动向前移动


    private CharacterController characterController;//角色控制器


    void Start()
    {

        characterController = GetComponent<CharacterController>();

    }

    // Update is called once per frame
    void Update()
    {

        OnPlayerMoveing(); // 调用移动事件
    }
    private void OnPlayerMoveing()
    {
        Vector3 movement = Vector3.zero;

        if (Application.isMobilePlatform)
        {
            // 移动设备上的触摸输入
            if (Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                if (touch.phase == TouchPhase.Moved)
                {
                    float moveX = touch.deltaPosition.x * Time.deltaTime;
                    movement = new Vector3(moveX, 0.0f, ForwardSpeed * Time.deltaTime);
                }
            }
        }
        else
        {
            // PC 上的键盘输入
            float moveHorizontal = Input.GetAxis("Horizontal");
            float _ForwardSpeed = Input.GetAxis("Vertical");
            if (AutoMoveForward)
            {
                movement = new Vector3(moveHorizontal * HorizontalSpeed, 0.0f, ForwardSpeed);
            }
            else
            {
                movement = new Vector3(moveHorizontal * HorizontalSpeed, 0.0f, _ForwardSpeed * ForwardSpeed);

            }
        }

        // 使用 CharacterController 移动
        characterController.Move(movement * Time.deltaTime);

    }
}

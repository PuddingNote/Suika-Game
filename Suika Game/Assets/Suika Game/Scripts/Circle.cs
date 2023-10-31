using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public float leftBorder;    // 왼쪽벽
    public float rightBorder;   // 오른쪽벽

    public bool isDrag;         // 
    Rigidbody2D rigid;          

    public int level;           // Circle 레벨
    Animator anim;

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        anim.SetInteger("Level", level);
    }

    void Update()
    {
        if (isDrag)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // 스크린 좌표계에있는 Vector값을 월드 좌표게로 바꿔준다.
            mousePos.y = 4.25f;
            mousePos.z = 0;

            // X축 경계 설정
            leftBorder = -2.9f + transform.localScale.x / 2f;
            rightBorder = 2.9f - transform.localScale.x / 2f;

            if (mousePos.x < leftBorder)
            {
                mousePos.x = leftBorder;
            }
            else if (mousePos.x > rightBorder)
            {
                mousePos.x = rightBorder;
            }
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f); // Vector3.Lerp() : 마우스위치로 천천히 이동
        }
       
    }

    public void Drag()
    {
        isDrag = true;
    }

    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }
}

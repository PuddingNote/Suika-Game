using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public GameManager gameManager;
    public Rigidbody2D rigid;
    public CircleCollider2D circle;
    public Animator anim;

    public float leftBorder;    // 왼쪽벽
    public float rightBorder;   // 오른쪽벽

    public bool isDrag;         // 
    public int level;           // Circle 레벨

    public bool isMerge;        // Circle 합치기

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
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

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Circle")
        {
            Circle otherCircle = collision.gameObject.GetComponent<Circle>();

            // Circle 합치기
            if (level == otherCircle.level && !isMerge && !otherCircle.isMerge && level < 7)
            {
                // 나와 상대편 위치 가져오기
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = otherCircle.transform.position.x;
                float otherY = otherCircle.transform.position.y;

                // 1. 내가 아래에 있을 때
                // 2. 동일한 높이일 때, 내가 오른쪽에 있을때
                if (meY < otherY || (meY == otherY && meY > otherX))
                {
                    // 상대방은 숨기기
                    otherCircle.Hide(transform.position);

                    // 나는 레벨업
                    LevelUp();
                }
            }
        }
    }

    public void Hide(Vector3 targerPos)
    {
        isMerge = true;
        rigid.simulated = false;
        circle.enabled = false;

        StartCoroutine(HideRoutine(targerPos));
    }

    IEnumerator HideRoutine(Vector3 targerPos)
    {
        int frameCount = 0;

        while (frameCount < 20)
        {
            frameCount++;
            transform.position = Vector3.Lerp(transform.position, targerPos, 0.5f);
            yield return null;
        }

        isMerge = false;
        gameObject.SetActive(false);
    }

    void LevelUp()
    {
        isMerge = true;
        rigid.velocity = Vector2.zero;  // 물리속도 제거
        rigid.angularVelocity = 0;      // 회전속도 제거

        StartCoroutine(LevelUpRoutine());
    }

    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        anim.SetInteger("Level", level + 1);

        yield return new WaitForSeconds(0.3f);
        level++;

        gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);

        isMerge = false;
    }

}

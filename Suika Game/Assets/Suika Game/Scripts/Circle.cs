using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public GameManager gameManager;
    public Rigidbody2D rigid;
    public CircleCollider2D circle;
    public Animator anim;

    public float leftBorder;    // ���ʺ�
    public float rightBorder;   // �����ʺ�

    public bool isDrag;         // 
    public int level;           // Circle ����

    public bool isMerge;        // Circle ��ġ��

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
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition); // ��ũ�� ��ǥ�迡�ִ� Vector���� ���� ��ǥ�Է� �ٲ��ش�.
            mousePos.y = 4.25f;
            mousePos.z = 0;

            // X�� ��� ����
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
            transform.position = Vector3.Lerp(transform.position, mousePos, 0.2f); // Vector3.Lerp() : ���콺��ġ�� õõ�� �̵�
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

            // Circle ��ġ��
            if (level == otherCircle.level && !isMerge && !otherCircle.isMerge && level < 7)
            {
                // ���� ����� ��ġ ��������
                float meX = transform.position.x;
                float meY = transform.position.y;
                float otherX = otherCircle.transform.position.x;
                float otherY = otherCircle.transform.position.y;

                // 1. ���� �Ʒ��� ���� ��
                // 2. ������ ������ ��, ���� �����ʿ� ������
                if (meY < otherY || (meY == otherY && meY > otherX))
                {
                    // ������ �����
                    otherCircle.Hide(transform.position);

                    // ���� ������
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
        rigid.velocity = Vector2.zero;  // �����ӵ� ����
        rigid.angularVelocity = 0;      // ȸ���ӵ� ����

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

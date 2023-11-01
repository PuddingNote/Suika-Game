using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour
{
    public GameManager gameManager;
    public ParticleSystem effect;
    public Rigidbody2D rigid;
    public CircleCollider2D circle;
    public Animator anim;
    public SpriteRenderer spriteRenderer;

    public float leftBorder;    // ���ʺ�
    public float rightBorder;   // �����ʺ�

    public bool isDrag;         // Ŭ�����϶�
    public int level;           // Circle ����

    public bool isMerge;        // Circle ��ġ��

    public float deadTime;      // ��輱�� �ɷ����� �� �ִ� �ð�

    void Awake()
    {
        rigid = GetComponent<Rigidbody2D>();
        circle = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void OnEnable()
    {
        anim.SetInteger("Level", level);
    }

    void OnDisable()
    {
        // Circle �Ӽ� �ʱ�ȭ
        level = 0;
        isDrag = false;
        isMerge = false;

        // Circle Transform �ʱ�ȭ
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.zero;

        // Circle ���� �ʱ�ȭ
        rigid.simulated = false;
        rigid.velocity = Vector2.zero;
        rigid.angularVelocity = 0;
        circle.enabled = true;
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

    // 
    public void Drag()
    {
        isDrag = true;
    }

    // 
    public void Drop()
    {
        isDrag = false;
        rigid.simulated = true;
    }
    
    // 
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

    // 
    public void Hide(Vector3 targetPos)
    {
        isMerge = true;
        rigid.simulated = false;
        circle.enabled = false;

        if (targetPos == Vector3.up * 100)
        {
            EffectPlay();
        }

        StartCoroutine(HideRoutine(targetPos));
    }

    // 
    IEnumerator HideRoutine(Vector3 targetPos)
    {
        int frameCount = 0;

        while (frameCount < 20)
        {
            frameCount++;

            if (targetPos != Vector3.up * 100)
            {
                transform.position = Vector3.Lerp(transform.position, targetPos, 0.5f);
            }
            else if(targetPos == Vector3.up * 100)
            {
                transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, 0.2f);
            }

            yield return null;
        }

        gameManager.score += (int)Mathf.Pow(2, level); // ���� ������ �ŵ�����

        isMerge = false;
        gameObject.SetActive(false);
    }

    // Circle ������ �Լ�
    void LevelUp()
    {
        isMerge = true;
        rigid.velocity = Vector2.zero;  // �����ӵ� ����
        rigid.angularVelocity = 0;      // ȸ���ӵ� ����

        StartCoroutine(LevelUpRoutine());
    }

    // Circle ������ �ڷ�ƾ
    IEnumerator LevelUpRoutine()
    {
        yield return new WaitForSeconds(0.2f);

        anim.SetInteger("Level", level + 1);
        EffectPlay();

        yield return new WaitForSeconds(0.3f);
        level++;

        gameManager.maxLevel = Mathf.Max(level, gameManager.maxLevel);

        isMerge = false;
    }

    // 
    void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime += Time.deltaTime;

            if (deadTime > 2)
            {
                spriteRenderer.color = Color.red;
            }
            if (deadTime > 5)
            {
                gameManager.GameOver();
            }
        }
    }

    // 
    void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Finish")
        {
            deadTime = 0;
            spriteRenderer.color = Color.white;
        }
    }

    // ����Ʈ ���� �Լ�
    void EffectPlay()
    {
        effect.transform.position = transform.position;
        effect.transform.localScale = transform.localScale;
        effect.Play();
    }

}

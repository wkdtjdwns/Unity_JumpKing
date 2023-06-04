using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    public int next_move; // ������ ������ �ൿ�� �����ϴ� ���� ( (-1) -> ���� �̵� / 0 -> ���ڸ� �̵� / 1 -> ������ �̵� )

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite_renderer;
    CapsuleCollider2D capsule_collider;

    void Awake() // ������ ������ �� �ѹ�
    {
        // �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        capsule_collider = GetComponent<CapsuleCollider2D>();

        Invoke("Think", 2); // ������ 2�� �̷�
    }

    void FixedUpdate() // ������ �����Ӹ��� 
    {
        // �������� ����ϴ� �ڵ�  /  �÷��̾ Ű���带 �Է��ؼ� �����̴� ���� �ƴ� �ڵ������� �����̴� ��������Ʈ�̱� ������ FixedUpdate()���� �̵��ϴ� �Լ��� �����Ŵ
        rigid.velocity = new Vector2(next_move, rigid.velocity.y); // �˾Ƽ� ������ �������� �̵�

        // ���� ������ Ű���� �ڵ� (���������� �������� �ʱ� ����)
        Vector2 frontVec = new Vector2(rigid.position.x + next_move*0.25f, rigid.position.y); // �ڽ��� �տ� ���������� �ִ��� Ȯ���ؾ� �ϱ� ������ Ray�� �ڽ��� ���� �տ��� �׸�
                                                                                              // next_move = 1 -> ���������� �̵��ϰ� �ֱ� ������ +1�� �����ָ� ������ ���� Ȯ�� �� �� ����
                                                                                              // next_move = 0 -> ������ ������ �ֱ� ������ ���� Ȯ�� �� �ʿ䰡 ����
                                                                                              // next_move = -1 -> �������� �̵��ϰ� �ֱ� ������ -1�� �����ָ� ���� ���� Ȯ�� �� �� ����
        // ���� ������ Ȯ���ϰ� �տ� ���� ���� �� �ٽ� �����ϰ� ����
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D ray_hit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (ray_hit.collider == null) // Ray�� �浹�� ��ü�� ���� �� (�ڽ��� ���� ���������� ��)
        {
            next_move *= -1;          // �ݴ�� �̵��ϰ�

            CancelInvoke();           // ��� Invoke() �Լ� ��, ������ ��� ���߰� ��  /  CancelInvoke() -> ���� �۵����� ��� Invoke() �Լ��� ���ߴ� �Լ�

            Invoke("Think", 2);       // �� �Ŀ� �ٽ� �����ϰ� ��
        }        
        
        // ���� �̵� ������ �Ĵٺ� �� �ְ� ����
        if (next_move != 0)                           // next_move�� ���� 0�� �ƴ� �� (next_move�� ���� 0�̸� ���� �ִϸ��̼��� �ٲ� �ʿ䰡 ���� ����)
            sprite_renderer.flipX = next_move == 1;   // next_move�� ���� 1�� �� (������ �̵��� ��) ��������Ʈ�� ������ 

    }

    void Think() // ���� ������ ��� �������� �����ϴ� �Լ�  /  ����Լ� : �Լ� �ڱ� �ڽ��� ������ ȣ���ϴ� �Լ� (������ ���� ����Լ��� ����ϴ� ���� ������ ����Ű�� �ֿ� ����! -> ����Լ��� �� �����̸� �����ؾ� ��)  /  �����̸� �ִ� �Լ� -> Invoke() : �־��� �ð��� ������ ������ �Լ��� ������
    {
        // Random -> ������ ������ ���� �����ϴ� Ŭ����  /  Range(�ּ�, �ִ�) -> �ּ� ~ (�ִ� - 1) ������ ���� �� ����
        next_move = Random.Range(-1, 2);              // -1 ~ 1������ ���� ��


        anim.SetInteger("walk_speed", next_move);     // next_move�� ���� ���� �ִϸ��̼��� �ٸ��� ����


        float next_think_time = Random.Range(2f, 5f); // ���� ���������� ������
        Invoke("Think", next_think_time);             // ������ �ð��� ���� �� �ڱ� �����θ� �ҷ���
    }

    public void OnDamaged() // ���� ���� �޾��� ��
    {
        sprite_renderer.color = new Color(1, 1, 1, 0.4f);    // ������ �ٲ���

        sprite_renderer.flipY = true;                        // ���� �Ųٷ� ������

        capsule_collider.enabled = false;                    // collider�� ��Ȱ��ȭ ��Ŵ

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse); // ���� �̵���

        Invoke("DeActive", 5);                               // 5�� �ڿ� ������Ʈ�� ��Ȱ��ȭ ��Ŵ
    }

    void DeActive() // ������Ʈ ��Ȱ��ȭ �Լ�
    {
        gameObject.SetActive(false); // ������Ʈ�� ��Ȱ��ȭ ��Ŵ
    }
    
}

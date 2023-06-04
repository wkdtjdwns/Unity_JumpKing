using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // �÷��̾ ���� ������
    // �̵� ���� ����
    public float max_speed;       // �ְ� �ӵ�
    public float jump_move;       // �����ϸ鼭 �̵��ϴ� �ӵ�
    public float flip_ratio = 1f; // ��������Ʈ flip(������) ���� ����

    // ���� ���� ����
    public bool is_charge;        // ������ �ϱ����� �⸦ ������ ���� ����
    public float charging_time;   // ���� Ű�� ������ �ִ� �ð�
    public float jump_power;      // ������ ����
    public float jump_time;       // ������ �ϰ� �ִ� �ð�
    
    // �ʱ�ȭ ���Ѿ� �� ������
    // GameManager�� ���� ������
    public GameObject game_manager_obj; // GameManager ������Ʈ ����
    public GameManager game_manager;    // GameManager ���۳�Ʈ ����

    // �÷��̾ ���� ������
    Rigidbody2D rigid;
    SpriteRenderer sprite_renderer;
    Animator anim;
    CapsuleCollider2D capsule_collider;


    public void Awake() // ������ ������ �� �ѹ�
    {
        // �ʱ�ȭ
        rigid = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsule_collider = GetComponent<CapsuleCollider2D>();

        // GameManager �ʱ�ȭ
        game_manager_obj = GameObject.Find("GameManager").gameObject;
        game_manager = game_manager_obj.GetComponent<GameManager>();

        // z���� �����ϱ� ���� �ʱ�ȭ (������Ű�� ������ ��������Ʈ�� ������)
        rigid.freezeRotation = true;
    }

    private void Update() // �� �����Ӹ���
    {
        charging_time += Time.deltaTime; // �����̽� �ٸ� ���� �ð��� ��� ������

        // �Ʒ��� �Լ����� Update() �Լ����� ���� ���ѵ� ������ �ֱ� ������ ���� �ݺ� ���� ����
        Move(); // �̵��ϴ� �Լ� ����
        Jump(); // �����ϴ� �Լ� ����
    }

    void FixedUpdate() // ������ ���ݸ���
    {
        // �¿� �̵�
        // Input.GetAxisRaw() -> -1, 0, 1 �� ���� �� �� �ϳ��� ��ȯ�� (Ű���� ���� ������ �� ��� �����ؾ��� �� ���)  /  Input.GetAixs() -> �⺻�����δ� Input.GetAxisRaw()�� ������ Input.GetAxisRaw()�� ��� ���� �޾ƿ��� Input.GetAixs()�� �ε巴�� ���� �޾ƿ�
        // Horizontal -> �¿�� (����Ű ����/������) �����̴� ���� �޾ƿ� (���� = -1 / ������ = 1)  /  Vertical ->  ��������(����Ű ��/�Ʒ�) �����̴� ���� �޾ƿ� (���� = 1 / �Ʒ��� = -1)
        float h = Input.GetAxisRaw("Horizontal");

        if (!anim.GetBool("is_jumping") && !is_charge) // ���� �ִϸ��̼��� ����ǰ� ���� ���� �� (���� �����ϰ� ���� ���� ��) && �⸦ ������ ���� ���� ��
        {
            // AddForce() -> RigidBody���� ���� ��������  /  ���� : AddForce(���� * ���� ��, ���� ����)
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        }

        if (rigid.velocity.x > max_speed)                                     // ���� �ӵ� (������)�� �ְ� �ӵ��� �Ѿ��� ��
        {
            rigid.velocity = new Vector2(max_speed, rigid.velocity.y);        // ���� �ӵ� (������)�� �ְ� �ӵ��� �ٲ�
        }

        else if (rigid.velocity.x < max_speed * (-1))                         // ���� �ӵ� (����)�� �ְ� �ӵ��� �Ѿ��� ��
        {
            rigid.velocity = new Vector2(max_speed * (-1), rigid.velocity.y); // ���� �ӵ� (����)�� �ְ� �ӵ��� �ٲ�
        }

        if (jump_time > 2.5f)                  // ���� �ִϸ��̼��� 2.5�� �̻� ����ǰ� �ִٸ� (������ 2.5�� �̻� �ϰ� �ִٸ�)
            anim.SetBool("is_jumping", false); // ���� �ִϸ��̼��� �����Ŵ

        // Debug -> ������ �����ϴ� ���� ���� ������ϱ� ���� ������ Ŭ����  
        // DrawRay() -> ����� �Բ�  ���� �������� (���� ���� + ����� ����) ���� ���� �׸�
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // �Ű����� : DrawRay(Start = ���� ����, dir = Ray�� ����� ����, Color = ������ ����, duration = ������ ǥ�� �ð�,  depthTest = ������ ǥ�� ����)
                                                                         // ���� ����� ���� DrawRay(Start, dir, Color)

        // RaycastHit -> Raycast���� ������ �ٽ� ��µ� �����  /  Raycast -> �������� Ray�� ���� �Ÿ� �̳��� ��ü�� �ִ��� ������ "�浹����"�� ����  /  LayerMask -> 32��Ʈ�� int��  / GetMask(string) -> �Ű������� �ִ� ���̾� �̸��� �ش��ϴ� ���� ���� �����ϴ� �Լ�
        RaycastHit2D ray_hit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // �Ű����� : Raycast(������ ������, ������ ����, RaycastHit ����, �ִ� �Ÿ�);

        if (rigid.velocity.y < 0 )                     // ���� �������� ���� �� (���� ������ ���� ���� ����)
        {
            if (ray_hit.collider != null)              // Ray�� �浹�� ��ü�� ���� ���� �� (Ray�� ��ü�� �浹���� ��)
            {
                if (ray_hit.distance < 0.5f)           // ���� �浹�� ��ü���� �Ÿ��� 0.5 ������ �� (�÷��̾ ���� ���� ��)
                {
                    anim.SetBool("is_jumping", false); // ���� �ִϸ��̼��� �����Ŵ
                }
            }
        }
    }

    void Move() // �÷��̾� �̵� �Լ�
    {
        // �ִϸ��̼� (���� ��ȯ)
        sprite_renderer.flipX = Input.GetAxisRaw("Horizontal") == -1; // Input.GetAxisRaw("Horizontal")�� ���� -1 (���� = -1)�̶�� flip�� X�� �ٲ� (�������� ������ ��ȯ��Ŵ)

        // ������ �� �ӵ� ���̱�
        if (Input.GetButtonUp("Horizontal") && !anim.GetBool("is_jumping")) // �¿� �̵� ��ư�� ������ �� && �⸦ ������ ���� ���� �� /  ��ư���� ���� ���� ���� "�ܹ�����" Ű���� �Է��� Update()���� ����ؾ� ���� �� ���ɼ��� ����
            // velocity -> rigid�� ���� �ӵ�  /  normalized -> ���� ũ�⸦ 1�� ���� ���� (��������  /  ���⿡ ���� �̵� �ӵ��� �������� �ϱ� ����)
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y); // �ӵ��� ����

        // �ִϸ��̼� (�ȱ�)
        // Mathf -> ���� ���� �Լ��� �����ϴ� Ŭ����  /  Abs() -> �Ű������� �ִ� ���� �������� �����
        if (Mathf.Abs(rigid.velocity.x) < 0.3) // ���� �ӵ��� ���� 0.3���� ���� ��
            anim.SetBool("is_walking", false); // is_walking �ִϸ��̼��� false(������ ����)�� �ٲ�

        else                                   // �׷��� ���� �� (���� �ӵ��� ���� 0.3���� Ŭ ��)
            anim.SetBool("is_walking", true);  // is_walking �ִϸ��̼����� true(�ȴ� �ִϸ��̼�)�� �ٲ�
    }
    
    bool Jump() // �÷��̾� ���� �Լ� (bool ���� is_charging�� ���� ���ֱ� ������ bool������ ������)
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool("is_jumping")) // ���� ��ư�� �Է����� �� && ���� �ִϸ��̼��� ����ǰ� ���� ���� �� (���� �����ϰ� ���� ���� ��)
        {
            // �Լ��� ����Ǿ��� �� �⺻������ ������ �ڵ�
            charging_time = 0.0f; // �����̽� �ٸ� ���� �ð� �ʱ�ȭ
            jump_move = 0.0f;     // �����ϸ鼭 �����̴� �ӵ� �ʱ�ȭ
            is_charge = true;     // is_charing ���� true�� �ٲ�

            // �������� �����ؼ� ������ �ڵ�
            if (Input.GetAxisRaw("Horizontal") == 1)       // ������ �̵� ��ư�� ������ ��
                jump_move = max_speed * 3.5f;

            else if (Input.GetAxisRaw("Horizontal") == -1) // ���� �̵� ��ư�� ������ ��
                jump_move = max_speed * (-3.5f);
        }

        if (Input.GetButtonUp("Jump") && !anim.GetBool("is_jumping") && !anim.GetBool("is_walking")) // ���� ��ư�� ���� �� && ���� �ִϸ��̼��� ����ǰ� ���� ���� �� (���� �����ϰ� ���� ���� ��) && �ȱ� �ִϸ��̼��� ����ǰ� ���� ���� �� (���� �Ȱ� ���� ���� ��)
        {
            is_charge = false;
            jump_time = 0.0f;

            if (charging_time >= 1.0f) // �⸦ ���� �ð��� 1���� Ŭ �� (���� ��ư�� ���� �ð��� ���� ������ ���⸦ �ٸ��� �ϱ� ���� ����)
            {
                // rigid.AddForce(Vector2.up * jump_power, ForceMode2D.Impulse); ó�� AddForce() �ε� ������ �� ����
                rigid.velocity = new Vector2(jump_move, jump_power * 2.0f);        // ������ ���⸦ �ٸ��� ���ְ�
                anim.SetBool("is_jumping", true);                                  // �����ϴ� �ִϸ��̼��� �����ϸ�
                SoundManager.instance.PlaySound("Jump");                           // �����ϴ� ���� �����

            }

            else if (charging_time >= 0.5f)
            {
                rigid.velocity = new Vector2(jump_move * 1.2f, jump_power * 1.5f); // ������ ���⸦ �ٸ��� ���ְ�
                anim.SetBool("is_jumping", true);                                  // �����ϴ� �ִϸ��̼��� �����ϸ�
                SoundManager.instance.PlaySound("Jump");                           // �����ϴ� ���� �����
            }

            else
            {
                rigid.velocity = new Vector2(jump_move * 1.5f, jump_power);       // ������ ���⸦ �ٸ��� ���ְ�
                anim.SetBool("is_jumping", true);                                 // �����ϴ� �ִϸ��̼��� �����ϸ�
                SoundManager.instance.PlaySound("Jump");                          // �����ϴ� ���� �����

            }
        }

        if (anim.GetBool("is_jumping"))  // ���� �ִϸ��̼��� ����ǰ� ���� ��(���� �����ϰ� ���� ��)
            jump_time += Time.deltaTime; // �����ϴ� �ð��� ��� ������

        return is_charge; // �� �Լ��� ���� �� is_charging�� ���� ������
    }        
       
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") // �÷��̾�� ���� ��ü�� tag�� "Enemy"(��)�̸� ����
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) // �Ʒ��� �������� �� && ���ͺ��� ���� ���� �� -> ���͸� ���� ������ ��
            {
                OnAttack(collision.transform);                                                 // ���� ����� �� �����ϴ� �Լ� ����
            }
            else                                                                               // �װ� �ƴ϶�� (���Ϳ��� �ǰ� ���ߴٸ�)
                OnDamaged(collision.transform.position);                                       // �������� �Ծ��� �� �����ϴ� �Լ� ����
        }

        else if (collision.gameObject.tag == "Flag") // �÷��̾�� ���� ��ü�� tag�� "Flag"(���)�̸� ����
            game_manager.NextStage();

        else if (collision.gameObject.tag == "Bounce")
            SoundManager.instance.PlaySound("Bounce"); // ƨ�ܳ��� ���� ���
    }

    void OnAttack(Transform enemy) // ���� ����� ��
    {
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // �÷��̾ �����ϴ� ���� �̵��� �ϰ� ��

        EnemyMove enemy_move = enemy.GetComponent<EnemyMove>(); // EnemyMove ��ũ��Ʈ�� �Լ��� ����� �� �ְ� ���۳�Ʈ�� ������
        enemy_move.OnDamaged();                                 // EnemyMove ��ũ��Ʈ�� OnDamaged() �Լ��� ������

        SoundManager.instance.PlaySound("Attack");              // �����ϴ� ���� ���
    }

    void OnDamaged(Vector2 target_pos) // �÷��̾ �������� �Ծ��� �� (���� ���·� ����� �Լ�)
    {
        game_manager.HealthDown();                                   // ü���� ���� ��Ŵ

        gameObject.layer = 9;                                        // gameObject�� Layer �� 9��° Layer(PlayerDamaged)�� �ٲ���

        sprite_renderer.color = new Color(1, 1, 1, 0.4f);            // ������ �ٲ���
                                                                     // �Ű����� -> ������� R, G, B, ������ ����

        int dirc = transform.position.x - target_pos.x > 0 ? 1 : -1; // �÷��̾��� x��ǥ -  ���� ���� x��ǥ�� 0���� ũ�� 1��, �ƴϸ� -1�� dirc������ ������ (���׿�����)
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse); // ������ ������ ������ ƨ�� ����

        Invoke("OffDamaged",1.25f);                                  // 1.25�ʰ� ������ ���� ���°� ������
    }

    void OffDamaged() // ���� ���¸� �����ϴ� �Լ�
    {
        gameObject.layer = 8;                          // gameObject�� Layer �� 8��° Layer(Player)�� �ٲ��� -> ������� ��������

        sprite_renderer.color = new Color(1, 1, 1, 1); // ������ ������� ��������
    }

    public void VelocityZero() // �÷��̾��� ���� �ӵ��� 0���� ����� �Լ�
    {
        rigid.velocity = Vector2.zero;
    }
}

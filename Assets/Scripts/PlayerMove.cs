using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    // 플레이어에 대한 변수들
    // 이동 관련 변수
    public float max_speed;       // 최고 속도
    public float jump_move;       // 점프하면서 이동하는 속도
    public float flip_ratio = 1f; // 스프라이트 flip(뒤집기) 위한 변수

    // 점프 관련 변수
    public bool is_charge;        // 점프를 하기위한 기를 모으는 지의 여부
    public float charging_time;   // 점프 키를 누르고 있는 시간
    public float jump_power;      // 점프의 세기
    public float jump_time;       // 점프를 하고 있는 시간
    
    // 초기화 시켜야 할 변수들
    // GameManager에 대한 변수들
    public GameObject game_manager_obj; // GameManager 오브젝트 변수
    public GameManager game_manager;    // GameManager 컴퍼넌트 변수

    // 플레이어에 대한 변수들
    Rigidbody2D rigid;
    SpriteRenderer sprite_renderer;
    Animator anim;
    CapsuleCollider2D capsule_collider;


    public void Awake() // 게임을 시작할 때 한번
    {
        // 초기화
        rigid = GetComponent<Rigidbody2D>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        capsule_collider = GetComponent<CapsuleCollider2D>();

        // GameManager 초기화
        game_manager_obj = GameObject.Find("GameManager").gameObject;
        game_manager = game_manager_obj.GetComponent<GameManager>();

        // z축을 고정하기 위한 초기화 (고정시키지 않으면 스프라이트가 굴러감)
        rigid.freezeRotation = true;
    }

    private void Update() // 매 프레임마다
    {
        charging_time += Time.deltaTime; // 스페이스 바를 누른 시간을 계속 누적함

        // 아래의 함수들은 Update() 함수에서 실행 시켜도 조건이 있기 때문에 무한 반복 되지 않음
        Move(); // 이동하는 함수 실행
        Jump(); // 점프하는 함수 실행
    }

    void FixedUpdate() // 일정한 간격마다
    {
        // 좌우 이동
        // Input.GetAxisRaw() -> -1, 0, 1 세 가지 값 중 하나가 반환됨 (키보드 값을 눌렀을 때 즉시 반응해야할 때 사용)  /  Input.GetAixs() -> 기본적으로는 Input.GetAxisRaw()과 같지만 Input.GetAxisRaw()는 즉시 값을 받아오고 Input.GetAixs()는 부드럽게 값을 받아옴
        // Horizontal -> 좌우로 (방향키 왼쪽/오른쪽) 움직이는 값을 받아옴 (왼쪽 = -1 / 오른쪽 = 1)  /  Vertical ->  수직으로(방향키 위/아래) 움직이는 값을 받아옴 (위쪽 = 1 / 아래쪽 = -1)
        float h = Input.GetAxisRaw("Horizontal");

        if (!anim.GetBool("is_jumping") && !is_charge) // 점프 애니매이션이 실행되고 있지 않을 때 (현재 점프하고 있지 않을 때) && 기를 모으고 있지 않을 때
        {
            // AddForce() -> RigidBody에게 힘을 전달해줌  /  사용법 : AddForce(방향 * 힘의 값, 힘의 종류)
            rigid.AddForce(Vector2.right * h, ForceMode2D.Impulse);
        }

        if (rigid.velocity.x > max_speed)                                     // 현재 속도 (오른쪽)가 최고 속도를 넘었을 때
        {
            rigid.velocity = new Vector2(max_speed, rigid.velocity.y);        // 현재 속도 (오른쪽)를 최고 속도로 바꿈
        }

        else if (rigid.velocity.x < max_speed * (-1))                         // 현재 속도 (왼쪽)가 최고 속도를 넘었을 때
        {
            rigid.velocity = new Vector2(max_speed * (-1), rigid.velocity.y); // 현재 속도 (왼쪽)를 최고 속도로 바꿈
        }

        if (jump_time > 2.5f)                  // 점프 애니매이션이 2.5초 이상 실행되고 있다면 (점프를 2.5초 이상 하고 있다면)
            anim.SetBool("is_jumping", false); // 점프 애니매이션을 종료시킴

        // Debug -> 게임을 개발하는 동안 쉽게 디버깅하기 위해 목적의 클래스  
        // DrawRay() -> 색상과 함께  시작 지점부터 (시작 지점 + 방향과 길이) 까지 선을 그림
        Debug.DrawRay(rigid.position, Vector3.down, new Color(0, 1, 0)); // 매개변수 : DrawRay(Start = 시작 지점, dir = Ray의 방향과 길이, Color = 라인의 색상, duration = 라인의 표시 시간,  depthTest = 라인의 표시 여부)
                                                                         // 지금 사용한 변수 DrawRay(Start, dir, Color)

        // RaycastHit -> Raycast에서 정보를 다시 얻는데 사용함  /  Raycast -> 원점에서 Ray를 날려 거리 이내에 물체가 있는지 없는지 "충돌감지"를 해줌  /  LayerMask -> 32비트의 int형  / GetMask(string) -> 매개변수에 있는 레이어 이름에 해당하는 정수 값을 리턴하는 함수
        RaycastHit2D ray_hit = Physics2D.Raycast(rigid.position, Vector3.down, 1, LayerMask.GetMask("Platform")); // 매개변수 : Raycast(광선의 시작점, 광선의 방향, RaycastHit 변수, 최대 거리);

        if (rigid.velocity.y < 0 )                     // 현재 떨어지고 있을 때 (무한 점프를 막기 위한 조건)
        {
            if (ray_hit.collider != null)              // Ray에 충돌한 물체가 없지 않을 때 (Ray에 물체가 충돌했을 때)
            {
                if (ray_hit.distance < 0.5f)           // 만약 충돌한 물체와의 거리가 0.5 이하일 때 (플레이어가 착지 했을 때)
                {
                    anim.SetBool("is_jumping", false); // 점프 애니매이션을 종료시킴
                }
            }
        }
    }

    void Move() // 플레이어 이동 함수
    {
        // 애니매이션 (방향 전환)
        sprite_renderer.flipX = Input.GetAxisRaw("Horizontal") == -1; // Input.GetAxisRaw("Horizontal")의 값이 -1 (왼쪽 = -1)이라면 flip을 X로 바꿈 (왼쪽으로 방향을 전환시킴)

        // 멈췄을 때 속도 줄이기
        if (Input.GetButtonUp("Horizontal") && !anim.GetBool("is_jumping")) // 좌우 이동 버튼을 때었을 때 && 기를 모으고 있지 않을 때 /  버튼에서 손을 때는 등의 "단발적인" 키보드 입력은 Update()에서 사용해야 누락 될 가능성이 낮음
            // velocity -> rigid의 현재 속도  /  normalized -> 벡터 크기를 1로 만든 상태 (단위벡터  /  방향에 따른 이동 속도가 같아지게 하기 위함)
            rigid.velocity = new Vector2(rigid.velocity.normalized.x * 0.5f, rigid.velocity.y); // 속도를 줄임

        // 애니매이션 (걷기)
        // Mathf -> 수학 관련 함수를 제공하는 클래스  /  Abs() -> 매개변수에 있는 값을 절댓값으로 출력함
        if (Mathf.Abs(rigid.velocity.x) < 0.3) // 만약 속도가 절댓값 0.3보다 작을 때
            anim.SetBool("is_walking", false); // is_walking 애니매이션을 false(가만히 서기)로 바꿈

        else                                   // 그렇지 않을 때 (만약 속도가 절댓값 0.3보다 클 때)
            anim.SetBool("is_walking", true);  // is_walking 애니매이션을을 true(걷는 애니매이션)로 바꿈
    }
    
    bool Jump() // 플레이어 점프 함수 (bool 변수 is_charging을 리턴 해주기 때문에 bool형으로 선언함)
    {
        if (Input.GetButtonDown("Jump") && !anim.GetBool("is_jumping")) // 점프 버튼을 입력했을 때 && 점프 애니매이션이 실행되고 있지 않을 때 (현재 점프하고 있지 않을 때)
        {
            // 함수가 실행되었을 때 기본적으로 실행할 코드
            charging_time = 0.0f; // 스페이스 바를 누른 시간 초기화
            jump_move = 0.0f;     // 점프하면서 움직이는 속도 초기화
            is_charge = true;     // is_charing 값을 true로 바꿈

            // 움직임의 관련해서 실행할 코드
            if (Input.GetAxisRaw("Horizontal") == 1)       // 오른쪽 이동 버튼을 눌렀을 때
                jump_move = max_speed * 3.5f;

            else if (Input.GetAxisRaw("Horizontal") == -1) // 왼쪽 이동 버튼을 눌렀을 때
                jump_move = max_speed * (-3.5f);
        }

        if (Input.GetButtonUp("Jump") && !anim.GetBool("is_jumping") && !anim.GetBool("is_walking")) // 점프 버튼을 땠을 때 && 점프 애니매이션이 실행되고 있지 않을 때 (현재 점프하고 있지 않을 때) && 걷기 애니매이션이 실행되고 있지 않을 때 (현재 걷고 있지 않을 때)
        {
            is_charge = false;
            jump_time = 0.0f;

            if (charging_time >= 1.0f) // 기를 모은 시간이 1보다 클 때 (점프 버튼을 누른 시간에 따라서 점프의 세기를 다르게 하기 위한 조건)
            {
                // rigid.AddForce(Vector2.up * jump_power, ForceMode2D.Impulse); 처럼 AddForce() 로도 구현할 수 있음
                rigid.velocity = new Vector2(jump_move, jump_power * 2.0f);        // 점프의 세기를 다르게 해주고
                anim.SetBool("is_jumping", true);                                  // 점프하는 애니매이션을 실행하며
                SoundManager.instance.PlaySound("Jump");                           // 점프하는 사운드 출력함

            }

            else if (charging_time >= 0.5f)
            {
                rigid.velocity = new Vector2(jump_move * 1.2f, jump_power * 1.5f); // 점프의 세기를 다르게 해주고
                anim.SetBool("is_jumping", true);                                  // 점프하는 애니매이션을 실행하며
                SoundManager.instance.PlaySound("Jump");                           // 점프하는 사운드 출력함
            }

            else
            {
                rigid.velocity = new Vector2(jump_move * 1.5f, jump_power);       // 점프의 세기를 다르게 해주고
                anim.SetBool("is_jumping", true);                                 // 점프하는 애니매이션을 실행하며
                SoundManager.instance.PlaySound("Jump");                          // 점프하는 사운드 출력함

            }
        }

        if (anim.GetBool("is_jumping"))  // 점프 애니매이션이 실행되고 있을 때(현재 점프하고 있을 때)
            jump_time += Time.deltaTime; // 점프하는 시간을 계속 누적함

        return is_charge; // 이 함수가 끝날 때 is_charging의 값을 돌려줌
    }        
       
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy") // 플레이어와 닿은 물체의 tag가 "Enemy"(적)이면 실행
        {
            if (rigid.velocity.y < 0 && transform.position.y > collision.transform.position.y) // 아래로 낙하중일 때 && 몬스터보다 위에 있을 때 -> 몬스터를 밟은 상태일 때
            {
                OnAttack(collision.transform);                                                 // 적을 밟았을 때 실행하는 함수 실행
            }
            else                                                                               // 그게 아니라면 (몬스터에게 피격 당했다면)
                OnDamaged(collision.transform.position);                                       // 데미지를 입었을 때 실행하는 함수 실행
        }

        else if (collision.gameObject.tag == "Flag") // 플레이어와 닿은 물체의 tag가 "Flag"(깃발)이면 실행
            game_manager.NextStage();

        else if (collision.gameObject.tag == "Bounce")
            SoundManager.instance.PlaySound("Bounce"); // 튕겨나는 사운드 출력
    }

    void OnAttack(Transform enemy) // 적을 밟았을 때
    {
        rigid.AddForce(Vector2.up * 10, ForceMode2D.Impulse);   // 플레이어가 점프하는 듯한 이동을 하게 함

        EnemyMove enemy_move = enemy.GetComponent<EnemyMove>(); // EnemyMove 스크립트의 함수를 사용할 수 있게 컴퍼넌트를 가져옴
        enemy_move.OnDamaged();                                 // EnemyMove 스크립트의 OnDamaged() 함수를 실행함

        SoundManager.instance.PlaySound("Attack");              // 공격하는 사운드 출력
    }

    void OnDamaged(Vector2 target_pos) // 플레이어가 데미지를 입었을 때 (무적 상태로 만드는 함수)
    {
        game_manager.HealthDown();                                   // 체력을 감소 시킴

        gameObject.layer = 9;                                        // gameObject의 Layer 중 9번째 Layer(PlayerDamaged)로 바꿔줌

        sprite_renderer.color = new Color(1, 1, 1, 0.4f);            // 색상을 바꿔줌
                                                                     // 매개변수 -> 순서대로 R, G, B, 투명도를 뜻함

        int dirc = transform.position.x - target_pos.x > 0 ? 1 : -1; // 플레이어의 x좌표 -  맞은 적의 x좌표가 0보다 크면 1을, 아니면 -1을 dirc변수에 대입함 (삼항연산자)
        rigid.AddForce(new Vector2(dirc, 1)*7, ForceMode2D.Impulse); // 위에서 대입한 방향대로 튕겨 나감

        Invoke("OffDamaged",1.25f);                                  // 1.25초가 지나면 무적 상태가 해제됨
    }

    void OffDamaged() // 무적 상태를 해제하는 함수
    {
        gameObject.layer = 8;                          // gameObject의 Layer 중 8번째 Layer(Player)로 바꿔줌 -> 원래대로 돌려놓음

        sprite_renderer.color = new Color(1, 1, 1, 1); // 색상을 원래대로 돌려놓음
    }

    public void VelocityZero() // 플레이어의 낙하 속도를 0으로 만드는 함수
    {
        rigid.velocity = Vector2.zero;
    }
}

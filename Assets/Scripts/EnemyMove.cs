using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{

    public int next_move; // 다음에 실행할 행동을 결정하는 변수 ( (-1) -> 왼쪽 이동 / 0 -> 제자리 이동 / 1 -> 오른쪽 이동 )

    Rigidbody2D rigid;
    Animator anim;
    SpriteRenderer sprite_renderer;
    CapsuleCollider2D capsule_collider;

    void Awake() // 게임을 시작할 때 한번
    {
        // 초기화
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        sprite_renderer = GetComponent<SpriteRenderer>();
        capsule_collider = GetComponent<CapsuleCollider2D>();

        Invoke("Think", 2); // 생각을 2초 미룸
    }

    void FixedUpdate() // 일정한 프레임마다 
    {
        // 움직임을 담당하는 코드  /  플레이어가 키보드를 입력해서 움직이는 것이 아닌 자동적으로 움직이는 스프라이트이기 때문에 FixedUpdate()에서 이동하는 함수를 실행시킴
        rigid.velocity = new Vector2(next_move, rigid.velocity.y); // 알아서 랜덤한 방향으로 이동

        // 적의 지능을 키워줄 코드 (낭떠러지에 떨어지지 않기 위함)
        Vector2 frontVec = new Vector2(rigid.position.x + next_move*0.25f, rigid.position.y); // 자신의 앞에 낭떠러지가 있는지 확인해야 하기 때문에 Ray를 자신의 방향 앞에서 그림
                                                                                              // next_move = 1 -> 오른쪽으로 이동하고 있기 때문에 +1을 더해주면 오른쪽 앞을 확인 할 수 있음
                                                                                              // next_move = 0 -> 어차피 가만히 있기 때문에 앞을 확인 할 필요가 없음
                                                                                              // next_move = -1 -> 왼쪽으로 이동하고 있기 때문에 -1을 더해주면 왼쪽 앞을 확인 할 수 있음
        // 앞의 지형을 확인하고 앞에 땅이 없을 때 다시 생각하게 만듦
        Debug.DrawRay(frontVec, Vector3.down, new Color(0, 1, 0));

        RaycastHit2D ray_hit = Physics2D.Raycast(frontVec, Vector3.down, 1, LayerMask.GetMask("Platform"));

        if (ray_hit.collider == null) // Ray에 충돌한 물체가 없을 때 (자신의 앞이 낭떠러지일 때)
        {
            next_move *= -1;          // 반대로 이동하고

            CancelInvoke();           // 모든 Invoke() 함수 즉, 생각을 잠시 멈추게 함  /  CancelInvoke() -> 현재 작동중인 모든 Invoke() 함수를 멈추는 함수

            Invoke("Think", 2);       // 그 후에 다시 생각하게 함
        }        
        
        // 적이 이동 방향을 쳐다볼 수 있게 만듦
        if (next_move != 0)                           // next_move의 값이 0이 아닐 때 (next_move의 값이 0이면 굳이 애니매이션을 바꿀 필요가 없기 때문)
            sprite_renderer.flipX = next_move == 1;   // next_move의 값이 1일 때 (오른쪽 이동일 때) 스프라이트를 뒤집음 

    }

    void Think() // 적이 다음에 어떻게 움직일지 결정하는 함수  /  재귀함수 : 함수 자기 자신을 스스로 호출하는 함수 (딜레이 없이 재귀함수를 사용하는 것은 에러를 일으키는 주요 원인! -> 재귀함수는 꼭 딜레이를 지참해야 함)  /  딜레이를 주는 함수 -> Invoke() : 주어진 시간이 지나면 지정된 함수를 실행함
    {
        // Random -> 랜덤에 관련한 수를 생성하는 클래스  /  Range(최소, 최대) -> 최소 ~ (최대 - 1) 범위의 랜덤 수 생성
        next_move = Random.Range(-1, 2);              // -1 ~ 1까지의 랜덤 수


        anim.SetInteger("walk_speed", next_move);     // next_move의 값에 따라서 애니매이션을 다르게 해줌


        float next_think_time = Random.Range(2f, 5f); // 다음 생각까지의 딜레이
        Invoke("Think", next_think_time);             // 랜덤한 시간이 지난 후 자기 스스로를 불러옴
    }

    public void OnDamaged() // 적이 공격 받았을 때
    {
        sprite_renderer.color = new Color(1, 1, 1, 0.4f);    // 색상을 바꿔줌

        sprite_renderer.flipY = true;                        // 적을 거꾸로 뒤집음

        capsule_collider.enabled = false;                    // collider를 비활성화 시킴

        rigid.AddForce(Vector2.up * 5, ForceMode2D.Impulse); // 위로 이동함

        Invoke("DeActive", 5);                               // 5초 뒤에 오브젝트를 비활성화 시킴
    }

    void DeActive() // 오브젝트 비활성화 함수
    {
        gameObject.SetActive(false); // 오브젝트를 비활성화 시킴
    }
    
}

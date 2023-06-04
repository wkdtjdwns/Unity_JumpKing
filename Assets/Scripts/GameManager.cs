using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager 스크립트를 가진 오브젝트 변수

    // 설명에 대한 변수들
    public GameObject explan; // 설명을 보여주는 이미지들을 관리하는 오브젝트 변수
    public Button explan_bnt; // 설명을 보여줄지 말지 선택할 수 있게 하는 버튼 변수
    public Text bnt_text;     // 위 버튼의 텍스트 변수
    public bool is_show;      // 설명을 보여주는지 여부

    public Text tip_text;     // 설명의 텍스트 중 tip 텍스트

    // 스테이지 전환에 대한 변수들
    public GameObject[] stages; // 스테이지들을 관리할 배열 변수
    public int stage_index;     // 스테이지의 번호 변수
    public Text stage_text;     // 스테이지 텍스트 변수

    public Camera main_camera; // 카메라 변수

    // 플레이어에 대한 변수들
    public int health;         // 플레이어의 체력

    // PlayerMove에 대한 변수
    public PlayerMove player; // PlayerMove 스크립트를 변수 player에 저장함

    // esc 창(옵션 창)에 대한 변수
    public Image option; // 옵션 창의 오브젝트 변수
    bool is_option;      // esc 창이 뜨고 있는지 여부
    bool is_live;        // 현재 시간이 흐르고 있는지 여부

    // esc 창(옵션 창)에서 관리 할 변수
    public GameObject stage_text_obj; // 스테이지 텍스트를 관리할 오브젝트
    public GameObject explan_bnt_obj; // 설명 버튼을 관리할 오브젝트

    public Text stage_bnt_text;       // 스테이지 텍스트 버튼을 누를 시 바뀔 텍스트 (보이기 / 숨기기)
    public Text explan_bnt_text;      // 설명 버튼을 누를 시 바뀔 텍스트 (보이기 / 숨기기)

    public bool is_show_text;         // 스테이지 텍스트를 보여줄지 여부
    public bool is_show_bnt;          // 설명 버튼을 보여줄지 여부

    // 재시작에 대한 변수
    public GameObject restart_bnt;
    public Text restart_text;

    // bgm에 대한 변수
    AudioSource bgm_player;
    public int bgm_index;
    public Button[] bgm_button;

    // DataManager에 대한 변수
    DataManager data_manager;    
    public GameObject data_manager_obj;

    void Awake() // 게임을 시작할 때 한번
    {
        instance = this;

        is_show = false;     // 설명을 숨김
        is_show_text = true; // 스테이지 텍스트를 보여줌
        is_show_bnt = true;  // 설명 버튼을 보여줌

        is_live = true;

        instance.GetComponent<BoxCollider>();

        data_manager = data_manager_obj.GetComponent<DataManager>();

        bgm_player = GameObject.Find("Bgm Player").GetComponent<AudioSource>();
    }

    void Start() // Awake() 함수 다음에
    {
        Invoke("LoadData", 0.1f);
    }

    void Update() // 매 프레임마다
    {
        // 각각의 bool 변수의 값에 따라 보여주거나 숨김
        explan.SetActive(is_show);
        stage_text_obj.SetActive(is_show_text);
        explan_bnt_obj.SetActive(is_show_bnt);
        
        // 스테이지에 따른 텍스트 전환
        stage_text.text = string.Format("{0} 스테이지", (stage_index + 1)); // 스테이지 텍스트를 { (stage_index의 값 + 1) } 스테이지 로 바꿈

        // 현재 체력에 따른 텍스트 전환
        if (health < 2) tip_text.text = string.Format("현재 생명은 총 {0}개! 큰일이야! 조심해!!!\r\n내리막 / 오르막 길에서는 점프 불가능해!\r\n점프가 안 될때에는 제자리 점프!\r\n만약 이동이 안 될에는 조금만 기다려보자!", health);
        else if (health < 4) tip_text.text = string.Format("현재 생명은 총 {0}개! 집중! 집중!\r\n내리막 / 오르막 길에서는 점프 불가능해!\r\n점프가 안 될때에는 제자리 점프!\r\n만약 이동이 안 될에는 조금만 기다려보자!", health);       
        else if (health < 6) tip_text.text = string.Format("현재 생명은 총 {0}개! 긴장 해야겠는데?\r\n내리막 / 오르막 길에서는 점프 불가능해!\r\n점프가 안 될때에는 제자리 점프!\r\n만약 이동이 안 될에는 조금만 기다려보자!", health);
        else tip_text.text = string.Format("현재 생명은 총 {0}개!\r\n내리막 / 오르막 길에서는 점프 불가능해!\r\n점프가 안 될때에는 제자리 점프!\r\n만약 이동이 안 될에는 조금만 기다려보자!", health);

        // 옵션 UI 관련 코드
        if (Input.GetButtonDown("Cancel")) // ESC 버튼을 눌렀을 때 (ESC를 누르면 true를 반환)
        {
            Option(); // 옵션 창을 켜거나 끔
        }
    }

    public void ClickExplanBnt() // 설명 숨기기 / 보이기 버튼을 눌렀을 때
    {
        is_show = !is_show;           // 설명을 보여주는지 여부를 반대로 만듦

        if (is_show)                  // is_show = true
            bnt_text.text = "숨기기"; // 버튼의 텍스트를 "숨기기"로 바꿈

        else                          // is_show = false
            bnt_text.text = "보이기"; // 버튼의 텍스트를 "보이기"로 바꿈

        SoundManager.instance.PlaySound("Button");
    }

    public void NextStage() // 스테이지를 전환시키는 함수
    { 
        SoundManager.instance.PlaySound("Clear");                 // 스테이지 클리어 사운드 출력

        if (stage_index < stages.Length-1)                        // 현재 스테이지 번호가 맵의 개수보다 작을 때
        {
            stages[stage_index].SetActive(false);                 // 현재 스테이지를 비활성화 시키고

            stage_index++;                                        // 스테이지의 번호를 1 더한뒤

            stages[stage_index].SetActive(true);                  // 그 번호에 맞는 스테이지를 활성화 시키고

            PlayerReposition();                                   // 플레이어를 원점으로 되돌림
        }

        else                                                      // 현재 스테이지 번호가 맵의 개수보다 크거나 같을 때 (게임에 있는 모든 스테이지를 클리어 했을 때)
        {
            // 게임 관련 코드
            Time.timeScale = 0;                                   // 시간을 멈춰두고

            Debug.Log("게임 클리어!");                            // 게임을 클리어 했다는 사실을 알려주고

            restart_text.text = "클리어!";                        // 재시작 버튼의 텍스트를 "클리어"로 바꾼 뒤

            restart_bnt.SetActive(true);                          // 재시작 버튼 활성화

            // 배경음 관련 코드
            bgm_player.clip = SoundManager.instance.bgm_clips[3]; // 배경음을 클리어 bgm으로 바꾸고
            bgm_player.Play();                                    // 배경음 플레이
        }

        // 스테이지 전환에 따른 카메라 전환 코드
        if (stage_index != 3) // 현재 스테이지가 특정 스테이지(3 스테이지)가 아닐 때
        {
            main_camera.transform.position = new Vector3(-7.5f, -4.5f, -10);  // 시점을 원래대로 돌려놓음

            instance.transform.position = new Vector3(0, -6.5f, 0);           // GamaManager를 제자리로 돌려 놓음 (떨어졌을 때 데미지를 입히기 위함)
        }

        else // 현재 스테이지가 특정 스테이지(3 스테이지)일 때
        {
            health = 1;                                                       // 플레이어의 체력을 1로 만들어서 1번만 데미지를 받아도 죽게 만듦 

            main_camera.transform.position = new Vector3(-7.5f, -9.5f, -10);  // 원활한 게임을 위해 시점을 조정함

            instance.transform.position = new Vector3(0, 10000, -1);          // GamaManager를 외딴 곳에 놓음 (떨어졌을 때 데미지를 입히지 않기 위함)

        }
    }

    public void HealthDown() // 플레이어의 체력을 감소시키는 함수
    {
        if (health > 1)                                              // 플레이어의 체력이 1보다 많다면
        { 
            health--;                                                // 체력을 감소 시킴

            SoundManager.instance.PlaySound("Damaged");              // 데미지을 받는 사운드 출력
        }

        else                                                         // 플레이어의 체력이 1보다 적거나 같다면
        {
            instance.transform.position = new Vector3(0, 10000, -1); // GamaManager를 외딴 곳에 놓음 (떨어져서 죽었을 때의 버그 방지)

            Time.timeScale = 1;                                      // 시간을 멈추게 함

            SoundManager.instance.PlaySound("Die");                  // 죽는 사운드 출력

            Debug.Log("죽었습니다!");                                // Consol창에 죽었다는 사실을 알려줌

            is_show = false;                                         // 설명을 숨김
            is_show_text = false;                                    // 스테이지 텍스트를 숨김
            is_show_bnt = false;                                     // 설명 버튼을 숨김

            restart_bnt.SetActive(true);                             // 재시작 버튼 활성화
        }
    }

    public void Restart() // 게임을 재시작하는 함수
    {
        Time.timeScale = 1;                         // 시간이 흘러가게 함

        SceneManager.LoadScene(0);                  // 게임 Scene을 불러옴

        SoundManager.instance.PlaySound("Button");  // 버튼을 누르는 사운드 출력
    }

    public void Option() // 옵션 UI에 대한 함수
    {
        is_option = !is_option;
        is_live = !is_live;

        if (is_show)                                // esc 창을 켤 때 설명을 보여주고 있었다면 
            is_show = !is_show;                     // esc 창을 닫았을 때 설명 창을 닫음

        option.gameObject.SetActive(is_option);     // 그 다음 옵션이 true 상태로 바뀌었다면 비활성화 되어 있던 ESC 창 UI를 활성화 시킴
        Time.timeScale = is_option == true ? 0 : 1; // esc 창이 열려 있다면(is_option == true) 시간을 멈추고 아니라면(is_option == false) 시간을 흐르게 함

        SoundManager.instance.PlaySound("Button");  // 버튼을 누르는 사운드 출력
    }

    public void ClickShowText()
    {
        is_show_text = !is_show_text;

        // is_show_text의 값에 따라 버튼 텍스트를 다르게 함 (숨기기 / 보이기)
        if (is_show_text)
            stage_bnt_text.text = "숨기기";
        else
            stage_bnt_text.text = "보이기";

        SoundManager.instance.PlaySound("Button"); // 버튼을 누르는 사운드 출력
    }

    public void ClickShowBnt()
    {
        is_show_bnt = !is_show_bnt;

        // is_show_bnt의 값에 따라 버튼 텍스트를 다르게 함 (보이기 / 숨기기)
        if (is_show_bnt)
            explan_bnt_text.text = "숨기기";

        else
            explan_bnt_text.text = "보이기";

        SoundManager.instance.PlaySound("Button"); // 버튼을 누르는 사운드 출력
    }

    public void ChangeBgm(int bgm_button) // 배경음 변경 버튼을 눌렀을 때  /  배열 bgm_button 중 누른 버튼의 인덱스 값을 int형으로 받아서 매개변수로 사용함 
    {
        SoundManager.instance.PlaySound("Button");                    // 버튼을 누르는 사운드를 출력하고

        bgm_index = bgm_button;                                       // 저장하기 쉽게 하기 위해 받아옴
        bgm_player.clip = SoundManager.instance.bgm_clips[bgm_index]; // 배경음 플레이어에 방금 받은 bgm_index 값에 해당하는 배경음으로 설정 후
        bgm_player.Play();                                            // 배경음을 플레이함
    }

    void OnCollisionEnter2D(Collision2D collision) // Collision 함수 (Enter[충돌한 순간], Exit[충돌이 해제된 순간], Stay[충돌 중인 순간 매 프래임 실행]) -> 말 그대로 "충돌"하면 실행  /  Trigger 함수 (Enter[충돌한 순간], Exit[충돌이 해제된 순간], Stay[충돌 중인 순간 매 프래임 실행]) -> Collision과 달리 한 물체에 다른 물체가 들어오면 실행
    {
        if (collision.gameObject.tag == "Player") // GameManager 오브젝트와 닿은 물체의 tag가 "Player"(플레이어)이면 실행  /  GameManager를 아래로 내렸기 때문에 GameManager에 플레이어가 닿았다는 것은 플레이어가 떨어졌다는 것을 의미함
        {
                           
            if (health > 1)                                                                 // 플레이어의 체력이 1보다 많으면 (떨어져서 죽는 체력이 아니라면)
            {
                PlayerReposition();                                                         // 플레이어를 원점으로 되돌림
                SoundManager.instance.PlaySound("Fall");                                    // 떨어지는 사운드 출력
            }

            else                                                                            // 죽었다면
                SoundManager.instance.PlaySound("Die");                                     // 죽는 사운드 출력
            
            HealthDown();                                                                   // 체력을 감소 시킴 
        }

    }

    void PlayerReposition() // 플레이어의 위치를 원점으로 되돌리는 함수
    {
        player.VelocityZero();                                     // 낙하 속도를 0으로 만들고
        player.transform.position = new Vector3(-7.5f, -4.5f, -1); // 플레이어의 위치를 원점으로 되돌림
    }

    void LoadData() // 저장한 데이터 불러오기
    {
        stages[stage_index].SetActive(true);                          // 저장된 스테이지를 불러옴

        bgm_player.clip = SoundManager.instance.bgm_clips[bgm_index]; // 배경음을 불러와서
        bgm_player.Play();                                            // 배경음을 플레이함
    }

    public void Exit() // 게임을 종료하는 함수
    {
        data_manager.JsonSave();                 // 현재 데이터를 저장한뒤

        SoundManager.instance.PlaySound("Exit"); // 게임을 종료하는 사운드를 출력하고
         
        Application.Quit();                      // 게임을 종료함
    }
}

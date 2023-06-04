using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // instance 변수를 활용해서 SoundManager에 쉽게 접근 할 수 있게 함

    public AudioClip[] audio_clips; // 여러 소리들이 있는 배열 변수
    public AudioClip[] bgm_clips;   // 여러 Bgm들이 있는 배열 변수

    // SoundManager에 있는 2개의 player 오브젝트 객체를 불러옴
    AudioSource bgm_player;
    AudioSource sfx_player;

    // 옵션에서 설정할 사운드 변수들
    public Slider bgm_slider; // 배경음 슬라이더 변수
    public Slider sfx_slider; // 효과음 슬라이더 변수

    void Awake() // 게임을 시작할 때 한번
    {
        instance = this;

        bgm_player = GameObject.Find("Bgm Player").GetComponent<AudioSource>(); // Bgm Player의 Audio Souce 컴퍼넌트를 가져옴
        sfx_player = GameObject.Find("Sfx Player").GetComponent<AudioSource>(); // Sfx Player의 Audio Souce 컴퍼넌트를 가져옴

        bgm_slider = bgm_slider.GetComponent<Slider>();                         // bgm_slider의 컴퍼넌트를 가져옴
        sfx_slider = sfx_slider.GetComponent<Slider>();                         // sfx_slider의 컴퍼넌트를 가져옴

        // onValueChanged() -> 값이 변경 되었을 때 실행할 이벤트를 지정할 수 있게 함
        bgm_slider.onValueChanged.AddListener(ChangeBgmSound);
        sfx_slider.onValueChanged.AddListener(ChangeSfxSound);
    }

    public void PlaySound(string type) // 소리를 내는 함수  /  위에서 선언한 배열을 사용함
    {
        int index = 0;

        // 각 상황에 맞는 사운드 출력
        switch (type)
        {
            case "Attack": index = 0; break;
            case "Bounce": index = 1; break;
            case "Button": index = 2; break;
            case "Clear": index = 3; break;
            case "Damaged": index = 4; break;
            case "Die": index = 5; break;
            case "Exit": index = 6; break;
            case "Fall": index = 7; break;
            case "Jump": index = 8; break;
        }

        sfx_player.clip = audio_clips[index];
        sfx_player.Play();
    }

    public void ChangeBgmSound(float value) // float형 값을 하나 받아온 뒤에
    {
        bgm_player.volume = value * 0.25f; // bgm_player.volume을 받아온 float형 매개변수로 저장함
    }

    void ChangeSfxSound(float value) // float형 값을 하나 받아온 뒤에
    {
        sfx_player.volume = value ;       // sfx_player.volume을 받아온 float형 매개변수로 저장함
    }
}

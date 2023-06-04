using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance; // instance ������ Ȱ���ؼ� SoundManager�� ���� ���� �� �� �ְ� ��

    public AudioClip[] audio_clips; // ���� �Ҹ����� �ִ� �迭 ����
    public AudioClip[] bgm_clips;   // ���� Bgm���� �ִ� �迭 ����

    // SoundManager�� �ִ� 2���� player ������Ʈ ��ü�� �ҷ���
    AudioSource bgm_player;
    AudioSource sfx_player;

    // �ɼǿ��� ������ ���� ������
    public Slider bgm_slider; // ����� �����̴� ����
    public Slider sfx_slider; // ȿ���� �����̴� ����

    void Awake() // ������ ������ �� �ѹ�
    {
        instance = this;

        bgm_player = GameObject.Find("Bgm Player").GetComponent<AudioSource>(); // Bgm Player�� Audio Souce ���۳�Ʈ�� ������
        sfx_player = GameObject.Find("Sfx Player").GetComponent<AudioSource>(); // Sfx Player�� Audio Souce ���۳�Ʈ�� ������

        bgm_slider = bgm_slider.GetComponent<Slider>();                         // bgm_slider�� ���۳�Ʈ�� ������
        sfx_slider = sfx_slider.GetComponent<Slider>();                         // sfx_slider�� ���۳�Ʈ�� ������

        // onValueChanged() -> ���� ���� �Ǿ��� �� ������ �̺�Ʈ�� ������ �� �ְ� ��
        bgm_slider.onValueChanged.AddListener(ChangeBgmSound);
        sfx_slider.onValueChanged.AddListener(ChangeSfxSound);
    }

    public void PlaySound(string type) // �Ҹ��� ���� �Լ�  /  ������ ������ �迭�� �����
    {
        int index = 0;

        // �� ��Ȳ�� �´� ���� ���
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

    public void ChangeBgmSound(float value) // float�� ���� �ϳ� �޾ƿ� �ڿ�
    {
        bgm_player.volume = value * 0.25f; // bgm_player.volume�� �޾ƿ� float�� �Ű������� ������
    }

    void ChangeSfxSound(float value) // float�� ���� �ϳ� �޾ƿ� �ڿ�
    {
        sfx_player.volume = value ;       // sfx_player.volume�� �޾ƿ� float�� �Ű������� ������
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance; // GameManager ��ũ��Ʈ�� ���� ������Ʈ ����

    // ���� ���� ������
    public GameObject explan; // ������ �����ִ� �̹������� �����ϴ� ������Ʈ ����
    public Button explan_bnt; // ������ �������� ���� ������ �� �ְ� �ϴ� ��ư ����
    public Text bnt_text;     // �� ��ư�� �ؽ�Ʈ ����
    public bool is_show;      // ������ �����ִ��� ����

    public Text tip_text;     // ������ �ؽ�Ʈ �� tip �ؽ�Ʈ

    // �������� ��ȯ�� ���� ������
    public GameObject[] stages; // ������������ ������ �迭 ����
    public int stage_index;     // ���������� ��ȣ ����
    public Text stage_text;     // �������� �ؽ�Ʈ ����

    public Camera main_camera; // ī�޶� ����

    // �÷��̾ ���� ������
    public int health;         // �÷��̾��� ü��

    // PlayerMove�� ���� ����
    public PlayerMove player; // PlayerMove ��ũ��Ʈ�� ���� player�� ������

    // esc â(�ɼ� â)�� ���� ����
    public Image option; // �ɼ� â�� ������Ʈ ����
    bool is_option;      // esc â�� �߰� �ִ��� ����
    bool is_live;        // ���� �ð��� �帣�� �ִ��� ����

    // esc â(�ɼ� â)���� ���� �� ����
    public GameObject stage_text_obj; // �������� �ؽ�Ʈ�� ������ ������Ʈ
    public GameObject explan_bnt_obj; // ���� ��ư�� ������ ������Ʈ

    public Text stage_bnt_text;       // �������� �ؽ�Ʈ ��ư�� ���� �� �ٲ� �ؽ�Ʈ (���̱� / �����)
    public Text explan_bnt_text;      // ���� ��ư�� ���� �� �ٲ� �ؽ�Ʈ (���̱� / �����)

    public bool is_show_text;         // �������� �ؽ�Ʈ�� �������� ����
    public bool is_show_bnt;          // ���� ��ư�� �������� ����

    // ����ۿ� ���� ����
    public GameObject restart_bnt;
    public Text restart_text;

    // bgm�� ���� ����
    AudioSource bgm_player;
    public int bgm_index;
    public Button[] bgm_button;

    // DataManager�� ���� ����
    DataManager data_manager;    
    public GameObject data_manager_obj;

    void Awake() // ������ ������ �� �ѹ�
    {
        instance = this;

        is_show = false;     // ������ ����
        is_show_text = true; // �������� �ؽ�Ʈ�� ������
        is_show_bnt = true;  // ���� ��ư�� ������

        is_live = true;

        instance.GetComponent<BoxCollider>();

        data_manager = data_manager_obj.GetComponent<DataManager>();

        bgm_player = GameObject.Find("Bgm Player").GetComponent<AudioSource>();
    }

    void Start() // Awake() �Լ� ������
    {
        Invoke("LoadData", 0.1f);
    }

    void Update() // �� �����Ӹ���
    {
        // ������ bool ������ ���� ���� �����ְų� ����
        explan.SetActive(is_show);
        stage_text_obj.SetActive(is_show_text);
        explan_bnt_obj.SetActive(is_show_bnt);
        
        // ���������� ���� �ؽ�Ʈ ��ȯ
        stage_text.text = string.Format("{0} ��������", (stage_index + 1)); // �������� �ؽ�Ʈ�� { (stage_index�� �� + 1) } �������� �� �ٲ�

        // ���� ü�¿� ���� �ؽ�Ʈ ��ȯ
        if (health < 2) tip_text.text = string.Format("���� ������ �� {0}��! ū���̾�! ������!!!\r\n������ / ������ �濡���� ���� �Ұ�����!\r\n������ �� �ɶ����� ���ڸ� ����!\r\n���� �̵��� �� �ɿ��� ���ݸ� ��ٷ�����!", health);
        else if (health < 4) tip_text.text = string.Format("���� ������ �� {0}��! ����! ����!\r\n������ / ������ �濡���� ���� �Ұ�����!\r\n������ �� �ɶ����� ���ڸ� ����!\r\n���� �̵��� �� �ɿ��� ���ݸ� ��ٷ�����!", health);       
        else if (health < 6) tip_text.text = string.Format("���� ������ �� {0}��! ���� �ؾ߰ڴµ�?\r\n������ / ������ �濡���� ���� �Ұ�����!\r\n������ �� �ɶ����� ���ڸ� ����!\r\n���� �̵��� �� �ɿ��� ���ݸ� ��ٷ�����!", health);
        else tip_text.text = string.Format("���� ������ �� {0}��!\r\n������ / ������ �濡���� ���� �Ұ�����!\r\n������ �� �ɶ����� ���ڸ� ����!\r\n���� �̵��� �� �ɿ��� ���ݸ� ��ٷ�����!", health);

        // �ɼ� UI ���� �ڵ�
        if (Input.GetButtonDown("Cancel")) // ESC ��ư�� ������ �� (ESC�� ������ true�� ��ȯ)
        {
            Option(); // �ɼ� â�� �Ѱų� ��
        }
    }

    public void ClickExplanBnt() // ���� ����� / ���̱� ��ư�� ������ ��
    {
        is_show = !is_show;           // ������ �����ִ��� ���θ� �ݴ�� ����

        if (is_show)                  // is_show = true
            bnt_text.text = "�����"; // ��ư�� �ؽ�Ʈ�� "�����"�� �ٲ�

        else                          // is_show = false
            bnt_text.text = "���̱�"; // ��ư�� �ؽ�Ʈ�� "���̱�"�� �ٲ�

        SoundManager.instance.PlaySound("Button");
    }

    public void NextStage() // ���������� ��ȯ��Ű�� �Լ�
    { 
        SoundManager.instance.PlaySound("Clear");                 // �������� Ŭ���� ���� ���

        if (stage_index < stages.Length-1)                        // ���� �������� ��ȣ�� ���� �������� ���� ��
        {
            stages[stage_index].SetActive(false);                 // ���� ���������� ��Ȱ��ȭ ��Ű��

            stage_index++;                                        // ���������� ��ȣ�� 1 ���ѵ�

            stages[stage_index].SetActive(true);                  // �� ��ȣ�� �´� ���������� Ȱ��ȭ ��Ű��

            PlayerReposition();                                   // �÷��̾ �������� �ǵ���
        }

        else                                                      // ���� �������� ��ȣ�� ���� �������� ũ�ų� ���� �� (���ӿ� �ִ� ��� ���������� Ŭ���� ���� ��)
        {
            // ���� ���� �ڵ�
            Time.timeScale = 0;                                   // �ð��� ����ΰ�

            Debug.Log("���� Ŭ����!");                            // ������ Ŭ���� �ߴٴ� ����� �˷��ְ�

            restart_text.text = "Ŭ����!";                        // ����� ��ư�� �ؽ�Ʈ�� "Ŭ����"�� �ٲ� ��

            restart_bnt.SetActive(true);                          // ����� ��ư Ȱ��ȭ

            // ����� ���� �ڵ�
            bgm_player.clip = SoundManager.instance.bgm_clips[3]; // ������� Ŭ���� bgm���� �ٲٰ�
            bgm_player.Play();                                    // ����� �÷���
        }

        // �������� ��ȯ�� ���� ī�޶� ��ȯ �ڵ�
        if (stage_index != 3) // ���� ���������� Ư�� ��������(3 ��������)�� �ƴ� ��
        {
            main_camera.transform.position = new Vector3(-7.5f, -4.5f, -10);  // ������ ������� ��������

            instance.transform.position = new Vector3(0, -6.5f, 0);           // GamaManager�� ���ڸ��� ���� ���� (�������� �� �������� ������ ����)
        }

        else // ���� ���������� Ư�� ��������(3 ��������)�� ��
        {
            health = 1;                                                       // �÷��̾��� ü���� 1�� ���� 1���� �������� �޾Ƶ� �װ� ���� 

            main_camera.transform.position = new Vector3(-7.5f, -9.5f, -10);  // ��Ȱ�� ������ ���� ������ ������

            instance.transform.position = new Vector3(0, 10000, -1);          // GamaManager�� �ܵ� ���� ���� (�������� �� �������� ������ �ʱ� ����)

        }
    }

    public void HealthDown() // �÷��̾��� ü���� ���ҽ�Ű�� �Լ�
    {
        if (health > 1)                                              // �÷��̾��� ü���� 1���� ���ٸ�
        { 
            health--;                                                // ü���� ���� ��Ŵ

            SoundManager.instance.PlaySound("Damaged");              // �������� �޴� ���� ���
        }

        else                                                         // �÷��̾��� ü���� 1���� ���ų� ���ٸ�
        {
            instance.transform.position = new Vector3(0, 10000, -1); // GamaManager�� �ܵ� ���� ���� (�������� �׾��� ���� ���� ����)

            Time.timeScale = 1;                                      // �ð��� ���߰� ��

            SoundManager.instance.PlaySound("Die");                  // �״� ���� ���

            Debug.Log("�׾����ϴ�!");                                // Consolâ�� �׾��ٴ� ����� �˷���

            is_show = false;                                         // ������ ����
            is_show_text = false;                                    // �������� �ؽ�Ʈ�� ����
            is_show_bnt = false;                                     // ���� ��ư�� ����

            restart_bnt.SetActive(true);                             // ����� ��ư Ȱ��ȭ
        }
    }

    public void Restart() // ������ ������ϴ� �Լ�
    {
        Time.timeScale = 1;                         // �ð��� �귯���� ��

        SceneManager.LoadScene(0);                  // ���� Scene�� �ҷ���

        SoundManager.instance.PlaySound("Button");  // ��ư�� ������ ���� ���
    }

    public void Option() // �ɼ� UI�� ���� �Լ�
    {
        is_option = !is_option;
        is_live = !is_live;

        if (is_show)                                // esc â�� �� �� ������ �����ְ� �־��ٸ� 
            is_show = !is_show;                     // esc â�� �ݾ��� �� ���� â�� ����

        option.gameObject.SetActive(is_option);     // �� ���� �ɼ��� true ���·� �ٲ���ٸ� ��Ȱ��ȭ �Ǿ� �ִ� ESC â UI�� Ȱ��ȭ ��Ŵ
        Time.timeScale = is_option == true ? 0 : 1; // esc â�� ���� �ִٸ�(is_option == true) �ð��� ���߰� �ƴ϶��(is_option == false) �ð��� �帣�� ��

        SoundManager.instance.PlaySound("Button");  // ��ư�� ������ ���� ���
    }

    public void ClickShowText()
    {
        is_show_text = !is_show_text;

        // is_show_text�� ���� ���� ��ư �ؽ�Ʈ�� �ٸ��� �� (����� / ���̱�)
        if (is_show_text)
            stage_bnt_text.text = "�����";
        else
            stage_bnt_text.text = "���̱�";

        SoundManager.instance.PlaySound("Button"); // ��ư�� ������ ���� ���
    }

    public void ClickShowBnt()
    {
        is_show_bnt = !is_show_bnt;

        // is_show_bnt�� ���� ���� ��ư �ؽ�Ʈ�� �ٸ��� �� (���̱� / �����)
        if (is_show_bnt)
            explan_bnt_text.text = "�����";

        else
            explan_bnt_text.text = "���̱�";

        SoundManager.instance.PlaySound("Button"); // ��ư�� ������ ���� ���
    }

    public void ChangeBgm(int bgm_button) // ����� ���� ��ư�� ������ ��  /  �迭 bgm_button �� ���� ��ư�� �ε��� ���� int������ �޾Ƽ� �Ű������� ����� 
    {
        SoundManager.instance.PlaySound("Button");                    // ��ư�� ������ ���带 ����ϰ�

        bgm_index = bgm_button;                                       // �����ϱ� ���� �ϱ� ���� �޾ƿ�
        bgm_player.clip = SoundManager.instance.bgm_clips[bgm_index]; // ����� �÷��̾ ��� ���� bgm_index ���� �ش��ϴ� ��������� ���� ��
        bgm_player.Play();                                            // ������� �÷�����
    }

    void OnCollisionEnter2D(Collision2D collision) // Collision �Լ� (Enter[�浹�� ����], Exit[�浹�� ������ ����], Stay[�浹 ���� ���� �� ������ ����]) -> �� �״�� "�浹"�ϸ� ����  /  Trigger �Լ� (Enter[�浹�� ����], Exit[�浹�� ������ ����], Stay[�浹 ���� ���� �� ������ ����]) -> Collision�� �޸� �� ��ü�� �ٸ� ��ü�� ������ ����
    {
        if (collision.gameObject.tag == "Player") // GameManager ������Ʈ�� ���� ��ü�� tag�� "Player"(�÷��̾�)�̸� ����  /  GameManager�� �Ʒ��� ���ȱ� ������ GameManager�� �÷��̾ ��Ҵٴ� ���� �÷��̾ �������ٴ� ���� �ǹ���
        {
                           
            if (health > 1)                                                                 // �÷��̾��� ü���� 1���� ������ (�������� �״� ü���� �ƴ϶��)
            {
                PlayerReposition();                                                         // �÷��̾ �������� �ǵ���
                SoundManager.instance.PlaySound("Fall");                                    // �������� ���� ���
            }

            else                                                                            // �׾��ٸ�
                SoundManager.instance.PlaySound("Die");                                     // �״� ���� ���
            
            HealthDown();                                                                   // ü���� ���� ��Ŵ 
        }

    }

    void PlayerReposition() // �÷��̾��� ��ġ�� �������� �ǵ����� �Լ�
    {
        player.VelocityZero();                                     // ���� �ӵ��� 0���� �����
        player.transform.position = new Vector3(-7.5f, -4.5f, -1); // �÷��̾��� ��ġ�� �������� �ǵ���
    }

    void LoadData() // ������ ������ �ҷ�����
    {
        stages[stage_index].SetActive(true);                          // ����� ���������� �ҷ���

        bgm_player.clip = SoundManager.instance.bgm_clips[bgm_index]; // ������� �ҷ��ͼ�
        bgm_player.Play();                                            // ������� �÷�����
    }

    public void Exit() // ������ �����ϴ� �Լ�
    {
        data_manager.JsonSave();                 // ���� �����͸� �����ѵ�

        SoundManager.instance.PlaySound("Exit"); // ������ �����ϴ� ���带 ����ϰ�
         
        Application.Quit();                      // ������ ������
    }
}

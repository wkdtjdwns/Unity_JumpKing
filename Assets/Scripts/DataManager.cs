using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable] // [Serializable]�� Ŭ���� �Ǵ� ����ü�� ����ȭ �� �� ������ ��Ÿ��
public class SaveData 
{
    // int�� ����
    public int stage_index; // ���������� ��ȣ ����
    public int health;      // �÷��̾��� ü��
    public int bgm_index;   // ������� ��ȣ

    // bool�� ����
    public bool is_show_text; // �������� �ؽ�Ʈ�� �������� ����
    public bool is_show_bnt;  // ���� ��ư�� �������� ����

    // SoundManager ����
    public float bgm_vol; // ����� ����
    public float sfx_vol; // ȿ���� ����
}

public class DataManager : MonoBehaviour
{
    string path;

    void Start()
    {
        path = Path.Combine(Application.dataPath, "database.json");
        JsonLoad();
    }

    // GameManager�� ����Ǿ� �ִ� �����͸� ���ο� ��ü(save_data)�� ����
    public void JsonSave()
    {
        SaveData save_data = new SaveData();

        save_data.stage_index = GameManager.instance.stage_index;   // save_data�� �������� ��ȣ�� GameManager�� ����
        save_data.health = GameManager.instance.health;             // save_data�� ü���� GameManager�� ����
        save_data.bgm_index = GameManager.instance.bgm_index;       // save_data�� ������� ��ȣ�� GameManager�� ����

        save_data.is_show_text = GameManager.instance.is_show_text; // save_data�� �������� �ؽ�Ʈ�� �������� ���θ� GameManager�� ����
        save_data.is_show_bnt = GameManager.instance.is_show_bnt;   // save_data�� ���� ��ư�� �������� ���θ� GameManager�� ����

        save_data.bgm_vol = SoundManager.instance.bgm_slider.value; // save_data�� ����� ������ GameManager�� ����
        save_data.sfx_vol = SoundManager.instance.sfx_slider.value; // save_data�� ȿ���� ������ GameManager�� ����

        string json = JsonUtility.ToJson(save_data, true);

        File.WriteAllText(path, json);
    }

    // ���ο� ��ü(save_data)�� �����ߴ� �����͸� GameManager�� ����
    public void JsonLoad() // �����͸� �ҷ����� �Լ�
    {
        SaveData save_data = new SaveData();

        if (!File.Exists(path)) // ���Ͽ� ����� ������ ���ٸ� (������ ó�� �����ߴٸ�) -> �ҷ��� �����Ͱ� ������ �ʱ�ȭ ���ְ� �ҷ���
        {
            // ������ �������� ���� ������ ���� �ʱ� ������ �ʱ�
            GameManager.instance.stage_index = 0; 
            GameManager.instance.health = 10;        
            GameManager.instance.bgm_index = 0;

            GameManager.instance.is_show = false;
            GameManager.instance.is_show_text = true;
            GameManager.instance.is_show_bnt = true;

            SoundManager.instance.bgm_slider.value = 0.5f;
            SoundManager.instance.sfx_slider.value = 0.5f;

            JsonSave(); // �ʱ�ȭ�� �������� ���� GameManager�� ������
        }

        else // �װ� �ƴ϶�� (���Ͽ� ����� ������ �ִٸ�)
        {
            string load_json = File.ReadAllText(path);
            save_data = JsonUtility.FromJson<SaveData>(load_json);

            if (save_data != null) // save_data�� ���� null�� �ƴϸ� (save_data�� ���� �ִٸ�)
            {
                GameManager.instance.stage_index = save_data.stage_index;   // ���� �������� ��ȣ�� save_data�� �������� ��ȣ�� ������
                GameManager.instance.health = save_data.health;             // ���� ü���� save_data�� ü������ ������
                GameManager.instance.bgm_index = save_data.bgm_index;       // ������� ��ȣ�� save_data�� ������� ��ȣ�� ������

                GameManager.instance.is_show_text = save_data.is_show_text; // �������� �ؽ�Ʈ�� �������� ���θ� save_data�� �������� �ؽ�Ʈ�� �������� ���η� ������
                GameManager.instance.is_show_bnt = save_data.is_show_bnt;   // ���� ��ư�� �������� ���θ� save_data�� ���� ��ư�� �������� ���η� ������

                SoundManager.instance.bgm_slider.value = save_data.bgm_vol; // ���� ����� ������ save_data�� ����� �������� ������
                SoundManager.instance.sfx_slider.value = save_data.sfx_vol; // ���� ȿ���� ������ save_data�� ȿ���� �������� ������
            }
        }
    }


}
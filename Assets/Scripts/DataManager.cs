using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[System.Serializable] // [Serializable]는 클래스 또는 구조체를 직렬화 할 수 있음을 나타냄
public class SaveData 
{
    // int형 변수
    public int stage_index; // 스테이지의 번호 변수
    public int health;      // 플레이어의 체력
    public int bgm_index;   // 배경음의 번호

    // bool형 변수
    public bool is_show_text; // 스테이지 텍스트를 보여줄지 여부
    public bool is_show_bnt;  // 설명 버튼을 보여줄지 여부

    // SoundManager 변수
    public float bgm_vol; // 배경음 볼륨
    public float sfx_vol; // 효과음 볼륨
}

public class DataManager : MonoBehaviour
{
    string path;

    void Start()
    {
        path = Path.Combine(Application.dataPath, "database.json");
        JsonLoad();
    }

    // GameManager에 저장되어 있는 데이터를 새로운 객체(save_data)에 저장
    public void JsonSave()
    {
        SaveData save_data = new SaveData();

        save_data.stage_index = GameManager.instance.stage_index;   // save_data의 스테이지 번호를 GameManager에 저장
        save_data.health = GameManager.instance.health;             // save_data의 체력을 GameManager에 저장
        save_data.bgm_index = GameManager.instance.bgm_index;       // save_data의 배경음의 번호를 GameManager에 저장

        save_data.is_show_text = GameManager.instance.is_show_text; // save_data의 스테이지 텍스트를 보여줄지 여부를 GameManager에 저장
        save_data.is_show_bnt = GameManager.instance.is_show_bnt;   // save_data의 설명 버튼을 보여줄지 여부를 GameManager에 저장

        save_data.bgm_vol = SoundManager.instance.bgm_slider.value; // save_data의 배경음 볼륨을 GameManager에 저장
        save_data.sfx_vol = SoundManager.instance.sfx_slider.value; // save_data의 효과음 볼륨을 GameManager에 저장

        string json = JsonUtility.ToJson(save_data, true);

        File.WriteAllText(path, json);
    }

    // 새로운 객체(save_data)에 저장했던 데이터를 GameManager에 저장
    public void JsonLoad() // 데이터를 불러오는 함수
    {
        SaveData save_data = new SaveData();

        if (!File.Exists(path)) // 파일에 저장된 정보가 없다면 (게임을 처음 시작했다면) -> 불러올 데이터가 없으니 초기화 해주고 불러옴
        {
            // 저장할 변수들의 값을 시작할 때의 초기 값으로 초기
            GameManager.instance.stage_index = 0; 
            GameManager.instance.health = 10;        
            GameManager.instance.bgm_index = 0;

            GameManager.instance.is_show = false;
            GameManager.instance.is_show_text = true;
            GameManager.instance.is_show_bnt = true;

            SoundManager.instance.bgm_slider.value = 0.5f;
            SoundManager.instance.sfx_slider.value = 0.5f;

            JsonSave(); // 초기화된 변수들의 값을 GameManager에 저장함
        }

        else // 그게 아니라면 (파일에 저장된 정보가 있다면)
        {
            string load_json = File.ReadAllText(path);
            save_data = JsonUtility.FromJson<SaveData>(load_json);

            if (save_data != null) // save_data의 값이 null이 아니면 (save_data의 값이 있다면)
            {
                GameManager.instance.stage_index = save_data.stage_index;   // 현재 스테이지 번호를 save_data의 스테이지 번호로 저장함
                GameManager.instance.health = save_data.health;             // 현재 체력을 save_data의 체력으로 저장함
                GameManager.instance.bgm_index = save_data.bgm_index;       // 배경음의 번호를 save_data의 배경음의 번호로 저장함

                GameManager.instance.is_show_text = save_data.is_show_text; // 스테이지 텍스트를 보여줄지 여부를 save_data의 스테이지 텍스트를 보여줄지 여부로 저장함
                GameManager.instance.is_show_bnt = save_data.is_show_bnt;   // 설명 버튼을 보여줄지 여부를 save_data의 설명 버튼을 보여줄지 여부로 저장함

                SoundManager.instance.bgm_slider.value = save_data.bgm_vol; // 현재 배경음 볼륨을 save_data의 배경음 볼륨으로 저장함
                SoundManager.instance.sfx_slider.value = save_data.sfx_vol; // 현재 효과음 볼륨을 save_data의 효과음 볼륨으로 저장함
            }
        }
    }


}
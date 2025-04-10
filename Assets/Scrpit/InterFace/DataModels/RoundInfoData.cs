using System;
using System.Collections.Generic;

[Serializable]
public class RoundInfoData
{
    public int currentLevel = 1;
    public List<LevelList> levelSceneList = new List<LevelList>
    {
        new LevelList
        {
            id = 1,
            name = "神秘森林",
            scenePath = "level-001",
            score = 0,
        },
        new LevelList
        {
            id = 2,
            name = "古代遗迹",
            scenePath = "level-002",
            score = 0,
        },
        new LevelList
        {
            id = 3,
            name = "未来城市",
            scenePath = "level-003",
            score = 0,
        },
        new LevelList
        {
            id = 4,
            name = "梦幻海洋",
            scenePath = "level-004",
            score = 0,
        },
        new LevelList
        {
            id = 5,
            name = "冰雪王国",
            scenePath = "level-005",
            score = 0,
        },
        new LevelList
        {
            id = 6,
            name = "火焰山脉",
            scenePath = "level-006",
            score = 0,
        },
        new LevelList
        {
            id = 7,
            name = "幽暗洞穴",
            scenePath = "level-007",
            score = 0,
        },
        new LevelList
        {
            id = 8,
            name = "天空之城",
            scenePath = "level-008",
            score = 0,
        },
        new LevelList
        {
            id = 9,
            name = "机械工厂",
            scenePath = "level-009",
            score = 0,
        },
        new LevelList
        {
            id = 10,
            name = "迷雾平原",
            scenePath = "level-010",
            score = 0,
        },
    };

    [NonSerialized] // 不需要保存的临时数据
    public string lastCheckpoint;
}

public class LevelList
{
    public int id;
    public string name;
    public string scenePath;
    public int score;
}

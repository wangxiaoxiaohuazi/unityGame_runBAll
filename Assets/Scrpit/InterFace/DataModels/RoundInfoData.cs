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
            name = "level-001",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 2,
            name = "level-002",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 3,
            name = "level-003",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 4,
            name = "level-005",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 5,
            name = "level-005",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 6,
            name = "level-006",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 7,
            name = "level-007",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 8,
            name = "level-008",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 9,
            name = "level-009",
            scenePath = "Level1",
            score = 0
        },
         new LevelList
        {
            id = 10,
            name = "level-010",
            scenePath = "Level1",
            score = 0
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
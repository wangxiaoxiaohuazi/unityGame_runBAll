using System;

[Serializable]
public class PublicGameData
{
    public PlayerData player = new PlayerData();
    public RoundInfoData roundInfo = new RoundInfoData();
    public CollectionData collections = new CollectionData();

    // 版本控制
    public int dataVersion = 1;
    public DateTime createTime = DateTime.UtcNow;
}
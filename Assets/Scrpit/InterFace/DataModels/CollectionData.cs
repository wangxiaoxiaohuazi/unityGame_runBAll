using System;
using System.Collections.Generic;

[Serializable]
public class CollectionData
{
    public List<SkinItem> skins = new List<SkinItem>();
    public List<BodyPart> bodyParts = new List<BodyPart>();
}

[Serializable]
public class SkinItem
{
    public int id;
    public string displayName;
    public bool isLocked = true;
    public DateTime? unlockTime;
    public DateTime? expireTime;

    public bool isValid =>
        !expireTime.HasValue ||
        DateTime.UtcNow < expireTime.Value;
}

[Serializable]
public class BodyPart
{
    public int id;
    public string prefabPath;
    public bool isEquipped;
}


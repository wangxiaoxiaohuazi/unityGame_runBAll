using System;
using System.Collections.Generic;

[Serializable]
public class CollectionData
{
	public List<SkinItem> skins = new List<SkinItem>{
	new SkinItem { sortNum = 1, id = 1, name = "b-skin11", isLocked = true, unlockTime = null, unlockCondition = 0, unlockValue = 0 },
	new SkinItem { sortNum = 2, id = 301, name = "Y-ball-02", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 3, id = 101, name = "b-skin10", isLocked = false, unlockTime = null, unlockCondition = 2, unlockValue = 2 },
	new SkinItem { sortNum = 4, id = 102, name = "b-skin7", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 5, id = 201, name = "Flag-004", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	// new SkinItem { sortNum = 6, id = 401, name = "X-Ball_8", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 7, id = 302, name = "Y-ball-08", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 8, id = 103, name = "b-skin13", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 9, id = 104, name = "b-skin16", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 10, id = 105, name = "b-skin17", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 11, id = 106, name = "b-skin22", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 12, id = 107, name = "b-skin23", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 13, id = 108, name = "b-skin24", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 14, id = 109, name = "b-skin25", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 15, id = 110, name = "b-skin26", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 16, id = 111, name = "b-skin27", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 17, id = 112, name = "b-skin28", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	// new SkinItem { sortNum = 19, id = 113, name = "b-skin29", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 20, id = 403, name = "X-Ball_6", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 21, id = 202, name = "Flag-001", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 22, id = 203, name = "Flag-002", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 23, id = 204, name = "Flag-003", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 24, id = 205, name = "Flag-005", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 25, id = 206, name = "Flag-006", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 26, id = 207, name = "Flag-007", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 27, id = 208, name = "Flag-008", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 28, id = 209, name = "Flag-009", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 29, id = 210, name = "Flag-010", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 30, id = 211, name = "Flag-011", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 31, id = 212, name = "Flag-012", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 32, id = 213, name = "Flag-013", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 33, id = 214, name = "Flag-014", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 34, id = 215, name = "Flag-015", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 35, id = 303, name = "Y-ball-05", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 36, id = 304, name = "Y-ball-06", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 37, id = 305, name = "Y-ball-07", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 38, id = 306, name = "Y-ball-13", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 39, id = 307, name = "Y-ball-14", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 40, id = 308, name = "Y-ball-15", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 41, id = 309, name = "Y-ball-16", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 42, id = 310, name = "Y-ball-17", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 43, id = 311, name = "Y-ball-18", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 44, id = 312, name = "Y-ball-19", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 45, id = 313, name = "Y-ball-20", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 46, id = 314, name = "Y-ball-21", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 47, id = 315, name = "Y-ball-22", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },

};
	public List<SkinItem> bodyParts = new List<SkinItem>{

	new SkinItem { sortNum = 1, id = 6, name = "GlowingOrb_6", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 2, id = 17, name = "GlowingOrb_17", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 3, id = 18, name = "GlowingOrb_18", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 4, id = 43, name = "GlowingOrb_43", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 10000 },
	new SkinItem { sortNum = 5, id = 25, name = "GlowingOrb_25", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 6, id = 28, name = "GlowingOrb_28", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 9, id = 38, name = "GlowingOrb_38", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 2000 },
	new SkinItem { sortNum = 11, id = 2, name = "GlowingOrb_2", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 13, id = 5, name = "GlowingOrb_5", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 14, id = 10, name = "GlowingOrb_10", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 15, id = 11, name = "GlowingOrb_11", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 16, id = 13, name = "GlowingOrb_13", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 18, id = 20, name = "GlowingOrb_20", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 20, id = 24, name = "GlowingOrb_24", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 25, id = 41, name = "GlowingOrb_41", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 26, id = 42, name = "GlowingOrb_42", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 27, id = 44, name = "GlowingOrb_44", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 28, id = 45, name = "GlowingOrb_45", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 3000 },
	new SkinItem { sortNum = 30, id = 1, name = "GlowingOrb_1", isLocked = false, unlockTime = null, unlockCondition = 0, unlockValue = 5000 },
	new SkinItem { sortNum = 31, id = 7, name = "GlowingOrb_7", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 32, id = 12, name = "GlowingOrb_12", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 33, id = 15, name = "GlowingOrb_15", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 34, id = 19, name = "GlowingOrb_19", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 35, id = 23, name = "GlowingOrb_23", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 37, id = 34, name = "GlowingOrb_34", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 38, id = 46, name = "GlowingOrb_46", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 5000 },
	new SkinItem { sortNum = 39, id = 4, name = "GlowingOrb_4", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 10000 },
	new SkinItem { sortNum = 40, id = 16, name = "GlowingOrb_16", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 10000 },
	new SkinItem { sortNum = 41, id = 22, name = "GlowingOrb_22", isLocked = false, unlockTime = null, unlockCondition = 1, unlockValue = 10000 },

};
}


[Serializable]
public class SkinItem
{
	public int id;
	public int sortNum;
	public string name;
	public bool isLocked = true;
	public int unlockCondition = 1;//0:无条件解锁，1:金币解锁，2:广告解锁 
	public int unlockValue = 0;//解锁条件值
	public DateTime? unlockTime;

	public bool isValid =>
		!unlockTime.HasValue ||
		DateTime.UtcNow < unlockTime.Value;
}

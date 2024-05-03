using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "List Levels")]
public class ListLevels : ScriptableObject
{
    [System.Serializable]
    public class LevelDetails
    {
        public int levelId;
        public bool isLock;
        public int pickedStars;
    }

    public List<LevelDetails> listLevelDetails = new List<LevelDetails>();
}

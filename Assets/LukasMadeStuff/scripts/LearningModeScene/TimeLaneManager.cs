using System.Collections.Generic;
using UnityEngine;

public class TimeLaneManager : MonoBehaviour
{
    public Transform scoringPlank;

    public GameObject gTargetPrefab;
    public GameObject emTargetPrefab;

    public float scrollSpeed = 5f;

    const float fixedY = 1.85f;
    const float fixedZ = -7.3f;

    Dictionary<string, GameObject> prefabMap;

    void Awake()
    {
        prefabMap = new Dictionary<string, GameObject>()
        {
            { "G", gTargetPrefab },
            { "Em", emTargetPrefab }
        };
    }

    public GameObject SpawnNote(NoteData note, float songTime)
    {
        GameObject prefab = prefabMap[note.chord];

        float timeUntilHit = note.time - songTime;

        float xOffset = timeUntilHit * scrollSpeed;

        Vector3 spawnPos = new Vector3(
            scoringPlank.position.x + xOffset,
            fixedY,
            fixedZ
        );

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        note.obj = obj;

        return obj;
    }
}
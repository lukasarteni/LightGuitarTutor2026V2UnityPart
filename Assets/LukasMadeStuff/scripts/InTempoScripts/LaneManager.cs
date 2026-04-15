using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public Transform scoringPlank;

    public GameObject aTargetPrefab;
    public GameObject bTargetPrefab;
    public GameObject cTargetPrefab;
    public GameObject dTargetPrefab;
    public GameObject eTargetPrefab;
    public GameObject fTargetPrefab;
    public GameObject gTargetPrefab;
    public GameObject amTargetPrefab;
    public GameObject dmTargetPrefab;
    public GameObject emTargetPrefab;
    public float scrollSpeed = 5f;

    const float fixedY = 1.85f;
    const float fixedZ = -7.3f;

    Dictionary<string, GameObject> prefabMap;

    void Awake()
    {
        prefabMap = new Dictionary<string, GameObject>()
        {
            { "A", aTargetPrefab },
            { "B", bTargetPrefab },
            { "C", cTargetPrefab },
            { "D", dTargetPrefab },
            { "E", eTargetPrefab },
            { "F", fTargetPrefab },
            { "G", gTargetPrefab },
            { "Am", amTargetPrefab },
            { "Dm", dmTargetPrefab },
            { "Em", emTargetPrefab },
        };
    }

    public GameObject SpawnNote(NoteData note, float songTime)
    {
        GameObject prefab = prefabMap[note.chord];
        if (prefab == null)
            return null;

        float timeUntilHit = note.time - songTime;

        float xOffset = timeUntilHit * scrollSpeed;

        Vector3 spawnPos = new Vector3(scoringPlank.position.x + xOffset, fixedY, fixedZ);

        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        note.obj = obj;

        return obj;
    }
}

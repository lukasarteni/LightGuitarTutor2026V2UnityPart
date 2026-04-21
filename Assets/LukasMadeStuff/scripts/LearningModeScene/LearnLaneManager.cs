using System.Collections.Generic;
using UnityEngine;

public class TestLaneManager : MonoBehaviour
{
    public Transform scoringPlank;

    public GameObject aTargetPrefab;
    public GameObject amTargetPrefab;
    public GameObject bTargetPrefab;
    public GameObject bmTargetPrefab;
    public GameObject cTargetPrefab;
    public GameObject cmTargetPrefab;
    public GameObject dTargetPrefab;
    public GameObject dmTargetPrefab;
    public GameObject eTargetPrefab;
    public GameObject emTargetPrefab;
    public GameObject fTargetPrefab;
    public GameObject fmTargetPrefab;
    public GameObject gTargetPrefab;
    public GameObject gmTargetPrefab;
    public GameObject NullTargetPrefab;

    public float scrollSpeed = 5f;

    const float fixedY = 1.85f;
    const float fixedZ = -7.3f;

    Dictionary<string, GameObject> prefabMap;

    void Awake()
    {
        prefabMap = new Dictionary<string, GameObject>()
        {
            { "A", aTargetPrefab },
            { "Am", amTargetPrefab },
            { "B", bTargetPrefab },
            { "Bm", bmTargetPrefab },
            { "C", cTargetPrefab },
            { "Cm", cmTargetPrefab },
            { "D", dTargetPrefab },
            { "Dm", dmTargetPrefab },
            { "E", eTargetPrefab },
            { "Em", emTargetPrefab },
            { "F", fTargetPrefab },
            { "Fm", fmTargetPrefab },
            { "G", gTargetPrefab },
            { "Gm", gmTargetPrefab },
            { "N", NullTargetPrefab },
        };
    }

    public GameObject SpawnNote(NoteData note, float songTime)
    {
        GameObject prefab = prefabMap[note.chord];

        float timeUntilHit = note.time - songTime;

        float xOffset = timeUntilHit * scrollSpeed;

        Vector3 spawnPos = new Vector3(scoringPlank.position.x + xOffset, fixedY, fixedZ);
        GameObject obj;
        if (prefab == null)
            obj = Instantiate(NullTargetPrefab, spawnPos, Quaternion.identity);
        else
            obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        note.obj = obj;

        return obj;
    }
}

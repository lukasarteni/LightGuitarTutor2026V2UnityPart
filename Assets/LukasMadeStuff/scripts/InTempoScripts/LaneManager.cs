using System.Collections.Generic;
using UnityEngine;

public class LaneManager : MonoBehaviour
{
    public Transform scoringPlank;

    public AudioSource music;

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
        GameObject prefab = prefabMap.ContainsKey(note.chord) ? prefabMap[note.chord] : null;
        if (prefab == null)
            prefab = NullTargetPrefab;

        float timeUntilHit = note.time - songTime;
        float xOffset = timeUntilHit * scrollSpeed;

        Vector3 spawnPos = new Vector3(scoringPlank.position.x + xOffset, fixedY, fixedZ);
        GameObject obj = Instantiate(prefab, spawnPos, Quaternion.identity);

        // Configure the NoteScroller so it positions based on music.time
        NoteScroller scroller = obj.GetComponent<NoteScroller>();
        if (scroller != null)
        {
            scroller.speed = scrollSpeed;
            scroller.noteTime = note.time;
            scroller.scoringPlankX = scoringPlank.position.x;
            scroller.music = music;
        }

        note.obj = obj;

        return obj;
    }
}

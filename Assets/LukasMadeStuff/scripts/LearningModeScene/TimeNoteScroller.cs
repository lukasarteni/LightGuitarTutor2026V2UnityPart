using UnityEngine;

public class TimeNoteScroller : MonoBehaviour
{
    public float speed = 5f;

    const float fixedY = 1.85f;
    const float fixedZ = -7.3f;

    void Update()
    {
        transform.position += Vector3.left * speed * Time.deltaTime;

        transform.position = new Vector3(
            transform.position.x,
            fixedY,
            fixedZ
        );
    }
}
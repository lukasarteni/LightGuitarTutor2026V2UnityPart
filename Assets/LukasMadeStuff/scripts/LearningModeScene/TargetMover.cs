using UnityEngine;

public class TargetMover : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public float speed = -5.0f;

    void Update()
    {
        // Moves the object along its own Z-axis (forward) at a continuous speed
        transform.Translate(Vector3.right * speed * Time.deltaTime);
    }

    public void FreezeAllThisNote()
    {
        speed = 0f;
    }

    public void UnfreezeAllThisNote()
    {
        speed = -5f;
    }
}

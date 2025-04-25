using UnityEngine;

public class TransformFollowAnim : MonoBehaviour
{
    public float amplitude;
    public float freq;

    // Update is called once per frame
    void Update()
    {
        float newYPos = transform.localPosition.y + Mathf.Sin(Time.time * freq) * amplitude * Time.deltaTime;
        transform.localPosition = new Vector3(transform.localPosition.x, newYPos);

    }
}

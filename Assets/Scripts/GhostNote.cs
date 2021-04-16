using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostNote : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 endPos;
    public float duration;

    public IEnumerator MoveNote()
    {
        float time = 0;
        float completion;
        yield return null;
        do
        {
            time += Time.deltaTime;
            completion = time / duration;

            transform.position = Vector3.Lerp(startPos, endPos, completion);
            yield return null;
        } while (completion < 1);
        Destroy(gameObject);
    }
}

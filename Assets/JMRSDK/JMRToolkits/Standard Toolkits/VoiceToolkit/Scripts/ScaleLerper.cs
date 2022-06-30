using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScaleLerper : MonoBehaviour
{
    // Start is called before the first frame update
    Vector3 startScale;
    public Vector3 EndScale = new Vector3(1.5f, 1.5f, 1.5f);
    public float speed = 1.0f;
    public float duration = 2.0f;
    public bool repeatable;
    // Update is called once per frame
  public IEnumerator start()
    {
        startScale = transform.localScale;
        while(repeatable)
        {
            yield return Repeater(startScale,EndScale,duration);
        }
        
    }
    public IEnumerator Repeater(Vector3 a, Vector3 b, float time)
    {
        float i = 0.0f;
        
        float rate = (1.0f /time) * speed;
        while(i<1.0f)
        {
            i += Time.deltaTime * rate;
            transform.localScale = Vector3.Lerp(a, b, i);
            yield return null;
        }
    }
}

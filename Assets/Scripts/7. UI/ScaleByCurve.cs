using UnityEngine;

public class ScaleByCurve : MonoBehaviour
{
    [SerializeField] private AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 1, 1, 1.1f);
    [SerializeField] private float duration = 1f;
    [SerializeField] private Vector3 baseScale = Vector3.one;

    private float timer;

    void Start()
    {
        baseScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;
        float t = (timer % duration) / duration;
        float curveValue = scaleCurve.Evaluate(t);
        transform.localScale = baseScale * curveValue;
    }
}
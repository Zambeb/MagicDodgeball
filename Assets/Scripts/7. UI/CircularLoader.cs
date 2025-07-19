using UnityEngine;
using UnityEngine.UI;

public class CircularLoader : MonoBehaviour
{
    public Image loaderImage;
    public float speed = 1f;

    private bool increasing = true;

    void Update()
    {
        if (loaderImage == null) return;

        float delta = speed * Time.deltaTime;

        if (increasing)
        {
            loaderImage.fillAmount += delta;
            if (loaderImage.fillAmount >= 1f)
            {
                loaderImage.fillAmount = 1f;
                increasing = false;
                loaderImage.fillClockwise = false; 
            }
        }
        else
        {
            loaderImage.fillAmount -= delta;
            if (loaderImage.fillAmount <= 0f)
            {
                loaderImage.fillAmount = 0f;
                increasing = true;
                loaderImage.fillClockwise = true; 
            }
        }
    }
}
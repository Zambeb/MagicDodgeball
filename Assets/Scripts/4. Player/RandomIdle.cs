using UnityEngine;

public class RandomIdle : MonoBehaviour
{
    public Animator animator;
    [SerializeField] float idleSwitchTime = 2f; // Adjustable delay between idles
    public int idleCount = 4; // Number of idle states

    [SerializeField] int idleIndex = 1;
    float timer;

    void Start()
    {
        if (!animator) animator = GetComponent<Animator>();
        timer = idleSwitchTime;
        SetIdle();
    }

    void Update()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
        {
            idleIndex++;
            if (idleIndex > idleCount) idleIndex = 1;
            SetIdle();
            timer = idleSwitchTime;
        }
    }

    void SetIdle()
    {
        animator.SetInteger("IdleIndex", idleIndex);
    }
}
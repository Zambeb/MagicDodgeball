using MoreMountains.Feedbacks;
using UnityEngine;

public class FeelManager : MonoBehaviour
{
    public static FeelManager Instance { get; private set; }
    
    public MMF_Player screenShake;
    public GamepadRumble rumble;
    
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void GetHitCameraShake()
    {
        screenShake.PlayFeedbacks();
    }
}

using UnityEngine;

public class IdolGun : MonoBehaviour
{
    
    [SerializeField] private Transform firingPoint;
    [SerializeField] private GameObject projectilePrefab;

    public PlayerController ownerPlayer;
    private GameObject projectileCollector;
    public GameObject shootVFX;
    
    void Awake()
    {
        projectileCollector = GameObject.Find("ProjectileCollector");
    }
    
    public void Shoot(int index, int bounces, float speed)
    {
        RoundManager.Instance.projectileCount++;
        
        GameObject projectile = Instantiate(projectilePrefab, firingPoint.position, firingPoint.rotation, projectileCollector.transform);
        
        Projectile proj = projectile.GetComponent<Projectile>();
        proj.ownerPlayer = ownerPlayer;
        proj.isMiniBall = true;
        proj.maxBounces = bounces;
        proj.projectileSpeed = speed;
        proj.accelerationAfterBounce = 1f;
        proj.canStun = false;
        proj.stunDuration = 0f;
        proj.playerIndex = index;
        proj.projectileCount = RoundManager.Instance.projectileCount;
        
        if (proj.ballsVisuals != null && index >= 0 && index < proj.ballsVisuals.Length)
        {
            proj.ballsVisuals[index].SetActive(true);
        }
        
        GameObject effect = Instantiate(shootVFX, firingPoint.position, firingPoint.rotation);
        Destroy(effect, 2);
    }
}

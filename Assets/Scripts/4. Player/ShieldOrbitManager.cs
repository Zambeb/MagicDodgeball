using System.Collections.Generic;
using UnityEngine;

public class ShieldOrbitManager : MonoBehaviour
{
    public GameObject shieldPrefab;
    public float radius = 2f;
    public float rotationSpeed = 60f;

    private List<GameObject> shields = new List<GameObject>();
    private float rotationOffset = 0f;

    void Update()
    {
        rotationOffset += rotationSpeed * Time.deltaTime;
        UpdateShieldPositions();
        
        if (Input.GetKeyDown(KeyCode.Q))
        {
            AddShield();
        }
    }

    public void AddShield()
    {
        GameObject shield = Instantiate(shieldPrefab, transform);
        shields.Add(shield);
    }
    
    public void ClearShields()
    {
        foreach (GameObject shield in shields)
        {
            if (shield != null)
            {
                Destroy(shield);
            }
        }
        shields.Clear();
    }

    private void UpdateShieldPositions()
    {
        int count = shields.Count;
        for (int i = 0; i < count; i++)
        {
            float angleDeg = rotationOffset + (360f / count) * i;
            float angleRad = angleDeg * Mathf.Deg2Rad;

            Vector3 offset = new Vector3(Mathf.Cos(angleRad), 0f, Mathf.Sin(angleRad)) * radius;
            Vector3 worldPos = transform.position + offset;
            
            shields[i].transform.position = worldPos;
            
            shields[i].transform.rotation = Quaternion.LookRotation(-offset.normalized, Vector3.up);
        }
    }
}
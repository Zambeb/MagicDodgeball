using UnityEngine;

public class PingPongMover : MonoBehaviour
{
    public Transform pointA;
    public Transform pointB;
    public float maxSpeed = 5f;           // Максимальная скорость перемещения
    public float accelerationDistance = 2f;  // Расстояние, на котором начинается замедление/ускорение

    private Vector3 target;
    private Vector3 start;
    private float speed = 0f;
    private bool movingToB = true;

    public Animator animator;

    void Start()
    {
        start = pointA.position;
        target = pointB.position;
        transform.position = start;
    }

    void Update()
    {
        Vector3 direction = (target - transform.position).normalized;
        float distanceToTarget = Vector3.Distance(transform.position, target);

        // Управляем скоростью: плавное ускорение и замедление
        if (distanceToTarget < accelerationDistance)
        {
            // Замедляемся при приближении
            speed = Mathf.Lerp(0, maxSpeed, distanceToTarget / accelerationDistance);
        }
        else
        {
            // Нарастаем скорость при старте
            speed = Mathf.Lerp(speed, maxSpeed, Time.deltaTime * 2f);
        }

        // Двигаемся вперед
        transform.position += direction * speed * Time.deltaTime;

        // Проверяем, достигли ли цель
        if (distanceToTarget < 0.1f)
        {
            // Меняем направление
            movingToB = !movingToB;
            target = movingToB ? pointB.position : pointA.position;
            start = transform.position;
            speed = 0f; // сбрасываем скорость для плавного старта
        }

        // Обновляем параметр скорости для анимации
        if (animator != null)
        {
            float signedSpeed = speed * (movingToB ? 1f : -1f) / 5;
            animator.SetFloat("Speed", signedSpeed);
        }
    }
}

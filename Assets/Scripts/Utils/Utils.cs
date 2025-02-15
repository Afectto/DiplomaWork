using System.Linq;
using UnityEngine;

public static class Utils
{
    public static Vector3 GetRandomPositionInCircle(Vector3 center, float radius1, float radius2)
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        float u = Random.Range(radius1, radius2);
        float x = u * Mathf.Cos(angle);
        float y = u * Mathf.Sin(angle);
        return new Vector3(x, y, 0) + center;
    }
    
    public static Vector3 GetRandomPositionInSphere(float radius)
    {
        float angle = Random.Range(0, 2 * Mathf.PI);
        float u = Random.Range(-1, 1);
        float r = Mathf.Sqrt(1 - u * u);
        float x = r * Mathf.Cos(angle);
        float y = r * Mathf.Sin(angle);
        return new Vector3(x, y, 0) * radius;
    }
    
    public static Vector3 GetRandomPositionInRectangle(float maxX, float maxY)
    {
        return new Vector3(Random.Range(-maxX, maxX), Random.Range(-maxY, maxY), 0);
    }

    public static Vector3 GetMouseWorldPosition()
    {
        Vector3 vec = GetMouseWorldPositionWithZ(Input.mousePosition, Camera.main);
        vec.z = 0;
        return vec;
    }

    public static Vector3 GetMouseWorldPositionWithZ(Vector3 mousePosition, Camera main)
    {
        return main.ScreenToWorldPoint(mousePosition);
    }

    public static void MoveStep(Transform targetMove, Transform target, float speed, bool isSmoothRotate = false, float speedRotate = 5)
    {
        MoveStep(targetMove, target.position, speed);
        if (!isSmoothRotate)
        {
            RotateToTarget(targetMove, target);
        }
        else
        {
            SmoothRotate(targetMove, target.position - targetMove.position, speedRotate);
        }
    }    
    
    public static void MoveStep(Transform targetMove, Vector3 target, float speed, bool isSmoothRotate = false, float speedRotate = 5)
    {
        MoveStep(targetMove, target, speed);
        if (!isSmoothRotate)
        {
            RotateToTarget(targetMove, target);
        }
        else
        {
            SmoothRotate(targetMove, target - targetMove.position, speedRotate);
        }
    }    
    
    public static void MoveStep(Transform targetMove, Vector3 target, float speed)
    {
        Vector3 direction = (target - targetMove.position).normalized;

        Vector3 newPosition = targetMove.position + direction * speed * Time.fixedDeltaTime;
        targetMove.position = newPosition;
    } 
    
    public static void RotateToTarget(Transform target, Vector3 targetToRotate)
    {
        Vector3 direction = (targetToRotate- target.position).normalized;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        target.rotation = Quaternion.Euler(0, 0, angle);
    }

    public static void RotateToTarget(Transform target, Transform targetToRotate)
    {
        RotateToTarget(target, targetToRotate.position);
    }

    public static Transform FindNearestTarget(Transform origin, float searchRadius)
    {
        var targetables = Physics2D.OverlapCircleAll(origin.position, searchRadius)
            .Where(collider2D => collider2D.gameObject.activeSelf && collider2D.TryGetComponent<ITargetable>(out _) && collider2D.gameObject != origin.gameObject)
            .Select(collider2D => collider2D.GetComponent<ITargetable>())
            .ToList();

        float shortestDistance = Mathf.Infinity;
        Transform nearestEnemy = null;

        foreach (var target in targetables)
        {
            float distance = Vector2.Distance(origin.position, target.Transform.position);
            if (distance < shortestDistance)
            {
                shortestDistance = distance;
                nearestEnemy = target.Transform;
            }
        }

        return nearestEnemy;
    }    
    
    public static void SmoothRotate(Transform target, Vector3 direction, float rotationSpeed)
    {
        if (direction != Vector3.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            Quaternion targetRotation = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            target.rotation = Quaternion.Lerp(target.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
    
    public static void PlayParticleOnDeath(ParticleSystem particleSystem, Vector3 position, float scale = 1)
    {
        if (particleSystem == null) return;

        var tempParticleSystem = Object.Instantiate(particleSystem, position, Quaternion.identity);
        tempParticleSystem.transform.localScale = new Vector3(scale, scale, scale);
        tempParticleSystem.Play();

        Object.Destroy(tempParticleSystem.gameObject, tempParticleSystem.main.duration + tempParticleSystem.main.startLifetime.constantMax);
    }
}

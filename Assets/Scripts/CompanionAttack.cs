using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class CompanionAttack : MonoBehaviour
{
    [SerializeField]
    private PoolableObject Prefab;
    [SerializeField]
    [Range(0.1f, 1f)]
    private float AttackDelay = 0.33f;
    [SerializeField]
    private float AttackMoveSpeed = 3;
    [SerializeField]
    private Companion Companion;

    private Coroutine AttackCoroutine;

    private List<Attackable> AttackableObjects = new List<Attackable>();

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Attackable>(out Attackable attackable))
        {
            AttackableObjects.Add(attackable);

            if (AttackCoroutine != null)
            {
                StopCoroutine(AttackCoroutine);
            }
            AttackCoroutine = StartCoroutine(Attack());
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Attackable>(out Attackable attackable))
        {
            AttackableObjects.Remove(attackable);
            if (AttackableObjects.Count == 0)
            {
                StopCoroutine(AttackCoroutine);
            }
        }
    }

    private IEnumerator Attack()
    {
        WaitForSeconds Wait = new WaitForSeconds(AttackDelay);
        while (AttackableObjects.Count > 0)
        {
            yield return Wait;

            Attackable closestAttackable = FindClosestAttackable();
            
            ObjectPool pool = ObjectPool.CreateInstance(Prefab, 10);
            PoolableObject pooledObject = pool.GetObject();
            pooledObject.transform.position = transform.position;

            StartCoroutine(MoveAttack(pooledObject, closestAttackable));
        }
    }

    private IEnumerator MoveAttack(PoolableObject PooledObject, Attackable Attackable)
    {
        Vector3 startPosition = PooledObject.transform.position;

        float distance = Vector3.Distance(PooledObject.transform.position, Attackable.transform.position);
        float startingDistance = distance;

        while(distance > 0)
        {
            PooledObject.transform.position = Vector3.Lerp(startPosition, Attackable.transform.position, 1 - (distance / startingDistance));

            distance -= Time.deltaTime * AttackMoveSpeed;

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        PooledObject.gameObject.SetActive(false);
    }

    private Attackable FindClosestAttackable()
    {
        float closestDistance = float.MaxValue;
        int closestIndex = 0;
        for (int i = 0; i < AttackableObjects.Count; i++)
        {
            float distance = Vector3.Distance(transform.position, AttackableObjects[i].transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return AttackableObjects[closestIndex];
    }
}

using Unity.Netcode;
using UnityEngine;

public class PlayerCombat : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork _playerNetwork;
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _attackRange = 5f;
    [SerializeField] private KeyCode _attackKey = KeyCode.Mouse0;

    private void Awake()
    {
        if (_playerNetwork == null)
            _playerNetwork = GetComponent<PlayerNetwork>();
    }

    private void Update()
    {
        if (!IsOwner) return;

        if (Input.GetKeyDown(_attackKey))
        {
            TryAttack();
        }
    }

    public void TryAttack()
    {
        if (!IsOwner) return;

        PlayerNetwork target = FindNearestTarget();

        if (target == null)
        {
            Debug.Log("[PlayerCombat] Цель не найдена");
            return;
        }

        if (target == _playerNetwork)
        {
            Debug.Log("[PlayerCombat] Нельзя атаковать себя!");
            return;
        }

        Debug.Log($"[PlayerCombat] Атака по: {target.Nickname.Value}");
        DealDamageServerRpc(target.NetworkObjectId, _damage);
    }

    [ServerRpc]
    private void DealDamageServerRpc(ulong targetObjectId, int damage)
    {
        if (!NetworkManager.Singleton.SpawnManager.SpawnedObjects.TryGetValue(targetObjectId, out NetworkObject targetObject))
        {
            Debug.LogWarning("[ServerRpc] Цель не найдена");
            return;
        }

        PlayerNetwork targetPlayer = targetObject.GetComponent<PlayerNetwork>();

        if (targetPlayer == null || targetPlayer == _playerNetwork)
        {
            Debug.LogWarning("[ServerRpc] Некорректная цель");
            return;
        }

        targetPlayer.TakeDamage(damage);
    }

    private PlayerNetwork FindNearestTarget()
    {
        PlayerNetwork nearest = null;
        float nearestDistance = _attackRange;

        foreach (var spawnedObject in NetworkManager.Singleton.SpawnManager.SpawnedObjects.Values)
        {
            if (spawnedObject == NetworkObject) continue;

            PlayerNetwork otherPlayer = spawnedObject.GetComponent<PlayerNetwork>();
            if (otherPlayer == null) continue;

            float distance = Vector3.Distance(transform.position, spawnedObject.transform.position);

            if (distance <= nearestDistance)
            {
                nearest = otherPlayer;
                nearestDistance = distance;
            }
        }
        return nearest;
    }
}

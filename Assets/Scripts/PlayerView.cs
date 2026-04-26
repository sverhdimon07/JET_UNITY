using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

public class PlayerView : NetworkBehaviour
{
    [SerializeField] private PlayerNetwork _playerNetwork;
    [SerializeField] private TMP_Text _nicknameText;
    [SerializeField] private TMP_Text _hpText;

    private void Awake()
    {
        if (_playerNetwork == null)
            _playerNetwork = GetComponent<PlayerNetwork>();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (_playerNetwork == null)
        {
            Debug.LogError("[PlayerView] PlayerNetwork не найден!");
            return;
        }

        _playerNetwork.Nickname.OnValueChanged += OnNicknameChanged;
        _playerNetwork.HP.OnValueChanged += OnHpChanged;

        OnNicknameChanged(default, _playerNetwork.Nickname.Value);
        OnHpChanged(0, _playerNetwork.HP.Value);

        Debug.Log($"[PlayerView] Подписан на изменения: {_playerNetwork.Nickname.Value}");
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        if (_playerNetwork != null)
        {
            _playerNetwork.Nickname.OnValueChanged -= OnNicknameChanged;
            _playerNetwork.HP.OnValueChanged -= OnHpChanged;
        }
    }

    private void OnNicknameChanged(FixedString32Bytes oldValue, FixedString32Bytes newValue)
    {
        if (_nicknameText != null)
            _nicknameText.text = newValue.ToString();
    }

    private void OnHpChanged(int oldValue, int newValue)
    {
        if (_hpText != null)
            _hpText.text = $"HP: {newValue}";
    }
}

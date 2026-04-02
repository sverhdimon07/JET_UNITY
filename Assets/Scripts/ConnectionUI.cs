/*
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ConnectionUI : MonoBehaviour
{

    [SerializeField] private TMP_InputField _nicknameInput;
    [SerializeField] private GameObject _menuPanel;

    // Статическое поле для передачи ника сетевому объекту
    public static string PlayerNickname { get; private set; } = "Player";

    private void Awake()
    {
        // Сохраняем ссылку на меню для последующего скрытия
        _menuPanel = _menuPanel ?? transform.Find("Panel").gameObject;
    }

    public void StartAsHost()
    {
        SaveNickname();
        Debug.Log($"[Host] Запуск с ником: {PlayerNickname}");
        NetworkManager.Singleton.StartHost();
        HideMenu();
    }

    public void StartAsClient()
    {
        SaveNickname();
        Debug.Log($"[Client] Подключение с ником: {PlayerNickname}");
        NetworkManager.Singleton.StartClient();
        HideMenu();
    }

    private void SaveNickname()
    {
        string rawValue = _nicknameInput != null ? _nicknameInput.text : string.Empty;
        PlayerNickname = string.IsNullOrWhiteSpace(rawValue) ? "Player" : rawValue.Trim();
    }

    private void HideMenu()
    {
        if (_menuPanel != null)
            _menuPanel.SetActive(false);
    }

    // Проверка подключения для скрытия меню
    private void OnNetworkReady()
    {
        if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.Singleton.IsHost)
        {
            HideMenu();
        }
    }
}
*/



using TMPro;
using Unity.Netcode;
using UnityEngine;

public class ConnectionUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField _nicknameInput;
    [SerializeField] private GameObject _menuPanel;

    public static string PlayerNickname { get; private set; } = "Player";
    public static bool IsConnected { get; private set; } = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("Контроллер не найден в сцене");
            return;
        }

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        NetworkManager.Singleton.OnServerStopped += OnServerStopped;

        Debug.Log("Контроллер инициализирован");
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
        {
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
            NetworkManager.Singleton.OnServerStopped -= OnServerStopped;
        }
    }

    public void StartAsHost()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("Контроллер не найден");
            return;
        }

        SaveNickname();
        NetworkManager.Singleton.StartHost();
        Debug.Log($"Хост запущен. Ник хоста: {PlayerNickname}");
    }

    public void StartAsClient()
    {
        if (NetworkManager.Singleton == null)
        {
            Debug.LogError("Контроллер не найден");
            return;
        }

        SaveNickname();
        NetworkManager.Singleton.StartClient();
        Debug.Log($"Client запущен. Ник клиента: {PlayerNickname}");
    }

    private void SaveNickname()
    {
        string rawValue = _nicknameInput != null ? _nicknameInput.text : string.Empty;
        PlayerNickname = string.IsNullOrWhiteSpace(rawValue) ? "Player" : rawValue.Trim();
    }

    private void OnClientConnected(ulong clientId)
    {
        IsConnected = true;
        Debug.Log($"Подключён. ClientID: {clientId}");

        if (_menuPanel != null)
            _menuPanel.SetActive(false);
    }

    private void OnServerStopped(bool cleanly)
    {
        IsConnected = false;
        Debug.Log($"Сервер остановлен");

        if (_menuPanel != null)
            _menuPanel.SetActive(true);
    }
}

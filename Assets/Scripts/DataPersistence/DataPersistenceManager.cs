using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Playables;
using UnityEngine.SceneManagement;

public class DataPersistenceManager : MonoBehaviour {
    public static DataPersistenceManager Instance => GameObject.Find(nameof(DataPersistenceManager)).GetComponent<DataPersistenceManager>();

    [Header("File Storage Config")]
    [SerializeField] private string fileNamePrefix;

    public GameData Data => data;

    private FileDataHandler dataHandler;
    private GameData data;

    private void Awake() {
        gameObject.name = nameof(DataPersistenceManager);

        var manager = Instance;
        if(manager != null && manager != this) {
            Debug.LogError("Found more than one Data Persistence Manager in the scene");
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        dataHandler = new FileDataHandler(Application.persistentDataPath);
        data = new GameData();
    }

    public void ApplyCurrentData() {
        foreach(IDataPersistence dataPersistenceObj in FindAllDataPersistenceObjects()) {
            dataPersistenceObj.LoadData(data);
        }
    }

    public void StoreCurrentState() {
        data = new();
        foreach(IDataPersistence dataPersistenceObj in FindAllDataPersistenceObjects()) {
            dataPersistenceObj.SaveData(ref data);
        }
    }

    public void LoadGame(int idx) {
        data = dataHandler.Load(fileNamePrefix + idx);
        if(data == null) {
            Debug.Log("No data was found. Initializing data to defaults.");
            data = new();
        }

        if(!string.IsNullOrEmpty(data.CurrScene)) SceneManager.LoadScene(data.CurrScene);
        ApplyCurrentData();
    }

    public void SaveGame(int idx, string saveName) {
        StoreCurrentState();

        data.saveName = saveName;
        data.CurrScene = SceneManager.GetActiveScene().name;
        dataHandler.Save(data, fileNamePrefix + idx);
    }

    private List<IDataPersistence> FindAllDataPersistenceObjects() {
        IEnumerable<IDataPersistence> dataPersistenceObjects = FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.None).OfType<IDataPersistence>();
        return new List<IDataPersistence>(dataPersistenceObjects);
    }
}

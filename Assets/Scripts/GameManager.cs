using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    public GameObject enviromentPrefab;

    public void Start()
    {
        Application.targetFrameRate = 60;
        spawnLevel();
    }

    public void spawnLevel()
    {
        if (!GameObject.Find(enviromentPrefab.name))
        {
            GameObject enviroment = Instantiate(enviromentPrefab);
            enviroment.name = enviromentPrefab.name;
        }
    }
}

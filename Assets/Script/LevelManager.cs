using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public Transform levelParent;
    public GameObject[] levelPrefabs;

    private int currentLevelIndex = 0;
    private GameObject currentLevelInstance;

    void Start()
    {
        LoadLevel(currentLevelIndex);
    }

    public void LoadLevel(int index)
    {
        if (currentLevelInstance != null)
        {
            Destroy(currentLevelInstance);
        }

        currentLevelInstance = Instantiate(levelPrefabs[index], levelParent);

        SnakeController snake = currentLevelInstance.GetComponentInChildren<SnakeController>();
        if (snake != null)
        {
            snake.SetLevelManager(this);
        }
    }

    public void LoadNextLevel()
    {
        currentLevelIndex++;

        if (currentLevelIndex < levelPrefabs.Length)
        {
            LoadLevel(currentLevelIndex);
        }
        else
        {
            Debug.Log("All levels completed!");
        }
    }
}

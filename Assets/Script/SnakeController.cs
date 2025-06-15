using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;
using UnityEngine.UI;

public class SnakeController : MonoBehaviour
{
    private bool hasTriggeredVictory = false;
    private bool isTransitioning = false;

    private LevelManager levelManager;
    private bool hasStartedFalling = false;
    public Transform snakeHead;
    public GameObject bodyPrefab;
    public Transform snakeFace;
    [SerializeField] private Transform bodyParent;

    private Vector2Int direction = Vector2Int.right;
    private List<Transform> bodyParts = new List<Transform>();
    private List<Vector3> previousPositions = new List<Vector3>();

    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckSize = 0.2f;
    [SerializeField] private Rigidbody2D snakeHeadRb;

    [SerializeField] private GameObject hole1; 
    [SerializeField] private GameObject hole2;

    public int score = 0;
    [SerializeField] private Text scoreText;
    void Start()
    {
        bodyParts.Add(snakeHead);
        previousPositions.Add(snakeHead.position);

        if (hole2 != null) hole2.SetActive(false);

    }

    void Update()
    {
        if (isTransitioning || hasStartedFalling) return;

        HandleInputAndMove();
        CheckFall();
    }

    void HandleInputAndMove()
    {
        Vector2Int newDirection = direction;

        if (Input.GetKeyDown(KeyCode.UpArrow) && direction != Vector2Int.down)
            newDirection = Vector2Int.up;
        else if (Input.GetKeyDown(KeyCode.DownArrow) && direction != Vector2Int.up)
            newDirection = Vector2Int.down;
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && direction != Vector2Int.right)
            newDirection = Vector2Int.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow) && direction != Vector2Int.left)
            newDirection = Vector2Int.right;
        else
            return; // Không có phím hợp lệ nào được nhấn → không di chuyển

        // Có phím được nhấn → cập nhật hướng và di chuyển
        direction = newDirection;
        Move();
    }

    void Move()
    {
        if (hasStartedFalling) return;

        Vector3 nextPos = snakeHead.position + new Vector3(direction.x, direction.y, 0);

        // Lưu vị trí hiện tại của đầu
        previousPositions.Insert(0, snakeHead.position);

        // Di chuyển đầu
        snakeHead.position = nextPos;

        // Di chuyển thân theo vị trí trước đó
        for (int i = 1; i < bodyParts.Count; i++)
        {
            bodyParts[i].position = previousPositions[i - 1];
        }

        // Giữ danh sách độ dài phù hợp
        if (previousPositions.Count > bodyParts.Count)
        {
            previousPositions.RemoveAt(previousPositions.Count - 1);
        }
        UpdateFaceRotation();
    }

    void UpdateFaceRotation()
    {
        float angle = 0f;

        if (direction == Vector2Int.up)
            angle = 180f;
        else if (direction == Vector2Int.right)
            angle = 90f;
        else if (direction == Vector2Int.down)
            angle = 0f;
        else if (direction == Vector2Int.left)
            angle = -90f;

        if (snakeFace != null)
        {
            snakeFace.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    public void Grow()
    {
        Vector3 spawnPos = bodyParts[bodyParts.Count - 1].position;
        GameObject newPart = Instantiate(bodyPrefab, spawnPos, Quaternion.identity, bodyParent);
        bodyParts.Add(newPart.transform);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Fruit"))
        {
            Grow();
            Destroy(other.gameObject);

            score += 1;

            if (scoreText != null)
            {
                scoreText.text = "Score: " + score.ToString();
            }

            StartCoroutine(CheckVictoryNextFrame());
        }
        else if (other.CompareTag("Hole") && hole2 != null && hole2.activeSelf && !hasTriggeredVictory)
        {
            hasTriggeredVictory = true;
            Debug.Log("Victory!");
            StartCoroutine(LoadNextLevelAfterDelay(3f));
        }
    }

    private System.Collections.IEnumerator LoadNextLevelAfterDelay(float delay)
    {
        isTransitioning = true;

        // (tuỳ chọn: thêm hiệu ứng DOTween ở đây)
        yield return new WaitForSeconds(delay);

        if (levelManager != null)
        {
            levelManager.LoadNextLevel();
            Destroy(gameObject); // huỷ SnakeController sau khi load level mới
        }
        else
        {
            Debug.LogWarning("LevelManager not assigned!");
        }
    }


    private System.Collections.IEnumerator CheckVictoryNextFrame()
    {
        yield return null; // đợi 1 frame
        CheckVictory();
    }

    public void CheckVictory()
    {
        GameObject[] fruits = GameObject.FindGameObjectsWithTag("Fruit");
        if (fruits.Length == 0)
        {
            Debug.Log("All fruits eaten!");

            // Mở hố
            if (hole1 != null) hole1.SetActive(false);
            if (hole2 != null) hole2.SetActive(true);
        }
    }
    public void SetLevelManager(LevelManager manager)
    {
        levelManager = manager;
    }

    void CheckFall()
    {
        if (hasStartedFalling) return; // tránh kiểm tra lại

        bool allOffGround = true;

        foreach (Transform part in bodyParts)
        {
            Collider2D hit = Physics2D.OverlapBox(part.position, Vector2.one * groundCheckSize, 0f, groundLayer);
            if (hit != null)
            {
                allOffGround = false;
                break;
            }
        }

        if (allOffGround)
        {
            hasStartedFalling = true;

            // Rơi đầu
            if (snakeHeadRb != null)
            {
                snakeHeadRb.bodyType = RigidbodyType2D.Dynamic;
                snakeHeadRb.gravityScale = 1f;
            }

            // Rơi các phần thân
            for (int i = 1; i < bodyParts.Count; i++)
            {
                Transform part = bodyParts[i];

                // Gỡ logic di chuyển thủ công bằng position
                // và giao cho Rigidbody xử lý rơi

                Rigidbody2D rb = part.gameObject.AddComponent<Rigidbody2D>();
                rb.gravityScale = 1f;
                rb.bodyType = RigidbodyType2D.Dynamic;
            }

            Debug.Log("Snake fell off the map!");
        }
    }

}

using System.Collections.Generic;
using Praktikum.Scenes.Snake.Assets.Scripts.Score;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SnakeController : MonoBehaviour
{

    public Camera mainCamera;
    public new GameObject collider;
    
    public Transform snakePart;
    public int initialSize;

    private List<Transform> _parts;
    private Vector2 _direction;
    private Bounds _bounds;

    private IScoreHandler _scoreHandler;
    private int _currentScore;

    private void Start()
    {
        _scoreHandler = new ScoreHandler();
        
        _direction = Vector2.up;
        _bounds = collider.GetComponent<BoxCollider2D>().bounds;
        
        _parts = new List<Transform>
        {
            transform
        };
        
        GrowDefault();
    }

    private void OnApplicationQuit()
    {
    }

    private void Update()
    {
        ChangeDirection();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Powerup"))
        {
            Grow();
            _currentScore += 1;
        } else if (other.CompareTag("Player"))
        {
            Death();
        }
    }

    private void Death()
    {
        _scoreHandler.Add(_currentScore);
        _scoreHandler.Save();
        SceneManager.LoadScene(0);
    }

    private void Move()
    {
        MoveParts();
        
        var position = transform.position;
        transform.position = new Vector3(
            Mathf.Round(position.x) + _direction.x * 0.5f,
            Mathf.Round(position.y) + _direction.y * 0.5f,
            mainCamera.farClipPlane - 1
        );
        
        CheckForOutOfBounds();
    }

    private void MoveParts()
    {
        // Move parts from end to front like a worm
        for (var i = _parts.Count - 1; i > 0; i--)
        {
            _parts[i].position = _parts[i - 1].position;
        }
    }

    private void Grow()
    {
        var part = Instantiate(snakePart);
        part.position = _parts[^1].position; // new part should be where last part was/is

        _parts.Add(part);
    }

    private void GrowDefault()
    {
        for (var i = 0; i < initialSize; i++)
        {
            Grow();
        }   
    }

    private void ChangeDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow) && _direction != Vector2.down)
        {
            _direction = Vector2.up;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && _direction != Vector2.right)
        {
            _direction = Vector2.left;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && _direction != Vector2.up)
        {
            _direction = Vector2.down;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && _direction != Vector2.left)
        {
            _direction = Vector2.right;
        }
    }

    private void CheckForOutOfBounds()
    {
        var bounds = _bounds;
        var position = transform.position;
        
        var xPosition = position.x;
        if (xPosition >= bounds.max.x)
        {
            position.x = xPosition * -1;
        } else if (xPosition <= bounds.min.x)
        {
            position.x = xPosition * -1;
        }
        
        var yPosition = position.y;
        if (yPosition >= bounds.max.y)
        {
            position.y = yPosition * -1;
        } else if (yPosition <= bounds.min.y)
        {
            position.y = yPosition * -1;
        }

        transform.position = position;
    }
}
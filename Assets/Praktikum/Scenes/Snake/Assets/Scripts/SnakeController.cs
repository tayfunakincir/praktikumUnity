using System.Collections.Generic;
using Praktikum.Scenes.Snake.Assets.Scripts.Score;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Praktikum.Scenes.Snake.Assets.Scripts
{
    public class SnakeController : MonoBehaviour
    {

        public Camera mainCamera;
        public new GameObject collider;
    
        public Transform snakePart;

        private int _initialSize;
        private float _speedFactor;
        
        private List<Transform> _parts;
        private Vector2 _direction;
        private Bounds _bounds;

        private IScoreHandler _scoreHandler;

        private void Awake()
        {
            _scoreHandler?.Reset();
        }

        private void Start()
        {
            Setup();
        }

        private void OnApplicationQuit()
        {
            _scoreHandler.Save();
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
                IncreaseSpeed(0.0015f);
                _scoreHandler.Increase();
            } else if (other.CompareTag("Player"))
            {
                Death();
            }
        }

        private void Setup()
        {
            _scoreHandler = gameObject.AddComponent<ScoreHandler>();
        
            _direction = Vector3.up;
            _bounds = collider.GetComponent<BoxCollider2D>().bounds;

            _initialSize = 1;
            _speedFactor = 0.03f;
            
            _parts = new List<Transform>
            {
                transform
            };
            
            ChangeDifficultySettings();
            GrowDefault();
        }

        private void Death()
        {
            _scoreHandler.AddScore();
            _scoreHandler.Save();
            SceneManager.LoadScene(0);
        }

        private void Move()
        {
            MoveParts();
        
            var position = transform.position;
            transform.position = new Vector3(
                Mathf.Round(position.x) + _direction.x,
                Mathf.Round(position.y) + _direction.y,
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
            for (var i = 0; i < _initialSize; i++)
            {
                Grow();
            }   
        }

        private void ChangeDifficultySettings()
        {
            switch (GetDifficulty())
            {
                case PrefKeys.Easy:
                    _speedFactor = 0.1f;
                    break;
                case PrefKeys.Medium:
                    _speedFactor = 0.08f;
                    _initialSize *= 3;  
                    break;
                case PrefKeys.Hard:
                    _speedFactor = 0.06f;
                    _initialSize *= 5;
                    break;
            }

            Time.fixedDeltaTime = _speedFactor;
        }

        private void IncreaseSpeed(float speed)
        {
            _speedFactor -= speed;
            Time.fixedDeltaTime = _speedFactor;
        }

        private void ChangeDirection()
        {
            if (CheckForOutOfBounds(false))
            {
                return;
            }
            
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

        private bool CheckForOutOfBounds(bool changePosition = true)
        {
            var outOfBounds = false;
            var bounds = _bounds;
            var position = transform.position;
        
            var xPosition = position.x;
            if (xPosition >= bounds.max.x)
            {
                position.x = xPosition * -1;
                outOfBounds = true;
            } else if (xPosition <= bounds.min.x)
            {
                position.x = xPosition * -1;
                outOfBounds = true;
            }
        
            var yPosition = position.y;
            if (yPosition >= bounds.max.y)
            {
                position.y = yPosition * -1;
                outOfBounds = true;
            } else if (yPosition <= bounds.min.y)
            {
                position.y = yPosition * -1;
                outOfBounds = true;
            }

            if (changePosition)
            {
                transform.position = position;   
            }
            return outOfBounds;
        }

        private static int GetDifficulty()
        {
            return PlayerPrefs.GetInt(PrefKeys.Difficulty);
        }
    }
}
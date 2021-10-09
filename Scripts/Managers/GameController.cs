using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pausePanel;
    [SerializeField] private float dropInterval = 0.9f;
    [SerializeField] [Range(0.02f, 1f)] private float baseKeyRepeatRate = 0.02f;
    [SerializeField] private IconToggle rotationIconToggle;
    [SerializeField] private ParticlePlayer gameOverFx;

    private const float MinDropRate = 0.05f;
    private const float MaxDropRate = 1f;
    
    private Board _board;
    private Spawner _spawner;
    private SoundManager _soundManager;
    private ScoreManager _scoreManager;
    private Shape _activeShape;
    private Ghost _ghost;
    private Holder _shapeHolder;
    private float _timeToDrop;
    private float _timeToNextKey;
    private bool _gameOver;
    private bool _clockwise = true;
    private bool _isPaused = false;
    private float _dropIntervalModded;

    private void Start()
    {
        _board = FindObjectOfType<Board>() ?? throw new MissingSceneObjectException(typeof(Board));
        _spawner = FindObjectOfType<Spawner>() ?? throw new MissingSceneObjectException(typeof(Spawner));
        _soundManager = FindObjectOfType<SoundManager>() ?? throw new MissingSceneObjectException(typeof(SoundManager));
        _scoreManager = FindObjectOfType<ScoreManager>() ?? throw new MissingSceneObjectException(typeof(ScoreManager));
        _ghost = FindObjectOfType<Ghost>() ?? throw new MissingSceneObjectException(typeof(Ghost));
        _shapeHolder = FindObjectOfType<Holder>() ?? throw new MissingSceneObjectException(typeof(Holder));
        _timeToNextKey = Time.time;
        
        if (_activeShape == null)
        {
            _activeShape = _spawner.SpawnShape();
        }
        _spawner.transform.position = Vector3Int.RoundToInt(_spawner.transform.position);

        if (gameOverPanel)
        {
            gameOverPanel.SetActive(false);
        }
        if (pausePanel)
        {
            pausePanel.SetActive(false);
        }

        _dropIntervalModded = dropInterval;
    }

    private void Update()
    {
        if (_gameOver || !_board || !_spawner || !_activeShape || !_soundManager) return;
        HandlePlayerInput();
    }

    private void LateUpdate()
    {
        _ghost.DrawGhost(_activeShape, _board);
    }

    private void HandlePlayerInput()
    {
        if (Input.GetButton("MoveRight") && Time.time > _timeToNextKey || Input.GetButtonDown("MoveRight"))
        {
            _activeShape.MoveRight();
            _timeToNextKey = Time.time + baseKeyRepeatRate * 10;
            ValidateMove(_activeShape.MoveLeft);
        }
        else if (Input.GetButton("MoveLeft") && Time.time > _timeToNextKey || Input.GetButtonDown("MoveLeft"))
        {
            _activeShape.MoveLeft();
            _timeToNextKey = Time.time + baseKeyRepeatRate * 10;
            ValidateMove(_activeShape.MoveRight);
        }
        else if (Input.GetButtonDown("Rotate") && Time.time > _timeToNextKey)
        {
            _activeShape.RotateClockwise(_clockwise);
            _timeToNextKey = Time.time + baseKeyRepeatRate * 6;
            ValidateMove(() => _activeShape.RotateClockwise(!_clockwise));
        }
        else if (Input.GetButton("MoveDown") && Time.time > _timeToNextKey || Time.time > _timeToDrop)
        {
            _timeToNextKey = Time.time + baseKeyRepeatRate;
            HandleDroppingShape();
        }
        else if (Input.GetButtonDown("ToggleRotation"))
        {
            ToggleRotationDirection();
        }
        else if (Input.GetButtonDown("Pause"))
        {
            TogglePause();
        }
        else if (Input.GetButtonDown("Hold"))
        {
            HoldShape();
        }
    }

    private void ValidateMove(Action moveCorrectionMethod)
    {
        if (!_board.IsValidPosition(_activeShape))
        {
            moveCorrectionMethod?.Invoke();
            _soundManager.PlayFx(SoundName.Error, 0.5f);
        }
        else
        {
            _soundManager.PlayFx(SoundName.Move, 0.5f);
        }
    }

    private void HandleDroppingShape()
    {
        _timeToDrop = Time.time + _dropIntervalModded;
        if (!_activeShape) return;
        
        _activeShape.MoveDown();
        if (_board.IsValidPosition(_activeShape)) return;
        if (_board.IsOverLimit(_activeShape))
        {
            GameOver();
        }
        else
        {
            _activeShape.MoveUp();
            _activeShape.LandShapeFx();
            _board.StoreShapeInGrid(_activeShape); 
            _ghost.ResetGhostShape();
            _shapeHolder.CanRelease = true;
            
            _activeShape = _spawner.SpawnShape();
            StartCoroutine(_board.ClearAllRows());
            _soundManager.PlayFx(SoundName.Drop);
            
            if (_board.CompletedRows <= 0) return;
            
            _scoreManager.ScoreLines(_board.CompletedRows);
            if (_scoreManager.DidLevelUp)
            {
                _soundManager.PlayFx(SoundName.LevelUp);
                _dropIntervalModded = Mathf.Clamp(dropInterval - (_scoreManager.Level - 1) * 0.05f, MinDropRate, MaxDropRate);
            }
            else
            {
                if (_board.CompletedRows > 1)
                {
                    _soundManager.PlayRandomVocal();
                }
            }
            _soundManager.PlayFx(SoundName.ClearRow);
        }
    }

    private void GameOver()
    {
        _soundManager.PlayFx(SoundName.GameOver, 5f);
        _soundManager.PlayFx(SoundName.GameOverVocal, 5f);
        _gameOver = true;
        _activeShape.MoveUp();
        StartCoroutine(GameOverRoutine());
    }

    private IEnumerator GameOverRoutine()
    {
        if (gameOverFx) gameOverFx.Play();
        yield return new WaitForSeconds(0.3f);
        if (gameOverPanel) gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ToggleRotationDirection()
    {
        _clockwise = !_clockwise;
        if (rotationIconToggle)
        {
            rotationIconToggle.ToggleIcon(_clockwise);
        }
    }

    public void TogglePause()
    {
        if (_gameOver)
        {
            return;
        }

        _isPaused = !_isPaused;
        if (!pausePanel) return;
        
        pausePanel.SetActive(_isPaused);
        Time.timeScale = _isPaused ? 0 : 1;
    }

    public void HoldShape()
    {
        if (_shapeHolder.IsEmpty())
        {
            _shapeHolder.Catch(_activeShape);
            _activeShape = _spawner.SpawnShape();
            _soundManager.PlayFx(SoundName.Hold);
        }
        else if (_shapeHolder.CanRelease)
        {
            Shape tempShape = _activeShape;
            _activeShape = _shapeHolder.Release();
            _activeShape.transform.position = _spawner.transform.position;
            _shapeHolder.Catch(tempShape);
            _soundManager.PlayFx(SoundName.Hold);
        }
        else
        {
            _soundManager.PlayFx(SoundName.Error);
        }

        _ghost.ResetGhostShape();
    }
}

using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text linesText;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private int linesPerLevel = 5;
    [SerializeField] private ParticlePlayer levelUpFx;
    
    public bool DidLevelUp { get; private set; }
    public int Level { get; private set; } = 1;
    
    private int _score = 0;
    private int _lines;

    private const int MinLines = 1;
    private const int MaxLines = 4;

    private readonly Dictionary<int, int> _levelPointDictionary = new Dictionary<int, int>
    {
        { 1, 40 },
        { 2, 100 },
        { 3, 300 },
        { 4, 1200 },
    };

    private void Start()
    {
        ResetScore();
    }

    public void ScoreLines(int n)
    {
        DidLevelUp = false;
        n = Mathf.Clamp(n, MinLines, MaxLines);
        _score += _levelPointDictionary[n] * Level;
        
        LevelUp(n);
        UpdateUIText();
    }

    private void ResetScore()
    {
        Level = 1;
        _lines = linesPerLevel * Level;
        UpdateUIText();
    }

    private void LevelUp(int n)
    {
        _lines -= n;
        if (_lines > 0) return;
        
        Level++;
        _lines = linesPerLevel * Level;
        DidLevelUp = true;
        if (levelUpFx) levelUpFx.Play();
    }

    private void UpdateUIText()
    {
        if (scoreText)
        {
            scoreText.text = _score.ToString("D5");
        }
        if (linesText)
        {
            linesText.text = _lines.ToString();
        }
        if (levelText)
        {
            levelText.text = Level.ToString();
        }
    }
}

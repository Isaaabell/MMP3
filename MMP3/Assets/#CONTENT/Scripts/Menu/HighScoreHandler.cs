using System.Collections;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

public class HighScoreHandler : MonoBehaviour
{
    [SerializeField] private int _maxCount = 5;
    [SerializeField] private List<HighScoreElement> _highScoreList = new List<HighScoreElement>();
    [SerializeField] private string _fileName;

    public delegate void OnHighScoreListChanged(List<HighScoreElement> highScoreList);
    public static event OnHighScoreListChanged onHighScoreListChanged;
    void Start()
    {
        LoadHighScores();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void LoadHighScores()
    {
        _highScoreList = FileHandler.ReadListFromJSON<HighScoreElement>(_fileName);
        while (_highScoreList.Count > _maxCount)
        {
            _highScoreList.RemoveAt(_maxCount);
        }

        if (onHighScoreListChanged != null)
        {
            onHighScoreListChanged.Invoke(_highScoreList);
        }

    }

    private void SaveHighScores()
    {
        FileHandler.SaveToJSON(_highScoreList, _fileName);
    }

    public void AddHighScoreIfPossible(HighScoreElement highScoreElement)
    {
        for (int i = 0; i < _maxCount; i++)
        {
            if (i >= _highScoreList.Count || highScoreElement.score >= _highScoreList[i].score)
            {
                _highScoreList.Insert(i, highScoreElement);

                while (_highScoreList.Count > _maxCount)
                {
                    _highScoreList.RemoveAt(_maxCount);
                }

                SaveHighScores();
                if (onHighScoreListChanged != null)
                {
                    onHighScoreListChanged.Invoke(_highScoreList);
                }

                break;
            }

        }

    }
}

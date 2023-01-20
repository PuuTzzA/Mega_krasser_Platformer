using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPreview : MonoBehaviour
{

    [SerializeField]
    private GameObject[] levels;

    private int _currentIndex = 0;

    private GameObject _currentPreview;

    // Start is called before the first frame update
    void Start()
    {
        _currentPreview = Object.Instantiate(levels[_currentIndex]);
        _currentPreview.AddComponent<RotateObject>();

    }

    public void nextPreview()
    {
        Destroy(_currentPreview);
        _currentIndex = (_currentIndex + 1) % levels.Length;
        _currentPreview = Object.Instantiate(levels[_currentIndex]);
        _currentPreview.AddComponent<RotateObject>();
    }

    public void lastPreview()
    {
        Destroy(_currentPreview);
        _currentIndex = (_currentIndex - 1 + levels.Length) % levels.Length;
        _currentPreview = Object.Instantiate(levels[_currentIndex]);
        _currentPreview.AddComponent<RotateObject>();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIStart : MonoBehaviour
{


    [SerializeField]
    private GameObject[] levels;

    private int _currentIndex = 0;

    private GameObject _currentPreview;

    private UIDocument _Doc;

    // Start is called before the first frame update
    void Start()
    {

        _currentPreview = Object.Instantiate(levels[_currentIndex]);
        _currentPreview.AddComponent<RotateObject>();

        _Doc = GetComponent<UIDocument>();
        Button playButton = _Doc.rootVisualElement.Q<Button>("PlayButton");
        Button forwardButton = _Doc.rootVisualElement.Q<Button>("ForwardButton");
        Button backwardButton = _Doc.rootVisualElement.Q<Button>("BackwardButton");
        playButton.clicked += playButtonOnClicked;
        forwardButton.clicked += forwardButtonOnClicked;
        backwardButton.clicked += backwardButtonOnClicked;
    }

    private void playButtonOnClicked()
    {
        Debug.Log("play Button pressed");
    }
    private void forwardButtonOnClicked()
    {
        nextPreview();
    }
    private void backwardButtonOnClicked()
    {
        lastPreview();
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

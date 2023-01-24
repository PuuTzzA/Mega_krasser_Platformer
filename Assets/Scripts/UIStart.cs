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

    private Button _playButton;

    // Start is called before the first frame update
    void Start()
    {

        _currentPreview = Object.Instantiate(levels[_currentIndex]);
        _currentPreview.AddComponent<RotateObject>();

        _Doc = GetComponent<UIDocument>();
        _playButton = _Doc.rootVisualElement.Q<Button>("PlayButton");
        Button forwardButton = _Doc.rootVisualElement.Q<Button>("ForwardButton");
        Button backwardButton = _Doc.rootVisualElement.Q<Button>("BackwardButton");
        _playButton.clicked += playButtonOnClicked;
        forwardButton.clicked += forwardButtonOnClicked;
        backwardButton.clicked += backwardButtonOnClicked;

         _Doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(ev =>
        {
            if (ev.oldRect.width != ev.newRect.width && ev.oldRect.height != ev.newRect.height)
            {
                _playButton.style.fontSize = _playButton.resolvedStyle.height;
            }

        });
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

using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIStart : MonoBehaviour
{


    [SerializeField]
    private GameObject[] levels;

    private int _currentIndex = 0;

    private GameObject _currentPreview;

    private UIDocument _Doc;

    private Button _playButton;

    private Label _record;

    // Start is called before the first frame update
    void Start()
    {

        _currentPreview = Instantiate(levels[_currentIndex]);

        _Doc = GetComponent<UIDocument>();
        _playButton = _Doc.rootVisualElement.Q<Button>("PlayButton");
        Button forwardButton = _Doc.rootVisualElement.Q<Button>("ForwardButton");
        Button backwardButton = _Doc.rootVisualElement.Q<Button>("BackwardButton");
        _record = _Doc.rootVisualElement.Q<Label>("Record");


        float fastestTime = _currentPreview.GetComponent<PreviewSettings>().settings.fastestTime;
        int minutes = Mathf.FloorToInt(fastestTime / 60F);
        int seconds = Mathf.FloorToInt(fastestTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt(fastestTime * 1000);
        milliseconds = milliseconds % 1000;
        milliseconds /= 10;
        _record.text = fastestTime == -1 ? "-----------" : string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds); _playButton.clicked += PlayButtonOnClicked;
        forwardButton.clicked += ForwardButtonOnClicked;
        backwardButton.clicked += BackwardButtonOnClicked;

        _Doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(ev =>
       {
           if (ev.oldRect.width != ev.newRect.width && ev.oldRect.height != ev.newRect.height)
           {
               _playButton.style.fontSize = _playButton.resolvedStyle.height * 90 / 100;
               _record.style.fontSize = _record.resolvedStyle.height * 4 / 10;
           }

       });
    }

    public void SetCollectableText(string text)
    {
        _record.text = text;
    }



    private void PlayButtonOnClicked()
    {
        SceneManager.LoadScene(_currentPreview.GetComponent<PreviewSettings>().GetScene(), LoadSceneMode.Single);
    }
    private void ForwardButtonOnClicked()
    {
        NextPreview();
    }
    private void BackwardButtonOnClicked()
    {
        LastPreview();
    }


    public void NextPreview()
    {
        Destroy(_currentPreview);
        _currentIndex = (_currentIndex + 1) % levels.Length;
        _currentPreview = Instantiate(levels[_currentIndex]);

        float fastestTime = _currentPreview.GetComponent<PreviewSettings>().settings.fastestTime;
        int minutes = Mathf.FloorToInt(fastestTime / 60F);
        int seconds = Mathf.FloorToInt(fastestTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt(fastestTime * 1000);
        milliseconds = milliseconds % 1000;
        milliseconds /= 10;
        _record.text = fastestTime == -1 ? "-----------" : string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

    }

    public void LastPreview()
    {
        Destroy(_currentPreview);
        _currentIndex = (_currentIndex - 1 + levels.Length) % levels.Length;
        _currentPreview = Instantiate(levels[_currentIndex]);

        float fastestTime = _currentPreview.GetComponent<PreviewSettings>().settings.fastestTime;
        int minutes = Mathf.FloorToInt(fastestTime / 60F);
        int seconds = Mathf.FloorToInt(fastestTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt(fastestTime * 1000);
        milliseconds = milliseconds % 1000;
        milliseconds /= 10;
        _record.text = fastestTime == -1 ? "-----------" : string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
    }
}

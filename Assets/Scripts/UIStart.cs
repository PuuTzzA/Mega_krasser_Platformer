using System;
using System.Collections;
using System.Collections.Generic;
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

    private Label _coinRecord;

    [SerializeField] private Material[] skyboxes;

    // Start is called before the first frame update
    void Start()
    {
        _currentPreview = Instantiate(levels[_currentIndex]);

        _Doc = GetComponent<UIDocument>();
        _playButton = _Doc.rootVisualElement.Q<Button>("PlayButton");
        Button forwardButton = _Doc.rootVisualElement.Q<Button>("ForwardButton");
        Button backwardButton = _Doc.rootVisualElement.Q<Button>("BackwardButton");
        _record = _Doc.rootVisualElement.Q<Label>("Record");
        _coinRecord = _Doc.rootVisualElement.Q<Label>("CoinRecord");

        Debug.Log(_currentPreview.GetComponent<PreviewSettings>());
        LevelSettings l = _currentPreview.GetComponent<PreviewSettings>().settings;
        int minutes = Mathf.FloorToInt(l.fastestTime / 60F);
        int seconds = Mathf.FloorToInt(l.fastestTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt(l.fastestTime * 1000);
        milliseconds = milliseconds % 1000;
        milliseconds /= 10;
        _record.text = l.fastestTime == -1 ? "-----------" : string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds); _playButton.clicked += PlayButtonOnClicked;
        _coinRecord.text = l.collectablesCollected + "/" + _currentPreview.GetComponent<PreviewSettings>().totalCollectables;
        forwardButton.clicked += ForwardButtonOnClicked;
        backwardButton.clicked += BackwardButtonOnClicked;

        _playButton.Focus();

        _playButton.RegisterCallback<MouseOverEvent>((type) =>
        {
            _playButton.Focus();
        });

        forwardButton.RegisterCallback<MouseOverEvent>((type) =>
        {
            forwardButton.Focus();
        });

        backwardButton.RegisterCallback<MouseOverEvent>((type) =>
        {
            backwardButton.Focus();
        });

        _Doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(ev =>
       {
           if (ev.oldRect.width != ev.newRect.width && ev.oldRect.height != ev.newRect.height)
           {
               _playButton.style.fontSize = _playButton.resolvedStyle.height * 75 / 100;
               _record.style.fontSize = _record.resolvedStyle.height * 5 / 10;
               _coinRecord.style.fontSize = _coinRecord.resolvedStyle.height * 5 / 10;
           }

       });
    }

    public void SetRecordText(string text)
    {
        _record.text = text;
    }

    public void SetCoinRecordText(string text)
    {
        _coinRecord.text = text;
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

        LevelSettings l = _currentPreview.GetComponent<PreviewSettings>().settings;
        int minutes = Mathf.FloorToInt(l.fastestTime / 60F);
        int seconds = Mathf.FloorToInt(l.fastestTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt(l.fastestTime * 1000);
        milliseconds = milliseconds % 1000;
        milliseconds /= 10;
        _record.text = l.fastestTime == -1 ? "-----------" : string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        _coinRecord.text = l.collectablesCollected + "/" + _currentPreview.GetComponent<PreviewSettings>().totalCollectables;

        RenderSettings.skybox = skyboxes[_currentIndex];
    }

    public void LastPreview()
    {
        Destroy(_currentPreview);
        _currentIndex = (_currentIndex - 1 + levels.Length) % levels.Length;
        _currentPreview = Instantiate(levels[_currentIndex]);

        LevelSettings l = _currentPreview.GetComponent<PreviewSettings>().settings;
        int minutes = Mathf.FloorToInt(l.fastestTime / 60F);
        int seconds = Mathf.FloorToInt(l.fastestTime - minutes * 60);
        int milliseconds = Mathf.FloorToInt(l.fastestTime * 1000);
        milliseconds = milliseconds % 1000;
        milliseconds /= 10;
        _record.text = l.fastestTime == -1 ? "-----------" : string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
        _coinRecord.text = l.collectablesCollected + "/" + _currentPreview.GetComponent<PreviewSettings>().totalCollectables;

        RenderSettings.skybox = skyboxes[_currentIndex];
    }
}

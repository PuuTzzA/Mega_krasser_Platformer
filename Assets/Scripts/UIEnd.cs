using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIEnd : MonoBehaviour
{

    private UIDocument _Doc;

    private Button _retryButton;
    private Button _homeButton;
    private Label _collectedCoins;
    private Label _timeUsed;
    private Label _recordTime;

    [SerializeField]
    private string startScene;


    // Start is called before the first frame update
    void Awake()
    {
        _Doc = GetComponent<UIDocument>();
        _retryButton = _Doc.rootVisualElement.Q<Button>("Retry");
        _homeButton = _Doc.rootVisualElement.Q<Button>("Home");

        _collectedCoins = _Doc.rootVisualElement.Q<Label>("CollectedCoins");
        Debug.Log(_collectedCoins);
        _timeUsed = _Doc.rootVisualElement.Q<Label>("TimeUsed");
        _recordTime = _Doc.rootVisualElement.Q<Label>("RecordTime");

        _retryButton.clicked += RetryButtonOnClicked;
        _homeButton.clicked += HomeButtonOnClicked;

        _retryButton.Focus();

        _retryButton.RegisterCallback<MouseOverEvent>((type) =>
        {
            _retryButton.Focus();
        });

        _homeButton.RegisterCallback<MouseOverEvent>((type) =>
        {
            _homeButton.Focus();
        });


        _Doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(ev =>
       {
           if (ev.oldRect.width != ev.newRect.width && ev.oldRect.height != ev.newRect.height)
           {
               _collectedCoins.style.fontSize = _collectedCoins.resolvedStyle.height;
               _timeUsed.style.fontSize = _timeUsed.resolvedStyle.height;
               _recordTime.style.fontSize = _recordTime.resolvedStyle.height;
               _retryButton.style.fontSize = _retryButton.resolvedStyle.height;
               _homeButton.style.fontSize = _homeButton.resolvedStyle.height;
           }

       });
    }

    private void RetryButtonOnClicked()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void HomeButtonOnClicked()
    {
        SceneManager.LoadScene(startScene);
    }

    public void SetCollectedCoinsText(string text)
    {
        _collectedCoins.text = text;
    }

    public void SetTimeText(float text)
    {
        int minutes = Mathf.FloorToInt(text / 60F);
        int seconds = Mathf.FloorToInt(text - minutes * 60);
        int milliseconds = Mathf.FloorToInt(text * 1000);
        milliseconds = milliseconds % 1000;
        milliseconds /= 10;
        string format = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);

        _timeUsed.text = format;

    }

    public void SetRecordTimeText(float text)
    {
        if (text == -1)
        {
            _recordTime.text = "----------";
        }
        else
        {
            int minutes = Mathf.FloorToInt(text / 60F);
            int seconds = Mathf.FloorToInt(text - minutes * 60);
            int milliseconds = Mathf.FloorToInt(text * 1000);
            milliseconds = milliseconds % 1000;
            milliseconds /= 10;
            string format = string.Format("{0:00}:{1:00}:{2:00}", minutes, seconds, milliseconds);
            _recordTime.text = format;
        }
    }

}

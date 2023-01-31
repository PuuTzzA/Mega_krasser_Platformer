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

    public void SetTimeText(string text)
    {
        _timeUsed.text = text;
    }

    public void SetRecordTimeText(string text)
    {
        _recordTime.text = text;
    }

}

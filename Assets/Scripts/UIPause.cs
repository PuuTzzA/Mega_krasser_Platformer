using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UIPause : MonoBehaviour
{

    [SerializeField]
    private string startScene;

    private UIDocument _Doc;

    private Button _retryButton;
    private Button _homeButton;
    private Button _resumeButton;
    private Label _text;

    public Player player;

    // Start is called before the first frame update
    void Start()
    {
        _Doc = GetComponent<UIDocument>();
        _retryButton = _Doc.rootVisualElement.Q<Button>("Retry");
        _homeButton = _Doc.rootVisualElement.Q<Button>("Home");
        _resumeButton = _Doc.rootVisualElement.Q<Button>("Resume");
        _text = _Doc.rootVisualElement.Q<Label>("Text");

        _retryButton.clicked += RetryButtonOnClicked;

        _homeButton.clicked += HomeButtonOnClicked;
        _resumeButton.clicked += ResumeButtonOnClicked;


        _Doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(ev =>
        {
            if (ev.oldRect.width != ev.newRect.width && ev.oldRect.height != ev.newRect.height)
            {
                _retryButton.style.fontSize = _retryButton.resolvedStyle.height;
                _homeButton.style.fontSize = _homeButton.resolvedStyle.height;
                _resumeButton.style.fontSize = _resumeButton.resolvedStyle.height;
                _text.style.fontSize = _text.resolvedStyle.height * 6 / 10;
            }
        }

        );
    }


    private void RetryButtonOnClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        Destroy(gameObject);
    }

    private void HomeButtonOnClicked()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(startScene);
    }

    private void ResumeButtonOnClicked()
    {
        player.setPaused(false);
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
        Time.timeScale = 1;
        Destroy(gameObject);
    }
}

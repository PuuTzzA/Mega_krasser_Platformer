using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIEnd : MonoBehaviour
{

    private UIDocument _Doc;

    private Button _retryButton;
    private Button _homeButton;
    private VisualElement _winLoosePic;

    private bool won = true;

    [SerializeField]
    private Texture2D winPicture;
    [SerializeField]
    private Texture2D loosePicture;


    // Start is called before the first frame update
    void Start()
    {
        _Doc = GetComponent<UIDocument>();
        _retryButton = _Doc.rootVisualElement.Q<Button>("Retry");
        _homeButton = _Doc.rootVisualElement.Q<Button>("Home");
        _winLoosePic = _Doc.rootVisualElement.Q<Button>("WinLoosePic");

        _retryButton.clicked += RetryButtonOnClicked;
        _homeButton.clicked += RetryButtonOnClicked;

        _winLoosePic.style.backgroundImage = won ? winPicture : loosePicture;
    }

    private void RetryButtonOnClicked()
    {
        Debug.Log("play Button pressed");
    }

    private void HomeButtonOnClicked()
    {
        Debug.Log("play Button pressed");
    }

    private void SetWon(bool won)
    {
        if (this.won != won)
        {
            this.won = won;
            _winLoosePic.style.backgroundImage = won ? winPicture : loosePicture;
        }
    }


}
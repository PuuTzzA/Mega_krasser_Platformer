using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class MenuController : MonoBehaviour
{

    private UIDocument _Doc;
    private UIPreview _UIPreviewScript;

    // Start is called before the first frame update
    void Start()
    {
        _UIPreviewScript = GetComponent<UIPreview>();
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
        _UIPreviewScript.nextPreview();
    }
     private void backwardButtonOnClicked()
    {
        _UIPreviewScript.lastPreview();
    }
}

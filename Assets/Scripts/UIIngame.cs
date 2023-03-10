using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIIngame : MonoBehaviour
{


    private UIDocument _Doc;

    private Label _counter;
    private Label _timer;
    private VisualElement _heart1;
    private VisualElement _heart2;
    private VisualElement _heart3;
    private float _lives;
    [SerializeField]
    private Texture2D heart;
    [SerializeField]
    private Texture2D brokenHeart;

    public int collectables; 


    // Start is called before the first frame update
    void Awake()
    {
        _Doc = GetComponent<UIDocument>();
        _counter = _Doc.rootVisualElement.Q<Label>("Counter");
        _timer = _Doc.rootVisualElement.Q<Label>("Time");
        _heart1 = _Doc.rootVisualElement.Q<VisualElement>("Heart1");
        _heart2 = _Doc.rootVisualElement.Q<VisualElement>("Heart2");
        _heart3 = _Doc.rootVisualElement.Q<VisualElement>("Heart3");

        SetLives(3);
        SetCollectableText("0");

        _Doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(ev =>
        {
            if (ev.oldRect.width != ev.newRect.width && ev.oldRect.height != ev.newRect.height)
            {
                _counter.style.fontSize = _counter.resolvedStyle.height;
                _timer.style.fontSize = _timer.resolvedStyle.height * 9 / 10;
            }

        });

    }

    public void SetTimer(string text){
        _timer.text = text;
    }

    public void SetCollectableText(string text)
    {
        _counter.text = text;
    }

    public void SetLives(int lives)
    {
        if (lives < 4 && lives > -1 && this._lives != lives)
        {
            this._lives = lives;
            if (lives == 0)
            {
                _heart1.style.backgroundImage = brokenHeart;
                _heart2.style.backgroundImage = brokenHeart;
                _heart3.style.backgroundImage = brokenHeart;
            }
            else if (lives == 1)
            {
                _heart1.style.backgroundImage = heart;
                _heart2.style.backgroundImage = brokenHeart;
                _heart3.style.backgroundImage = brokenHeart;
            }
            else if (lives == 2)
            {
                _heart1.style.backgroundImage = heart;
                _heart2.style.backgroundImage = heart;
                _heart3.style.backgroundImage = brokenHeart;
            }
            else if (lives == 3)
            {
                _heart1.style.backgroundImage = heart;
                _heart2.style.backgroundImage = heart;
                _heart3.style.backgroundImage = heart;
            }

        }

    }



}

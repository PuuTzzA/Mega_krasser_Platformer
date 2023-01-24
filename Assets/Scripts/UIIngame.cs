using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIIngame : MonoBehaviour
{


    private UIDocument _Doc;

    private Label counter;
    private VisualElement heart1;
    private VisualElement heart2;
    private VisualElement heart3;
    private float lives;
    [SerializeField]
    private Texture2D heart;
    [SerializeField]
    private Texture2D brokenHeart;


    // Start is called before the first frame update
    void Awake()
    {
        _Doc = GetComponent<UIDocument>();
        counter = _Doc.rootVisualElement.Q<Label>("Counter");
        heart1 = _Doc.rootVisualElement.Q<VisualElement>("Heart1");
        heart2 = _Doc.rootVisualElement.Q<VisualElement>("Heart2");
        heart3 = _Doc.rootVisualElement.Q<VisualElement>("Heart3");

        setLives(1);
        setCollectableText("0");


    }
    private void OnEnable()
    {
        _Doc.rootVisualElement.RegisterCallback<GeometryChangedEvent>(ev =>
        {
            if (ev.oldRect.width != ev.newRect.width && ev.oldRect.height != ev.newRect.height)
            {
                counter.style.fontSize = counter.resolvedStyle.height;
            }

        });
    }

    public void setCollectableText(string text){
        counter.text = text;
    }

    public void setLives(int lives)
    {
        if (lives < 4 && lives > -1 && this.lives != lives)
        {
            this.lives = lives;
            if (lives == 0)
            {
                heart1.style.backgroundImage = brokenHeart;
                heart2.style.backgroundImage = brokenHeart;
                heart3.style.backgroundImage = brokenHeart;
            }
            else if (lives == 1)
            {
                heart1.style.backgroundImage = heart;
                heart2.style.backgroundImage = brokenHeart;
                heart3.style.backgroundImage = brokenHeart;
            }
            else if (lives == 2)
            {
                heart1.style.backgroundImage = heart;
                heart2.style.backgroundImage = heart;
                heart3.style.backgroundImage = brokenHeart;
            }
            else if (lives == 3)
            {
                heart1.style.backgroundImage = heart;
                heart2.style.backgroundImage = heart;
                heart3.style.backgroundImage = heart;
            }

        }

    }



}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleUnit : MonoBehaviour
{
    [SerializeField] EnemiesBase _base;
    [SerializeField] int level;
    [SerializeField] bool isPlayerUnit;

    public Enemies Enemy { get; set; }

    Image image;
    Vector3 orginalPos;
    Color originalColor;

    private void Awake()
    {
        image = GetComponent<Image>();
        orginalPos = image.transform.localPosition;
        originalColor = image.color;
    }

    //funkcja tworzaca odpowiedniego przeciwnika
    public void Setup(Enemies enemy)
    {
        Enemy = enemy;
        if (isPlayerUnit)
            image.sprite = Enemy.Base.PlayerSprite;
        else
            image.sprite = Enemy.Base.FrontSprite;

        image.color = originalColor;

        PlayEnterAnimation();
    }

    //animacje postaci za pomoca narzedzia Dotween
    public void PlayEnterAnimation()
    {
        if (isPlayerUnit)
            image.transform.localPosition = new Vector3(-500f, orginalPos.y);
        else
            image.transform.localPosition = new Vector3(480f, orginalPos.y);
        image.transform.DOLocalMoveX(orginalPos.x, 1.5f);
    }

    public void PlayAttackAnimation()
    {
        var sequence = DOTween.Sequence();
        if (isPlayerUnit)
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x + 50f, 0.25f));
        else
            sequence.Append(image.transform.DOLocalMoveX(orginalPos.x - 50f, 0.25f));

        sequence.Append(image.transform.DOLocalMoveX(orginalPos.x, 0.25f));
    }

    public void PlayHitAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.DOColor(Color.red, 0.1f));
        sequence.Append(image.DOColor(originalColor, 0.1f));
    }

    public void PlayFaintAnimation()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(image.transform.DOLocalMoveY(orginalPos.y - 150f, 0.5f));
        sequence.Join(image.DOFade(0f, 0.5f));
    }


}

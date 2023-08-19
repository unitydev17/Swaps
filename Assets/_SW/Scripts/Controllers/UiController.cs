using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UiController : MonoBehaviour
{
    public static event Action OnRetry;
    public static event Action OnForceNextLevel;
    
    [SerializeField] private Image _fadeImg;
    [SerializeField] private BounceButton _retryBtn;

    private AppModel _model;

    [Inject]
    public void Construct(AppModel model)
    {
        _model = model;
    }

    private void OnEnable()
    {
        BoardController.OnImpossibleToComplete += AppearRetryButton;
    }

    private void OnDisable()
    {
        BoardController.OnImpossibleToComplete -= AppearRetryButton;
    }


    public void ForceNextLevel()
    {
        if (_model.inputDenied) return;
        OnForceNextLevel?.Invoke();
        FadeUnfade();
    }
    
    public void ForceRetryLevel()
    {
        if (_model.inputDenied) return;
        
        OnRetry?.Invoke();
        FadeUnfade();
    }

    public void FadeUnfade()
    {
        _fadeImg.DOKill();
        _fadeImg.DOFade(0, 1).From(1);
    }

    private void AppearRetryButton()
    {
        _retryBtn.gameObject.SetActive(true);
    }
    public void DisappearRetryButton()
    {
        _retryBtn.Disappear();
    }

}
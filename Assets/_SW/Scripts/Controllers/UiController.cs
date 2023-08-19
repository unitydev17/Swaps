using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UiController : MonoBehaviour
{
    [SerializeField] private Image _fadeImg;
    [SerializeField] private BounceButton _retryBtn;

    private SignalBus _signalBus;
    private AppModel _model;

    [Inject]
    public void Construct(SignalBus signalBus, AppModel model)
    {
        _signalBus = signalBus;
        _model = model;
    }


    public void ForceNextLevel()
    {
        if (_model.inputDenied) return;
        _signalBus.Fire<ForceNextLevelSignal>();
        FadeUnfade();
    }
    
    public void ForceRetryLevel()
    {
        if (_model.inputDenied) return;
        _signalBus.Fire<RetryLevelSignal>();
        FadeUnfade();
    }

    public void FadeUnfade()
    {
        _fadeImg.DOKill();
        _fadeImg.DOFade(0, 1).From(1);
    }

    public void AppearRetryButton()
    {
        _retryBtn.gameObject.SetActive(true);
    }
    public void DisappearRetryButton()
    {
        _retryBtn.Disappear();
    }

}
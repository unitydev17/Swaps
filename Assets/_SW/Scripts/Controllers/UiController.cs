using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class UiController : MonoBehaviour
{
    [SerializeField] private Image _fadeImg;

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

    public void FadeUnfade()
    {
        _fadeImg.DOKill();
        _fadeImg.DOFade(0, 1).From(1);
    }
}
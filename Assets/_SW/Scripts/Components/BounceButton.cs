using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class BounceButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool _closeOnClick;

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOKill();
        if (_closeOnClick)
        {
            Disappear();
        }
        else
        {
            transform.DOScale(1f, 0.5f).From(0.5f).SetEase(Ease.OutBack);
        }
    }

    private void OnEnable()
    {
        transform.DOScale(1, 0.5f).From(0.5f).SetEase(Ease.OutBack);
    }

    public void Disappear()
    {
        transform.DOKill();
        transform.DOScale(0f, 0.25f).OnComplete(() => gameObject.SetActive(false));
    }
}
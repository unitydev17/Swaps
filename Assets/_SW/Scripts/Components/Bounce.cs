using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

public class Bounce : MonoBehaviour, IPointerClickHandler
{
    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOKill();
        transform.DOScale(1f, 0.5f).From(0.5f).SetEase(Ease.OutBack);
    }
}
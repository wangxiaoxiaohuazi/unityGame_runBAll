using UnityEngine;
using UnityEngine.EventSystems;

public class TouchHandler : MonoBehaviour, IDragHandler
{
    public CharacterCarousel carousel;
    private float swipeThreshold = 50f; // 滑动判定阈值

    public void OnDrag(PointerEventData eventData)
    {
        if(Mathf.Abs(eventData.delta.x) > swipeThreshold)
        {
            if(eventData.delta.x > 0) 
                carousel.ShowPrevious();
            else
                carousel.ShowNext();
        }
    }
}

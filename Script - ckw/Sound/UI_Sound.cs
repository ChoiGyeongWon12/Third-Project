using UnityEngine;
using UnityEngine.EventSystems;

public class UI_Sound : MonoBehaviour, IPointerDownHandler, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] AudioClip hoverSound;
    [SerializeField] AudioClip clickSound;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (SoundManager.instance != null && hoverSound != null)
            SoundManager.instance.PlaySFX(clickSound);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // if (SoundManager.instance != null && hoverSound != null)
        //     SoundManager.instance.PlaySFX(clickSound);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0) == false && SoundManager.instance != null && hoverSound != null)
            SoundManager.instance.PlaySFX(hoverSound);
    }
}

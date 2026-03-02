using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverEnlarge : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private float scaleMultiplier = 1.2f;
    [SerializeField] private float transitionSpeed = 10f;
    private bool isHovering;
    private RectTransform rectTransform;
    private Vector3 originalScale;
    private Vector3 targetScale;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Start()
    {
        originalScale = rectTransform.localScale;
        targetScale = originalScale;
    }
    private void Update()
    {
        rectTransform.localScale = Vector3.Slerp(rectTransform.localScale, targetScale,
            transitionSpeed * Time.unscaledDeltaTime);
    }
    
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        targetScale = originalScale * scaleMultiplier;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetScale = originalScale;
    }
}

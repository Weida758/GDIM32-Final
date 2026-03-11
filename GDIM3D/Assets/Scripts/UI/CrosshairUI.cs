using System;
using UnityEngine;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private Image crosshairImage;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color targetColor = Color.green;
    [SerializeField] private float defaultSize = 16f;
    [SerializeField] private float targetSize = 24f;
    [SerializeField] private float lerpSpeed = 10f;

    [SerializeField] private Player player;
    private RectTransform crosshairRect;
    private bool crosshairEnabled = true;

    private DialogueSystem dialogueSystem;

    private void Start()
    {
        crosshairRect = crosshairImage.GetComponent<RectTransform>();
        dialogueSystem = FindFirstObjectByType<DialogueSystem>();
    }

    private void OnEnable()
    {
        player.OnInventoryOpen +=  SetCrosshairDisable;
        player.OnInventoryClose +=  SetCrosshairEnable;
    }

    private void OnDisable()
    {
        player.OnInventoryOpen -=  SetCrosshairDisable;
        player.OnInventoryClose -= SetCrosshairEnable;
    }

    private void Update()
    {
        // Hide crosshair during dialogue or inventory
        bool shouldHide = !crosshairEnabled 
                          || (dialogueSystem != null && dialogueSystem.IsDialogueActive);
        crosshairImage.enabled = !shouldHide;
        
        if (shouldHide) return;

        bool hasTarget = player != null 
                         && (player.targetedItem != null || player.targetedNPC != null);

        Color goalColor = hasTarget ? targetColor : defaultColor;
        float goalSize = hasTarget ? targetSize : defaultSize;

        crosshairImage.color = Color.Lerp(crosshairImage.color, goalColor, lerpSpeed * Time.deltaTime);
        float currentSize = Mathf.Lerp(crosshairRect.sizeDelta.x, goalSize, lerpSpeed * Time.deltaTime);
        crosshairRect.sizeDelta = new Vector2(currentSize, currentSize);
    }


    public void SetCrosshairEnable()
    {
        crosshairEnabled = true;
    }

    public void SetCrosshairDisable()
    {
        crosshairEnabled = false;
    }
}
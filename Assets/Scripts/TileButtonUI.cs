using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Attached to each tile button in the toolbar.
/// Shows tile name, remaining count, and handles selection highlight.
/// </summary>
public class TileButtonUI : MonoBehaviour
{
    [Header("UI References")]
    public RawImage previewImage;
    public TMP_Text tileNameText;
    public TMP_Text countText;
    public Button button;
    public GameObject selectedOutline;

    private int remainingCount;
    private TilePrefabEntry tileEntry;
    private System.Action<TileButtonUI> onSelected;

    public TilePrefabEntry TileEntry => tileEntry;
    public int RemainingCount => remainingCount;

    public void Setup(TilePrefabEntry entry, System.Action<TileButtonUI> onSelectCallback)
    {
        tileEntry = entry;
        remainingCount = entry.count;
        onSelected = onSelectCallback;

        tileNameText.text = entry.tileName;
        UpdateCountText();

        button.onClick.AddListener(OnButtonClicked);
        SetSelected(false);
    }

    void OnButtonClicked()
    {
        if (remainingCount <= 0) return;
        onSelected?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        if (selectedOutline != null)
            selectedOutline.SetActive(selected);

        // Dim button if out of tiles
        button.interactable = remainingCount > 0;
    }

    public void UseOne()
    {
        remainingCount = Mathf.Max(0, remainingCount - 1);
        UpdateCountText();
        if (remainingCount <= 0)
            SetSelected(false);
    }

    public void ReturnOne()
    {
        remainingCount++;
        UpdateCountText();
        button.interactable = true;
    }

    void UpdateCountText()
    {
        countText.text = $"x{remainingCount}";
        countText.color = remainingCount > 0 ? Color.white : Color.red;
    }
}
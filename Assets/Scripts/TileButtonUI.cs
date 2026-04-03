using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TileButtonUI : MonoBehaviour
{
    [Header("UI References")]
    public RawImage previewImage;
    public TMP_Text tileNameText;
    public TMP_Text countText;
    public Button button;
    public GameObject selectedOutline;

    private int remainingCount;
    private GameObject tilePrefab;
    private System.Action<TileButtonUI> onSelected;

    public GameObject TilePrefab => tilePrefab;
    public int RemainingCount => remainingCount;

    public void Setup(string tileName, Texture tileIcon, int count, GameObject prefab, System.Action<TileButtonUI> onSelectCallback)
    {
        tilePrefab = prefab;
        remainingCount = count;
        onSelected = onSelectCallback;

        if (previewImage != null)
            previewImage.texture = tileIcon;

        if (tileNameText != null)
            tileNameText.text = tileName;

        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(OnButtonClicked);
        }

        UpdateCountText();
        SetSelected(false);
    }

    private void OnButtonClicked()
    {
        if (remainingCount <= 0) return;
        onSelected?.Invoke(this);
    }

    public void SetSelected(bool selected)
    {
        if (selectedOutline != null)
            selectedOutline.SetActive(selected);

        if (button != null)
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

        if (button != null)
            button.interactable = true;
    }

    private void UpdateCountText()
    {
        if (countText != null)
        {
            countText.text = $"x{remainingCount}";
            countText.color = remainingCount > 0 ? Color.white : Color.red;
        }
    }
}
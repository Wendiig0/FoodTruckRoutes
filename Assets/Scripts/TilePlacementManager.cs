using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Main manager for the tile toolbar and placement system.
/// - Builds toolbar from TilePrefabEntry list
/// - Handles tile selection and road tile swapping
/// - Tracks limited tile counts per level
/// </summary>
public class TilePlacementManager : MonoBehaviour
{
    [Header("Level Tile Config")]
    public TilePrefabEntry[] availableTiles;

    [Header("Toolbar UI")]
    public Transform toolbarContainer;
    public GameObject tileButtonPrefab;
    public GameObject toolbarPanel;

    [Header("Scene Setup")]
    public Camera mainCamera;
    public string roadTileTag = "RoadTile";

    [Header("Editor Toggle")]
    public Button editorToggleButton;
    public TMP_Text editorToggleLabel;

    [Header("Car")]
    public CarFollowRoad car;

    [Header("Highlight")]
    public Material highlightMaterial;

    // Internal state
    private List<TileButtonUI> tileButtons = new List<TileButtonUI>();
    private TileButtonUI selectedButton = null;
    private bool isEditorMode = false;

    // Hover highlight tracking
    private GameObject hoveredTile;
    private MeshRenderer hoveredRenderer;
    private Material hoveredOriginalMat;

    void Start()
    {
        if (mainCamera == null) mainCamera = Camera.main;

        BuildToolbar();

        toolbarPanel.SetActive(false);
        editorToggleButton.onClick.AddListener(ToggleEditorMode);
        UpdateToggleLabel();
    }

    void BuildToolbar()
    {
        for (int i = 0; i < availableTiles.Length; i++)
        {
            TilePrefabEntry entry = availableTiles[i];

            GameObject btnObj = Instantiate(tileButtonPrefab, toolbarContainer);
            TileButtonUI btnUI = btnObj.GetComponent<TileButtonUI>();
            btnUI.Setup(entry, OnTileButtonSelected);
            tileButtons.Add(btnUI);
        }
    }

    void OnTileButtonSelected(TileButtonUI btn)
    {
        // Deselect previous
        if (selectedButton != null)
            selectedButton.SetSelected(false);

        // Toggle off if clicking same tile again
        if (selectedButton == btn)
        {
            selectedButton = null;
            return;
        }

        selectedButton = btn;
        selectedButton.SetSelected(true);
    }

    void Update()
    {
        if (!isEditorMode) return;

        HandleHover();

        if (Input.GetMouseButtonDown(0))
            TryPlaceTile();

        // Touch support
        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
            TryPlaceTile(Input.GetTouch(0).position);
    }

    void HandleHover()
    {
        if (selectedButton == null) { RestoreHover(); return; }

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag(roadTileTag))
        {
            GameObject obj = hit.transform.gameObject;
            if (obj != hoveredTile)
            {
                RestoreHover();
                hoveredTile = obj;
                hoveredRenderer = obj.GetComponentInChildren<MeshRenderer>();
                if (hoveredRenderer != null && highlightMaterial != null)
                {
                    hoveredOriginalMat = hoveredRenderer.material;
                    hoveredRenderer.material = highlightMaterial;
                }
            }
        }
        else
        {
            RestoreHover();
        }
    }

    void TryPlaceTile(Vector2? touchPos = null)
    {
        if (selectedButton == null) return;

        Vector2 screenPos = touchPos ?? (Vector2)Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.transform.CompareTag(roadTileTag))
        {
            GameObject oldTile = hit.transform.gameObject;

            // Return tile to count if it matches an existing entry
            ReturnTileToInventory(oldTile);

            Vector3 pos = oldTile.transform.position;
            Quaternion rot = oldTile.transform.rotation;
            Transform parent = oldTile.transform.parent;

            RestoreHover();
            Destroy(oldTile);

            // Spawn new tile
            GameObject newTile = Instantiate(selectedButton.TileEntry.tilePrefab, pos, rot, parent);
            newTile.tag = roadTileTag;

            // Deduct count
            selectedButton.UseOne();
            if (selectedButton.RemainingCount <= 0)
                selectedButton = null;

            Debug.Log($"[TilePlacement] Placed: {selectedButton?.TileEntry.tileName ?? "none"}");
        }
    }

    void ReturnTileToInventory(GameObject tile)
    {
        // Check if the tile being replaced matches any toolbar entry — if so, return 1 to that slot
        foreach (TileButtonUI btn in tileButtons)
        {
            if (tile.name.Contains(btn.TileEntry.tilePrefab.name))
            {
                btn.ReturnOne();
                return;
            }
        }
    }

    void RestoreHover()
    {
        if (hoveredTile != null && hoveredRenderer != null && hoveredOriginalMat != null)
            hoveredRenderer.material = hoveredOriginalMat;

        hoveredTile = null;
        hoveredRenderer = null;
        hoveredOriginalMat = null;
    }

    void ToggleEditorMode()
    {
        isEditorMode = !isEditorMode;
        toolbarPanel.SetActive(isEditorMode);
        UpdateToggleLabel();

        if (isEditorMode)
        {
            if (car != null) car.StopMoving();
        }
        else
        {
            RestoreHover();
            selectedButton?.SetSelected(false);
            selectedButton = null;
        }
    }

    void UpdateToggleLabel()
    {
        if (editorToggleLabel != null)
            editorToggleLabel.text = isEditorMode ? "▶ Play" : "✏ Edit";
    }

}
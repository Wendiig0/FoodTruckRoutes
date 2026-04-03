using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class TilePlacementManager : MonoBehaviour
{
    [System.Serializable]
    public class TileEntry
    {
        public string tileName;
        public GameObject tilePrefab;
        public Texture2D tileIcon;
        public int count;
    }

    [Header("Level Tile Config")]
    public TileEntry[] availableTiles;

    [Header("Toolbar UI")]
    public Transform toolbarContainer;
    public GameObject tileButtonPrefab;
    public GameObject toolbarPanel;

    [Header("Scene Setup")]
    public Camera mainCamera;

    [Header("Editor Toggle")]
    public Button editorToggleButton;
    public TMP_Text editorToggleLabel;

    [Header("Car")]
    public CarFollowRoad car;

    [Header("Highlight")]
    public Material highlightMaterial;

    private readonly List<TileButtonUI> tileButtons = new List<TileButtonUI>();
    private TileButtonUI selectedButton = null;
    private bool isEditorMode = false;

    private GameObject hoveredTile;
    private MeshRenderer hoveredRenderer;
    private Material hoveredOriginalMat;

    private void Start()
    {
        if (mainCamera == null)
            mainCamera = Camera.main;

        BuildToolbar();

        if (toolbarPanel != null)
            toolbarPanel.SetActive(false);

        if (editorToggleButton != null)
            editorToggleButton.onClick.AddListener(ToggleEditorMode);

        UpdateToggleLabel();
    }

    private void BuildToolbar()
    {
        if (toolbarContainer == null || tileButtonPrefab == null) return;

        foreach (Transform child in toolbarContainer)
        {
            Destroy(child.gameObject);
        }

        tileButtons.Clear();

        foreach (TileEntry entry in availableTiles)
        {
            GameObject btnObj = Instantiate(tileButtonPrefab, toolbarContainer);
            TileButtonUI btnUI = btnObj.GetComponent<TileButtonUI>();

            if (btnUI != null)
            {
                btnUI.Setup(entry.tileName, entry.tileIcon, entry.count, entry.tilePrefab, OnTileButtonSelected);
                tileButtons.Add(btnUI);
            }
        }
    }

    private void OnTileButtonSelected(TileButtonUI btn)
    {
        if (selectedButton != null)
            selectedButton.SetSelected(false);

        if (selectedButton == btn)
        {
            selectedButton = null;
            return;
        }

        selectedButton = btn;

        if (selectedButton != null)
            selectedButton.SetSelected(true);
    }

    private void Update()
    {
        if (!isEditorMode) return;

        HandleHover();

        if (Input.GetMouseButtonDown(0))
        {
            HandleClick(Input.mousePosition);
        }

        if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began)
        {
            HandleClick(Input.GetTouch(0).position);
        }
    }

    private void HandleClick(Vector2 screenPos)
    {
        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(screenPos);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit)) return;

        RoadTile clickedTile = hit.transform.GetComponentInParent<RoadTile>();
        if (clickedTile == null) return;

        if (clickedTile.IsRotating) return;

        if (selectedButton != null)
        {
            ReplaceTile(clickedTile);
        }
        else
        {
            clickedTile.Rotate90();
        }
    }

    private void ReplaceTile(RoadTile oldTileComponent)
    {
        if (selectedButton == null || oldTileComponent == null) return;

        GameObject oldTile = oldTileComponent.gameObject;

        ReturnTileToInventory(oldTile);

        Vector3 pos = oldTile.transform.position;
        Quaternion rot = oldTile.transform.rotation;
        Transform parent = oldTile.transform.parent;

        RestoreHover();
        Destroy(oldTile);

        GameObject newTile = Instantiate(selectedButton.TilePrefab, pos, rot, parent);

        if (newTile.GetComponent<RoadTile>() == null)
        {
            Debug.LogWarning($"The prefab '{newTile.name}' does not have a RoadTile component.");
        }

        selectedButton.UseOne();

        if (selectedButton.RemainingCount <= 0)
        {
            selectedButton.SetSelected(false);
            selectedButton = null;
        }
    }

    private void ReturnTileToInventory(GameObject tile)
    {
        if (tile == null) return;

        foreach (TileButtonUI btn in tileButtons)
        {
            if (btn.TilePrefab == null) continue;

            string placedName = tile.name.Replace("(Clone)", "").Trim();
            string prefabName = btn.TilePrefab.name.Replace("(Clone)", "").Trim();

            if (placedName == prefabName)
            {
                btn.ReturnOne();
                return;
            }
        }
    }

    private void HandleHover()
    {
        if (selectedButton == null)
        {
            RestoreHover();
            return;
        }

        if (mainCamera == null) return;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            RoadTile tile = hit.transform.GetComponentInParent<RoadTile>();

            if (tile != null)
            {
                GameObject obj = tile.gameObject;

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

                return;
            }
        }

        RestoreHover();
    }

    private void RestoreHover()
    {
        if (hoveredRenderer != null && hoveredOriginalMat != null)
        {
            hoveredRenderer.material = hoveredOriginalMat;
        }

        hoveredTile = null;
        hoveredRenderer = null;
        hoveredOriginalMat = null;
    }

    private void ToggleEditorMode()
    {
        isEditorMode = !isEditorMode;

        if (toolbarPanel != null)
            toolbarPanel.SetActive(isEditorMode);

        UpdateToggleLabel();

        if (isEditorMode)
        {
            if (car != null)
                car.StopMoving();
        }
        else
        {
            RestoreHover();

            if (selectedButton != null)
            {
                selectedButton.SetSelected(false);
                selectedButton = null;
            }
        }
    }

    private void UpdateToggleLabel()
    {
        if (editorToggleLabel != null)
            editorToggleLabel.text = isEditorMode ? "Play" : "Edit";
    }
}
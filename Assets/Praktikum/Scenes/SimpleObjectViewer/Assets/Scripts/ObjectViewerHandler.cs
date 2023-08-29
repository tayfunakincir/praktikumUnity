using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ObjectViewerHandler : MonoBehaviour
{

    public List<GameObject> gameObjects;
    public GameObject spawnTransform;
    public Camera mainCamera;
    
    private GameObject _currentObject;

    public Image backGroundPanel, objectPanel;
    
    public Dropdown dropDown;
    public Button spawnButton, invserseButton;

    private Vector3 mouseInput;
    private bool _drag;
    
    [SerializeField] private float rotationSpeed;
    [SerializeField] private float scrollSpeed;
    
    private void Start()
    {
        Setup();
    }

    private void OnDisable()
    {
        Disable();
    }

    private void Update()
    {
        _drag = Input.GetMouseButton(0);
    }

    private void FixedUpdate()
    {
        Zoom();
        
        if (!_drag)
        {
            return;
        }

        if (_currentObject != null)
        {
            mouseInput.x += Input.GetAxis("Mouse X") * rotationSpeed;
            mouseInput.y += Input.GetAxis("Mouse Y") * rotationSpeed;
            
            _currentObject.transform.rotation = Quaternion.Euler(mouseInput.y, -mouseInput.x, 0);
        }
    }

    private void Setup()
    {
        LoadDropDownMenu();
        RegisterListener();
    }

    private void RegisterListener()
    {
        spawnButton.onClick.AddListener(SpawnSelectedObject);
        invserseButton.onClick.AddListener(InverseBackgroundColors);
    }

    private void Disable()
    {
        spawnButton.onClick.RemoveListener(SpawnSelectedObject);
        invserseButton.onClick.RemoveListener(InverseBackgroundColors);
    }

    private void LoadDropDownMenu()
    {
        dropDown.ClearOptions();

        foreach (var gObject in gameObjects)
        {
            dropDown.options.Add(new Dropdown.OptionData()
            {
                text = gObject.name
            });

            Instantiate(gObject);
        
            gObject.transform.position = spawnTransform.transform.position;
            gObject.transform.localScale = spawnTransform.transform.localScale;
            gObject.SetActive(false);
        }

        dropDown.value = 1;
        dropDown.value = 0;
    }

    private void Zoom()
    {
        var scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll == 0)
        {
            return;
        }
        
        var cameraPosition = mainCamera.transform.position;
        if (Input.mouseScrollDelta.y > 0 && Vector3.Distance(cameraPosition, spawnTransform.transform.position) > 0.5f)
        {
            mainCamera.transform.position += transform.forward * (Time.deltaTime * scrollSpeed);
        }
        else if (Input.mouseScrollDelta.y < 0 && Vector3.Distance(cameraPosition, spawnTransform.transform.position) < 5)
        {
            mainCamera.transform.position -= transform.forward * (Time.deltaTime * scrollSpeed);
        }
    }
    
    private void InverseBackgroundColors()
    {
        (backGroundPanel.color, objectPanel.color) = (objectPanel.color, backGroundPanel.color);
    }
    
    private void SpawnSelectedObject()
    {
        if (_currentObject != null)
        {
            _currentObject.SetActive(false);
        }

        mouseInput = Vector3.zero;
        
        _currentObject = gameObjects[dropDown.value];
        _currentObject.transform.rotation = Quaternion.Euler(0,0,0);
            
        _currentObject.SetActive(true);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Praktikum.Scenes.Grid_Editor.Assets.Scripts.Files;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace Praktikum.Scenes.Grid_Editor.Assets.Scripts.Grid
{
    public class GridGenerator : MonoBehaviour
    {

        public Transform spherePrefab;
        public Transform cubePrefab;
        public Transform capsulePrefab;

        public Slider slider;
        public Text sliderText;
        
        public int length = 100;
        public int gridSize = 1;

        private Camera _mainCamera;
        private Transform _selectedObject;
        private Plane _plane;

        private IFileHandler _fileHandler;

        private Dictionary<GridObjectType, Transform> _types;
        private List<Transform> _spheres;
        private GridData _gridData;
        private bool _grab;

        private void Start()
        {
            Setup();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnObjectOnRandomGridPoint();
            }

            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                DeleteSelectedObject();
            }

            if (Input.GetMouseButtonDown(0))
            {
                SelectObject();
            }

            _grab = Input.GetMouseButton(0);
            if (_grab)
            {
                MoveSelectedObject();
            }
            else
            {
                if (Input.GetMouseButtonUp(0))
                {
                    SnapSelectedObject();
                }
            }
        }

        private void OnApplicationQuit()
        {
            _fileHandler.Save(_gridData);
        }

        private void Setup()
        {
            _fileHandler = new FileHandler();
            
            _mainCamera = Camera.main;
            _spheres = new List<Transform>();
            _plane = new Plane(new Vector3(0, 0, 0), new Vector3(0, 0, CalculateMaximumLength()),
                new Vector3(CalculateMaximumLength(), 0, 0));
            
            _types = new Dictionary<GridObjectType, Transform>
            {
                { GridObjectType.Cube, cubePrefab },
                { GridObjectType.Capsule, capsulePrefab }
            }; // TODO: Automation

            slider.minValue = 1f;
            slider.maxValue = length - gridSize;
            slider.value = gridSize;
            sliderText.text = "" + slider.value;
            
            slider.onValueChanged.AddListener(delegate { UpdateSlider(); });
            
            SetupPreviousGrid();
            Generate();
            LoadPreviousObjects();
        }

        private void UpdateSlider()
        {
            var value = slider.value;
            
            sliderText.text = "" + value;
            
            gridSize = Mathf.RoundToInt(value);
            _plane = new Plane(new Vector3(0, 0, 0), new Vector3(0, 0, CalculateMaximumLength()),
                new Vector3(CalculateMaximumLength(), 0, 0));
            
            Regenerate();
        }

        private void Generate()
        {
            _spheres.Clear();
            
            for (var x = 0; x < length; x++)
            {
                if (x % gridSize != 0)
                {
                    continue;
                }
                
                for (var z = 0; z < length; z++)
                {
                    if (z % gridSize != 0)
                    {
                        continue;
                    }
                    _spheres.Add(Instantiate(spherePrefab, new Vector3(x, 0f, z), Quaternion.Euler(0f, 0f, 0f)));

                    // GenerateJoints(x, z, sphere);
                }
            }
            Debug.Log("Created a total of " + _spheres.Count + " spheres");
        }

        private void Regenerate()
        {
            foreach (var sphere in _spheres)
            {
                Destroy(sphere.gameObject);
            }

            if (_selectedObject != null)
            {
                _selectedObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
                _selectedObject = null;   
            }
            
            Generate();

            var gridObjects = _gridData.GridObjectDictionary.Values.ToList();
            foreach (var gridObject in gridObjects)
            {
                var id = gridObject.id;
                var cube = GameObject.Find(id.ToString());
            
                var x = gridObject.positionX;
                var y = gridObject.positionY;
                var z = gridObject.positionZ;
                cube.transform.position = GetNearestGridPointFromPosition(new Vector3(x, y, z));

                _gridData.GridObjectDictionary[gridObject.id] = gridObject.SetId(id).SetPosition(x, y, z);
            }
        }

        // private void GenerateJoints(float xPosition, float zPosition, Transform parent)
        // {
        //     var size = gridSize / 2f;
        //     var max = (float) (Math.Sqrt(_gridPoints.Count)) * gridSize;      
        //     if (zPosition <= max && xPosition < max)
        //     {
        //         var zJoint = Instantiate(jointPrefab, parent, true);
        //                 
        //         zJoint.position = new Vector3(xPosition + size, 0f, zPosition);
        //         zJoint.rotation = Quaternion.Euler(0f, 0f, 90f);
        //         zJoint.localScale = new Vector3(0.5f, size, 0.5f);
        //     }
        //     if (xPosition <= max && zPosition < max)
        //     {
        //         var xJoint = Instantiate(jointPrefab, parent, true);
        //                 
        //         xJoint.position = new Vector3(xPosition, 0f, zPosition + size);
        //         xJoint.rotation = Quaternion.Euler(90f, 0f, 0f);
        //         xJoint.localScale = new Vector3(0.5f, size, 0.5f);
        //     }
        // }

        private void SetupPreviousGrid()
        {
            _gridData = _fileHandler.Read()?? new GridData()
            {
                GridObjects = new GridObject[] {}
            };
            _gridData.GridObjectDictionary = new Dictionary<int, GridObject>();
        }
        
        private void LoadPreviousObjects()
        {
            if (_gridData == null)
            {
                return;
            }
            
            foreach (var gridObject in _gridData.GridObjects)
            {
                var cube = Instantiate(_types[gridObject.type]);
                var id = cube.GetInstanceID();
                cube.name = id.ToString();
                
                var x = gridObject.positionX;
                var y = gridObject.positionY;
                var z = gridObject.positionZ;
                cube.transform.position = GetNearestGridPointFromPosition(new Vector3(x, y, z));
                
                _gridData.GridObjectDictionary.Add(id, new GridObject()
                {
                    id = id,
                    type = gridObject.type,
                    positionX = gridObject.positionX,
                    positionY = gridObject.positionY,
                    positionZ = gridObject.positionZ,
                });
            }
            
            Debug.Log("Loaded a total of " + _gridData.GridObjects.Length + " previous objects");
        }

        private Vector3 GetNearestGridPointFromPosition(Vector3 position)
        {
            var max = CalculateMaximumLength();
            var x = Mathf.RoundToInt(position.x / gridSize);
            var z = Mathf.RoundToInt(position.z / gridSize);

            var result = new Vector3(x, 0f, z) * gridSize;
            if (result.x < 0)
            {
                result.x = 0;
            }
            if (result.x > max)
            {
                result.x = max;
            }
            
            if (result.z < 0)
            {
                result.z = 0;
            }
            if (result.z > max)
            {
                result.z = max;
            }

            return result;
        }

        private void SpawnObjectOnRandomGridPoint()
        {
            var gridPoint = _spheres[Random.Range(0, _spheres.Count)];
            var type = _types.ElementAt(Random.Range(0, _types.Count));
            var cube = Instantiate(type.Value);
            var id = cube.GetInstanceID();
            cube.name = id.ToString();

            var position = gridPoint.position;
            var x = position.x;
            var z = position.z;
            cube.transform.position = new Vector3(x, 0f, z);

            _gridData.GridObjectDictionary.Add(id, new GridObject()
            {
                id = id,
                type = type.Key,
                positionX = x,
                positionY = 0f,
                positionZ = z
            });
        }

        private void SelectObject()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            var raycast = Physics.Raycast(ray, out var raycastHit, 100) ? raycastHit : new RaycastHit();
            
            var raycastTransform = raycast.transform;
            if (raycastTransform == null)
            {
                return;
            }
            if (_selectedObject != null)
            {
                _selectedObject.GetComponent<MeshRenderer>().material.color = new Color(1, 0, 0);
                _selectedObject = null;
            }

            var meshRenderer = raycastTransform.GetComponent<MeshRenderer>();
            meshRenderer.material.color = new Color(1, 1, 1);
            
            _selectedObject = raycastTransform;
        }

        private void MoveSelectedObject()
        {
            if (_selectedObject == null)
            {
                return;
            }

            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            if (!_plane.Raycast(ray, out var enter))
            {
                return;
            }
            
            var worldPosition = ray.GetPoint(enter);
            var max = CalculateMaximumLength();
            
            if (worldPosition.x < 0f)
            {
                worldPosition.x = 0f;
            }
            if (worldPosition.x > max)
            {
                worldPosition.x = max;
            }
            
            if (worldPosition.z < 0f)
            {
                worldPosition.z = 0f;
            }
            if (worldPosition.z > max)
            {
                worldPosition.z = max;
            }
            
            _selectedObject.position = worldPosition;

            // var position = _selectedObject.position;
            // var mousePosition = Input.mousePosition;
            // var cameraPosition = _mainCamera.transform.position;
            //
            // var vector = new Vector3(mousePosition.x, mousePosition.y, cameraPosition.z + position.z);
            // var worldPosition = _mainCamera.ScreenToWorldPoint(vector);
            // worldPosition.y = 0f;
            //
            // var max = CalculateMaximumLength();
            // if (worldPosition.x < 0f)
            // {
            //     worldPosition.x = 0f;
            // }
            // if (worldPosition.x > max)
            // {
            //     worldPosition.x = max;
            // }
            //
            // if (worldPosition.z < 0f)
            // {
            //     worldPosition.z = 0f;
            // }
            // if (worldPosition.z > max)
            // {
            //     worldPosition.z = max;
            // }
            //         
            // _selectedObject.position = worldPosition;
        }

        private void SnapSelectedObject()
        {
            if (_selectedObject == null)
            {
                return;
            }
            
            var position = _selectedObject.position;
            var snapPosition = GetNearestGridPointFromPosition(position);
            _selectedObject.position = snapPosition;

            var oldGridObject = _gridData.GridObjectDictionary[_selectedObject.GetInstanceID()];
            _gridData.GridObjectDictionary[_selectedObject.GetInstanceID()] = oldGridObject
                .SetId(_selectedObject.GetInstanceID()).SetPosition(snapPosition.x, snapPosition.y, snapPosition.z);
        }
        
        private void DeleteSelectedObject()
        {
            if (_selectedObject == null)
            {
                return;
            }
            
            Destroy(_selectedObject.gameObject);
            _gridData.GridObjectDictionary.Remove(_selectedObject.GetInstanceID());
            
            _selectedObject = null;
        }

        private float CalculateMaximumLength()
        {
            return (float) (Math.Sqrt(_spheres.Count) - 1) * gridSize;
        }
    }
}

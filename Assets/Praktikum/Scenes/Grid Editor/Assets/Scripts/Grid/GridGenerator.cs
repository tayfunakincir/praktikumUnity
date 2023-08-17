using System.Collections.Generic;
using UnityEngine;

namespace Praktikum.Scenes.Grid_Editor.Assets.Scripts.Grid
{
    public class GridGenerator : MonoBehaviour
    {

        public Transform spherePrefab;
        public Transform jointPrefab;
        public Transform cubePrefab;
        
        public int length = 100;
        public int gridSize = 1;

        private Camera _mainCamera;
        private List<GridPoint> _gridPoints;
        private Transform _selectedObject;

        private bool _grab;

        private void Start()
        {
            Setup();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                SpawnObjectOnRandomGrid();
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

        private void Setup()
        {
            _mainCamera = Camera.main;
            _gridPoints = new List<GridPoint>();
            
            Generate();
        }

        private void Generate()
        {
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

                    var sphere = Instantiate(spherePrefab);
                    sphere.transform.position = new Vector3(x, 0f, z);
                    
                    _gridPoints.Add(new GridPoint()
                    {
                        PositionX = x,
                        PositionZ = z
                    });
                    
                    GenerateJoints(x, z, sphere);
                }
            }

            Debug.Log("Created a total of " + _gridPoints.Count + " spheres");
        }

        private void GenerateJoints(float xPosition, float zPosition, Transform parent)
        {
            var max = length - gridSize;
            var size = gridSize / 2f;
                    
            if (zPosition <= max && xPosition < max)
            {
                var zJoint = Instantiate(jointPrefab, parent, true);
                        
                zJoint.position = new Vector3(xPosition + size, 0f, zPosition);
                zJoint.rotation = Quaternion.Euler(0f, 0f, 90f);
                zJoint.localScale = new Vector3(0.5f, size, 0.5f);
            }
            if (xPosition <= max && zPosition < max)
            {
                var xJoint = Instantiate(jointPrefab, parent, true);
                        
                xJoint.position = new Vector3(xPosition, 0f, zPosition + size);
                xJoint.rotation = Quaternion.Euler(90f, 0f, 0f);
                xJoint.localScale = new Vector3(0.5f, size, 0.5f);
            }
        }

        private Vector3 GetNearestGridPointFromPosition(Vector3 position)
        {
            var max = length - gridSize;
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

        private void SpawnObjectOnRandomGrid()
        {
            var gridPoint = _gridPoints[Random.Range(0, _gridPoints.Count)];
            var cube = Instantiate(cubePrefab);

            cube.transform.position = new Vector3(gridPoint.PositionX, 0f, gridPoint.PositionZ);
        }

        private RaycastHit Raycast()
        {
            var ray = _mainCamera.ScreenPointToRay(Input.mousePosition);
            return Physics.Raycast(ray, out var raycastHit, 100) ? raycastHit : new RaycastHit();
        }

        private void SelectObject()
        {
            var raycast = Raycast();
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
            var position = _selectedObject.position;
            var mousePosition = Input.mousePosition;
            var cameraPosition = _mainCamera.transform.position;
            
            var vector = new Vector3(mousePosition.x, mousePosition.y, cameraPosition.z + position.z);
            var worldPosition = _mainCamera.ScreenToWorldPoint(vector);
                    
            _selectedObject.position = worldPosition;
        }

        private void SnapSelectedObject()
        {
            if (_selectedObject == null)
            {
                return;
            }
            
            var position = _selectedObject.position;
            _selectedObject.position = GetNearestGridPointFromPosition(position);
        }
        
        private void DeleteSelectedObject()
        {
            if (_selectedObject == null)
            {
                return;
            }
            
            Destroy(_selectedObject.gameObject);
            _selectedObject = null;
        }
    }
}

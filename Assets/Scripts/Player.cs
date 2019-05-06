using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class Player : MonoBehaviour {

    [SerializeField] float walkSpeed;
    [SerializeField] float runSpeed;
    [SerializeField] float rightStickSensitivity;
    [SerializeField] string placeTurret;
    [SerializeField] float placementDistance;
    [SerializeField] Vector3 placementOffset;
    [SerializeField] GameObject turretPrefab;

    private CharacterController CharacterController;
    Vector2 inputs;
    Vector2 rightInputs;
    MapGenerator mapGenerator;
    Vector2 centerOfScreen;

    private void Awake()
    {
        centerOfScreen = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    // Use this for initialization
    void Start () {
        CharacterController = GetComponent<CharacterController>();
        mapGenerator = MapGenerator.mapGenerator;
    }
	
	// Update is called once per frame
	void Update () {
        if (Input.GetButtonDown(placeTurret))
        {
            Ray ray = Camera.main.ScreenPointToRay(centerOfScreen);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, placementDistance))
            {
                if (hit.transform.gameObject.GetComponent<Tile>())
                {
                    PlaceCurrentTurret(hit);
                }
            }
        }
	}

    private void FixedUpdate()
    {
        ReadInputs();
        RotateTransformForward(rightInputs.x);
        Vector3 desiredMove = 
            transform.forward * inputs.y * walkSpeed + transform.right * inputs.x * walkSpeed;
        CharacterController.Move(desiredMove * Time.deltaTime);
    }

    void ReadInputs ()
    {
        inputs.x = Input.GetAxis("Horizontal");
        inputs.y = Input.GetAxis("Vertical");
        rightInputs.x = Input.GetAxis("R Horizontal");
        rightInputs.y = Input.GetAxis("R Vertical");
    }

    private void RotateTransformForward(float rotationHorizontal)
    {
        transform.rotation *= Quaternion.AngleAxis(rotationHorizontal * rightStickSensitivity, Vector3.up);
    }

    void PlaceCurrentTurret (RaycastHit hit)
    {
        Tile placementTile = mapGenerator.TileFromWorldPoint(hit.point);
        Vector3 placementLocation = placementTile.transform.position + placementOffset;
        GameObject placedTurret = Instantiate(turretPrefab);
        placedTurret.transform.position = placementLocation;
    }
}

using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public Camera cam;
    private InputAction pointer;
    private InputAction drop;
    Vector2 pointerPos;
    #if DEBUG
    public bool visualizeRays = false;
    public List<Vector3> rays;
    #endif
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (cam==null){
            cam = Camera.main;
        }
        pointer = InputSystem.actions.FindAction("Pointer");
        drop = InputSystem.actions.FindAction("Drop");
        drop.performed += context => RayFromCamera(context);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnDrawGizmos(){
        #if DEBUG
        if (visualizeRays){
            Gizmos.DrawLineList(rays.ToArray());
        }
        #endif
    }
    void RayFromCamera(InputAction.CallbackContext context){
        pointerPos = pointer.ReadValue<Vector2>();
        Ray ray = cam.ScreenPointToRay(pointerPos);
        #if DEBUG
        rays.Add(ray.origin);
        rays.Add(ray.origin+ray.direction);
        #endif
        LayerMask mask = LayerMask.GetMask("Game Grid");
        RaycastHit hit;
        bool backfacePhysics = Physics.queriesHitBackfaces;
        Physics.queriesHitBackfaces = true;
        bool raycastResult = Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask:mask);
        Physics.queriesHitBackfaces = backfacePhysics;
        if (raycastResult){
            Vector3 localHitPoints = hit.transform.worldToLocalMatrix* hit.point;
            print("Intersected Plane");
            hit.transform.parent.BroadcastMessage("ClickGrid", hit.textureCoord);
        }
    }
    
}

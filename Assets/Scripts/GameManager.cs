using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;

public class GameManager : MonoBehaviour
{
    public DroneController _DroneController;
    public Button _FlyButton;
    public Button _LandButton;

    public GameObject _MyControls;

    public Slider _ElevateSlider;
    // public GameObject _Drone;
   

    // AR Raycast
    public ARRaycastManager _RaycastManager;
    public ARPlaneManager _PlaneManager;
    List<ARRaycastHit> _HitResults = new List<ARRaycastHit>();
   
    struct DroneAnimationControls{
        public bool _moving;
        public bool _interpolatingAsc;
        public bool _interpolatingDesc;
        public float _axis;
        public float _direction;
    }

    DroneAnimationControls _MovingLeft;
    DroneAnimationControls _MovingBack;


    // Start is called before the first frame update
    void Start()
    {
        _FlyButton.onClick.AddListener(EventOnClickFlyButton);
        _LandButton.onClick.AddListener(EventOnClickLandButton);
    }

    // Update is called once per frames
    void Update()
    {
        // Keyboard controls
        // float speedX = Input.GetAxis("Horizontal");
        // float speedZ = Input.GetAxis("Vertical");
        
        UpdateControls(ref _MovingLeft);
        UpdateControls(ref _MovingBack);
        
        _ElevateSlider.value = _DroneController.transform.localPosition.y;

        _DroneController.Move(_MovingLeft._axis * _MovingLeft._direction, _MovingBack._axis * _MovingBack._direction);

        if(_DroneController.isIdle()){
            UpdateAR();
        }
    }

    void UpdateAR(){
        Vector2 positionScreenSpace = Camera.current.ViewportToScreenPoint(new Vector2(0.5f, 0.5f));
        _RaycastManager.Raycast(positionScreenSpace, _HitResults, UnityEngine.XR.ARSubsystems.TrackableType.PlaneWithinBounds);

        if(_HitResults.Count > 0){
            if(_PlaneManager.GetPlane(_HitResults[0].trackableId).alignment == UnityEngine.XR.ARSubsystems.PlaneAlignment.HorizontalUp){
                Pose pose = _HitResults[0].pose;
                _DroneController.transform.position = pose.position;
                _DroneController.gameObject.SetActive(true);
            }
        }
    }

    void UpdateControls(ref DroneAnimationControls _controls){
        if(_controls._moving || _controls._interpolatingAsc || _controls._interpolatingDesc){
            if(_controls._interpolatingAsc){
                _controls._axis += 0.05f;
                if(_controls._axis >= 1.0f){
                    _controls._axis = 1.0f;
                    _controls._interpolatingAsc = false;
                    _controls._interpolatingDesc = true;
                }
            }
            else if(!_controls._moving){
                _controls._axis -= 0.05f;
                if(_controls._axis <= 0.0f){
                    _controls._axis = 0.0f;
                    _controls._interpolatingDesc = false;
                } 
            }
        }
    }
  
    
    public void OnDrag(){
        Vector3 _droneElevation = _DroneController.transform.position;
        _droneElevation.y = _ElevateSlider.value;
        _DroneController.transform.localPosition = _droneElevation;
    }


    void EventOnClickFlyButton(){
        if(_DroneController.isIdle()){
            _DroneController.TakeOff();

            _FlyButton.gameObject.SetActive(false);
            _LandButton.gameObject.SetActive(true);
            
            _MyControls.SetActive(true);
            
        }
    }

    void EventOnClickLandButton(){
        if(_DroneController.isFlying()){
            _DroneController.Land();
     
            _LandButton.gameObject.SetActive(false);
            _FlyButton.gameObject.SetActive(true);
            
            _MyControls.SetActive(false);
        }
    }
    
    public void EventLeftButtonPressed(){
        _MovingLeft._moving = true;
        _MovingLeft._interpolatingAsc = true;
        _MovingLeft._direction = -1.0f;
    }
    public void EventLeftButtonReleased(){
        
        _MovingLeft._moving = false;
    }
    
    public void EventRightButtonPressed(){
        _MovingLeft._moving = true;
        _MovingLeft._interpolatingAsc = true;
        _MovingLeft._direction = 1.0f;
    }
    public void EventRightButtonReleased(){
        _MovingLeft._moving = false;
    }
    
    public void EventForwardButtonPressed(){
        _MovingBack._moving = true;
        _MovingBack._interpolatingAsc = true;
        _MovingBack._direction = 1.0f;
    }
    public void EventForwardButtonReleased(){
        _MovingBack._moving = false;
    }
    
    public void EventBackwardButtonPressed(){
        _MovingBack._moving = true;
        _MovingBack._interpolatingAsc = true;
        _MovingBack._direction = -1.0f;
    }
    public void EventBackwardButtonReleased(){
        _MovingBack._moving = false;
    }
    
}

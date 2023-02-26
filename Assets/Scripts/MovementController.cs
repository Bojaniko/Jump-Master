using Studio28.Utility;

using UnityEngine;

using JumpMaster.Controls;

namespace JumpMaster.LevelControllers
{
    public enum MovementState { STILL, JUMPING, DASHING, HANGING, FLOATING, FALLING };

    public class MovementController : LevelControllerBase
    {
        // ##### EVENTS #####

        public delegate void DashEventController(Vector3 dash_pivot_position, Quaternion direction);
        public event DashEventController OnDash;

        public delegate void JumpEventController(Vector3 jump_pivot_position);
        public event JumpEventController OnJump;

        // ##### SINGLETON #####

        private static MovementController s_instance;
        public static MovementController Instance
        {
            get
            {
                return s_instance;
            }
            private set
            {
                if (s_instance == null)
                    s_instance = value;
                else
                    Debug.LogError("There can only be one Movement Controller in the scene!");
            }
        }

        protected override void Initialize()
        {
            Instance = this;

            StateController = new("Movement", MovementState.STILL);

            _rigidbody = LevelController.Instance.PlayerGameObject.GetComponent<Rigidbody>();

            _bounds = LevelController.Instance.PlayerGameObject.GetComponent<BoxCollider>();

            _hangFromScreenWidth = Screen.width - HangingStickDistanceScreen;

            Restart();
        }

        protected override void Pause()
        {
            DisableInput();

            StateController.SetState(MovementState.STILL);
        }

        protected override void Unpause()
        {
            if (_jumpChain > 0)
                _jumpChainPenaltyTime += LevelController.LastPauseTime;

            if (_dashChain > 0)
                _dashChainPenaltyTime += LevelController.LastPauseTime;

            EnableInput();

            StateController.SetState(StateController.DeltaState);
        }

        protected override void PlayerDeath()
        {
            DisableInput();
        }

        protected override void Restart()
        {
            StateController.SetState(MovementState.STILL);

            _jumpChain = 0;
            _jumpChainPenaltyTime = -JumpChainPenaltyDuration;

            _dashChain = 0;
            _dashChainPenaltyTime = -DashChainPenaltyDuration;

            Vector3 startPosition = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width * 0.5f, 0f, Z_Position));
            startPosition.y += (_bounds.bounds.max.y - _bounds.bounds.min.y) * 0.5f;
            LevelController.Instance.PlayerGameObject.transform.position = startPosition;
        }

        private void EnableInput()
        {
            if (SwipeDetector.Instance != null)
            {
                SwipeDetector.Instance.OnSwipeDetected += SwipeInput;
            }

            if (InputController.Instance != null)
            {
                InputController.Instance.OnTap += TapInput;
                InputController.Instance.OnHoldStarted += StartJumpCharge;
                InputController.Instance.OnHoldPerformed += PerformJumpCharge;
                InputController.Instance.OnHoldCancelled += CancelJumpCharge;
            }
        }

        private void DisableInput()
        {
            if (SwipeDetector.Instance != null)
            {
                SwipeDetector.Instance.OnSwipeDetected -= SwipeInput;
            }

            if (InputController.Instance != null)
            {
                InputController.Instance.OnTap -= TapInput;
                InputController.Instance.OnHoldStarted -= StartJumpCharge;
                InputController.Instance.OnHoldPerformed -= PerformJumpCharge;
                InputController.Instance.OnHoldCancelled -= CancelJumpCharge;
            }
        }

        // ##### REFFERENCES #####

        public (Vector2 min, Vector2 max) BoundsScreenPosition { get; private set; }
        private BoxCollider _bounds;

        public StateMachine<MovementState> StateController { get; private set; }

        private Rigidbody _rigidbody;

        // ##### ATTRIBUTES #####

        [Range(0f, 20f)]
        public float Z_Position = 3f;

        // ##### DASH ATTRIBUTES #####

        [Header("Dashing")]
        [Range(1f, 50f)]
        public float DashForce = 10f;
        [Range(0.5f, 10f)]
        public float DashDistance = 2.5f;

        [Range(1, 10)]
        public int MaxDashChain = 2;
        [Range(0.5f, 10f)]
        public float DashChainPenaltyDuration = 2f;
        [Range(0.1f, 1f)]
        public float MinDashChainDistance = 0.5f;
        [Range(0f, 1f)]
        public float DashCrossChainVelocity = 0.5f;

        // ##### HANG ATTRIBUTES #####

        [Header("Hanging")]
        [Range(0.5f, 10f)]
        public float MinHangingDuration = 0.5f;
        [Range(0, 100)]
        public int HangingStickDistanceScreen = 20;
        [Range(0f, 2f)]
        public float MinHangingStickVelocity = 1f;

        // ##### JUMP ATTRIBUTES #####

        [Header("Jumping")]
        [Range(0.5f, 10f)]
        public float JumpHeight;
        [Range(2f, 50f)]
        public float JumpForce;

        [Range(1, 10)]
        public int MaxJumpChain = 3;
        [Range(0.5f, 10f)]
        public float JumpChainPenaltyDuration = 1.5f;
        [Range(0.1f, 1f)]
        public float MinJumpChainHeight = 0.5f;
        [Range(0.1f, 1f)]
        public float MaxJumpDashChainDistance = 0.9f;
        [Range(0f, 1f)]
        public float JumpCrossChainVelocity = 0.5f;

        [Header("Charged Jumping")]
        [Range(0.5f, 10f)]
        public float ChargedJumpHeight;
        [Range(2f, 50f)]
        public float ChargedJumpForce;
        [Range(0.1f, 1f)]
        public float MinChargeDuration;
        [Range(0.5f, 2f)]
        public float MaxChargeDuration;

        // ##### FLOAT ATTRIBUTES #####

        [Header("Floating")]
        [Range(0.1f, 2f)]
        public float FloatingForce = 0.5f;
        [Range(0.1f, 2f)]
        public float FloatingDuration = 1f;

        // ##### VARIABLES ######

        private int _verticalDirection = 0;
        private int _horizontalDirection = 0;

        // # DASH

        private float _dashDistancePercentage = 0f;
        private Vector3 _dashStartPosition;
        private Vector3 _dashVectorDirection;

        private int _dashChain = 0;
        private float _dashChainPenaltyTime = 0f;

        // # HANG

        private int _hangDirection = 0;
        private float _hangStartTime = 0f;

        private float _hangFromScreenWidth;

        // # FLOAT

        private float _floatStartTime = 0f;

        // # JUMP

        private int _jumpChain = 0;
        private float _jumpChainPenaltyTime = 0f;

        private float _jumpForce = 1f;
        private float _jumpHeight = 1f;
        private float _jumpStartHeight = 0f;

        private Vector3 _jumpDirection;

        private bool _jumpChargeStarted = false;
        private float _jumpChargeStartTime = 0f;

        // ##### PUBLIC PROPERTIES #####

        public int JumpChain
        {
            get
            {
                return _jumpChain;
            }
        }

        public int DashChain
        {
            get
            {
                return _dashChain;
            }
        }

        public int HorizontalDirection
        {
            get
            {
                return _horizontalDirection;
            }
        }

        // ##### MOVEMENT CHECKS #####

        public bool CanJump
        {
            get
            {
                if (_jumpChain >= MaxJumpChain) return false;

                if (StateController.CurrentState.Equals(MovementState.HANGING))
                    return false;

                if (StateController.CurrentState.Equals(MovementState.DASHING))
                {
                    if (_dashDistancePercentage < MinDashChainDistance)
                        return false;
                }

                // Allow the player to chain a jump if the current height from the start of the current jump is at a minimum percentage
                if (StateController.CurrentState.Equals(MovementState.JUMPING))
                {
                    if ((LevelController.Instance.PlayerGameObject.transform.position.y - _jumpStartHeight) / _jumpHeight < MinJumpChainHeight)
                        return false;
                }

                return true;
            }
        }

        public bool CanHang(int direction)
        {
            if (direction == 0)
                return false;

            if (_jumpChargeStarted)
                return false;

            if (!StateController.CurrentState.Equals(MovementState.DASHING))
            {
                if (StateController.CurrentState.Equals(MovementState.JUMPING))
                {
                    if (_jumpDirection.Equals(Vector3.up))
                        return false;
                } else if (!StateController.CurrentState.Equals(MovementState.FALLING))
                    return false;
            }

            if (direction == -1)
            {
                if (_rigidbody.velocity.x < -MinHangingStickVelocity)
                {
                    if (BoundsScreenPosition.min.x > HangingStickDistanceScreen)
                        return false;
                }
                else
                {
                    if (BoundsScreenPosition.min.x >= 0)
                        return false;
                }
            }

            if (direction == 1)
            {
                if (_rigidbody.velocity.x > MinHangingStickVelocity)
                {
                    if (BoundsScreenPosition.max.x < _hangFromScreenWidth)
                        return false;
                }
                else
                {
                    if (BoundsScreenPosition.max.x <= Screen.width)
                        return false;
                }
            }

            return true;
        }

        public bool CanDash(int direction)
        {
            if (direction == 0)
                return false;

            if (_jumpChargeStarted)
                return false;

            if (CameraController.Instance.AscendingStarted == false) return false;

            if (_dashChain >= MaxDashChain) return false;

            if (StateController.CurrentState.Equals(MovementState.HANGING))
            {
                if (Time.time - _hangStartTime < MinHangingDuration)
                    return false;
                if (_hangDirection == direction)
                    return false;
            }

            if (StateController.CurrentState.Equals(MovementState.JUMPING))
            {
                if ((LevelController.Instance.PlayerGameObject.transform.position.y - _jumpStartHeight) / _jumpHeight < MinJumpChainHeight)
                    return false;
            }

            if (StateController.CurrentState.Equals(MovementState.DASHING))
            {
                if (_dashDistancePercentage < MinDashChainDistance)
                    return false;
                if (_horizontalDirection != direction)
                    return false;
            }

            return true;
        }

        // ##### MOVEMENT PHYSICS #####

        private void Update()
        {
            if (LevelController.Paused)
                return;

            if (_jumpChain > 0 && _jumpChargeStarted == false && Time.time - _jumpChainPenaltyTime > JumpChainPenaltyDuration)
                _jumpChain = 0;

            if (_dashChain > 0 && Time.time - _dashChainPenaltyTime > DashChainPenaltyDuration)
                _dashChain = 0;

            BoundsScreenPosition = GetBoundsScreenPosition();
        }

        private void FixedUpdate()
        {
            switch (StateController.CurrentState)
            {
                case MovementState.STILL:
                    _rigidbody.velocity = Vector3.zero;
                    _rigidbody.useGravity = false;
                    break;

                case MovementState.FALLING:
                    if (CanHang(_horizontalDirection))
                        StartHang(_horizontalDirection);
                    break;

                case MovementState.JUMPING:
                    if (CanHang(_horizontalDirection))
                    {
                        StartHang(_horizontalDirection);
                        return;
                    }
                    Jump();
                    if (_rigidbody.velocity.y <= FloatingForce) EndJump();
                    break;

                case MovementState.FLOATING:
                    Float();
                    if (_jumpChargeStarted == false && Time.time - _floatStartTime >= FloatingDuration)
                        StartFall();
                    break;

                case MovementState.DASHING:
                    if (CanHang(_horizontalDirection))
                    {
                        StartHang(_horizontalDirection);
                        return;
                    }
                    Dash();
                    if (_dashDistancePercentage >= 1f) EndDash();
                    break;
            }
        }
        
        // ##### DASH FUNCTIONS #####

        private void StartDash(int direction)
        {
            _rigidbody.useGravity = false;

            _dashChain++;
            _dashChainPenaltyTime = Time.time;
            _horizontalDirection = direction;
            _dashStartPosition = LevelController.Instance.PlayerGameObject.transform.position;

            _dashVectorDirection = Vector3.right * _horizontalDirection;
            if (StateController.CurrentState.Equals(MovementState.JUMPING)
                || StateController.CurrentState.Equals(MovementState.FLOATING)
                || StateController.CurrentState.Equals(MovementState.HANGING))
                _dashVectorDirection += Vector3.up * DashCrossChainVelocity;

            StateController.SetState(MovementState.DASHING);

            if (OnDash != null)
            {
                Vector3 pivot = new Vector3(_bounds.bounds.min.x, LevelController.Instance.PlayerGameObject.transform.position.y, LevelController.Instance.PlayerGameObject.transform.position.z);
                Quaternion dash_direction = Quaternion.LookRotation(Vector3.right * _horizontalDirection, Vector3.up);
                OnDash(pivot, dash_direction);
            }
        }

        private void Dash()
        {
            _dashDistancePercentage = Mathf.Abs(_dashStartPosition.x - LevelController.Instance.PlayerGameObject.transform.position.x) / DashDistance;

            if (_dashDistancePercentage < MinDashChainDistance)
                _rigidbody.velocity = Vector3.Lerp(_dashVectorDirection * DashForce, Vector3.zero, _dashDistancePercentage);
            else
                _rigidbody.velocity = Vector3.Lerp(_dashVectorDirection * DashForce, Physics.gravity, _dashDistancePercentage);
        }

        private void EndDash()
        {
            StartFall();
        }

        // ##### HANG FUNCTIONS #####

        private void StartHang(int direction)
        {
            _rigidbody.useGravity = false;
            _rigidbody.velocity = Vector3.zero;

            _hangDirection = direction;
            _hangStartTime = Time.time;

            Vector3 hang_position = Vector3.zero;
            if (_hangDirection == 1)
                hang_position = Camera.main.ScreenToWorldPoint(new Vector2(Screen.width - ((BoundsScreenPosition.max.x - BoundsScreenPosition.min.x) / 2), 0));
            if (_hangDirection == -1)
                hang_position = Camera.main.ScreenToWorldPoint(new Vector2((BoundsScreenPosition.max.x - BoundsScreenPosition.min.x) / 2, 0));
            hang_position.y = LevelController.Instance.PlayerGameObject.transform.position.y;
            hang_position.z = LevelController.Instance.PlayerGameObject.transform.position.z;
            LevelController.Instance.PlayerGameObject.transform.position = hang_position;

            StateController.SetState(MovementState.HANGING);
        }

        // ##### JUMP FUNCTIONS #####

        private void StartJump()
        {
            _rigidbody.useGravity = false;

            _verticalDirection = 1;

            _jumpChargeStarted = false;

            _jumpChain++;
            _jumpChainPenaltyTime = Time.time;
            _jumpStartHeight = LevelController.Instance.PlayerGameObject.transform.position.y;

            _jumpDirection = Vector3.up;
            if (StateController.CurrentState.Equals(MovementState.DASHING) && _dashDistancePercentage < MaxJumpDashChainDistance)
                _jumpDirection += Vector3.right * _horizontalDirection * JumpCrossChainVelocity;

            if (StateController.CurrentState.Equals(MovementState.FALLING) && _horizontalDirection != 0)
                _jumpDirection += Vector3.right * _horizontalDirection * JumpCrossChainVelocity;

            StateController.SetState(MovementState.JUMPING);

            if (OnJump != null)
            {
                Vector3 pivot = LevelController.Instance.PlayerGameObject.transform.position;
                pivot.y = _bounds.bounds.min.y;
                OnJump(pivot);
            }
        }

        private void Jump()
        {
            float heightPercentage = (LevelController.Instance.PlayerGameObject.transform.position.y - _jumpStartHeight) / _jumpHeight;
            _rigidbody.velocity = Vector3.Lerp(_jumpDirection * _jumpForce, Vector3.zero, heightPercentage);
        }

        private void EndJump()
        {
            if (_jumpDirection.Equals(Vector3.up) && _jumpForce == JumpForce)
            {
                StartFloat();
                return;
            }
            StartFall();
        }

        // ##### FLOAT FUNCTIONS #####

        private void StartFloat()
        {
            _floatStartTime = Time.time;
            _rigidbody.useGravity = false;

            StateController.SetState(MovementState.FLOATING);
        }

        private void Float()
        {
            _rigidbody.velocity = Vector2.up * _verticalDirection * FloatingForce;
        }

        // ##### FALL FUNCTIONS #####

        private void StartFall()
        {
            _rigidbody.useGravity = true;

            StateController.SetState(MovementState.FALLING);
        }

        // ##### INPUT HANDLING #####

        private void TapInput()
        {
            if (CanJump)
            {
                _jumpHeight = JumpHeight;
                _jumpForce = JumpForce;
                StartJump();
            }
        }

        private void StartJumpCharge(Vector2 position, float min_hold_duration)
        {
            if (!CanJump)
                return;

            _verticalDirection = -1;

            _jumpChargeStarted = true;
            _jumpChargeStartTime = Time.time + min_hold_duration;

            StateController.SetState(MovementState.FLOATING);
        }

        private void PerformJumpCharge()
        {
            if (!_jumpChargeStarted)
                return;

            _jumpChargeStarted = false;

            float durationPercentage = (Time.time - _jumpChargeStartTime) / MaxChargeDuration;

            if (durationPercentage == 0f)
                return;

            durationPercentage = Mathf.Clamp(durationPercentage, MinChargeDuration / MaxChargeDuration, 1f);

            _jumpHeight = ChargedJumpHeight * durationPercentage;
            _jumpForce = ChargedJumpForce * durationPercentage;
            StartJump();
        }

        private void CancelJumpCharge()
        {
            _jumpChargeStarted = false;
        }

        private void SwipeInput(SwipeDirection direction)
        {
            int dir = 0;
            if (direction.Equals(SwipeDirection.LEFT)) dir = -1;
            if (direction.Equals(SwipeDirection.RIGHT)) dir = 1;

            if (CanDash(dir))
                StartDash(dir);
        }

        private (Vector2 min, Vector2 max) GetBoundsScreenPosition()
        {
            Vector2 min = Camera.main.WorldToScreenPoint(_bounds.bounds.min);
            Vector3 max = Camera.main.WorldToScreenPoint(_bounds.bounds.max);
            return (min, max);
        }
    }
}
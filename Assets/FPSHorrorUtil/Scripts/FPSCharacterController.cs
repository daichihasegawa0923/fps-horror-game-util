using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondGame.FPSHorrorUtil
{
    /// <summary>
    /// ??????????????MonoBehaviour
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FPSCharacterController : MonoBehaviour
    {

        public FPSCharacterStatus Status { get => _status; set => _status = value; }
        [SerializeField]
        protected FPSCharacterStatus _status;

        /// <summary>
        /// ????????
        /// </summary>
        [SerializeField]
        protected Camera _perspectiveCamera;

        /// <summary>
        /// ?????
        /// </summary>
        public CharacterController CharacterController { get => _characterControler; }

        /// <summary>
        /// ?????
        /// </summary>
        protected CharacterController _characterControler;

        /// <summary>
        /// ?????
        /// </summary>
        protected float _currentGravity = 0.0f;

        /// <summary>
        /// ????????????
        /// </summary>
        [SerializeField]
        protected List<KeyCode> _dashKeyCodes;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _characterControler = GetComponent<CharacterController>();
            Cursor.lockState = Status._isCursorLock ? CursorLockMode.Locked : CursorLockMode.None;
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            MoveByController();
        }

        protected virtual void FixedUpdate()
        {
            ExecuteGravity();
        }

        /// <summary>
        /// ????
        /// </summary>
        /// <returns>?????????????</returns>
        public virtual bool IsFoot
        {
            get
            {
                var boxRays = Physics.BoxCastAll(transform.position, Vector3.one, Vector3.down, Quaternion.identity, Status._footLength);
                foreach (var boxRay in boxRays)
                {
                    if (boxRay.collider.gameObject != gameObject) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// ?????
        /// </summary>
        protected virtual void ExecuteGravity()
        {
            if (IsFoot)
            {
                _currentGravity = 0.0f;
                return;
            }

            if (_currentGravity < Status._maxGravityPower) _currentGravity = _currentGravity == 0.0f
                     ? _currentGravity += Status._gravityPower
                     : _currentGravity *= (1 + Status._gravityPower);
            var gravitySpeed = Vector3.zero;
            gravitySpeed.y -= _currentGravity;
            CharacterController.Move(gravitySpeed);
        }

        /// <summary>
        /// ????????
        /// </summary>
        public bool IsDash
        {
            get
            {
                foreach (var keyCode in _dashKeyCodes)
                {
                    if (Input.GetKey(keyCode)) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// ???????????
        /// </summary>
        protected virtual void MoveByController()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var mouseX = Input.GetAxis("Mouse X") * Status._mouseSensivity;
            var mouseY = Input.GetAxis("Mouse Y") * Status._mouseSensivity;

            CharacterController.Move((transform.forward * vertical + transform.right * horizontal) * Status._moveSpeed * (IsDash ? Status._dashRate : 1));

            var spin = Vector3.zero;
            spin.y += mouseX;
            transform.eulerAngles += spin;

            var cameraSpin = Vector3.zero;
            cameraSpin.x -= mouseY;
            _perspectiveCamera.transform.localEulerAngles += cameraSpin;

            // ???????????????????????????????
            var cameraAdjustSpin = _perspectiveCamera.transform.localEulerAngles;
            Debug.Log(cameraAdjustSpin.x);
            if (cameraAdjustSpin.x > 70 && cameraAdjustSpin.x < 180) cameraAdjustSpin.x = 70;
            if (cameraAdjustSpin.x >= 180 && cameraAdjustSpin.x < 290) cameraAdjustSpin.x = 290;

            _perspectiveCamera.transform.localEulerAngles = cameraAdjustSpin;
        }
    }

    [Serializable]
    public class FPSCharacterStatus
    {
        /// <summary>
        /// ????
        /// </summary>
        [SerializeField]
        public float _moveSpeed = 1.0f;

        /// <summary>
        /// ??????????
        /// </summary>
        [SerializeField]
        public float _mouseSensivity = 1;

        /// <summary>
        /// ?????
        /// </summary>
        [SerializeField]
        public float _gravityPower = 0.01f;

        /// <summary>
        /// ??????????
        /// </summary>
        [SerializeField]
        public float _maxGravityPower = 0.1f;

        /// <summary>
        /// ????
        /// </summary>
        [SerializeField]
        public float _footLength = 0.8f;

        /// <summary>
        /// ??????????
        /// </summary>
        [SerializeField]
        public float _dashRate = 3;

        /// <summary>
        /// ?????????????
        /// </summary>
        [SerializeField]
        public bool _isCursorLock = true;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondGame.FPSHorrorUtil
{
    /// <summary>
    /// 一人称視点のホラーゲーム用のMonoBehaviour
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FPSCharacterController : MonoBehaviour
    {
        /// <summary>
        /// 主観となるカメラ
        /// </summary>
        [SerializeField]
        protected Camera _perspectiveCamera;

        /// <summary>
        /// 移動速度
        /// </summary>
        [SerializeField]
        protected float _moveSpeed = 1.0f;

        /// <summary>
        /// カメラのセンシビティ
        /// </summary>
        [SerializeField]
        protected float _mouseSensivity = 1;

        /// <summary>
        /// キャラコン
        /// </summary>
        public CharacterController CharacterController { get => _characterControler; }
        
        /// <summary>
        /// キャラコン
        /// </summary>
        protected CharacterController _characterControler;

        /// <summary>
        /// カーソルをロックするか否か
        /// </summary>
        [SerializeField]
        protected bool _isCursorLock = true;

        /// <summary>
        /// 重力の強さ
        /// </summary>
        [SerializeField]
        protected float _gravityPower = 0.01f;
        
        /// <summary>
        /// 重力位の強さの最大値
        /// </summary>
        [SerializeField]
        protected float _maxGravityPower = 0.1f;

        /// <summary>
        /// 現在の重力
        /// </summary>
        protected float _currentGravity = 0.0f;

        /// <summary>
        /// 足の長さ
        /// </summary>
        [SerializeField]
        protected float _footLength = 0.8f;

        /// <summary>
        /// 走った時の速度の倍率
        /// </summary>
        [SerializeField]
        protected float _dashRate = 3;

        /// <summary>
        /// ダッシュを行うキーコード
        /// </summary>
        [SerializeField]
        protected List<KeyCode> _dashKeyCodes;

        // Start is called before the first frame update
        protected virtual void Start()
        {
            _characterControler = GetComponent<CharacterController>();
            Cursor.lockState = _isCursorLock ? CursorLockMode.Locked : CursorLockMode.None;
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
        /// 設置判定
        /// </summary>
        /// <returns>足が地面についているか否か</returns>
        public virtual bool IsFoot
        {
            get
            {
                var boxRays = Physics.BoxCastAll(transform.position, Vector3.one, Vector3.down, Quaternion.identity, _footLength);
                foreach (var boxRay in boxRays)
                {
                    if (boxRay.collider.gameObject != gameObject) return true;
                }

                return false;
            }
        }

        /// <summary>
        /// 重力の実行
        /// </summary>
        protected virtual void ExecuteGravity()
        {
            if (IsFoot)
            {
                _currentGravity = 0.0f;
                return;
            }

            if(_currentGravity < _maxGravityPower) _currentGravity = _currentGravity == 0.0f
                    ? _currentGravity += _gravityPower 
                    : _currentGravity *= (1 + _gravityPower);
            var gravitySpeed = Vector3.zero;
            gravitySpeed.y -= _currentGravity;
            CharacterController.Move(gravitySpeed);
        }

        /// <summary>
        /// 走っているか否か
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
        /// コントロールによる制御
        /// </summary>
        protected virtual void MoveByController()
        {
            var horizontal = Input.GetAxis("Horizontal");
            var vertical = Input.GetAxis("Vertical");

            var mouseX = Input.GetAxis("Mouse X") * _mouseSensivity;
            var mouseY = Input.GetAxis("Mouse Y") * _mouseSensivity;

            CharacterController.Move((transform.forward * vertical + transform.right * horizontal) * _moveSpeed * (IsDash ? _dashRate : 1));

            var spin = Vector3.zero;
            spin.y += mouseX;
            transform.eulerAngles += spin;

            var cameraSpin = Vector3.zero;       
            cameraSpin.x -= mouseY;
            _perspectiveCamera.transform.localEulerAngles += cameraSpin;

            // カメラの回転が意図せず上下反転する場合があるので、制限をかける
            var cameraAdjustSpin = _perspectiveCamera.transform.localEulerAngles;
            Debug.Log(cameraAdjustSpin.x);
            if (cameraAdjustSpin.x > 70 && cameraAdjustSpin.x < 180) cameraAdjustSpin.x = 70;
            if (cameraAdjustSpin.x >= 180 && cameraAdjustSpin.x <290) cameraAdjustSpin.x = 290;

            _perspectiveCamera.transform.localEulerAngles = cameraAdjustSpin;
        }
    }
}
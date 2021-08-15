using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiamondGame.FPSHorrorUtil
{
    /// <summary>
    /// ��l�̎��_�̃z���[�Q�[���p��MonoBehaviour
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class FPSCharacterController : MonoBehaviour
    {
        /// <summary>
        /// ��ςƂȂ�J����
        /// </summary>
        [SerializeField]
        protected Camera _perspectiveCamera;

        /// <summary>
        /// �ړ����x
        /// </summary>
        [SerializeField]
        protected float _moveSpeed = 1.0f;

        /// <summary>
        /// �J�����̃Z���V�r�e�B
        /// </summary>
        [SerializeField]
        protected float _mouseSensivity = 1;

        /// <summary>
        /// �L�����R��
        /// </summary>
        public CharacterController CharacterController { get => _characterControler; }
        
        /// <summary>
        /// �L�����R��
        /// </summary>
        protected CharacterController _characterControler;

        /// <summary>
        /// �J�[�\�������b�N���邩�ۂ�
        /// </summary>
        [SerializeField]
        protected bool _isCursorLock = true;

        /// <summary>
        /// �d�͂̋���
        /// </summary>
        [SerializeField]
        protected float _gravityPower = 0.01f;
        
        /// <summary>
        /// �d�͈ʂ̋����̍ő�l
        /// </summary>
        [SerializeField]
        protected float _maxGravityPower = 0.1f;

        /// <summary>
        /// ���݂̏d��
        /// </summary>
        protected float _currentGravity = 0.0f;

        /// <summary>
        /// ���̒���
        /// </summary>
        [SerializeField]
        protected float _footLength = 0.8f;

        /// <summary>
        /// ���������̑��x�̔{��
        /// </summary>
        [SerializeField]
        protected float _dashRate = 3;

        /// <summary>
        /// �_�b�V�����s���L�[�R�[�h
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
        /// �ݒu����
        /// </summary>
        /// <returns>�����n�ʂɂ��Ă��邩�ۂ�</returns>
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
        /// �d�͂̎��s
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
        /// �����Ă��邩�ۂ�
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
        /// �R���g���[���ɂ�鐧��
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

            // �J�����̉�]���Ӑ}�����㉺���]����ꍇ������̂ŁA������������
            var cameraAdjustSpin = _perspectiveCamera.transform.localEulerAngles;
            Debug.Log(cameraAdjustSpin.x);
            if (cameraAdjustSpin.x > 70 && cameraAdjustSpin.x < 180) cameraAdjustSpin.x = 70;
            if (cameraAdjustSpin.x >= 180 && cameraAdjustSpin.x <290) cameraAdjustSpin.x = 290;

            _perspectiveCamera.transform.localEulerAngles = cameraAdjustSpin;
        }
    }
}
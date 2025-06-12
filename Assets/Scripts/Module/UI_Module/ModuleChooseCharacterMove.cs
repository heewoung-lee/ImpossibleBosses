using NetWork.NGO;
using UnityEngine;
using UnityEngine.UI;

namespace Module.UI_Module
{
    public enum SelectDirection
    {
        LeftClick = -1,
        RightClick = 1
    }


    public class ModuleChooseCharacterMove : MonoBehaviour
    {

        private const int Movevalue = 4;
        private int _currentSelectCharactorIndex = 0;
        private Transform _chooseCameraTr;
        private CharacterSelectorNgo _characterSelectorNgo;

        private Button _previousButton;
        private Button _nextButton;

        private int _playerChooseIndex;

        public int PlayerChooseIndex => _playerChooseIndex;

        public CharacterSelectorNgo CharacterSelectorNgo
        {
            get
            {
                if(_characterSelectorNgo == null)
                {
                    _characterSelectorNgo = GetComponent<CharacterSelectorNgo>();
                }

                return _characterSelectorNgo;
            }
        }


        public Button PreviousButton
        {
            get
            {
                if(_previousButton == null)
                {
                    _previousButton = CharacterSelectorNgo.PreViousButton;
                }
                return _previousButton;
            }
        }

        public Button NextButton
        {
            get
            {
                if (_nextButton == null)
                {
                    _nextButton = CharacterSelectorNgo.NextButton;
                }
                return _nextButton;
            }
        }
        private void Start()
        {
            NextButton.onClick.AddListener(() => MoveSelectCamera(SelectDirection.RightClick));
            PreviousButton.onClick.AddListener(() => MoveSelectCamera(SelectDirection.LeftClick));
        }

        public void MoveSelectCamera(SelectDirection direction)
        {
            int index = _currentSelectCharactorIndex;
            if (direction == SelectDirection.LeftClick)
            {
                _currentSelectCharactorIndex--;
                _currentSelectCharactorIndex = Mathf.Clamp(_currentSelectCharactorIndex, 0, 4);
            }
            else
            {
                _currentSelectCharactorIndex++;
                _currentSelectCharactorIndex = Mathf.Clamp(_currentSelectCharactorIndex, 0, 4);
            }
            if(index != _currentSelectCharactorIndex)
            {
                CharacterSelectorNgo.SetCameraPositionServerRpc((int)direction * Vector3.right * Movevalue,CharacterSelectorNgo.CameraOperation.Add);
                _playerChooseIndex = _currentSelectCharactorIndex;
            }
        }
    }
}
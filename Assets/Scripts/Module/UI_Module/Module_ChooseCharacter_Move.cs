using JetBrains.Annotations;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public enum SelectDirection
{
    LeftClick = -1,
    RightClick = 1
}


public class Module_ChooseCharacter_Move : MonoBehaviour
{

    private const int MOVEVALUE = 4;
    private int _currentSelectCharactorIndex = 0;
    private Transform _chooseCameraTr;
    private CharacterSelectorNGO _characterSelectorNGO;

    private Button _previousButton;
    private Button _nextButton;

    private int _playerChooseIndex;

    public int PlayerChooseIndex => _playerChooseIndex;

    public CharacterSelectorNGO CharacterSelectorNGO
    {
        get
        {
            if(_characterSelectorNGO == null)
            {
                _characterSelectorNGO = GetComponent<CharacterSelectorNGO>();
            }

            return _characterSelectorNGO;
        }
    }


    public Button PreviousButton
    {
        get
        {
            if(_previousButton == null)
            {
                _previousButton = CharacterSelectorNGO.PreViousButton;
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
                _nextButton = CharacterSelectorNGO.NextButton;
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
            CharacterSelectorNGO.SetCameraPositionServerRpc((int)direction * Vector3.right * MOVEVALUE,CharacterSelectorNGO.CameraOperation.Add);
            _playerChooseIndex = _currentSelectCharactorIndex;
        }
    }
}

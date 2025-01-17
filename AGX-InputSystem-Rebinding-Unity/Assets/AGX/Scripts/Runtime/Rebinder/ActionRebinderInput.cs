using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionRebinderInput : MonoBehaviour
{
    [BoxGroup("References"), SerializeField, Range(0, 5)] private int _selectedBinding;


    [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text _textRebind;
    [BoxGroup("References/UI"), SerializeField, Required] private Button   _buttonReset;
}
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace AGX.Scripts.Rebinder
{
    public class RebindControls : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required]              private InputActionReference              inputActionReference;
        [BoxGroup("References"), SerializeField, Required]              private bool                              excludeMouse = true;
        [BoxGroup("References"), SerializeField, Required, Range(0, 5)] private int                               selectedBinding;
        [BoxGroup("References"), SerializeField, Required]              private InputBinding.DisplayStringOptions displayStringOptions;
        [BoxGroup("References"), SerializeField, Required, ReadOnly]    private InputBinding                      inputBinding;

        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text   actionText;
        [BoxGroup("References/UI"), SerializeField, Required] private Button     rebindButton;
        [BoxGroup("References/UI"), SerializeField, Required] private GameObject rebindOverlay;
        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text   rebindOverlayText;
        [BoxGroup("References/UI"), SerializeField, Required] private TMP_Text   rebindText;
        [BoxGroup("References/UI"), SerializeField, Required] private Button     resetButton;

        [ShowNonSerializedField] private int    bindingIndex;
        [ShowNonSerializedField] private int    selectBinding;
        [ShowNonSerializedField] private string actionName;

        private void OnEnable()
        {
            rebindButton.onClick.AddListener(DoRebind);
            resetButton.onClick.AddListener(ResetBinding);

            if (inputActionReference != null)
            {
                InputManager.LoadBindingOverride(actionName);
                GetBindingInfo(selectBinding);
                UpdateUI();
            }

            InputManager.rebindComplete += UpdateUI;
            InputManager.rebindCanceled += UpdateUI;
        }

        private void OnDisable()
        {
            InputManager.rebindComplete -= UpdateUI;
            InputManager.rebindCanceled -= UpdateUI;
        }

        private void OnValidate()
        {
            if (inputActionReference == null)
                return;

            GetBindingInfo(selectBinding);
            UpdateUI();
        }

        private void GetBindingInfo(int selectBinding)
        {
            if (inputActionReference.action != null)
                actionName = inputActionReference.action.name;

            if (inputActionReference.action.bindings.Count > selectedBinding)
            {
                inputBinding = inputActionReference.action.bindings[selectedBinding = selectBinding];
                bindingIndex = selectedBinding;
            }
        }

        private void UpdateUI()
        {
            if (actionText != null)
                actionText.text = actionName;

            if (rebindText == null) return;

            if (Application.isPlaying)
            {
                rebindText.text = InputManager.GetBindingName(actionName, bindingIndex);
            }
            else
                rebindText.text = inputActionReference.action.GetBindingDisplayString(bindingIndex);
        }

        private void DoRebind()
        {
            InputManager.StartRebind(actionName, bindingIndex, rebindText, rebindOverlayText, rebindOverlay, excludeMouse);
        }

        private void ResetBinding()
        {
            InputManager.ResetBinding(actionName, bindingIndex);
            UpdateUI();
        }

        public void SelectKeyboardAndMouseBindingInfo()
        {
            GetBindingInfo(0); // Change number to K&M binding if not 0
            UpdateUI();
        }

        public void SelectControllerBindingInfo()
        {
            GetBindingInfo(1); // Change number to Controller binding if not 1
            UpdateUI();
        }

        public void ContollerVectorBindingInfo()
        {
            GetBindingInfo(5); // Change number to controller move binding
            UpdateUI();
        }
    }
}
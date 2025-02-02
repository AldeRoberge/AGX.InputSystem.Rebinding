using Sirenix.OdinInspector;
using UnityEngine;

namespace AGX.Scripts.Runtime.Rebinder
{
    public class ActionRebindersContent : MonoBehaviour
    {
        [BoxGroup("References"), SerializeField, Required] private ActionRebindersCategory _actionRebindersCategoryPrefab;
        [BoxGroup("References"), SerializeField, Required] private ActionRebinders         _actionRebindersPrefab;


        [BoxGroup("Data"), SerializeField, Required] private InputRebindings _inputRebindings;


        public void Awake()
        {
            _actionRebindersCategoryPrefab.gameObject.SetActive(false);
            _actionRebindersPrefab.gameObject.SetActive(false);

            if (_inputRebindings != null)
                ParseActionMap();
        }

        private void ParseActionMap()
        {
            foreach (var inputRebinding in _inputRebindings.InputActionRebindings)
            {
                // Skip non-included rebindings
                if (!inputRebinding.IsIncludedInRebindingUI)
                    continue;

                CreateActionMapCategoryHeader(inputRebinding.ActionMap);

                foreach (ControlsData? binding in inputRebinding.Controls)
                {
                    AddActionRebindingButton(binding);
                }
            }
        }

        private void AddActionRebindingButton(ControlsData? bindingData)
        {
            if (bindingData == null)
                return;

            // Instantiate a new ActionRebinder for the binding
            var newRebinder = Instantiate(_actionRebindersPrefab, transform);
            newRebinder.SetBindingData(bindingData); // Assuming SetBindingData configures the rebinder
            newRebinder.gameObject.SetActive(true);
        }

        private void CreateActionMapCategoryHeader(string category)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                Debug.LogWarning("Category name cannot be null or empty.");
                return;
            }

            // Instantiate a new category prefab
            var newCategory = Instantiate(_actionRebindersCategoryPrefab, transform);
            newCategory.SetCategoryName(category);
            newCategory.gameObject.SetActive(true);

            Debug.Log($"Category '{category}' added.");
        }
    }
}
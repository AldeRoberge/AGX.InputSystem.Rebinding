using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AGX.Scripts.Runtime.Prompts
{
    [DefaultExecutionOrder(-999)]
    public class InputDeviceIcons : MonoBehaviour
    {
        public List<TextAsset> _icons = new();

        private static readonly Dictionary<string, string> InputDeviceToSprite = new();

        private static InputDeviceIcons Instance;

        public static string GetSprite(string input)
        {
            if (Instance == null)
                Instance = FindObjectOfType<InputDeviceIcons>(true);

            if (Instance != null) 
                return Instance.GetSpriteImpl(input);

            Debug.LogError("No InputDevicePrompts found in scene");
            return input;
        }

        [ReadOnly]
        private static int _spriteCount;

        /// <summary>
        /// For an input, like
        /// <code>
        ///     "/Keyboard/anyKey"
        /// </code>
        /// will return
        /// <code>
        ///     <sprite="Prompts/Keyboard" name="keyboard_any"/>
        /// </code>
        /// </summary>
        public string GetSpriteImpl(string input)
        {
            // Since GetSprite will sometimes run in edit mode, we need to check if the application is playing
            // To avoid the map not being initialized
            if (!Application.isPlaying)
            {
                return input;
            }

            // remove < and >
            input = input.Replace("<", "").Replace(">", "");

            // add / at the beginning
            if (!input.StartsWith("/"))
                input = "/" + input;

            if (InputDeviceToSprite.TryGetValue(input, out var sprite))
                return sprite;

            // Empty action (no binding)
            if (input == "/")
                return "";

            Debug.LogWarning($"No sprite found for input: '{input}'");

            return input;
        }

        public void Awake()
        {
            bool debug = false;

            InputDeviceToSprite.Clear();

            foreach (var prompt in _icons)
            {
                if (debug)
                    Debug.Log($"Prompt: {prompt.text}");

                var inputDevicePrompt = Newtonsoft.Json.JsonConvert.DeserializeObject<InputDevicePrompt>(prompt.text);

                if (inputDevicePrompt == null)
                {
                    Debug.LogError("InputDevicePrompt is null!");
                    continue;
                }

                if (debug)
                {
                    Debug.Log($"Name: {inputDevicePrompt.Name}");
                    Debug.Log($"SpriteAsset: {inputDevicePrompt.SpriteAsset}");

                    foreach (var mapping in inputDevicePrompt.Sprites)
                    {
                        if (debug)
                            Debug.Log($"Path: {mapping.Key}, Sprite: {mapping.Value}");
                    }
                }


                foreach (var mapping in inputDevicePrompt.Sprites)
                {
                    InputDeviceToSprite[mapping.Key] = GetFullPath($"{inputDevicePrompt.SpriteAsset}/{inputDevicePrompt.Name}", mapping.Value);
                }
            }

            _spriteCount = InputDeviceToSprite.Count;

            Debug.Log("InputDevicePrompts.Start");
        }

        private string GetFullPath(string spriteAsset, string mappingSprite)
        {
            return $"<sprite=\"{spriteAsset}\" name=\"{mappingSprite}\">";
        }
    }
    
    public class Mapping
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class InputDevicePrompt
    {
        public string Name { get; set; }
        public string SpriteAsset { get; set; }
        public List<Mapping> Sprites { get; set; } = new();
    }
}
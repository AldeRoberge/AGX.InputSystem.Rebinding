using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

namespace AGX.Scripts.Runtime.Prompts
{
    [DefaultExecutionOrder(-999)]
    public class InputDevicePrompts : MonoBehaviour
    {
        public List<TextAsset> _prompts = new();

        private static readonly Dictionary<string, string> InputDeviceSpriteMap = new();

        private static InputDevicePrompts Instance;

        public static string GetSprite(string input)
        {
            if (Instance == null)
                Instance = FindObjectOfType<InputDevicePrompts>(true);

            if (Instance == null)
            {
                Debug.LogError("No InputDevicePrompts found in scene");
                return input;
            }

            return Instance.GetSpriteImpl(input);
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

            if (InputDeviceSpriteMap.TryGetValue(input, out var sprite))
                return sprite;

            Debug.LogWarning($"No sprite found for input: '{input}'");

            // Empty action (no binding)
            if (input == "/")
                return "";

            return input;
        }

        public void Awake()
        {
            InputDeviceSpriteMap.Clear();

            foreach (var prompt in _prompts)
            {
                Debug.Log($"Prompt: {prompt.text}");

                var inputDevicePrompt = Newtonsoft.Json.JsonConvert.DeserializeObject<InputDevicePrompt>(prompt.text);
                Debug.Log($"Name: {inputDevicePrompt.Name}");
                Debug.Log($"SpriteAsset: {inputDevicePrompt.SpriteAsset}");
                if (inputDevicePrompt.Mappings == null)
                {
                    Debug.LogError("Mappings are null!");
                }
                else
                {
                    foreach (var mapping in inputDevicePrompt.Mappings)
                    {
                        Debug.Log($"Path: {mapping.Path}, Sprite: {mapping.Sprite}");
                    }
                }

                foreach (var mapping in inputDevicePrompt.Mappings)
                {
                    if (mapping.Path == null)
                    {
                        Debug.LogError("Input is null");
                        continue;
                    }

                    InputDeviceSpriteMap[mapping.Path] = GetFullPath($"{inputDevicePrompt.SpriteAsset}/{inputDevicePrompt.Name}", mapping.Sprite);
                }
            }

            _spriteCount = InputDeviceSpriteMap.Count;

            Debug.Log("InputDevicePrompts.Start");
        }

        private string GetFullPath(string spriteAsset, string mappingSprite)
        {
            return $"<sprite=\"{spriteAsset}\" name=\"{mappingSprite}\">";
        }
    }


    public class Mapping
    {
        public string Path { get; set; }
        public string Sprite { get; set; }
    }

    public class InputDevicePrompt
    {
        public string Name { get; set; }
        public string SpriteAsset { get; set; }
        public List<Mapping> Mappings { get; set; }
    }
}
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace AGX.Scripts.Runtime.Icons
{
    [DefaultExecutionOrder(-999)]
    public class SpriteMapReference : MonoBehaviour
    {
        public List<TextAsset> Icons = new();

        private static readonly Dictionary<string, string> InputDeviceToSprite = new();

        private static SpriteMapReference _instance;

        public static string GetSprite(string input)
        {
            if (_instance == null)
                _instance = FindObjectOfType<SpriteMapReference>(true);

            if (_instance != null)
                return _instance.GetSpriteImpl(input);

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
        public string GetSpriteImpl(string key)
        {
            // Since GetSprite will sometimes run in edit mode, we need to check if the application is playing
            // To avoid the map not being initialized
            if (!Application.isPlaying)
            {
                return key;
            }

            if (InputDeviceToSprite.TryGetValue(key, out var sprite))
            {
                return sprite;
            }

            Debug.LogWarning($"No sprite found for key '{key}'.");
            return key;
        }

        public void Awake()
        {
            bool debug = false;

            InputDeviceToSprite.Clear();

            foreach (var prompt in Icons)
            {
                if (debug)
                    Debug.Log($"Prompt: {prompt.text}");

                var spriteMap = Newtonsoft.Json.JsonConvert.DeserializeObject<SpriteMap>(prompt.text);

                if (spriteMap == null)
                {
                    Debug.LogError("InputDevicePrompt is null!");
                    continue;
                }

                if (debug)
                {
                    Debug.Log($"Name: {spriteMap.Name}");
                    Debug.Log($"SpriteAsset: {spriteMap.SpriteAsset}");

                    foreach (var mapping in spriteMap.Sprites)
                    {
                        if (debug)
                            Debug.Log($"Path: {mapping.Key}, Sprite: {mapping.Value}");
                    }
                }


                foreach (var mapping in spriteMap.Sprites)
                {
                    InputDeviceToSprite[mapping.Key] = GetFullPath($"{spriteMap.SpriteAsset}/{spriteMap.Name}", mapping.Value);
                }
            }

            _spriteCount = InputDeviceToSprite.Count;

            Debug.Log("InputDevicePrompts.Start");
        }

        private string GetFullPath(string spriteAsset, string spriteName)
        {
            return $"<sprite=\"{spriteAsset}\" name=\"{spriteName}\">";
        }
    }

    public class SpriteValue
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class SpriteMap
    {
        public string Name { get; set; }
        public string SpriteAsset { get; set; }
        public List<SpriteValue> Sprites { get; set; } = new();
    }
}
using System;
using System.Collections.Generic;
using FredericRP.GenericSingleton;
using Unity.Plastic.Newtonsoft.Json;
using UnityEngine;

namespace AGX.Scripts.Runtime.Prompts
{
    [DefaultExecutionOrder(-999)]
    public class InputDevicePrompts : Singleton<InputDevicePrompts>
    {
        public List<TextAsset> Prompts = new();

        public static readonly Dictionary<string, string> InputDeviceSpriteMap = new();

        public static string GetSprite(string input) => Instance.GetSpriteImpl(input);


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
            // remove < and >
            input = input.Replace("<", "").Replace(">", "");

            // add / at the beginning
            if (!input.StartsWith("/"))
                input = "/" + input;

            if (InputDeviceSpriteMap.TryGetValue(input, out var sprite))
            {
                return sprite;
            }

            Debug.LogError($"No sprite found for input: '{input}'");

            // Empty action (no binding)
            if (input == "/")
                return "";

            return input;
        }

        public void Awake()
        {
            Prompts.Clear();

            foreach (var prompt in Prompts)
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
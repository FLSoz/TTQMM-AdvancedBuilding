using System;
using System.Reflection;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace Exund.AdvancedBuilding
{
    class PaletteTextFilter
    {
        static readonly FieldInfo m_UpdateGrid = BlockPicker.T_UIPaletteBlockSelect.GetField("m_UpdateGrid", BindingFlags.NonPublic | BindingFlags.Instance);
        public static MethodInfo SetUIInputMode = typeof(ManInput).GetMethod("SetUIInputMode", BindingFlags.NonPublic | BindingFlags.Instance);

        public static readonly Font ExoRegular = Resources.FindObjectsOfTypeAll<Font>().First(f => f.name == "Exo-Regular");
        public static readonly Sprite Options_Unticked = Resources.FindObjectsOfTypeAll<Sprite>().First(f => f.name == "Options_Unticked");

        public static bool clearOnCollapse = true;

        static UIInputMode mode;
        static bool wasFocused = false;
        static InputField inputField;
        static RectTransform inputFieldRect; 
        static UIPaletteBlockSelect blockPalette;

        static string filter = "";
        public static bool BlockFilterFunction(BlockTypes blockType)
        {
            if (filter == "") return true;
            var blockName = StringLookup.GetItemName(ObjectTypes.Block, (int)blockType).ToLower();
            return blockName.Contains(filter.ToLower());
        }

        static void OnTextChanged(string text)
        {
            filter = text;
            m_UpdateGrid.SetValue(blockPalette, true);
        }

        public static void Init(UIPaletteBlockSelect palette)
        {
            blockPalette = palette;
            var inputFieldGo = DefaultControls.CreateInputField(new DefaultControls.Resources() {
                inputField = Options_Unticked
            });

            inputField = inputFieldGo.GetComponent<InputField>();
            inputField.onValueChanged.AddListener(OnTextChanged);

            foreach (var text in inputFieldGo.GetComponentsInChildren<Text>())
            {
                text.text = "";
                text.alignment = TextAnchor.MiddleLeft;
                text.font = ExoRegular;
                text.fontSize = 20;
                text.fontStyle = FontStyle.Normal;
                text.color = Color.white;
                text.lineSpacing = 1;
            }

            inputField.placeholder.enabled = true;
            var placeholderText = inputField.placeholder.GetComponent<Text>();
            placeholderText.fontStyle = FontStyle.Italic;
            placeholderText.text = "Block name";

            inputField.transform.SetParent(blockPalette.transform.Find("HUD_BlockPainting_BG"), false);
            var rect = inputFieldGo.GetComponent<RectTransform>();
            rect.pivot = rect.anchorMax = rect.anchorMin = new Vector2(1, 1);
            rect.anchoredPosition3D = new Vector3(-5, -5, 77);
            rect.sizeDelta = new Vector2(210, 40);

            var scrollviewRect = blockPalette.transform.Find("HUD_BlockPainting_BG/Scroll View") as RectTransform;
            var anchoredPosition3D = scrollviewRect.anchoredPosition3D;
            anchoredPosition3D.y -= 40;
            scrollviewRect.anchoredPosition3D = anchoredPosition3D;
            var sizeDelta = scrollviewRect.sizeDelta;
            sizeDelta.y -= 40;
            scrollviewRect.sizeDelta = sizeDelta;

            var scrollbarRect = blockPalette.transform.Find("HUD_BlockPainting_BG/Scrollbar") as RectTransform;
            anchoredPosition3D = scrollbarRect.anchoredPosition3D;
            anchoredPosition3D.y -= 40;
            scrollbarRect.anchoredPosition3D = anchoredPosition3D;
            sizeDelta = scrollbarRect.sizeDelta;
            sizeDelta.y -= 40;
            scrollbarRect.sizeDelta = sizeDelta;

            inputFieldRect = rect;

            Singleton.Manager<ManGameMode>.inst.ModeSwitchEvent.Subscribe(OnModeChange);
        }

        internal static void HandleInputFieldFocus()
        {
            if (inputField)
            {
                if (inputField.isFocused)
                {
                    if (!wasFocused)
                    {
                        wasFocused = true;
                        mode = ManInput.inst.GetCurrentUIInputMode();
                        Singleton.Manager<ManInput>.inst.SetControllerMapsForUI(ManUI.inst, true, UIInputMode.FullscreenUI);
                        SetUIInputMode.Invoke(ManInput.inst, new object[] { mode, UIInputMode.FullscreenUI });
                    }
                }
                else if (wasFocused)
                {
                    wasFocused = false;
                    Singleton.Manager<ManInput>.inst.SetControllerMapsForUI(ManUI.inst, true, UIInputMode.BlockBuilding);
                    SetUIInputMode.Invoke(ManInput.inst, new object[] { UIInputMode.FullscreenUI, UIInputMode.BlockBuilding });
                }
            }
        }

        public static bool PreventPause()
        {
            return !(!inputField || inputField.isFocused);
        }

        internal static void OnPaletteCollapse(bool collapse)
        {
            if (clearOnCollapse && collapse)
            {
                ClearInput();
            } 
        }

        internal static void OnModeChange()
        {
            ClearInput();
        }

        static void ClearInput()
        {
            if(inputField) inputField.text = "";
        }
    }
}

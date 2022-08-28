using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;
using Nuterra.NativeOptions;
using ModHelper;

namespace Exund.AdvancedBuilding
{
    public class AdvancedBuildingMod : ModBase
    {
        public static string PreciseSnapshotsFolder = Path.Combine(Application.dataPath, "../PreciseSnapshots");
        private static GameObject _holder;
        internal static AssetBundle assetBundle;
        internal static RuntimeGizmos.TransformGizmo transformGizmo;
        internal static ModConfig config = new ModConfig();
        internal static string asm_path = Assembly.GetExecutingAssembly().Location.Replace("Exund.AdvancedBuilding.dll", "");
        internal const string HarmonyID = "exund.advancedbuilding";
        internal static Harmony harmony = new Harmony(HarmonyID);

        internal static ModContainer ThisContainer;

        private static void LoadGUIElements()
        {
            AdvancedEditor.transform_types = new GUIContent[]
            {
                new GUIContent(ThisContainer.Contents.FindAsset("move_tool.png") as Texture2D, "Move"),
                new GUIContent(ThisContainer.Contents.FindAsset("rotate_tool.png") as Texture2D, "Rotate"),
                new GUIContent(ThisContainer.Contents.FindAsset("scale_tool.png") as Texture2D, "Scale")
            };
            AdvancedEditor.pivot_types = new GUIContent[]
            {
                new GUIContent("Pivot", ThisContainer.Contents.FindAsset("pivot_tool.png") as Texture2D, "Individual blocks centers"),
                new GUIContent("Center", ThisContainer.Contents.FindAsset("center_tool.png") as Texture2D, "Average center of selected blocks")
            };
            AdvancedEditor.space_types = new GUIContent[]
            {
                new GUIContent("Global", ThisContainer.Contents.FindAsset("global_tool.png") as Texture2D, "Global absolute space"),
                new GUIContent("Local", ThisContainer.Contents.FindAsset("local_tool.png") as Texture2D, "Block local space (affected by block rotation)")
            };
        }

        private static void Load()
        {
            assetBundle = AssetBundle.LoadFromFile(asm_path + "advancedbuilding.assetbundle");

            try
            {
                _holder = new GameObject();
                _holder.AddComponent<AdvancedEditor>();

                if (Directory.Exists(PreciseSnapshotsFolder))
                {
                    _holder.AddComponent<LoadWindow>();
                }

                UnityEngine.Object.DontDestroyOnLoad(_holder);

                transformGizmo = Singleton.cameraTrans.gameObject.AddComponent<RuntimeGizmos.TransformGizmo>();
                transformGizmo.enabled = false;

                config.TryGetConfig<float>("position_step", ref AdvancedEditor.position_step);
                config.TryGetConfig<float>("rotation_step", ref AdvancedEditor.rotation_step);
                config.TryGetConfig<float>("scale_step", ref AdvancedEditor.scale_step);

                NativeOptionsMod.onOptionsSaved.AddListener(() => { config.WriteConfigJsonFile(); });
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static float NumberField(float value)
        {
            var h = GUILayout.Height(25f);
            float.TryParse(GUILayout.TextField(value.ToString(CultureInfo.InvariantCulture), h), out float val);
            val = (float)Math.Round(val, 6);
            if (Math.Abs(val - value) > 1e-6)
            {
                GUI.changed = true;
            }

            return val;
        }

        public static float NumberField(float value, float interval)
        {
            var h = GUILayout.Height(25f);
            var w = GUILayout.Width(25f);

            float val;
            GUILayout.BeginHorizontal(h);
            {
                float.TryParse(GUILayout.TextField(value.ToString(CultureInfo.InvariantCulture), h), out val);
                if (GUILayout.Button("+", w, h))
                {
                    val += interval;
                }

                if (GUILayout.Button("-", w, h))
                {
                    val -= interval;
                }
            }
            GUILayout.EndHorizontal();

            val = (float)Math.Round(val, 6);
            if (Math.Abs(val - value) > 1e-6)
            {
                GUI.changed = true;
            }

            return val;
        }

        public static Vector3 Vector3Field(Vector3 value, float interval, Vector3 defaultValue, string additionalText, params GUILayoutOption[] options)
        {
            Vector3 ret;
            GUILayout.BeginVertical(options);
            {
                var x = value.x;
                var y = value.y;
                var z = value.z;

                GUILayout.Label($"X {additionalText}");
                x = NumberField(x, interval);

                GUILayout.Label($"Y {additionalText}");
                y = NumberField(y, interval);

                GUILayout.Label($"Z {additionalText}");
                z = NumberField(z, interval);

                ret = new Vector3(x, y, z);

                GUILayout.BeginHorizontal();
                {
                    if (GUILayout.Button("Snap to closest"))
                    {
                        for (int i = 0; i < 3; i++)
                        {
                            ret[i] = Mathf.Round(ret[i] / interval) * interval;
                        }
                    }

                    GUILayout.FlexibleSpace();

                    if (GUILayout.Button("Reset"))
                    {
                        ret = defaultValue;
                    }
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();

            return ret;
        }

        private static bool Inited = false;

        public override void EarlyInit()
        {
            if (!Inited)
            {
                Inited = true;
                Dictionary<string, ModContainer> mods = (Dictionary<string, ModContainer>)AccessTools.Field(typeof(ManMods), "m_Mods").GetValue(Singleton.Manager<ManMods>.inst);
                if (mods.TryGetValue("Advanced Building", out ModContainer thisContainer))
                {
                    ThisContainer = thisContainer;
                    LoadGUIElements();
                }
                else
                {
                    Console.WriteLine("FAILED TO FETCH BuilderTools ModContainer");
                }
                Load();
            }
        }

        public override bool HasEarlyInit()
        {
            return true;
        }

        public override void Init()
        {
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public override void DeInit()
        {
            harmony.UnpatchAll(HarmonyID);
        }
    }
}

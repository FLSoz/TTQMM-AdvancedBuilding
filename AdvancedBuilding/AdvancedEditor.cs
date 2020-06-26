using System;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Nuterra.BlockInjector;

namespace Exund.AdvancedBuilding
{
    class AdvancedEditor : MonoBehaviour
    {
        static readonly GUIContent[] transform_types = new GUIContent[]
        {
            new GUIContent(GameObjectJSON.ImageFromFile(Path.Combine(AdvancedBuildingMod.asm_path, "Assets/move_tool.png")), "Move"),
            new GUIContent(GameObjectJSON.ImageFromFile(Path.Combine(AdvancedBuildingMod.asm_path, "Assets/rotate_tool.png")), "Rotate"),
            new GUIContent(GameObjectJSON.ImageFromFile(Path.Combine(AdvancedBuildingMod.asm_path, "Assets/scale_tool.png")), "Scale")
        };

        static readonly GUIContent[] pivot_types = new GUIContent[]
        {
            new GUIContent("Pivot", GameObjectJSON.ImageFromFile(Path.Combine(AdvancedBuildingMod.asm_path, "Assets/pivot_tool.png")), "Individual blocks centers"),
            new GUIContent("Center", GameObjectJSON.ImageFromFile(Path.Combine(AdvancedBuildingMod.asm_path, "Assets/center_tool.png")), "Average center of selected blocks")
        };

        static readonly GUIContent[] space_types = new GUIContent[]
        {
            new GUIContent("Global", GameObjectJSON.ImageFromFile(Path.Combine(AdvancedBuildingMod.asm_path, "Assets/global_tool.png")), "Global absolute space"), 
            new GUIContent("Local", GameObjectJSON.ImageFromFile(Path.Combine(AdvancedBuildingMod.asm_path, "Assets/local_tool.png")), "Block local space (affected by block rotation)")
        };


        internal static float position_step = 0.125f;
        internal static float rotation_step = 15f;
        internal static float scale_step = 0.1f;

        private readonly int BlockInfo_ID = 7777;
        private readonly int Transform_ID = 7778;
        private readonly int Settings_ID = 7779;
        private readonly int Tools_ID = 7780;

        GUIStyle RightAlign;

        private Rect BlockInfo_Win = new Rect(Screen.width - 200, (Screen.height - 200f) / 2f, 200f, 250f);
        private Rect Transform_Win = new Rect(Screen.width - 600f, 0, 600f, 250f);
        private Rect Setting_Win = new Rect(Screen.width - 200f, 250f, 200f, 200f);
        private Rect Tools_Win = new Rect(0, 0, 500f, 75f);

        private bool Settings_visible = false;

        internal static TankBlock block;
        internal static ModuleOffgridStore module;
        private Vector3 position;
        private Vector3 rotation;
        private Vector3 scale = Vector3.one;

        List<Transform> targets = new List<Transform>();

        private void Update()
        {
            if(block && !block.gameObject.activeInHierarchy || module && !module.block.gameObject.activeInHierarchy)
            {
                Clean();
            }
            if (Input.GetMouseButtonDown(2))
            {
                if (block)
                {
                    block.visible.EnableOutlineGlow(false, cakeslice.Outline.OutlineEnableReason.ScriptHighlight);
                }

                try
                {
                    Tank tank = null;
                    if(block && module)
                    {
                        tank = block.tank;
                    }
                    var temp_block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    if(tank && temp_block.tank && tank != temp_block.tank)
                    {
                        Clean();
                    }
                    block = temp_block;
                    block.visible.EnableOutlineGlow(true, cakeslice.Outline.OutlineEnableReason.ScriptHighlight);
                }
                catch
                {
                    Clean();
                }

                if (block && block.tank && block.tank.GetComponentInChildren<ModuleOffgridStore>())
                {
                    module = block.tank.GetComponentInChildren<ModuleOffgridStore>();
                    targets.Clear();
                    AdvancedBuildingMod.transformGizmo.ClearTargets();
                    AdvancedBuildingMod.transformGizmo.AddTarget(block.trans);
                    position = block.trans.localPosition;
                    rotation = block.trans.localEulerAngles;
                    scale = block.trans.localScale;
                }
                if(!block)
                {
                    AdvancedBuildingMod.transformGizmo.enabled = false;
                }

                useGUILayout = block;
            }
        }

        private void OnGUI()
        {
            if (!block) return;
            try
            {
                if (RightAlign == null)
                {
                    RightAlign = new GUIStyle(GUI.skin.label)
                    {
                        alignment = TextAnchor.MiddleRight
                    };
                }
                BlockInfo_Win = GUI.Window(BlockInfo_ID, BlockInfo_Win, BlockInfoWindow, "Block Infos");

                if (block.tank && block.tank.GetComponentInChildren<ModuleOffgridStore>())
                {
                    if (AdvancedBuildingMod.transformGizmo.enabled)
                    {
                        position = block.trans.localPosition;
                        rotation = block.trans.localEulerAngles;
                        scale = block.trans.localScale;

                        if(Singleton.Manager<ManPointer>.inst.BuildMode != (ManPointer.BuildingMode)10) Singleton.Manager<ManPointer>.inst.ChangeBuildMode((ManPointer.BuildingMode)10);
                        Tools_Win = GUI.Window(Tools_ID, Tools_Win, ToolsWindow, "Tools");
                    } 
                    else
                    {
                        Transform_Win = GUI.Window(Transform_ID, Transform_Win, TransformWindow, "Block transform");

                        if (position != block.trans.localPosition) block.trans.localPosition = position;
                        if (rotation != block.trans.localEulerAngles) block.trans.localEulerAngles = rotation;
                        if (scale != block.trans.localScale) block.trans.localScale = scale;

                        if (Settings_visible)
                        {
                            Setting_Win = GUI.Window(Settings_ID, Setting_Win, SettingsWindow, "Settings");
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }

            if(GUI.tooltip != "" && GUI.tooltip != null)
            {
                var size = GUI.skin.label.CalcSize(new GUIContent(GUI.tooltip));
                GUI.Label(new Rect(Mathf.Max(0, Input.mousePosition.x - size.x / 2), Screen.height - Input.mousePosition.y - size.y, size.x, size.y), GUI.tooltip);
            }
        }

        private void ToolsWindow(int id)
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal(GUILayout.Height(35f));
            AdvancedBuildingMod.transformGizmo.transformType = (RuntimeGizmos.TransformType)GUILayout.Toolbar((int)AdvancedBuildingMod.transformGizmo.transformType, transform_types, GUILayout.Width(150f), GUILayout.Height(35f));
            GUILayout.FlexibleSpace();
            AdvancedBuildingMod.transformGizmo.pivot = (RuntimeGizmos.TransformPivot)GUILayout.Toolbar((int)AdvancedBuildingMod.transformGizmo.pivot, pivot_types);
            GUILayout.FlexibleSpace();
            AdvancedBuildingMod.transformGizmo.space = (RuntimeGizmos.TransformSpace)GUILayout.Toolbar((int)AdvancedBuildingMod.transformGizmo.space, space_types);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
        }

        private void BlockInfoWindow(int id)
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type");
            GUILayout.FlexibleSpace();
            GUILayout.Label(block.BlockType.ToString(), RightAlign);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Category");
            GUILayout.FlexibleSpace();
            GUILayout.Label(block.BlockCategory.ToString(), RightAlign);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rarity");
            GUILayout.FlexibleSpace();
            GUILayout.Label(block.BlockRarity.ToString(), RightAlign);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Centre of Mass");
            GUILayout.FlexibleSpace();
            GUILayout.Label(block.CentreOfMass.ToString(), RightAlign);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mass");
            GUILayout.FlexibleSpace();
            GUILayout.Label(block.CurrentMass.ToString(), RightAlign);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Health");
            GUILayout.FlexibleSpace();
            GUILayout.Label(block.visible.damageable.Health.ToString(), RightAlign);
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();

            if (block.tank && block.tank.GetComponentInChildren<ModuleOffgridStore>() && GUILayout.Button(AdvancedBuildingMod.transformGizmo.enabled ? "Disable gizmos" : "Enable gizmos"))
            {
                if(AdvancedBuildingMod.transformGizmo.enabled)
                {
                    targets = new List<Transform>(AdvancedBuildingMod.transformGizmo.TargetRootsOrdered);
                    Singleton.Manager<ManPointer>.inst.ChangeBuildMode(ManPointer.BuildingMode.Grab);
                }
                AdvancedBuildingMod.transformGizmo.enabled = !AdvancedBuildingMod.transformGizmo.enabled;
                if(AdvancedBuildingMod.transformGizmo.enabled)
                {
                    foreach (var target in targets)
                    {
                        AdvancedBuildingMod.transformGizmo.AddTarget(target);
                    }
                    SaveConfig();
                    Singleton.Manager<ManPointer>.inst.ChangeBuildMode((ManPointer.BuildingMode)10);
                }
            }

            if (GUILayout.Button("Close"))
            {
                Clean();
            }
        }

        private void TransformWindow(int id)
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            {
                GUILayout.BeginVertical(GUILayout.Width(150f));
                var x = position.x;
                var y = position.y;
                var z = position.z;

                GUILayout.Label("X position");
                x = AdvancedBuildingMod.NumberField(x, position_step);

                GUILayout.Label("Y position");
                y = AdvancedBuildingMod.NumberField(y, position_step);

                GUILayout.Label("Z position");
                z = AdvancedBuildingMod.NumberField(z, position_step);

                GUILayout.BeginHorizontal();
                if(GUILayout.Button("Snap to closest"))
                {
                    x = Mathf.Round(x / position_step) * position_step;
                    y = Mathf.Round(y / position_step) * position_step;
                    z = Mathf.Round(z / position_step) * position_step;
                }

                position = new Vector3(x, y, z);

                if (GUILayout.Button("Reset"))
                {
                    position = block.cachedLocalPosition;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            {
                GUILayout.BeginVertical(GUILayout.Width(150f));
                var x = rotation.x;
                var y = rotation.y;
                var z = rotation.z;

                GUILayout.Label("X rotation");
                x = AdvancedBuildingMod.NumberField(x, rotation_step);

                GUILayout.Label("Y rotation");
                y = AdvancedBuildingMod.NumberField(y, rotation_step);

                GUILayout.Label("Z rotation");
                z = AdvancedBuildingMod.NumberField(z, rotation_step);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Snap to closest"))
                {
                    x = Mathf.Round(x / rotation_step) * rotation_step;
                    y = Mathf.Round(y / rotation_step) * rotation_step;
                    z = Mathf.Round(z / rotation_step) * rotation_step;
                }

                rotation = new Vector3(x, y, z);

                if (GUILayout.Button("Reset"))
                {
                    rotation = block.cachedLocalRotation.ToEulers();
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            {
                GUILayout.BeginVertical(GUILayout.Width(150f));
                var x = scale.x;
                var y = scale.y;
                var z = scale.z;

                GUILayout.Label("X scale");
                x = AdvancedBuildingMod.NumberField(x, scale_step);

                GUILayout.Label("Y scale");
                y = AdvancedBuildingMod.NumberField(y, scale_step);

                GUILayout.Label("Z scale");
                z = AdvancedBuildingMod.NumberField(z, scale_step);

                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Snap to closest"))
                {
                    x = Mathf.Round(x / scale_step) * scale_step;
                    y = Mathf.Round(y / scale_step) * scale_step;
                    z = Mathf.Round(z / scale_step) * scale_step;
                }

                scale = new Vector3(x, y, z);

                if (GUILayout.Button("Reset"))
                {
                    scale = Vector3.one;
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Open settings")) Settings_visible = true;
            GUILayout.FlexibleSpace();
        }

        private void SettingsWindow(int id)
        {
            GUILayout.Label("Position step");
            position_step = AdvancedBuildingMod.NumberField(position_step);
            AdvancedBuildingMod.transformGizmo.movementSnap = position_step;
            
            GUILayout.Label("Rotation step");
            rotation_step = AdvancedBuildingMod.NumberField(rotation_step);
            AdvancedBuildingMod.transformGizmo.rotationSnap = rotation_step;
            
            GUILayout.Label("Scale step");
            scale_step = AdvancedBuildingMod.NumberField(scale_step);
            AdvancedBuildingMod.transformGizmo.scaleSnap = scale_step;
            
            if(GUILayout.Button("Close"))
            {
                Settings_visible = false;
                SaveConfig();
            }
        }

        private void SaveConfig()
        {
            AdvancedBuildingMod.config["position_step"] = position_step;
            AdvancedBuildingMod.config["position_step"] = position_step;
            AdvancedBuildingMod.config["position_step"] = position_step;
            AdvancedBuildingMod.config.WriteConfigJsonFile();
        }

        private void Clean()
        {
            if(block) block.visible.EnableOutlineGlow(false, cakeslice.Outline.OutlineEnableReason.ScriptHighlight);
            block = null;
            module = null;
            useGUILayout = false;
            Singleton.Manager<ManPointer>.inst.ChangeBuildMode(ManPointer.BuildingMode.Grab);
            AdvancedBuildingMod.transformGizmo.transformType = RuntimeGizmos.TransformType.Move;
            AdvancedBuildingMod.transformGizmo.enabled = false;
            SaveConfig();
        }
    }
}

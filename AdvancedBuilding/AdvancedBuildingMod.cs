using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using UnityEngine;
using Harmony;
using Nuterra.BlockInjector;
using ModHelper.Config;
using Nuterra.NativeOptions;


namespace Exund.AdvancedBuilding
{
    public class AdvancedBuildingMod
    {
        public static string PreciseSnapshotsFolder = Path.Combine(Application.dataPath, "../PreciseSnapshots");
        private static GameObject _holder;
        internal static AssetBundle assetBundle;
        internal static RuntimeGizmos.TransformGizmo transformGizmo;
        internal static ModConfig config = new ModConfig();
        internal static string asm_path = Assembly.GetExecutingAssembly().Location.Replace("Exund.AdvancedBuilding.dll", "");

        public static void Load()
        {
            var harmony = HarmonyInstance.Create("exund.advancedbuilding");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            assetBundle = AssetBundle.LoadFromFile(asm_path + "Assets/runtimegizmos");

            try
            {
                _holder = new GameObject();
                _holder.AddComponent<AdvancedEditor>();
                _holder.AddComponent<LoadWindow>();
                UnityEngine.Object.DontDestroyOnLoad(_holder);

                transformGizmo = Singleton.cameraTrans.gameObject.AddComponent<RuntimeGizmos.TransformGizmo>();
                transformGizmo.enabled = false;

                config.TryGetConfig<float>("position_step", ref AdvancedEditor.position_step);
                config.TryGetConfig<float>("rotation_step", ref AdvancedEditor.rotation_step);
                config.TryGetConfig<float>("scale_step", ref AdvancedEditor.scale_step);

                if (!Directory.Exists(PreciseSnapshotsFolder))
                {
                    Directory.CreateDirectory(PreciseSnapshotsFolder);
                }

                new BlockPrefabBuilder(BlockTypes.GSOBlock_111, true)
                    .SetBlockID(7020)
                    .SetName("Reticule Research Hadamard Superposer")
                    .SetDescription("This block can register quantum fluctuations applied on the tech's blocks and stabilize them during the snapshot process.\n\n<b>Warning</b>: Can cause temporary quantum jumps if it enters in contact with zero-stasis gluons.\nUsed to activate Advanced Building.")
                    .SetFaction(FactionSubTypes.EXP)
                    .SetCategory(BlockCategories.Accessories)
                    .SetRarity(BlockRarity.Rare)
                    .SetPrice(58860)
                    .SetRecipe(new Dictionary<ChunkTypes, int> {
                        { ChunkTypes.SeedAI, 5 }
                    })
                    .SetModel(GameObjectJSON.MeshFromFile(asm_path + "Assets/hadamard_superposer.obj"), true, GameObjectJSON.GetObjectFromGameResources<Material>("RR_Main"))
                    .SetIcon(GameObjectJSON.ImageFromFile(asm_path + "Assets/hadamard_superposer.png"))
                    .AddComponent<ModuleOffgridStore>()
                    .RegisterLater();

            } 
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static float NumberField(float value)
        {
            var h = GUILayout.Height(25f);
            float.TryParse(GUILayout.TextField(value.ToString(), h), out float val);
            val = (float)Math.Round(val, 6);
            if (val != value)
            {
                GUI.changed = true;
            }

            return val;
        }

        public static float NumberField(float value, float interval)
        {
            var h = GUILayout.Height(25f);
            var w = GUILayout.Width(25f);

            GUILayout.BeginHorizontal(h);
            float.TryParse(GUILayout.TextField(value.ToString(), h), out float val);
            if (GUILayout.Button("+", w, h)) val += interval;
            if (GUILayout.Button("-", w, h)) val -= interval;
            GUILayout.EndHorizontal();
            val = (float)Math.Round(val, 6);
            if (val != value)
            {
                GUI.changed = true;
            }

            return val;
        }

        private static class Patches
        {
            [HarmonyPatch(typeof(TankPreset.BlockSpec), "InitFromBlockState")]
            private static class BlockSpec_InitFromBlockState
            {
                static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
                {
                    var codes = new List<CodeInstruction>(instructions);
                    var niv = codes.FindIndex(op => op.opcode == OpCodes.Newobj);
                    codes[niv - 2].operand = typeof(TankBlock).GetProperty("cachedLocalPosition", BindingFlags.Instance | BindingFlags.Public).GetGetMethod(false);
                    codes[niv - 1] = new CodeInstruction(OpCodes.Nop);

                    /*foreach (var ci in codes)
                    {
                        try
                        {
                            Console.WriteLine(ci.opcode.ToString() + "\t" + ci.operand.ToString());
                        } catch {
                            Console.WriteLine(ci.opcode.ToString());
                        }
                    }*/
                    return codes;
                }
            }
        }
    }
}

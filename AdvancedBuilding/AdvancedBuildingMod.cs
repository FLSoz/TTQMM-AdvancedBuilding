using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Reflection;
using System.Reflection.Emit;
using Nuterra.BlockInjector;
using Harmony;


namespace Exund.AdvancedBuilding
{
    public class AdvancedBuildingMod
    {
        public static string PreciseSnapshotsFolder = Path.Combine(Application.dataPath, "../PreciseSnapshots");
        private static GameObject _holder;

        public static void Load()
        {
            var harmony = HarmonyInstance.Create("exund.advancedbuilding");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            try
            {
                _holder = new GameObject();
                _holder.AddComponent<RotateBlocks>();
                _holder.AddComponent<TranslateBlocks>();
                _holder.AddComponent<ScaleBlocks>();
                _holder.AddComponent<BlocksInfo>();
                //_holder.AddComponent<SaveWindow>();
                //_holder.AddComponent<LoadWindow>();
                UnityEngine.Object.DontDestroyOnLoad(_holder);

                if (!Directory.Exists(PreciseSnapshotsFolder))
                {
                    Directory.CreateDirectory(PreciseSnapshotsFolder);
                }

                var asm_path = Assembly.GetExecutingAssembly().Location.Replace("Exund.AdvancedBuilding.dll", "");

                new BlockPrefabBuilder(BlockTypes.GSOBlock_111, true)
                    .SetBlockID(7020)
                    .SetName("Reticule Research Hadamard Superposer")
                    .SetDescription("This block can register quantum fluctuations applied on the tech's blocks and stabilize them during the snapshot process.\n\n<b>Warning</b>: Can cause temporary quantum jumps if it enters in contact with zero-stasis gluons.")
                    .SetFaction(FactionSubTypes.EXP)
                    .SetCategory(BlockCategories.Accessories)
                    .SetRarity(BlockRarity.Rare)
                    .SetPrice(15000)
                    .SetRecipe(new Dictionary<ChunkTypes, int> {
                        { ChunkTypes.SeedAI, 5 }
                    })
                    .SetModel(GameObjectJSON.MeshFromFile(asm_path + "hadamard_superposer.obj"), true, GameObjectJSON.GetObjectFromGameResources<Material>("RR_Main"))
                    .SetIcon(GameObjectJSON.ImageFromFile(asm_path + "hadamard_superposer.png"))
                    .AddComponent<ModuleOffgridStore>()
                    .RegisterLater();
            } catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static float NumberField(float value, float interval)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(30));
            float.TryParse(GUILayout.TextField(value.ToString()), out float val);
            if (GUILayout.Button("+")) val += interval;
            if (GUILayout.Button("-")) val -= interval;
            GUILayout.EndHorizontal();
            return (float)Math.Round(val, 6);
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

using System;
using System.Linq;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;

namespace Exund.AdvancedBuilding
{
    class BlocksInfo : MonoBehaviour
    {
        private int ID = 7777;

        private bool visible = false;

        internal static TankBlock block;

        private float posX, posY;

        private Rect win;

        private bool registered = false;

        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                win = new Rect(Input.mousePosition.x - 400f, Screen.height - Input.mousePosition.y, 200f, 200f);
                try
                {
                    block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                }
                catch (Exception ex)
                {
                    block = null;
                }
                visible = block;
            }
        }

        private void OnGUI()
        {
            if (!visible || !block) return;
            if (!AdvancedBuildingMod.Nuterra && AdvancedBuildingMod.ModExists("Nuterra.UI", out Assembly asm))
            {          
                var type = asm.GetTypes().First(t => t.Name.Contains("NuterraGUI"));
                AdvancedBuildingMod.Nuterra = (GUISkin)type.GetProperty("Skin").GetValue(null, null);          
            }
            if (AdvancedBuildingMod.Nuterra)
            {
                GUI.skin = AdvancedBuildingMod.Nuterra;
            }

            if (!registered && AdvancedBuildingMod.ModExists("Exund.CommandConsole", out asm))
            {
                var cmdctor = asm.GetTypes().First(t => t.Name.Contains("TTCommand")).GetConstructors()[0];

                cmdctor.Invoke(new object[]
                {
                    "BlockInfo",
                    "Get the selected block info in console",
                    new Func<Dictionary<string, string>, string>(delegate (Dictionary<string, string> arguments)
                    {
                            if (!block) return "<color=yellow>No block selected</color>";
                            string info = block.BlockType.ToString() + "\n" + block.CurrentMass;
                            return info;
                    }),
                    new Dictionary<string, string> {}
                });
                registered = true;
            }
            /*GUI.skin = NuterraGUI.Skin;/*.window = new GUIStyle(GUI.skin.window)
            {
                normal =
            {
                background = NuterraGUI.LoadImage("Border_BG.png"),
                textColor = Color.white
            },
                border = new RectOffset(16, 16, 16, 16),
            }; 
            GUI.skin.label.margin.top = 5;
            GUI.skin.label.margin.bottom = 0;*/
            try
            {
                win = GUI.Window(ID, win, new GUI.WindowFunction(DoWindow), "Block Infos");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DoWindow(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Type");
            GUILayout.Label(block.BlockType.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Category");
            GUILayout.Label(block.BlockCategory.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Rarity");
            GUILayout.Label(block.BlockRarity.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Centre of Mass");
            GUILayout.Label(block.CentreOfMass.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Mass");
            GUILayout.Label(block.CurrentMass.ToString());
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Health");
            GUILayout.Label(block.visible.damageable.Health.ToString());
            GUILayout.EndHorizontal();


            if (GUILayout.Button("Close"))
            {
                visible = false;
                block = null;
            }
            GUI.DragWindow();
        }
    }
}

using System;
using UnityEngine;

namespace Exund.AdvancedBuilding
{
    class ScaleBlocks : MonoBehaviour
    {
        private int ID = 7779;

        private bool visible = false;

        private TankBlock block;

        private float x = 0, y = 0, z = 0;

        private Rect win;

        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                win = new Rect(Input.mousePosition.x + 200f, Screen.height - Input.mousePosition.y, 200f, 200f);
                try
                {
                    block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    x = block.trans.localScale.x;
                    y = block.trans.localScale.y;
                    z = block.trans.localScale.z;
                    //Console.WriteLine(block.trans.rotation.eulerAngles);
                }
                catch (Exception ex)
                {
                    //Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
                    block = null;
                }
                visible = block;
            }
        }

        private void OnGUI()
        {
            if (!visible || !block) return;
            if (AdvancedBuildingMod.Nuterra)
            {
                GUI.skin = AdvancedBuildingMod.Nuterra;
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
            GUI.skin.label.margin.bottom = 5;*/
            try
            {
                win = GUI.Window(ID, win, new GUI.WindowFunction(DoWindow), "Block scale");
                block.trans.localScale = new Vector3(x, y, z);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DoWindow(int id)
        {
            GUILayout.Label("X scale");
            x = AdvancedBuildingMod.NumberField(x, 0.1f);
            //float.TryParse(GUILayout.TextField(x.ToString()), out x);

            GUILayout.Label("Y scale");
            y = AdvancedBuildingMod.NumberField(y, 0.1f);
            //float.TryParse(GUILayout.TextField(y.ToString()), out y);

            GUILayout.Label("Z scale");
            z = AdvancedBuildingMod.NumberField(z, 0.1f);
            //float.TryParse(GUILayout.TextField(z.ToString()), out z);
            if (GUILayout.Button("Close"))
            {
                visible = false;
                block = null;
            }
            GUI.DragWindow();
        }
    }
}

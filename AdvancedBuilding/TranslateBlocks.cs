using System;
using UnityEngine;

namespace Exund.AdvancedBuilding
{
    class TranslateBlocks : MonoBehaviour
    {
        private int ID = 7778;
        private bool visible = false;

        private TankBlock block;

        private float x = 0, y = 0, z = 0;

        private Rect win;

        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                win = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 200f, 200f);
                try
                {
                    block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    x = block.trans.localPosition.x;
                    y = block.trans.localPosition.y;
                    z = block.trans.localPosition.z;
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

        /*private void Inst_MouseEvent(ManPointer.Event arg1, bool arg2, bool arg3)
        {
            if (arg1 == ManPointer.Event.MMB && arg2 && Singleton.Manager<ManPointer>.inst.targetVisible.block)
            {
                
            }
        }*/

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
                win = GUI.Window(ID, win, new GUI.WindowFunction(DoWindow), "Block position");
                block.trans.localPosition = new Vector3(x, y, z);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DoWindow(int id)
        {
            GUILayout.Label("X position");
            x = AdvancedBuildingMod.NumberField(x, 0.125f);
            //float.TryParse(GUILayout.TextField(x.ToString()), out x);

            GUILayout.Label("Y position");
            y = AdvancedBuildingMod.NumberField(y, 0.125f);
            //float.TryParse(GUILayout.TextField(y.ToString()), out y);

            GUILayout.Label("Z position");
            z = AdvancedBuildingMod.NumberField(z, 0.125f);
            //float.TryParse(GUILayout.TextField(z.ToString()), out z);

            //GUILayout.Label(((IntVector3)block.trans.localPosition).ToString());

            if (GUILayout.Button("Close"))
            {
                visible = false;
                block = null;
            }
            GUI.DragWindow();
        }
    }
}

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
                if (Singleton.Manager<ManPointer>.inst.targetVisible &&
                Singleton.Manager<ManPointer>.inst.targetVisible.block &&
                Singleton.Manager<ManPointer>.inst.targetVisible.block.tank &&
                !Singleton.Manager<ManPointer>.inst.targetVisible.block.tank.transform.GetComponentInChildren<ModuleOffgridStore>())
                {
                    return;
                }

                win = new Rect(Input.mousePosition.x, Screen.height - Input.mousePosition.y, 200f, 200f);
                try
                {
                    block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    x = block.trans.localPosition.x;
                    y = block.trans.localPosition.y;
                    z = block.trans.localPosition.z;
                }
                catch
                {
                    block = null;
                }
                visible = block;
            }
            useGUILayout = visible;
        }

        private void OnGUI()
        {
            if (!visible || !block) return;
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

            if (GUILayout.Button("Close"))
            {
                visible = false;
                block = null;
            }
            GUI.DragWindow();
        }
    }
}

using System;
using UnityEngine;

namespace Exund.AdvancedBuilding
{
    class RotateBlocks : MonoBehaviour
    {
        private int ID = 7780;
        private bool visible = false;
        private TankBlock block;
        private float x = 0, y = 0, z = 0;
        private Rect win;

        private void Update()
        {
            if(Input.GetMouseButtonDown(2))
            {
                if (Singleton.Manager<ManPointer>.inst.targetVisible &&
                Singleton.Manager<ManPointer>.inst.targetVisible.block &&
                Singleton.Manager<ManPointer>.inst.targetVisible.block.tank &&
                !Singleton.Manager<ManPointer>.inst.targetVisible.block.tank.transform.GetComponentInChildren<ModuleOffgridStore>())
                {
                    return;
                }

                win = new Rect(Input.mousePosition.x - 200f, Screen.height - Input.mousePosition.y, 200f, 200f);
                try
                {
                    block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    x = block.trans.localRotation.eulerAngles.x;
                    y = block.trans.localRotation.eulerAngles.y;
                    z = block.trans.localRotation.eulerAngles.z;
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
            if (!visible||!block) return;
            try
            {
                win = GUI.Window(ID, win, new GUI.WindowFunction(DoWindow), "Block Rotation");
                block.trans.localRotation = Quaternion.Euler(x, y, z);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private void DoWindow(int id)
        {
            GUILayout.Label("X rotation");
            x = AdvancedBuildingMod.NumberField(x, 15f);
            //float.TryParse(GUILayout.TextField(x.ToString()),out x);

            GUILayout.Label("Y rotation");
            y = AdvancedBuildingMod.NumberField(y, 15f);
            //float.TryParse(GUILayout.TextField(y.ToString()), out y);

            GUILayout.Label("Z rotation");
            z = AdvancedBuildingMod.NumberField(z, 15f);
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

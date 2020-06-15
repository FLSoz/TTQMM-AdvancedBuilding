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
                if (Singleton.Manager<ManPointer>.inst.targetVisible &&
                Singleton.Manager<ManPointer>.inst.targetVisible.block &&
                Singleton.Manager<ManPointer>.inst.targetVisible.block.tank &&
                !Singleton.Manager<ManPointer>.inst.targetVisible.block.tank.transform.GetComponentInChildren<ModuleOffgridStore>())
                {
                    return;
                }

                win = new Rect(Input.mousePosition.x + 200f, Screen.height - Input.mousePosition.y, 200f, 200f);
                try
                {
                    block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
                    x = block.trans.localScale.x;
                    y = block.trans.localScale.y;
                    z = block.trans.localScale.z;
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

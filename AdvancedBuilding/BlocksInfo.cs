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
        private Rect win;

        private void Update()
        {
            if (Input.GetMouseButtonDown(2))
            {
                win = new Rect(Input.mousePosition.x - 400f, Screen.height - Input.mousePosition.y, 200f, 200f);
                try
                {
                    block = Singleton.Manager<ManPointer>.inst.targetVisible.block;
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

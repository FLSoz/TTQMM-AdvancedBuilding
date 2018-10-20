using UnityEngine;
using System.IO;


namespace Exund.AdvancedBuilding
{
    class SaveWindow : MonoBehaviour
    {
        private int ID = 7782;

        private string path;

        private bool visible = false;

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.S) && Input.GetKey(KeyCode.LeftAlt) && Singleton.playerTank)
            {
                path = Path.GetFullPath(Path.Combine(AdvancedBuildingMod.PreciseSnapshotsFolder, Singleton.playerTank.name + ".json"));
                if(File.Exists(path))
                {
                    visible = true;
                    return;
                }
                PreciseSnapshot.Save(Singleton.playerTank, AdvancedBuildingMod.PreciseSnapshotsFolder);
            }
        }

        private void OnGUI()
        {
            if (!visible) return;
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
            };*/
            GUI.Window(ID, new Rect((Screen.width - 700f) / 2, (Screen.height - 200f) / 2, 700f, 200f), new GUI.WindowFunction(DoWindow), "");
        }

        private void DoWindow(int id)
        {
            GUILayout.Label(new GUIContent("<color=yellow>WARNING</color>"), new GUIStyle { richText = true, alignment = TextAnchor.MiddleCenter, fontSize = 32 });
            GUILayout.Label("The path \"" + path + "\" already exists.\n Do you want to replace it ?");
            GUILayout.BeginHorizontal();
            if(GUILayout.Button("Cancel")) visible = false;
            if(GUILayout.Button("Overwrite"))
            {
                visible = false;
                PreciseSnapshot.Save(Singleton.playerTank, AdvancedBuildingMod.PreciseSnapshotsFolder);
            }
            GUILayout.EndHorizontal();
        }
    }
}

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
                } 
                else
                {
                    PreciseSnapshot.Save(Singleton.playerTank, AdvancedBuildingMod.PreciseSnapshotsFolder);
                }
            }
            useGUILayout = visible;
        }

        private void OnGUI()
        {
            if (!visible) return;
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

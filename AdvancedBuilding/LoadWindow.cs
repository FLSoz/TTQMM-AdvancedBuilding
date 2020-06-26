using UnityEngine;
using System.IO;

namespace Exund.AdvancedBuilding
{
    class LoadWindow : MonoBehaviour
    {
        private int ID = 7781;

        private bool visible = false;

        private Vector2 scrollPos = Vector2.zero;

        public Tank loadedTech;

        private string path = "";

        private static readonly string folder_path = Path.GetFullPath(AdvancedBuildingMod.PreciseSnapshotsFolder);

        public static string[] files = new string[0]; 

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.O) && Input.GetKey(KeyCode.LeftAlt) && Singleton.playerTank)
            {
                files = Directory.GetFiles(folder_path, "*.json");
                visible = files.Length > 0;
            }
            useGUILayout = visible;
        }

        private void OnGUI()
        {
            if (!visible) return;
            GUI.Window(ID, new Rect((Screen.width - 700f) / 2, (Screen.height - 500f) / 2, 700f, 500f), new GUI.WindowFunction(DoWindow), "Techs");
        }

        private void DoWindow(int id)
        {

            scrollPos = GUILayout.BeginScrollView(scrollPos);
            foreach (var tech in files)
            {
                if (GUILayout.Button(tech,new GUIStyle(GUI.skin.button) { richText = true, alignment = TextAnchor.MiddleLeft }))
                {
                    path = tech;
                }
            }
            GUILayout.EndScrollView();
            GUILayout.TextField(path);
            if (GUILayout.Button("Load"))
            {
                try
                {
                    Vector3 position = Singleton.playerTank.trans.position;
                    Quaternion rotation = Singleton.playerTank.trans.rotation;

                    loadedTech = PreciseSnapshot.Load(path, position, rotation);

                    Tank playerTank = Singleton.playerTank;
                    Singleton.Manager<ManTechs>.inst.RequestSetPlayerTank(null, true);
                    playerTank.visible.RemoveFromGame();
                    Singleton.Manager<ManTechs>.inst.RequestSetPlayerTank(loadedTech, true);
                }
                catch { }
                visible = false;
            }
            if (GUILayout.Button("Cancel")) visible = false;

        }
    }
}

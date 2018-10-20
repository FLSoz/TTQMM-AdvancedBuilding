using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using UnityEngine;
using System.Reflection;

namespace Exund.AdvancedBuilding
{
    public class AdvancedBuildingMod
    {
        public static string PreciseSnapshotsFolder = Path.Combine(Application.dataPath, "../PreciseSnapshots");
        private static GameObject _holder;
        internal static GUISkin Nuterra;

        public static bool ModExists(string name,out Assembly asm)
        {
            asm = null;
            foreach (var assembly in System.AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.FullName.StartsWith(name))
                {
                    asm = assembly;
                    return true;
                }
            }
            return false;
        }

        public static void Load()
        {
            try
            {
                _holder = new GameObject();
                _holder.AddComponent<RotateBlocks>();
                _holder.AddComponent<TranslateBlocks>();
                _holder.AddComponent<ScaleBlocks>();
                _holder.AddComponent<BlocksInfo>();
                _holder.AddComponent<SaveWindow>();
                _holder.AddComponent<LoadWindow>();
                UnityEngine.Object.DontDestroyOnLoad(_holder);

                if (!Directory.Exists(PreciseSnapshotsFolder))
                {
                    Directory.CreateDirectory(PreciseSnapshotsFolder);
                }

                var a = new Func<Dictionary<string, string>, string>(delegate (Dictionary<string, string> arguments)
                {
                     if (!Singleton.playerTank) return "<color=yellow>Specified Tech not found</color>";

                     Vector3 newVel = Singleton.playerTank.rbody.velocity;

                     if (arguments.TryGetValue("X", out string argX)) if (int.TryParse(argX, out int intX)) newVel.x = intX;
                     if (arguments.TryGetValue("Y", out string argY)) if (int.TryParse(argY, out int intY)) newVel.y = intY;
                     if (arguments.TryGetValue("Z", out string argZ)) if (int.TryParse(argZ, out int intZ)) newVel.z = intZ;
                     Singleton.playerTank.rbody.velocity = newVel;

                     return "Tech velocity set to " + newVel.ToString();
                });
                            
            

            } catch(Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public static float NumberField(float value, float interval)
        {
            GUILayout.BeginHorizontal(GUILayout.Height(30));
            float.TryParse(GUILayout.TextField(value.ToString()), out float val);
            if (GUILayout.Button("+")) val += interval;
            if (GUILayout.Button("-")) val -= interval;
            GUILayout.EndHorizontal();
            return val;
        }
    }
}

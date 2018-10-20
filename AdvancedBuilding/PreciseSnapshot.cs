using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;

namespace Exund.AdvancedBuilding
{
    public static class PreciseSnapshot
    {
        public static void Save(Tank tech, string folder)
        {
            if (!Directory.Exists(folder))
            {
                Console.WriteLine("PreciseSnapshot : Specified path \"" + folder + "\" doesn't exists !");
                return;
            }
            string json = JsonConvert.SerializeObject(JSONTech.FromTech(tech), Formatting.Indented);
            File.WriteAllText(Path.Combine(folder, tech.name + ".json"), json, Encoding.UTF8);
        }

        public static Tank Load(string path, Vector3 position, Quaternion rotation)
        {
            string json = File.ReadAllText(path);
            JSONTech jTech = JsonConvert.DeserializeObject<JSONTech>(json);
            return jTech.ToTech(position, rotation);
        }

        public struct JSONTech
        {
            public string Name;
            public List<JSONBlock> Blocks;

            public static JSONTech FromTech(Tank tech)
            {
                var jTech = new JSONTech {
                    Name = tech.name,
                    Blocks = new List<JSONBlock>()
                };
                foreach (var block in tech.blockman.IterateBlocks())
                {
                    jTech.Blocks.Add(JSONBlock.FromBlock(block));
                }

                return jTech;
            }

            public Tank ToTech(Vector3 position, Quaternion rotation)
            {
                Tank tech = Singleton.Manager<ManSpawn>.inst.SpawnEmptyTechRef(0, position, rotation, true, false, this.Name).visible.tank;
                var root = this.Blocks.First(b => b.IsRootBlock);
                this.Blocks.Remove(root);
                var rBlock = root.ToBlock(position - Vector3.one, rotation, true);
                tech.blockman.AddBlock(ref rBlock, new IntVector3(root.Position), new OrthoRotation(root.OrthoRotation));
                rBlock.trans.localPosition = root.localPosition;
                rBlock.trans.localEulerAngles = root.localEulerAngles;

                foreach (var b in Blocks)
                {
                    var rB = b.ToBlock(position - Vector3.one, rotation, true);
                    tech.blockman.AddBlock(ref rB, new IntVector3(b.Position), new OrthoRotation(b.OrthoRotation));
                    rB.trans.localPosition = b.localPosition;
                    rB.trans.localEulerAngles = b.localEulerAngles;
                }

                return tech;
            }
        }

        public struct JSONBlock
        {
            public BlockTypes Type;
            public OrthoRotation.r OrthoRotation;
            public Vector3 Position;

            public Vector3 localPosition;
            public Vector3 localEulerAngles;
            public Vector3 localScale;

            public bool IsRootBlock;

            public static JSONBlock FromBlock(TankBlock block)
            {
                var jBlock = new JSONBlock()
                {
                    Type = block.BlockType,
                    OrthoRotation = block.cachedLocalRotation.rot,
                    Position = block.cachedLocalPosition,

                    localPosition = block.trans.localPosition,
                    localEulerAngles = block.trans.localEulerAngles,
                    localScale = block.trans.localScale
                };

                if(block.tank)
                {
                    jBlock.IsRootBlock = block.tank.blockman.IsRootBlock(block);
                }

                return jBlock;
            }

            public TankBlock ToBlock(Vector3 position, Quaternion rotation, bool forceTransform = false)
            {
                TankBlock block = Singleton.Manager<ManSpawn>.inst.SpawnBlock(this.Type, position, rotation);
                if (forceTransform)
                {
                    block.trans.localPosition = this.localPosition;
                    block.trans.localEulerAngles = this.localEulerAngles;
                    block.trans.localScale = this.localScale;
                }
                return block;
            }
        }
    }

    
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System.Reflection;

namespace Exund.AdvancedBuilding
{
    public static class PreciseSnapshot
    {
        private static PropertyInfo cachedLocalRotation;
        static PreciseSnapshot()
        {
            cachedLocalRotation = typeof(TankBlock).GetProperty("cachedLocalRotation");
        }

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

                var spawnParams = new ManSpawn.TankSpawnParams()
                {
                    forceSpawn = true,
                    grounded = true,
                    position = position,
                    rotation = rotation,
                    placement = ManSpawn.TankSpawnParams.Placement.PlacedAtPosition,
                    techData = new TechData()
                    {
                        Name = this.Name,
                        m_CreationData = new TechData.CreationData(),
                        m_BlockSpecs = new List<TankPreset.BlockSpec>()
                    }
                };

                /*var root = this.Blocks.First(b => b.IsRootBlock);
                this.Blocks.Remove(root);
                var rblock = root.ToBlock(position - Vector3.one, rotation);
                rblock.trans.localPosition = root.Position;
                cachedLocalRotation.SetValue(rblock, root.OrthoRotation, null);
                var rblockspec = new TankPreset.BlockSpec();
                rblockspec.InitFromBlockState(rblock, true);
                spawnParams.techData.m_BlockSpecs.Add(rblockspec);

                rblock.visible.RemoveFromGame();*/

                foreach (var b in Blocks)
                {
                    var rB = b.ToBlock(position - Vector3.one, rotation);
                    rB.trans.localPosition = b.Position;
                    cachedLocalRotation.SetValue(rB, b.OrthoRotation, null);
                    var rBSpec = new TankPreset.BlockSpec();
                    rBSpec.InitFromBlockState(rB, true);
                    spawnParams.techData.m_BlockSpecs.Add(rBSpec);
                    rB.visible.RemoveFromGame();
                }

                var tech = ManSpawn.inst.SpawnUnmanagedTank(spawnParams);

                var blockman = tech.blockman;

                foreach (var b in Blocks)
                {
                    var block = blockman.GetBlockAtPosition(b.Position);
                    block.trans.localPosition = b.localPosition;
                    block.trans.localEulerAngles = b.localEulerAngles;
                    block.trans.localScale = b.localScale;
                }

                /*Blocks.Add(root);

                var blocks = tech.blockman.GetLowestBlocks();
                foreach (var b in blocks)
                {
                    var jb = Blocks.First(bb => bb.Position == b.cachedLocalPosition);
                    b.trans.localEulerAngles = jb.localEulerAngles;
                    b.trans.localPosition = jb.localPosition;
                    b.trans.localScale = jb.localScale;
                }*/

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

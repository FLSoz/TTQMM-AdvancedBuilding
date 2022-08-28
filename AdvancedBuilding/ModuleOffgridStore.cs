using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Exund.AdvancedBuilding
{
    public class ModuleOffgridStore : Module
    {
		private SerialData data;

        private void OnPool()
		{
			base.block.serializeEvent.Subscribe(this.OnSerialize);
			base.block.serializeTextEvent.Subscribe(this.OnSerialize);
        }

		private void OnTankPostSpawn()
		{
			if (data == null) return;
			var blockman = base.block.tank.blockman;
			foreach (var ogBlock in data.blocks)
			{
				var block = blockman.GetBlockAtPosition(ogBlock.position);
				if(block)
				{
					block.trans.localPosition = ogBlock.localPosition;
					block.trans.localEulerAngles = ogBlock.localEulerAngles;
					block.trans.localScale = ogBlock.localScale;
				}
			}
			base.block.tank.ResetPhysicsEvent.Unsubscribe(this.OnTankPostSpawn);
		}

		private void OnSerialize(bool saving, TankPreset.BlockSpec blockSpec)
        {
			if (saving)
			{
				var serialDataSave = new SerialData();
				var list = new List<OutgridBlock>();
				foreach (var block in base.block.tank.blockman.IterateBlocks())
				{
					list.Add(new OutgridBlock(block));
				}
				serialDataSave.blocks = list.ToArray();
				serialDataSave.Store(blockSpec.saveState);
				return;
			}
			SerialData serialData = Module.SerialData<ModuleOffgridStore.SerialData>.Retrieve(blockSpec.saveState);
			if (serialData != null)
			{
				data = serialData;
				base.block.tank.ResetPhysicsEvent.Subscribe(this.OnTankPostSpawn);
			}
		}

		private class SerialData : Module.SerialData<ModuleOffgridStore.SerialData>
		{
			public OutgridBlock[] blocks;
		}

		[Serializable]
		private struct OutgridBlock
		{
			public Vector3 position;
			public Vector3 localPosition;
			public Vector3 localEulerAngles;
			public Vector3 localScale;
			
			public OutgridBlock(TankBlock block)
			{
				position = block.cachedLocalPosition;
				localPosition = block.trans.localPosition;
				localEulerAngles = block.trans.localEulerAngles;
				localScale = block.trans.localScale;
			}
		}
    }
}

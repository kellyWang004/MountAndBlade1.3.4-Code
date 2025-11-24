using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.View;

public static class MountVisualCreator
{
	public static void SetMaterialProperties(ItemObject mountItem, MetaMesh mountMesh, MountCreationKey key, ref uint maneMeshMultiplier)
	{
		//IL_0027: Unknown result type (might be due to invalid IL or missing references)
		//IL_002c: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0041: Invalid comparison between Unknown and I4
		//IL_0049: Unknown result type (might be due to invalid IL or missing references)
		//IL_0063: Unknown result type (might be due to invalid IL or missing references)
		HorseComponent horseComponent = mountItem.HorseComponent;
		int index = MathF.Min((int)key.MaterialIndex, ((List<MaterialProperty>)(object)horseComponent.HorseMaterialNames).Count - 1);
		MaterialProperty val = ((List<MaterialProperty>)(object)horseComponent.HorseMaterialNames)[index];
		Material fromResource = Material.GetFromResource(((MaterialProperty)(ref val)).Name);
		if ((int)mountItem.ItemType == 1)
		{
			int num = MathF.Min((int)key.MeshMultiplierIndex, val.MeshMultiplier.Count - 1);
			if (num != -1)
			{
				maneMeshMultiplier = val.MeshMultiplier[num].Item1;
			}
			mountMesh.SetMaterialToSubMeshesWithTag(fromResource, "horse_body");
			mountMesh.SetFactorColorToSubMeshesWithTag(maneMeshMultiplier, "horse_tail");
		}
		else
		{
			mountMesh.SetMaterial(fromResource);
		}
	}

	public static List<MetaMesh> AddMountMesh(MBAgentVisuals agentVisual, ItemObject mountItem, ItemObject harnessItem, string mountCreationKeyStr, Agent agent = null)
	{
		//IL_003a: Unknown result type (might be due to invalid IL or missing references)
		//IL_0040: Invalid comparison between Unknown and I4
		//IL_00ff: Unknown result type (might be due to invalid IL or missing references)
		//IL_0105: Invalid comparison between Unknown and I4
		//IL_01c2: Unknown result type (might be due to invalid IL or missing references)
		//IL_01cf: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d3: Unknown result type (might be due to invalid IL or missing references)
		//IL_01d8: Unknown result type (might be due to invalid IL or missing references)
		//IL_0119: Unknown result type (might be due to invalid IL or missing references)
		//IL_011f: Invalid comparison between Unknown and I4
		//IL_014f: Unknown result type (might be due to invalid IL or missing references)
		List<MetaMesh> list = new List<MetaMesh>();
		HorseComponent horseComponent = mountItem.HorseComponent;
		uint maneMeshMultiplier = uint.MaxValue;
		MetaMesh multiMesh = mountItem.GetMultiMesh(isFemale: false, hasGloves: false, needBatchedVersion: true);
		MountCreationKey val = null;
		if (string.IsNullOrEmpty(mountCreationKeyStr))
		{
			mountCreationKeyStr = MountCreationKey.GetRandomMountKeyString(mountItem, MBRandom.RandomInt());
		}
		val = MountCreationKey.FromString(mountCreationKeyStr);
		if ((int)mountItem.ItemType == 1)
		{
			SetHorseColors(multiMesh, val);
		}
		if (horseComponent.HorseMaterialNames != null && ((List<MaterialProperty>)(object)horseComponent.HorseMaterialNames).Count > 0)
		{
			SetMaterialProperties(mountItem, multiMesh, val, ref maneMeshMultiplier);
		}
		int nondeterministicRandomInt = MBRandom.NondeterministicRandomInt;
		SetVoiceDefinition(agent, nondeterministicRandomInt);
		MetaMesh val2 = null;
		if (harnessItem != null)
		{
			val2 = harnessItem.GetMultiMesh(isFemale: false, hasGloves: false, needBatchedVersion: true);
		}
		foreach (KeyValuePair<string, bool> additionalMeshesName in horseComponent.AdditionalMeshesNameList)
		{
			if (additionalMeshesName.Key.Length <= 0)
			{
				continue;
			}
			string text = additionalMeshesName.Key;
			if (harnessItem == null || !additionalMeshesName.Value)
			{
				MetaMesh copy = MetaMesh.GetCopy(text, true, false);
				if (maneMeshMultiplier != uint.MaxValue)
				{
					copy.SetFactor1Linear(maneMeshMultiplier);
				}
				list.Add(copy);
				continue;
			}
			ArmorComponent armorComponent = harnessItem.ArmorComponent;
			if (armorComponent != null && (int)armorComponent.ManeCoverType == 3)
			{
				continue;
			}
			ArmorComponent armorComponent2 = harnessItem.ArmorComponent;
			if (armorComponent2 != null && (int)armorComponent2.ManeCoverType > 0)
			{
				string text2 = text;
				HorseHarnessCoverTypes? obj;
				if (harnessItem == null)
				{
					obj = null;
				}
				else
				{
					ArmorComponent armorComponent3 = harnessItem.ArmorComponent;
					obj = ((armorComponent3 != null) ? new HorseHarnessCoverTypes?(armorComponent3.ManeCoverType) : ((HorseHarnessCoverTypes?)null));
				}
				text = text2 + "_" + obj;
			}
			MetaMesh copy2 = MetaMesh.GetCopy(text, true, false);
			if (maneMeshMultiplier != uint.MaxValue)
			{
				copy2.SetFactor1Linear(maneMeshMultiplier);
			}
			list.Add(copy2);
		}
		if ((NativeObject)(object)multiMesh != (NativeObject)null)
		{
			if (harnessItem != null)
			{
				ArmorComponent armorComponent4 = harnessItem.ArmorComponent;
				if (((armorComponent4 != null) ? new HorseTailCoverTypes?(armorComponent4.TailCoverType) : ((HorseTailCoverTypes?)null)) == (HorseTailCoverTypes?)1)
				{
					multiMesh.RemoveMeshesWithTag("horse_tail");
				}
			}
			list.Add(multiMesh);
		}
		if ((NativeObject)(object)val2 != (NativeObject)null)
		{
			if ((NativeObject)(object)agentVisual != (NativeObject)null)
			{
				MetaMesh val3 = null;
				if (NativeConfig.CharacterDetail > 2 && harnessItem.ArmorComponent != null)
				{
					val3 = MetaMesh.GetCopy(harnessItem.ArmorComponent.ReinsRopeMesh, false, true);
				}
				ArmorComponent armorComponent5 = harnessItem.ArmorComponent;
				MetaMesh copy3 = MetaMesh.GetCopy((armorComponent5 != null) ? armorComponent5.ReinsMesh : null, false, true);
				if ((NativeObject)(object)val3 != (NativeObject)null && (NativeObject)(object)copy3 != (NativeObject)null)
				{
					agentVisual.AddHorseReinsClothMesh(copy3, val3);
					((NativeObject)val3).ManualInvalidate();
				}
				if ((NativeObject)(object)copy3 != (NativeObject)null)
				{
					list.Add(copy3);
				}
			}
			else if (harnessItem.ArmorComponent != null)
			{
				MetaMesh copy4 = MetaMesh.GetCopy(harnessItem.ArmorComponent.ReinsMesh, true, true);
				if ((NativeObject)(object)copy4 != (NativeObject)null)
				{
					list.Add(copy4);
				}
			}
			list.Add(val2);
		}
		return list;
	}

	public static void SetHorseColors(MetaMesh horseMesh, MountCreationKey mountCreationKey)
	{
		horseMesh.SetVectorArgument((float)(int)mountCreationKey._leftFrontLegColorIndex, (float)(int)mountCreationKey._rightFrontLegColorIndex, (float)(int)mountCreationKey._leftBackLegColorIndex, (float)(int)mountCreationKey._rightBackLegColorIndex);
	}

	public static void ClearMountMesh(GameEntity gameEntity)
	{
		gameEntity.RemoveAllChildren();
		gameEntity.Remove(106);
	}

	private static void SetVoiceDefinition(Agent agent, int seedForRandomVoiceTypeAndPitch)
	{
		MBAgentVisuals val = ((agent != null) ? agent.AgentVisuals : null);
		if ((NativeObject)(object)val != (NativeObject)null)
		{
			string soundAndCollisionInfoClassName = agent.GetSoundAndCollisionInfoClassName();
			int num = ((!string.IsNullOrEmpty(soundAndCollisionInfoClassName)) ? SkinVoiceManager.GetVoiceDefinitionCountWithMonsterSoundAndCollisionInfoClassName(soundAndCollisionInfoClassName) : 0);
			if (num == 0)
			{
				val.SetVoiceDefinitionIndex(-1, 0f);
				return;
			}
			int num2 = MathF.Abs(seedForRandomVoiceTypeAndPitch);
			float num3 = (float)num2 * 4.656613E-10f;
			int[] array = new int[num];
			SkinVoiceManager.GetVoiceDefinitionListWithMonsterSoundAndCollisionInfoClassName(soundAndCollisionInfoClassName, array);
			int num4 = array[num2 % num];
			val.SetVoiceDefinitionIndex(num4, num3);
		}
	}

	public static void AddMountMeshToEntity(GameEntity gameEntity, ItemObject mountItem, ItemObject harnessItem, string mountCreationKeyStr, Agent agent = null)
	{
		foreach (MetaMesh item in AddMountMesh(null, mountItem, harnessItem, mountCreationKeyStr, agent))
		{
			gameEntity.AddMultiMeshToSkeleton(item);
			((NativeObject)item).ManualInvalidate();
		}
	}

	public static void AddMountMeshToAgentVisual(MBAgentVisuals agentVisual, ItemObject mountItem, ItemObject harnessItem, string mountCreationKeyStr, Agent agent = null)
	{
		//IL_0062: Unknown result type (might be due to invalid IL or missing references)
		foreach (MetaMesh item in AddMountMesh(agentVisual, mountItem, harnessItem, mountCreationKeyStr, agent))
		{
			agentVisual.AddMultiMesh(item, (BodyMeshTypes)(-1));
			((NativeObject)item).ManualInvalidate();
		}
		HorseComponent horseComponent = mountItem.HorseComponent;
		if (((horseComponent != null) ? horseComponent.SkeletonScale : null) != null)
		{
			agentVisual.ApplySkeletonScale(mountItem.HorseComponent.SkeletonScale.MountSitBoneScale, mountItem.HorseComponent.SkeletonScale.MountRadiusAdder, mountItem.HorseComponent.SkeletonScale.BoneIndices, mountItem.HorseComponent.SkeletonScale.Scales);
		}
	}
}

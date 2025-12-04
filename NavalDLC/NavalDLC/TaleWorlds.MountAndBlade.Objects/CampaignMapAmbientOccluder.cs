using System.Collections.Generic;
using NavalDLC;
using NavalDLC.Map;
using TaleWorlds.Engine;
using TaleWorlds.Library;

namespace TaleWorlds.MountAndBlade.Objects;

internal class CampaignMapAmbientOccluder : ScriptComponentBehavior
{
	protected override void OnInit()
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0036: Unknown result type (might be due to invalid IL or missing references)
		//IL_003b: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		Mesh firstMesh = ((WeakGameEntity)(ref gameEntity)).GetFirstMesh();
		int num = MathF.Max(NavalDLCManager.Instance.GameModels.MapStormModel.MaximumNumberOfStorms, 16);
		firstMesh.SetupAdditionalBoneBuffer(num);
		for (int i = 0; i < num; i++)
		{
			MatrixFrame zero = MatrixFrame.Zero;
			firstMesh.SetAdditionalBoneFrame(i, ref zero);
		}
	}

	protected override void OnTick(float dt)
	{
		//IL_0001: Unknown result type (might be due to invalid IL or missing references)
		//IL_0006: Unknown result type (might be due to invalid IL or missing references)
		//IL_0032: Unknown result type (might be due to invalid IL or missing references)
		//IL_0037: Unknown result type (might be due to invalid IL or missing references)
		//IL_004f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0054: Unknown result type (might be due to invalid IL or missing references)
		//IL_0061: Unknown result type (might be due to invalid IL or missing references)
		//IL_0066: Unknown result type (might be due to invalid IL or missing references)
		//IL_006f: Unknown result type (might be due to invalid IL or missing references)
		//IL_0074: Unknown result type (might be due to invalid IL or missing references)
		//IL_00bd: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c2: Unknown result type (might be due to invalid IL or missing references)
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		Mesh firstMesh = ((WeakGameEntity)(ref gameEntity)).GetFirstMesh();
		int i = 0;
		foreach (Storm item in (List<Storm>)(object)NavalDLCManager.Instance.StormManager.SpawnedStorms)
		{
			MatrixFrame identity = MatrixFrame.Identity;
			Vec3 val = new Vec3(60f, 0f, 0f, -1f);
			((MatrixFrame)(ref identity)).Scale(ref val);
			Vec2 currentPosition = item.CurrentPosition;
			identity.origin = ((Vec2)(ref currentPosition)).ToVec3(0f);
			firstMesh.SetAdditionalBoneFrame(i, ref identity);
			i++;
		}
		for (int num = MathF.Max(NavalDLCManager.Instance.GameModels.MapStormModel.MaximumNumberOfStorms, 16); i < num; i++)
		{
			MatrixFrame zero = MatrixFrame.Zero;
			firstMesh.SetAdditionalBoneFrame(i, ref zero);
		}
	}

	protected override void OnEditorInit()
	{
	}

	protected override void OnEditorTick(float dt)
	{
	}

	public override TickRequirement GetTickRequirement()
	{
		return (TickRequirement)2;
	}
}

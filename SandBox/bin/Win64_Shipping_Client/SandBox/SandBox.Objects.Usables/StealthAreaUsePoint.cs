using System;
using System.Collections.Generic;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.InputSystem;
using TaleWorlds.Library;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;

namespace SandBox.Objects.Usables;

public class StealthAreaUsePoint : UsableMissionObject
{
	private bool _isEnabled;

	public string ActionStringId;

	public string DescriptionStringId;

	private bool _isAlreadyUsed;

	public StealthAreaUsePoint()
		: base(false)
	{
	}

	protected override void OnInit()
	{
		((UsableMissionObject)this).OnInit();
		_isAlreadyUsed = false;
		base.ActionMessage = GameTexts.FindText(string.IsNullOrEmpty(ActionStringId) ? "str_call_troops" : ActionStringId, (string)null);
		base.ActionMessage.SetTextVariable("KEY", HyperlinkTexts.GetKeyHyperlinkText(HotKeyManager.GetHotKeyId("CombatHotKeyCategory", 13), 1f));
		base.DescriptionMessage = GameTexts.FindText(string.IsNullOrEmpty(DescriptionStringId) ? "str_call_troops_description" : DescriptionStringId, (string)null);
	}

	public override TextObject GetDescriptionText(WeakGameEntity gameEntity)
	{
		return base.DescriptionMessage;
	}

	public override void OnUse(Agent userAgent, sbyte agentBoneIndex)
	{
		//IL_0016: Unknown result type (might be due to invalid IL or missing references)
		//IL_001b: Unknown result type (might be due to invalid IL or missing references)
		((UsableMissionObject)this).OnUse(userAgent, agentBoneIndex);
		if (userAgent.IsMainAgent)
		{
			Vec3 position = userAgent.Position;
			SoundManager.StartOneShotEvent("event:/mission/combat/pickup_arrows", ref position);
			_isAlreadyUsed = true;
			userAgent.StopUsingGameObject(true, (StopUsingGameObjectFlags)1);
		}
		DisableAgentAIs();
	}

	public override void OnUseStopped(Agent userAgent, bool isSuccessful, int preferenceIndex)
	{
		((UsableMissionObject)this).OnUseStopped(userAgent, isSuccessful, preferenceIndex);
		if (((UsableMissionObject)this).LockUserFrames || ((UsableMissionObject)this).LockUserPositions)
		{
			userAgent.ClearTargetFrame();
		}
	}

	public void DisableAgentAIs()
	{
		//IL_003e: Unknown result type (might be due to invalid IL or missing references)
		WorldPosition val = default(WorldPosition);
		foreach (Agent item in (List<Agent>)(object)Mission.Current.Agents)
		{
			if (item.IsActive() && item.IsAIControlled)
			{
				item.SetIsAIPaused(true);
				((WorldPosition)(ref val))._002Ector(Mission.Current.Scene, item.Position);
				item.SetScriptedPosition(ref val, false, (AIScriptedFrameFlags)0);
			}
		}
	}

	public override bool IsDisabledForAgent(Agent agent)
	{
		if (!agent.IsMainAgent)
		{
			if (!_isAlreadyUsed)
			{
				return !_isEnabled;
			}
			return true;
		}
		return false;
	}

	public override bool IsUsableByAgent(Agent userAgent)
	{
		if (userAgent.IsMainAgent && !_isAlreadyUsed && _isEnabled)
		{
			return !IsInCombat();
		}
		return false;
	}

	private bool IsInCombat()
	{
		//IL_0025: Unknown result type (might be due to invalid IL or missing references)
		bool result = false;
		foreach (Agent item in (List<Agent>)(object)Mission.Current.AllAgents)
		{
			if (item.IsActive() && ((Enum)item.AIStateFlags).HasFlag((Enum)(object)(AIStateFlag)3))
			{
				result = true;
				break;
			}
		}
		return result;
	}

	public void EnableStealthAreaUsePoint()
	{
		//IL_000d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Unknown result type (might be due to invalid IL or missing references)
		//IL_001a: Unknown result type (might be due to invalid IL or missing references)
		_isEnabled = true;
		WeakGameEntity gameEntity = ((ScriptComponentBehavior)this).GameEntity;
		Vec3 globalPosition = ((WeakGameEntity)(ref gameEntity)).GlobalPosition;
		SoundManager.StartOneShotEvent("event:/ui/notification/quest_update", ref globalPosition);
	}
}

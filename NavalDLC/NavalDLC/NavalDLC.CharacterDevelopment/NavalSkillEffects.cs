using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Localization;

namespace NavalDLC.CharacterDevelopment;

public class NavalSkillEffects
{
	private SkillEffect _effectWindBonus;

	private SkillEffect _effectNavalAutoBattleSimulationAdvantage;

	private SkillEffect _effectNavalAutoBattleCombatPenaltyNegation;

	private SkillEffect _effectNavalBattleCombatPenaltyNegation;

	private SkillEffect _effectShipDamageReduction;

	private static NavalSkillEffects Instance => NavalDLCManager.Instance.NavalSkillEffects;

	public static SkillEffect WindBonus => Instance._effectWindBonus;

	public static SkillEffect NavalAutoBattleSimulationAdvantage => Instance._effectNavalAutoBattleSimulationAdvantage;

	public static SkillEffect NavalAutoBattleCombatPenaltyNegation => Instance._effectNavalAutoBattleCombatPenaltyNegation;

	public static SkillEffect NavalBattleCombatPenaltyNegation => Instance._effectNavalBattleCombatPenaltyNegation;

	public static SkillEffect ShipDamageReduction => Instance._effectShipDamageReduction;

	public NavalSkillEffects()
	{
		RegisterAll();
	}

	private void RegisterAll()
	{
		_effectWindBonus = Create("WindBonus");
		_effectNavalAutoBattleSimulationAdvantage = Create("NavalAutoBattleSimulationAdvantage");
		_effectNavalAutoBattleCombatPenaltyNegation = Create("NavalAutoBattleCombatPenaltyNegation");
		_effectNavalBattleCombatPenaltyNegation = Create("NavalBattleCombatPenaltyNegation");
		_effectShipDamageReduction = Create("ShipDamageReduction");
		InitializeAll();
	}

	private SkillEffect Create(string stringId)
	{
		//IL_000b: Unknown result type (might be due to invalid IL or missing references)
		//IL_0015: Expected O, but got Unknown
		return Game.Current.ObjectManager.RegisterPresumedObject<SkillEffect>(new SkillEffect(stringId));
	}

	private void InitializeAll()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0031: Expected O, but got Unknown
		//IL_003d: Unknown result type (might be due to invalid IL or missing references)
		//IL_0062: Expected O, but got Unknown
		//IL_006e: Unknown result type (might be due to invalid IL or missing references)
		//IL_0093: Expected O, but got Unknown
		//IL_009f: Unknown result type (might be due to invalid IL or missing references)
		//IL_00c4: Expected O, but got Unknown
		//IL_00d0: Unknown result type (might be due to invalid IL or missing references)
		//IL_00f5: Expected O, but got Unknown
		_effectWindBonus.Initialize(new TextObject("{=LxA3WTjm}Sailing speed increased by {a0}%", (Dictionary<string, object>)null), NavalSkills.Shipmaster, (PartyRole)5, 0.0005f, (EffectIncrementType)1, 0f, float.MinValue, float.MaxValue);
		_effectNavalAutoBattleSimulationAdvantage.Initialize(new TextObject("{=Z2uaBxah}Naval simulation advantage: +{a0}%", (Dictionary<string, object>)null), NavalSkills.Mariner, (PartyRole)5, 0.001f, (EffectIncrementType)1, 0f, float.MinValue, float.MaxValue);
		_effectNavalAutoBattleCombatPenaltyNegation.Initialize(new TextObject("{=7XMyYI9e}Naval Auto Battle Combat Penalty Negation Effect", (Dictionary<string, object>)null), NavalSkills.Mariner, (PartyRole)5, 0.5f, (EffectIncrementType)1, 0f, float.MinValue, float.MaxValue);
		_effectNavalBattleCombatPenaltyNegation.Initialize(new TextObject("{=k6EubLby}Naval Battle Combat Penalty Negation Effect", (Dictionary<string, object>)null), NavalSkills.Mariner, (PartyRole)5, -0.005f, (EffectIncrementType)1, 0f, float.MinValue, float.MaxValue);
		_effectShipDamageReduction.Initialize(new TextObject("{=CyZvyfRa}Reduce ships' received damage by {a0}%", (Dictionary<string, object>)null), NavalSkills.Boatswain, (PartyRole)5, -0.0025f, (EffectIncrementType)1, 0f, float.MinValue, float.MaxValue);
	}
}

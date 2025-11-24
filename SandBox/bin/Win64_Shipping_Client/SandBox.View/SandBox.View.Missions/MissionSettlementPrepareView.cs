using System;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.Settlements;
using TaleWorlds.Core;
using TaleWorlds.DotNet;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;
using TaleWorlds.MountAndBlade.View.MissionViews;
using TaleWorlds.MountAndBlade.View.Tableaus.Thumbnails;

namespace SandBox.View.Missions;

[DefaultView]
public class MissionSettlementPrepareView : MissionView
{
	public const string BannerTagId = "bd_banner_b";

	public override void AfterStart()
	{
		((MissionBehavior)this).AfterStart();
		SetOwnerBanner();
	}

	private void SetOwnerBanner()
	{
		//IL_000c: Unknown result type (might be due to invalid IL or missing references)
		//IL_0012: Invalid comparison between Unknown and I4
		//IL_00af: Unknown result type (might be due to invalid IL or missing references)
		//IL_00b4: Unknown result type (might be due to invalid IL or missing references)
		Campaign current = Campaign.Current;
		if (current == null || (int)current.GameMode != 1)
		{
			return;
		}
		Settlement currentSettlement = Settlement.CurrentSettlement;
		object obj;
		if (currentSettlement == null)
		{
			obj = null;
		}
		else
		{
			Clan ownerClan = currentSettlement.OwnerClan;
			obj = ((ownerClan != null) ? ownerClan.Banner : null);
		}
		if (obj == null || !((NativeObject)(object)((MissionBehavior)this).Mission.Scene != (NativeObject)null))
		{
			return;
		}
		foreach (GameEntity item in ((MissionBehavior)this).Mission.Scene.FindEntitiesWithTag("bd_banner_b"))
		{
			_ = item;
			Action<Texture> action = delegate(Texture tex)
			{
				Material material = Mesh.GetFromResource("bd_banner_b").GetMaterial();
				uint num = (uint)material.GetShader().GetMaterialShaderFlagMask("use_tableau_blending", true);
				ulong shaderFlags = material.GetShaderFlags();
				material.SetShaderFlags(shaderFlags | num);
				material.SetTexture((MBTextureType)1, tex);
			};
			Banner banner = Settlement.CurrentSettlement.OwnerClan.Banner;
			BannerDebugInfo val = BannerDebugInfo.CreateManual(((object)this).GetType().Name);
			BannerVisualExtensions.GetTableauTextureLarge(banner, ref val, action);
		}
	}
}

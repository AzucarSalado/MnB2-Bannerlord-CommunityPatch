using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using Helpers;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using static System.Reflection.BindingFlags;
using static CommunityPatch.HarmonyHelpers;

namespace CommunityPatch.Patches.Perks.Cunning.Roguery {

  public sealed class RaidingPartyPatch : PerkPatchBase<RaidingPartyPatch> {

    public override bool Applied { get; protected set; }

    private static readonly MethodInfo TargetMethodInfo = RaidingHelper.TargetMethodInfo;

    private static readonly MethodInfo PatchMethodInfo = typeof(RaidingPartyPatch).GetMethod(nameof(Prefix), Public | NonPublic | Static | DeclaredOnly);

    public override IEnumerable<MethodBase> GetMethodsChecked() {
      yield return TargetMethodInfo;
    }

    public static byte[][] Hashes => RaidingHelper.Hashes;

    public RaidingPartyPatch() : base("pI0j13oK") {
    }

    public override bool? IsApplicable(Game game) {
      if (Perk == null) return false;
      if (Perk.PrimaryBonus.IsDifferentFrom(.10f)) return false;
      if (TargetMethodInfo == null) return false;
      if (RaidingHelper.NextSettlementDamage == null) return false;
      if (RaidingHelper.IsFinishCalled == null) return false;

      var patchInfo = Harmony.GetPatchInfo(TargetMethodInfo);
      if (AlreadyPatchedByOthers(patchInfo)) return false;

      var hash = TargetMethodInfo.MakeCilSignatureSha256();
      return hash.MatchesAnySha256(Hashes);
    }

    public override void Apply(Game game) {
      Perk.SetPrimaryBonus(10f);

      if (Applied) return;

      CommunityPatchSubModule.Harmony.Patch(TargetMethodInfo, new HarmonyMethod(PatchMethodInfo));
      Applied = true;
    }

    public static void Prefix(ref MapEvent __instance) {
      if (RaidingHelper.IsNotRaidingEvent(__instance)) return;
      if (RaidingHelper.IsTheRaidHitNotHappeningNow(__instance, out var damageAccumulated)) return;

      ApplyPerkBonusToRaidHit(__instance, damageAccumulated);
    }

    private static void ApplyPerkBonusToRaidHit(MapEvent __instance, float hitDamage) {
      var perk = ActivePatch.Perk;
      var partyMemberHitDamage = new ExplainedNumber(hitDamage);

      foreach (var party in __instance.AttackerSide.Parties.Where(x => x.MobileParty != null))
        PerkHelper.AddPerkBonusForParty(perk, party.MobileParty, ref partyMemberHitDamage);

      RaidingHelper.SetHitDamage(__instance, partyMemberHitDamage.ResultNumber);
    }

  }

}
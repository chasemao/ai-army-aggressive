using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaleWorlds.Core;
using TaleWorlds.Localization;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using HarmonyLib;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Party;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.CampaignSystem.SandBox.GameComponents;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors.AiBehaviors;
using TaleWorlds.CampaignSystem.Siege;
using TaleWorlds.CampaignSystem.Actions;
using TaleWorlds.CampaignSystem.SandBox.GameComponents.Map;
using TaleWorlds.CampaignSystem.ViewModelCollection.Craft.WeaponDesign;
using System.Collections.ObjectModel;
using TaleWorlds.Library;
using TaleWorlds.Engine;
using Newtonsoft.Json;

namespace AIArmyAggressive
{
    public class Main : MBSubModuleBase
    {
        private Harmony harmonyKit;
        protected override void OnBeforeInitialModuleScreenSetAsRoot()
        {
            base.OnBeforeInitialModuleScreenSetAsRoot();
            try
            {
                this.harmonyKit = new Harmony("AIArmyAggressive.harmony");
                this.harmonyKit.PatchAll();
                InformationManager.DisplayMessage(new InformationMessage("AIArmyAggressive loaded"));
            }
            catch (Exception ex)
            {
                FileLog.Log("err:" + ex.ToString());
                FileLog.FlushBuffer();
                InformationManager.DisplayMessage(new InformationMessage("err:" + ex.ToString()));
            }
        }
    }
    [HarmonyPatch(typeof(AiArmyMemberBehavior), "AiHourlyTick")]
    public class PatchAiArmyMemberBehaviorAiHourlyTick
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            if (MyUtils.IsInMainClanAndDuringSiegeAndLastSiegeInThreeDays(mobileParty))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AiBanditPatrollingBehavior), "AiHourlyTick")]
    public class PatchAiBanditPatrollingBehaviorAiHourlyTick
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            if (MyUtils.IsInMainClanAndDuringSiegeAndLastSiegeInThreeDays(mobileParty))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AiEngagePartyBehavior), "AiHourlyTick")]
    public class PatchAiEngagePartyBehaviorAiHourlyTick
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            if (MyUtils.IsInMainClanAndDuringSiegeAndLastSiegeInThreeDays(mobileParty))
            {
                return false;
            }
            return true;
        }
    }
    public static class MyUtils
    {
        private static Dictionary<MobileParty, CampaignTime> mobileParty2CampaignTime;
        public static bool IsInMainClanAndDuringSiegeAndLastSiegeInThreeDays(MobileParty mobileParty)
        {
            if (mobileParty.ActualClan != Clan.PlayerClan)
            {
                return false;
            }
            if (mobileParty.DefaultBehavior != AiBehavior.BesiegeSettlement)
            {
                return false;
            }
            if (mobileParty2CampaignTime == null)
            {
                mobileParty2CampaignTime = new Dictionary<MobileParty, CampaignTime>();
            }
            if (mobileParty2CampaignTime.ContainsKey(mobileParty))
            {
                CampaignTime startTime = mobileParty2CampaignTime[mobileParty];
                if (startTime.ElapsedDaysUntilNow < 3f)
                {
                    return true;
                }
                else
                {
                    mobileParty2CampaignTime.Remove(mobileParty);
                    return false;
                }
            }
            else
            {
                mobileParty2CampaignTime.Add(mobileParty, CampaignTime.Now);
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AiMilitaryBehavior), "AiHourlyTick")]
    public class PatchAiMilitaryBehaviorAiHourlyTick
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            if (MyUtils.IsInMainClanAndDuringSiegeAndLastSiegeInThreeDays(mobileParty))
            {
                return false;
            }
            return true;
        }
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> source = new List<CodeInstruction>(instructions);
            if (source.Count == 578)
                source.RemoveRange(375, 26);
            return source.AsEnumerable<CodeInstruction>();
        }
    }

    [HarmonyPatch(typeof(AiPatrollingBehavior), "AiHourlyTick")]
    public class PatchAiPatrollingBehaviorAiHourlyTick
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            if (MyUtils.IsInMainClanAndDuringSiegeAndLastSiegeInThreeDays(mobileParty))
            {
                return false;
            }
            return true;
        }
    }

    [HarmonyPatch(typeof(AiVisitSettlementBehavior), "AiHourlyTick")]
    public class PatchAiVisitSettlementBehaviorAiHourlyTick
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            if (MyUtils.IsInMainClanAndDuringSiegeAndLastSiegeInThreeDays(mobileParty))
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(AiPartyThinkBehavior), "PartyHourlyAiTick")]
    public class PatchPartyHourlyAiTick
    {
        public static bool Prefix(MobileParty mobileParty)
        {
            if (MyUtils.IsInMainClanAndDuringSiegeAndLastSiegeInThreeDays(mobileParty))
            {
                return false;
            }
            return true;
        }
    }
    /*    [HarmonyPatch(typeof(MobileParty), "OnAiTickInternal")]
    public class PatchOnAiTickInternal
    {
        public static bool Prefix(MobileParty __instance)
        {
            if (MyUtils.IsDuringSiegeAndLastSiegeInThreeDays(__instance))
            {
                return false;
            }
            return true;
        }
    }*/
    [HarmonyPatch(typeof(DefaultArmyManagementCalculationModel), "GetMobilePartiesToCallToArmy")]
    public class PatchGetMobilePartiesToCallToArmy
    {
        public static void Postfix(MobileParty leaderParty, List<MobileParty> __result)
        {
            if (leaderParty.ActualClan != Clan.PlayerClan)
            {
                return;
            }
            if (__result.Count == 0)
            {
                return;
            }
            if (leaderParty.ActualClan.Kingdom.Armies.Count >= 20)
            {
                __result.Clear();
                return;
            }
            while (__result.Count > 10)
            {
                __result.RemoveAt(__result.Count - 1);
            }
            return;
        }
    }
    /*[HarmonyPatch(typeof(DefaultLogsCampaignBehavior), "OnArmyCreated")]
    public class PatchOnArmyCreated
    {
        public static bool Prefix()
        {
            return false;
        }
    }*/
    [HarmonyPatch(typeof(RansomOfferCampaignBehavior), "DailyTickHero")]
    public class PatchDailyTickHero
    {
        public static bool Prefix(Hero hero)
        {
            if (hero.Clan != Clan.PlayerClan)
            {
                return false;
            }
            return true;
        }
    }
    [HarmonyPatch(typeof(DefaultClanTierModel), "GetPartyLimitForTier")]
    public class PatchGetPartyLimitForTier
    {
        public static void Postfix(ref int __result)
        {
            __result = 10000;
        }
    }
    [HarmonyPatch(typeof(DefaultClanTierModel), "GetCompanionLimitFromTier")]
    public class PatchGetCompanionLimitFromTier
    {
        public static void Postfix(ref int __result)
        {
            __result = 10000;
        }
    }
    [HarmonyPatch(typeof(UrbanCharactersCampaignBehavior), "WeeklyTick")]
    public class PatchWeeklyTick
    {
        public static void Prefix(ref float ____randomCompanionSpawnFrequencyInWeeks)
        {
            ____randomCompanionSpawnFrequencyInWeeks = 1f;
        }
    }
    [HarmonyPatch(typeof(Hero), "CanLeadParty")]
    public class PatchCanLeadParty
    {
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
    [HarmonyPatch(typeof(UrbanCharactersCampaignBehavior), "OnSettlementEntered")]
    public class PatchOnSettlementEntered
    {
        public static bool Prefix(MobileParty mobileParty, Settlement settlement, Hero hero, List<Hero> ____companions)
        {
            if (mobileParty != MobileParty.MainParty || !settlement.IsTown || ____companions.Count <= 0)
                return false;
            foreach (Hero hero1 in ____companions)
            {
                hero1.ChangeState(Hero.CharacterStates.Active);
                EnterSettlementAction.ApplyForCharacterOnly(hero1, settlement);
            }
            ____companions.RemoveAll(h => true);
            return false;
        }
    }
    [HarmonyPatch(typeof(BesiegerCamp), "StartingAssaultOnBesiegedSettlementIsLogical")]
    public class PatchStartingAssaultOnBesiegedSettlementIsLogical
    {
        public static void Postfix(ref bool __result)
        {
            __result = true;
        }
    }
    [HarmonyPatch(typeof(DefaultPartyWageModel), "MaxWage", MethodType.Getter)]
    public class PatchMaxWage
    {
        public static void Postfix(ref int __result)
        {
            __result = 10000;
        }
    }
}

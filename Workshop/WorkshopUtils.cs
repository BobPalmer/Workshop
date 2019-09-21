﻿using System;
using System.Text;
using Workshop.Recipes;
using ToolbarControl_NS;


namespace Workshop
{
    using System.Linq;

    using UnityEngine;

    using W_KIS;

    public class WorkshopUtils
    {
        public enum ProductivityType { printer, recycler};

        public static float GetProductivityBonus(Part part, string ExperienceEffect, float SpecialistEfficiencyFactor, float ProductivityFactor, ProductivityType ptype)
        {
            float adjustedProductivity = ProductivityFactor;

            try
            {
                int crewCount = part.protoModuleCrew.Count;
                if (crewCount == 0)
                {
                    if (ptype == ProductivityType.printer)
                        return adjustedProductivity / (float)(HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().processingTimeMultiplier * HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().overallTimeMultiplier);
                    else
                        return adjustedProductivity / (float)(HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().recyclingTimeMultiplier * HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().overallTimeMultiplier);
                }

                ProtoCrewMember worker;
                GameParameters.AdvancedParams advancedParams = HighLogic.CurrentGame.Parameters.CustomParams<GameParameters.AdvancedParams>();
                float productivityBonus = 1.0f;

                //Find all crews with the build skill and adjust productivity based upon their skill
                for (int index = 0; index < crewCount; index++)
                {
                    worker = part.protoModuleCrew[index];

                    //Adjust productivity if efficiency is enabled
                    if (WorkshopOptions.EfficiencyEnabled)
                    {
                        if (worker.HasEffect(ExperienceEffect))
                        {
                            if (advancedParams.EnableKerbalExperience)
                                productivityBonus = worker.experienceTrait.CrewMemberExperienceLevel() * SpecialistEfficiencyFactor;
                            else
                                productivityBonus = 5.0f * SpecialistEfficiencyFactor;
                        }

                        //Adjust for stupidity
                        if (WorkshopOptions.StupidityAffectsEfficiency)
                        {
                            productivityBonus *= (1 - worker.stupidity);
                        }
                        adjustedProductivity += productivityBonus;
                    }
                }
            }
            catch (Exception ex)
            {
                LogError("Error encountered while trying to calculate productivity bonus", ex);
            }

            if (ptype == ProductivityType.printer)
                return adjustedProductivity / (float)(HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().processingTimeMultiplier * HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().overallTimeMultiplier);
            else
                return adjustedProductivity / (float)(HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().recyclingTimeMultiplier * HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().overallTimeMultiplier);
        }

        public static float GetPackedPartVolume(AvailablePart part)
        {
            var moduleKisItem = KISWrapper.GetKisItem(part.partPrefab);
            return moduleKisItem != null ? moduleKisItem.volumeOverride : KIS_Shared.GetPartVolume(part);
        }

        public static bool IsOccupied(ModuleKISInventory inventory)
        {
            return
                inventory.invType != ModuleKISInventory.InventoryType.Pod ||
                inventory.part.protoModuleCrew.Any(protoCrewMember => protoCrewMember.seatIdx == inventory.podSeat);
        }

        public static bool HasFreeSpace(ModuleKISInventory inventory, WorkshopItem item)
        {
            return inventory.GetContentVolume() + KIS_Shared.GetPartVolume(item.Part) <= inventory.maxVolume;
        }

        public static bool HasFreeSlot(ModuleKISInventory inventory)
        {
            return !inventory.isFull();
        }

        public static bool PartResearched(AvailablePart p)
        {
            return ResearchAndDevelopment.PartTechAvailable(p) && ResearchAndDevelopment.PartModelPurchased(p);
        }

        public static Texture2D LoadTexture(string path)
        {
            Texture2D tex = new Texture2D(25,25) ;
            ToolbarControl.LoadImageFromFile(ref tex, "GameData/" + path);
            return tex;
#if false
            var texture = GameDatabase.Instance.GetTexture(path, false);
            if (texture == null)
            {
                LogError($"Filter - Unable to load texture file {path}");
                return new Texture2D(25, 25);
            }
            return texture;
#endif
        }

        public static string GetKisStats(AvailablePart part)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Mass: " + part.partPrefab.mass + " tons");
            sb.AppendLine("Volume: " + KIS_Shared.GetPartVolume(part).ToString("0.0") + " litres");
            sb.AppendLine("Costs: " + part.cost + "$");

            foreach (var resourceInfo in part.partPrefab.Resources)
            {
                if (WorkshopRecipeDatabase.HasResourceRecipe(resourceInfo.resourceName))
                {
                    sb.AppendLine(resourceInfo.resourceName + ": " + resourceInfo.maxAmount + " / " + resourceInfo.maxAmount);
                }
                else
                {
                    sb.AppendLine(resourceInfo.resourceName + ": 0 / " + resourceInfo.maxAmount);
                }
            }
            return sb.ToString();
        }

        public static void Log(string message)
        {
            Debug.Log(string.Format("[OSE] - {0}", message));
        }

        public static void LogError(string message)
        {
            Debug.LogError(string.Format("[OSE] - {0}", message));
        }
        public static void LogError(string message, Exception ex)
        {
            Debug.LogError(string.Format("[OSE] - {0}", message));
            Debug.LogException(ex);
        }

        public static void LogVerbose(string message)
        {
            if (GameSettings.VERBOSE_DEBUG_LOG)
                Log(message);
        }

        public static bool PreLaunch()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().noLocalProcessing)
                return false;
            if (FlightGlobals.ActiveVessel.situation == Vessel.Situations.PRELAUNCH)
                return true;
            if (FlightGlobals.ActiveVessel.situation == Vessel.Situations.LANDED && (
                FlightGlobals.ActiveVessel.LandedInKSC || FlightGlobals.ActiveVessel.LandedInStockLaunchSite))
                return true;

            return false;
        }
    }
}

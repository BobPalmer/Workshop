namespace Workshop
{
    public class WorkshopOptions : GameParameters.CustomParameterNode
    {
        [GameParameters.CustomParameterUI("Experience affects efficiency", toolTip = "If enabled, then engineering skills can improve efficiency.", autoPersistance = true)]
        public bool enableEfficiency = true;

        [GameParameters.CustomParameterUI("Stupidity affects efficiency", toolTip = "If enabled, stupidity affects efficiency; the lower the better.", autoPersistance = true)]
        public bool stupidityAffectsEfficiency = false;

        [GameParameters.CustomParameterUI("Printing parts costs Funds", toolTip = "If enabled, printing new parts will cost Funds in addition to resources.", autoPersistance = true)]
        public bool partsCostFunds = true;

        [GameParameters.CustomParameterUI("Enable Recipes", toolTip = "If enabled, printing new parts may require a variety of different resources.", autoPersistance = true)]
        public bool enableRecipes = true;

        [GameParameters.CustomParameterUI("Print requests create KAC alarms", toolTip = "If enabled, print jobs will add a KAC alarm.", autoPersistance = true)]
        public bool enableKACIntegration = true;

        public override string DisplaySection => Section;

        public static bool EfficiencyEnabled
        {
            get
            {
                WorkshopOptions options = HighLogic.CurrentGame.Parameters.CustomParams<WorkshopOptions>();
                return options.enableEfficiency;
            }
        }

        public static bool StupidityAffectsEfficiency
        {
            get
            {
                WorkshopOptions options = HighLogic.CurrentGame.Parameters.CustomParams<WorkshopOptions>();
                return options.stupidityAffectsEfficiency;
            }
        }

        public static bool PrintingCostsFunds
        {
            get
            {
                WorkshopOptions options = HighLogic.CurrentGame.Parameters.CustomParams<WorkshopOptions>();
                return options.partsCostFunds;
            }
        }

        public static bool EnableRecipes
        {
            get
            {
                WorkshopOptions options = HighLogic.CurrentGame.Parameters.CustomParams<WorkshopOptions>();
                return options.enableRecipes;
            }
        }

        public static bool EnableKACIntegration
        {
            get
            {
                WorkshopOptions options = HighLogic.CurrentGame.Parameters.CustomParams<WorkshopOptions>();
                return options.enableKACIntegration;
            }
        }

        #region CustomParameterNode
        public override string Section =>"Workshop";

        public override string Title => "Efficiency";

        public override int SectionOrder => 0;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            base.SetDifficultyPreset(preset);
        }

        public override GameParameters.GameMode GameMode => GameParameters.GameMode.ANY;

        public override bool HasPresets => false;

        public override bool Enabled(System.Reflection.MemberInfo member, GameParameters parameters)
        {
            if (member.Name == "stupidityAffectsEfficiency" && enableEfficiency)
                return true;
            if (member.Name == "stupidityAffectsEfficiency")
                return false;

            return base.Enabled(member, parameters);
        }
        #endregion
    }
}

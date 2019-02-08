using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

namespace Workshop
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    public class Workshop_Settings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Difficulty"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Workshop"; } }
        public override string DisplaySection { get { return "Difficulty"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return true; } }


        [GameParameters.CustomParameterUI("No local printing or recycling",
            toolTip = "No processing is allowed on any runway or launch facility")]
        public bool noLocalProcessing = false;

        [GameParameters.CustomParameterUI("Use complexity values",
            toolTip = "Only available if recipes are used")]
        public bool useComplexity = true;

        [GameParameters.CustomParameterUI("Create Kerbal Alarm Clock alarms during printing",
            toolTip ="Only useful if Kerbal Alarm Clock is installed")]
        public bool setPrintKAC = true;

        [GameParameters.CustomParameterUI("Create Kerbal Alarm Clock alarms during recycling",
                    toolTip = "Only useful if Kerbal Alarm Clock is installed")]
        public bool setRecycleKAC = true;

        [GameParameters.CustomFloatParameterUI("Overall time multiplier", minValue = 1, maxValue = 30f,
            toolTip ="This will apply to both processing time and recycling time equally")]
        public double overallTimeMultiplier = 1;


        [GameParameters.CustomFloatParameterUI("Processing time multiplier", minValue = 1, maxValue = 30f,
            toolTip = "This will be applied to processing time only")]
        public double processingTimeMultiplier = 1;

        [GameParameters.CustomFloatParameterUI("Recycling time multiplier", minValue = 1, maxValue = 5f,
            toolTip = "This will be applied to the recycling time only")]
        public double recyclingTimeMultiplier = 1;



        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            Debug.Log("Setting difficulty preset");
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    noLocalProcessing = false;
                    overallTimeMultiplier = 1;
                    processingTimeMultiplier = 1f;
                    recyclingTimeMultiplier = 1f;
                    break;

                case GameParameters.Preset.Normal:
                    noLocalProcessing = true;
                    overallTimeMultiplier = 1;
                    processingTimeMultiplier = 10f;
                    recyclingTimeMultiplier = 2f;
                    break;

                case GameParameters.Preset.Moderate:
                    noLocalProcessing = true;
                    overallTimeMultiplier = 2;
                    processingTimeMultiplier = 20f;
                    recyclingTimeMultiplier = 3f;
                    break;

                case GameParameters.Preset.Hard:
                    noLocalProcessing = true;
                    overallTimeMultiplier = 3;
                    processingTimeMultiplier = 30f;
                    recyclingTimeMultiplier = 5f;
                    break;
            }
        }

        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {
            return true; //otherwise return true
        }

        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {

            return true;
            //            return true; //otherwise return true
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }

    }
}

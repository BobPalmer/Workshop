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


        [GameParameters.CustomParameterUI("No local recycling")]
        public bool noLocalRecycling = false;

        [GameParameters.CustomParameterUI("Use complexity values")]
        public bool useComplexity = true;

        [GameParameters.CustomFloatParameterUI("Overall time multiplier", minValue = 1, maxValue = 30f,
            toolTip ="This will apply to both processing time and recycling time equally")]
        public double overallTimeMultiplier = 5;


        [GameParameters.CustomFloatParameterUI("Processing time multiplier", minValue = 1, maxValue = 30f,
            toolTip = "This will be applied to processing time only")]
        public double processingTimeMultiplier = 1;

        [GameParameters.CustomFloatParameterUI("Recycling time multiplier", minValue = 1, maxValue = 30f,
            toolTip = "This will be applied to the recycling time only")]
        public double recyclingTimeMultiplier = 1;

        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            Debug.Log("Setting difficulty preset");
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    noLocalRecycling = false;
                    overallTimeMultiplier = 1;
                    processingTimeMultiplier = 1f;
                    recyclingTimeMultiplier = 1f;
                    break;

                case GameParameters.Preset.Normal:
                    noLocalRecycling = true;
                    overallTimeMultiplier = 5;
                    processingTimeMultiplier = 10f;
                    recyclingTimeMultiplier = 10f;
                    break;

                case GameParameters.Preset.Moderate:
                    noLocalRecycling = true;
                    overallTimeMultiplier = 10;
                    processingTimeMultiplier = 20f;
                    recyclingTimeMultiplier = 20f;
                    break;

                case GameParameters.Preset.Hard:
                    noLocalRecycling = true;
                    overallTimeMultiplier = 15;
                    processingTimeMultiplier = 30f;
                    recyclingTimeMultiplier = 30f;
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

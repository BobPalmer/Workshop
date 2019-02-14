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

        [GameParameters.CustomParameterUI("Require unpacking before use",
            toolTip = "Recycler & Workshop need to be unpacked before use, and packed before acceleration")]
        public bool requireUnpacking = false;

        [GameParameters.CustomParameterUI("Unpacked Accel causes damage",
            toolTip = "If unpacked, high acceleration causes damage")]
        public bool unpackedAccelCausesDamage = false;

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
        public double overallTimeMultiplier = 1f;

        [GameParameters.CustomFloatParameterUI("Processing time multiplier", minValue = 1, maxValue = 30f,
            toolTip = "This will be applied to processing time only")]
        public double processingTimeMultiplier = 1f;

        [GameParameters.CustomFloatParameterUI("Recycling time multiplier", minValue = 1, maxValue = 5f,
            toolTip = "This will be applied to the recycling time only")]
        public double recyclingTimeMultiplier = 1f;

        [GameParameters.CustomFloatParameterUI("G-Force Damage multiplier ", minValue = 1, maxValue = 10f,
            toolTip = "The base G-force damange is multiplied by this")]
        public double geeForceDamageMultipler = 1f;



        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            Debug.Log("Setting difficulty preset");
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    noLocalProcessing = false;
                    requireUnpacking = false;
                    overallTimeMultiplier = 1;
                    processingTimeMultiplier = 1f;
                    recyclingTimeMultiplier = 1f;
                    geeForceDamageMultipler = 1f;
                    break;

                case GameParameters.Preset.Normal:
                    noLocalProcessing = true;
                    requireUnpacking = true;
                    overallTimeMultiplier = 1;
                    processingTimeMultiplier = 10f;
                    recyclingTimeMultiplier = 2f;
                    geeForceDamageMultipler = 2f;
                    break;

                case GameParameters.Preset.Moderate:
                    noLocalProcessing = true;
                    requireUnpacking = true;
                    overallTimeMultiplier = 2;
                    processingTimeMultiplier = 20f;
                    recyclingTimeMultiplier = 3f;
                    geeForceDamageMultipler = 4f;
                    break;

                case GameParameters.Preset.Hard:
                    noLocalProcessing = true;
                    requireUnpacking = true;
                    overallTimeMultiplier = 3;
                    processingTimeMultiplier = 30f;
                    recyclingTimeMultiplier = 5f;
                    geeForceDamageMultipler = 8f;
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



    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    public class Workshop_MiscSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Misc"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Workshop"; } }
        public override string DisplaySection { get { return "Misc"; } }
        public override int SectionOrder { get { return 3; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Use alternate skin")]
        public bool useAlternateSkin = false;

        [GameParameters.CustomParameterUI("Add Misc category",
            toolTip = "Some mods have set the category to 'none', and those parts don't show up in any category (MKS, among others).\n" +
                      "This enables a catchall category.  However, some other mods have deprecated parts, and some\n" +
                      "use the category to prevent part from showing if they aren't 'real' parts (MechJeb, among others)")]
        public bool showMiscCategory = false;


        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {

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

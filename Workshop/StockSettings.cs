using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;
using KSPColorPicker;


namespace Workshop
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings
    public class Workshop_Settings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Difficulty"; } } // column heading
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Workshop"; } }
        public override string DisplaySection { get { return "Workshop"; } }
        public override int SectionOrder { get { return 1; } }
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
            if (KSP_ColorPicker.showPicker)
            {
                KSP_ColorPicker.colorPickerInstance.PingTime();
                return false;
            }
            return true;
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
        public override string DisplaySection { get { return "Workshop"; } }
        public override int SectionOrder { get { return 2; } }
        public override bool HasPresets { get { return false; } }

        [GameParameters.CustomParameterUI("Use alternate skin")]
        public bool useAlternateSkin = false;

        [GameParameters.CustomParameterUI("Add Misc category",
            toolTip = "Some mods have set the category to 'none', and those parts don't show up in any category (MKS, among others).\n" +
                      "This enables a catchall category.  However, some other mods have deprecated parts, and some\n" +
                      "use the category to prevent part from showing if they aren't 'real' parts (MechJeb, among others)")]
        public bool showMiscCategory = false;

        [GameParameters.CustomParameterUI("Show tooltips")]
        public bool showTooltips = true;


        [GameParameters.CustomParameterUI("Display popup dialog when shortage ocurs",
            toolTip = "Shortages can be Crew, EC, Funds, Resources, or Free Space")]
        public bool showPopup = true;


        [GameParameters.CustomStringParameterUI("Highlight Color Settings", autoPersistance = true, lines = 2, title = "Highlight Color Settings", toolTip = "test string tooltip")]
        public string UIstring = "";

        [GameParameters.CustomParameterUI("Highlight parts which have shortages",
            toolTip = "Shortages can be Crew, EC, Funds, Resources, or Free Space")]
        public bool doHighlighting = true;

        [GameParameters.CustomParameterUI("Stop warp when shortage occurs",
            toolTip = "Shortages can be Crew, EC, Funds, Resources, or Free Space")]
        public bool stopWarp = true;


        [GameParameters.CustomParameterUI("Show Color Picker",
           toolTip = "Show the Color Picker dialog")]
        public bool showColorPicker = false;

        [GameParameters.CustomFloatParameterUI("Red value", minValue = 0, maxValue = 1f, stepCount = 101,displayFormat ="F4",
            toolTip = "Amount of red to be used in the highlight. range is from 0-1")]
        public float highlightRed = 1f;


        [GameParameters.CustomFloatParameterUI("Green value", minValue = 0, maxValue = 1f, stepCount = 101, displayFormat = "F4",
            toolTip = "Amount of green to be used in the highlight. range is from 0-1")]
        public float highlightGreen = 0.01f;


        [GameParameters.CustomFloatParameterUI("Blue value", minValue = 0, maxValue = 1f, stepCount = 101, displayFormat = "F4",
            toolTip = "Amount of blue to be used in the highlight. range is from 0-1")]
        public float highlightBlue = 0.1f;



        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {

        }

        bool unread = false;
        public override bool Interactible(MemberInfo member, GameParameters parameters)
        {
            if (KSP_ColorPicker.showPicker)
            {
                unread = true;
                KSP_ColorPicker.colorPickerInstance.PingTime();
                return false;
            }
            else
            {
                if (KSP_ColorPicker.success && unread)
                {
                    unread = false;
                    highlightBlue = (float)KSP_ColorPicker.SelectedColor.b;
                    highlightGreen = (float)KSP_ColorPicker.SelectedColor.g;
                    highlightRed = (float)KSP_ColorPicker.SelectedColor.r;
                }
            }
            return true;
        }

        public override IList ValidValues(MemberInfo member)
        {
            return null;
        }


        public override bool Enabled(MemberInfo member, GameParameters parameters)
        {

            if (member.Name == "highlightRed" || member.Name == "highlightGreen" || member.Name == "highlightBlue") return false;

            if (showColorPicker)
            {
                showColorPicker = false;
                Color c = new Color(1, 1, 1, 1);
                c.b = highlightBlue;
                c.g = highlightGreen;
                c.r = highlightRed;
                KSP_ColorPicker.CreateColorPicker(c, false, "Colorpicker-Texture");
            }
            return true;
        }


    }

}

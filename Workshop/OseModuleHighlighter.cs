using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using KSP_PartHighlighter;
using Highlighting;


namespace Workshop
{

    public  class OseModuleHighlighter : PartModule
    {
        [KSPField(isPersistant = true)]
        internal bool eventAcknowledged = false;

        [KSPEvent(guiActive = false, guiActiveUnfocused = true, unfocusedRange = 20.0f, guiActiveEditor = false, guiName = "Acknowledge Workshop Condition")]
        public void AcknowledgeCondition()
        {
            eventAcknowledged = true;
            Events["AcknowledgeCondition"].guiActive = false;
        }

        

        [KSPField(isPersistant = true)]
        bool _notEnoughCrew = false;

        internal bool notEnoughCrew { get { return _notEnoughCrew; } set { _notEnoughCrew = value;  } }

        [KSPField(isPersistant = true)]
        bool _notEnoughEC = false;
        internal bool notEnoughEC { get { return _notEnoughEC; } set { _notEnoughEC = value;  } }

        [KSPField(isPersistant = true)]
        bool _notEnoughFunds = false;
        internal bool notEnoughFunds { get { return _notEnoughFunds; } set { _notEnoughFunds = value;  } }

        [KSPField(isPersistant = true)]
        bool _notEnoughResources = false;
        internal bool notEnoughResources { get { return _notEnoughResources; } set { _notEnoughResources = value;  } }

        [KSPField(isPersistant = true)]
        bool _notEnoughFreeSpace = false;
        internal bool notEnoughFreeSpace { get { return _notEnoughFreeSpace; } set { _notEnoughFreeSpace = value;  } }

        bool NotEnough { get {
                bool rc = _notEnoughCrew || _notEnoughEC || _notEnoughFunds || _notEnoughResources || _notEnoughFreeSpace;
                switch (rc)
                {
                    case true:
                        if (!eventAcknowledged)
                            Events["AcknowledgeCondition"].guiActive = true;
                        break;
                    case false:
                        eventAcknowledged = false;
                        ActiveHighlighting = false;
                        break;
                }
                
                return rc;
            }
        }

        //bool highlight = false;

        bool ActiveHighlighting = false;

        internal Part highlightPart = null;

        
        public void Clear()
        {
            notEnoughCrew = false;
            notEnoughEC = false;
            notEnoughFunds = false;
            notEnoughResources = false;
        }

        PartHighlighter phl = null;
        int highlightID;

        protected void Start()
        {
            phl = PartHighlighter.CreatePartHighlighter();
            if (phl == false)
                return;
            highlightID = phl.CreateHighlightList();
            if (highlightID < 0)
                return;

            phl.AddPartToHighlight(highlightID, this.part);
            UpdateColors();
            GameEvents.OnGameSettingsApplied.Add(UpdateColors);
        }

        void OnDestroy()
        {
            GameEvents.OnGameSettingsApplied.Remove(UpdateColors);
        }

        void LateUpdate()
        {
            if (NotEnough)
            {
                if (!ActiveHighlighting && !eventAcknowledged)
                {
                    if (HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().stopWarp)
                    {
                        TimeWarp.fetch.CancelAutoWarp();
                        TimeWarp.SetRate(0, false);
                    }
#if false
                if (HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().doHighlighting)
                    StartCoroutine(CycleHighlighting());
#endif
                    phl.SetHighlighting(highlightID, true);
                }
                else
                    if (eventAcknowledged)
                        phl.SetHighlighting(highlightID, false);
            }
            else
                phl.SetHighlighting(highlightID, false);

        }

        void UpdateColors()
        {
            Color c = new Color();
            c.b = HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().highlightBlue;
            c.r = HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().highlightRed;
            c.g = HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().highlightGreen;
            c.a = 1;

            phl.UpdateHighlightColors(highlightID, c);
        }

    }
}

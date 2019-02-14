using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

namespace Workshop
{
    public class WorkshopAnimateGeneric : ModuleAnimateGeneric
    {
        [KSPField]
        public string Unpack = "Unpack";

        [KSPField]
        public string Pack = "Pack";

        [KSPField(isPersistant = true)]
        public bool packed = true;


        public bool Busy { get; set; }
        public bool Packing { get; private set; }

        [KSPEvent(guiName = "Unpack Workshop", guiActive = true, guiActiveEditor = false)]
        public void UnpackModule()
        {
            if (Packing)
                return;
            if (packed)
            {
                allowManualControl = true;
                base.Toggle();
                allowManualControl = false;
                packed = false;
                Events["UnpackModule"].guiName = Pack;
               // Events["UnpackModule"].guiActive = false;
                StartCoroutine(Wait("Unpacking ", Pack));
            }
            else
            {
                // cannot pack if:
                //  1. printing in progress
                if (!Busy)
                {
                    allowManualControl = true;
                    base.Toggle();
                    allowManualControl = false;
                    packed = true;
                    Events["UnpackModule"].guiName = Unpack;
                    //Events["UnpackModule"].guiActive = false;
                    StartCoroutine(Wait("Packing ", Unpack));
                }
            }
        }


        private IEnumerator Wait(string curAction, string setToNextAction)
        {
            Packing = true;
            string st = curAction;
            Events["UnpackModule"].guiName = st;
            for (int i = 0; i < 8; i++)
            {
                yield return new WaitForSeconds(0.5f);
                st += ".";
                Events["UnpackModule"].guiName = st;
            }
            //Events["UnpackModule"].guiActive = true;
            Events["UnpackModule"].guiName = setToNextAction;
            Packing = false;
        }


        public void Start()
        {
            OnGameSettingsApplied();


            if (packed)
                Events["UnpackModule"].guiName = Unpack;
            else
                Events["UnpackModule"].guiName = Pack;

            foreach (string s in new string[] { "status", "startEventGUIName", "endEventGUIName", "actionGUIName" })
            {
                Fields[s].guiActive = false;
                Fields[s].guiActiveEditor = false;
            }

            GameEvents.OnGameSettingsApplied.Add(OnGameSettingsApplied);
        }

        void OnGameSettingsApplied()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().requireUnpacking)
                Events["UnpackModule"].guiActive = false;
            else
                Events["UnpackModule"].guiActive = true;
        }


    }
}

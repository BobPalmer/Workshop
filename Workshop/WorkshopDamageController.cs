using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using UnityEngine;

using KISAPIv1;

namespace Workshop
{
    using System;
    using System.Linq;

    using UnityEngine;

    using W_KIS;
    using Recipes;
    using System.Text;

    public class WorkshopDamageController : PartModule
    {
        [KSPField(guiName = "Repair Status", guiActive = false)]
        public string RepairStatus = "Online";

        const string DUCTTAPEPART = "Duct Tape";
        const string DUCTTAPERESOURCE = "DuctTape";
        const float DAMAGECHECKDELAY = 0.5f;

        [KSPField(isPersistant=true)]
        private double curDamage = 3;


        [KSPField]
        public string UpkeepResource = "ElectricCharge";

        [KSPField]
        public float UpkeepAmount = 1.0f;

        bool repairInProgress = false;
        List<KeyValuePair<int, W_KIS_Item>> ductTapeInInventory;
        Coroutine coroutine;

       [KSPEvent(guiName = "Start Repair", guiActive = true, guiActiveEditor = false)]
        public void Repair()
        {
            if (repairInProgress)
            {
                StopCoroutine(coroutine);
                repairInProgress = false;
                Events["Repair"].guiName = "Start Repair";
            }
            else
            {

                if (part.protoModuleCrew.Count < 1)
                {
                    RepairStatus = "No Crew to do repairs";
                    Fields["RepairStatus"].guiActive = true;
                    return;
                }

                double availDuctTape = 0;

                // check to see if duct tape is in inventory of module occupants

                var availableItems = KISWrapper.GetInventories(this.part).SelectMany(i => i.items).ToArray();
                ductTapeInInventory = new List<KeyValuePair<int, W_KIS_Item>>();
                foreach (KeyValuePair<int, W_KIS_Item> i in availableItems)
                {
                    Log.Info("Part in inventory: " + i.Value.availablePart.name + ", " + i.Value.quantity);
                    if (i.Value.availablePart.name == DUCTTAPEPART)
                    {
                        double? res = KISAPI.PartNodeUtils.UpdateResource(i.Value.partNode, DUCTTAPERESOURCE, 0, true);
                        availDuctTape += (double)res;
                        ductTapeInInventory.Add(i);
                    }
                }
                if (availDuctTape > 0)
                {
                    Events["Repair"].guiName = "Stop Repair";
                    repairInProgress = true;
                     coroutine = StartCoroutine(DoRepair(availDuctTape));
                }
            }
        }

        public double CurDamageImpact
        {
            get { if (curDamage == 0) return 1; return Math.Sqrt(1+curDamage); }
            //private set { curDamage = value; SetRepairStatus(); }
        }

        public double RepairDamage(double repair)
        {
            curDamage -= repair;
            return curDamage;
        }

        void SetRepairStatus()
        {
           Events["Repair"].guiActive = (curDamage > 0);
        }

        void CheckForDamage(double secs)
        {
            if (HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().unpackedAccelCausesDamage)
            {
                if (Math.Abs(vessel.geeForce) > 2)
                {
                    curDamage += Math.Abs(vessel.geeForce - 2) / 10 * HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().geeForceDamageMultipler * secs;
                }
            }
        }


        void Start()
        {
            SetRepairStatus();
            StartCoroutine(DoDamageCheck());
        }

        IEnumerator DoDamageCheck()
        {
            while (true)
            {
                yield return new WaitForSeconds(DAMAGECHECKDELAY);
                CheckForDamage(DAMAGECHECKDELAY);
            }
        }

        IEnumerator DoRepair(double availDuctTape)
        {
            double curTime = Planetarium.GetUniversalTime();
            double elapsedTime;

            while (curDamage > 0 && ductTapeInInventory.Count > 0 && availDuctTape > 0)
            {
                yield return new WaitForSeconds(1.0f);

                elapsedTime = Planetarium.GetUniversalTime() - curTime;
                curTime = Planetarium.GetUniversalTime();

                if (AmountAvailable(UpkeepResource) < elapsedTime * UpkeepAmount)
                {
                    RepairStatus = "Not enough " + UpkeepResource;
                    Fields["RepairStatus"].guiActive = true;
                }
                else
                {
                    RequestResource(UpkeepResource, elapsedTime * UpkeepAmount);

                    double neededDuctTape = Math.Max(Math.Min(0.1f * elapsedTime, availDuctTape) * part.protoModuleCrew.Count, 0);
                    Log.Info("DoRepair 1, curDamage: " + curDamage + ", availDuctTape: " + availDuctTape + ", neededDuctTape: " + neededDuctTape);

                    curDamage = Math.Max(0, curDamage - neededDuctTape / 10);
                    this.part.RequestResource(DUCTTAPERESOURCE, neededDuctTape);

                    // This little loop tries to use up all the neededDuctTape, going to additional rolls if necessary
                    while (ductTapeInInventory.Count > 0 && neededDuctTape > 0)
                    {
                        var d = ductTapeInInventory.First();

                        double amt = Math.Min(neededDuctTape, d.Value.quantity);

                        double? res = KISAPI.PartNodeUtils.UpdateResource(d.Value.partNode, DUCTTAPERESOURCE, -amt, true);
                        neededDuctTape -= amt;
                        availDuctTape -= amt;
                        if (res <= 0)
                        {
                            Log.Info("Deleting empty DuctTape from inventory");

                            ductTapeInInventory.Remove(d);
                            d.Value.StackRemove(1);
                        }
                        Log.Info("ductTapeInInventory.Count: " + ductTapeInInventory.Count + ", neededDuctTape: " + neededDuctTape);
                    }
                    Events["Repair"].guiName = "Stop Repair (dmg: " + curDamage.ToString("F2") + ")";
                }
            }
            if (ductTapeInInventory.Count == 0)
            {
                RepairStatus = "No DuctTape available";
                Fields["RepairStatus"].guiActive = true;
                yield return null;
            }
            else
            {
                repairInProgress = false;
                Events["Repair"].guiName = "Start Repair";
                Fields["RepairStatus"].guiActive = false;
                yield return null;
            }
        }

        void FixedUpdate()
        {
            if (curDamage > 0)
            {
                if (!repairInProgress)
                {
                    Events["Repair"].guiName = "Start Repair (dmg: " + curDamage.ToString("F2") + ")";
                    Events["Repair"].guiActive = true;
                }               
            }
            else
                Events["Repair"].guiActive = false;
        }


        private double AmountAvailable(string resource)
        {
            var res = PartResourceLibrary.Instance.GetDefinition(resource);
            double amount, maxAmount;
            part.GetConnectedResourceTotals(res.id, out amount, out maxAmount);
            return amount;
        }
        private float RequestResource(string resource, double amount)
        {
            var res = PartResourceLibrary.Instance.GetDefinition(resource);
            return (float)this.part.RequestResource(res.id, amount);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace Workshop
{
    public partial class OseModuleRecycler
    {
        void KAC_onAlarmStateChanged(KACWrapper.KACAPI.AlarmStateChangedEventArgs args)
        {

        }

        void setKACRecycleAlarm(double totalPrintTime)
        {
            if (!KACWrapper.AssemblyExists)
                return;
            if (!KACWrapper.APIReady)
                return;

            //Delete the alarm if it exists
            if (!string.IsNullOrEmpty(KACAlarmID))
                KACWrapper.KAC.DeleteAlarm(KACAlarmID);

            //Get the start time
            double startTime = Planetarium.GetUniversalTime();

            //Now set the alarm
            double buildTimeSeconds = Planetarium.GetUniversalTime() + totalPrintTime;
            KACAlarmID = KACWrapper.KAC.CreateAlarm(KACWrapper.KACAPI.AlarmTypeEnum.Raw, "Recycling job completed", buildTimeSeconds);
            kacAlarm = getKACAlarm();
            if (kacAlarm != null)
            {
                kacAlarm.AlarmMargin = 5.0f;
                kacAlarm.Notes = part.vessel.vesselName + " completed recycling job.";
                kacAlarm.VesselID = FlightGlobals.ActiveVessel.id.ToString();
            }
            else
                Log.Info("setKACAlarm, alarm not set");
        }

        KACWrapper.KACAPI.KACAlarm getKACAlarm()
        {
            if (KACWrapper.AssemblyExists && KACWrapper.APIReady && !string.IsNullOrEmpty(KACAlarmID))
            {
                int totalAlarms = KACWrapper.KAC.Alarms.Count;
                for (int index = 0; index < totalAlarms; index++)
                {
                    if (KACWrapper.KAC.Alarms[index].ID == KACAlarmID)
                    {
                        return KACWrapper.KAC.Alarms[index];
                    }
                }
            }

            return null;
        }

        void updateKACAlarm()
        {
            if (!WorkshopOptions.EnableKACIntegration)
                return;
            if (!KACWrapper.AssemblyExists)
                return;
            if (!KACWrapper.APIReady)
                return;

            if (_queue.Count == 0 && _processedBlueprint == null)
            {
                deleteKACAlarm();
                return;
            }

            
            //Calculate total print time.
            double totalRecycleTime = 0;
            //int totalItems = _queue.Count;
            for (int index = _queue.Count - 1; index >= 0; index--)
            {
                totalRecycleTime += _queue[index].PartBlueprint.GetBuildTime(WorkshopUtils.ProductivityType.recycler, adjustedProductivity);
            }

            if (_processedBlueprint != null)
                totalRecycleTime += _processedBlueprint.GetBuildTime(WorkshopUtils.ProductivityType.recycler, adjustedProductivity);

            Log.Info("updateKACAlarm, adjustedProductivity: " + adjustedProductivity + ", totalRecycleTime: " + totalRecycleTime);
            //Create the alarm if needed.
            if (string.IsNullOrEmpty(KACAlarmID))
            {
                setKACRecycleAlarm(totalRecycleTime);
            }
            else
            {
                //Find the alarm if needed and then update it
                if (kacAlarm == null)
                {
                    for (int index = KACWrapper.KAC.Alarms.Count - 1; index >= 0; index--)
                    {
                        var alarm = KACWrapper.KAC.Alarms[index];
                        if (KACWrapper.KAC.Alarms[index].ID == KACAlarmID)
                        {
                            kacAlarm = alarm;
                            kacAlarm.AlarmTime = Planetarium.GetUniversalTime() + totalRecycleTime;
                            return;
                        }
                    }
                }

                //Update the alarm
                else
                {
                    kacAlarm.AlarmTime = Planetarium.GetUniversalTime() + totalRecycleTime;
                }
            }
        }

        void deleteKACAlarm()
        {
            if (!string.IsNullOrEmpty(KACAlarmID))
            {
                if (KACWrapper.AssemblyExists && KACWrapper.APIReady)
                {
                    int totalAlarms = KACWrapper.KAC.Alarms.Count;
                    for (int index = 0; index < totalAlarms; index++)
                    {
                        if (KACWrapper.KAC.Alarms[index].ID == KACAlarmID)
                        {
                            KACWrapper.KAC.DeleteAlarm(KACAlarmID);
                            KACAlarmID = string.Empty;
                            kacAlarm = null;
                            return;
                        }
                    }
                }
                KACAlarmID = string.Empty;
                kacAlarm = null;
                Log.Info("Alarm not deleted");
            }
        }
    }
}

﻿using System.Collections.Generic;
using KSP.UI.Screens;
using UnityEngine;
using KSP_Log;

namespace Workshop
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class OseAddonEditorFilter : MonoBehaviour
    {
        private static readonly List<string> AvPartItems = new List<string>();
        internal string Category = "Filter by Function";
        internal string SubCategoryTitle = "Workshop Items";
        internal string IconName = "R&D_node_icon_advmetalworks";
        const string CategoryButtonLocalizationId = "#autoLOC_453547"; // filter by function


        void Awake()
        {
            GameEvents.onGUIEditorToolbarReady.Add(SubCategories);

            AvPartItems.Clear();
            AvPartItems.AddRange(new[]
                                 {
                                     "OSE.Workshop",
                                     "OSE.Converter",
                                     "ose3000",
                                     "ose6000",
                                     "OSE.Workshop2",
                                     "OSE.WorkshopAiCore",
                                     "OSE.WorkshopChemical",
                                     "Duct.Tape"
                                 });
        }

        private void SubCategories()
        {
            var icon = PartCategorizer.Instance.iconLoader.GetIcon(IconName);
            var filter = PartCategorizer.Instance.filters.Find(f => f.button.categorydisplayName == CategoryButtonLocalizationId);
            if (filter == null)
            {
                WorkshopUtils.LogError(string.Format("Cannot find 'Filter by function' button for category: {0}", SubCategoryTitle));
                return;
            }
            PartCategorizer.AddCustomSubcategoryFilter(filter, SubCategoryTitle, SubCategoryTitle, icon, p => AvPartItems.Contains(p.name));
        }
    }
}

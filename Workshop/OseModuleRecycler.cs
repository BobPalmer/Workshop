namespace Workshop
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using W_KIS;
    using Recipes;
    using System.Text;
    
    using ClickThroughFix;

    public partial class OseModuleRecycler : OseModuleHighlighter
    {
        private const double kBackgroundProcessInterval = 3600;

        private Blueprint _processedBlueprint;
        private WorkshopItem _processedItem;

        //private readonly ResourceBroker _broker;
        private readonly WorkshopQueue _queue;

        // GUI Properties
        private int _activePage;
        private int _selectedPage;

        private Rect _windowPos = new Rect(50, 50, 640, 680);
        private bool _showGui;

        private Texture2D _pauseTexture;
        private Texture2D _playTexture;
        private Texture2D _binTexture;


        [KSPField(isPersistant = true)]
        public bool recyclingPaused;

        [KSPField(isPersistant = true)]
        public float progress;

        [KSPField(isPersistant = true)]
        public double lastUpdateTime;

        [KSPField]
        public float ConversionRate = 0.25f;

        [KSPField]
        public float ProductivityFactor = 0.1f;

        [KSPField]
        public string UpkeepResource = "ElectricCharge";

        [KSPField]
        public float UpkeepAmount = 1.0f;

        [KSPField]
        public int MinimumCrew = 2;

        [KSPField()]
        public bool UseSpecializationBonus = true;

        [KSPField()]
        public string ExperienceEffect = "RepairSkill";

        [KSPField()]
        public float SpecialistEfficiencyFactor = 0.02f;

        [KSPField(guiName = "Recycler Status", guiActive = true)]
        public string RecyclerStatus = "Online";

        [KSPField]
        string Error = "";
        [KSPField]
        string LastError = "";

        [KSPField(isPersistant = true)]
        public string KACAlarmID = string.Empty;
        KACWrapper.KACAPI.KACAlarm kacAlarm = null;

        protected float adjustedProductivity = 1.0f;

        [KSPField(isPersistant = true)]
        double maxGeeForce = 1;


        [KSPEvent(guiActive = true, guiName = "Open Recycler")]
        public void ContextMenuOpenRecycler()
        {
            if (_showGui)
            {
                foreach (var inventory in KISWrapper.GetInventories(vessel).Where(i => i.showGui == false).ToList())
                {
                    foreach (var item in inventory.items)
                    {
                        item.Value.DisableIcon();
                    }
                    foreach (var item in _queue)
                    {
                        item.DisableIcon();
                    }
                    if (_processedItem != null)
                    {
                        _processedItem.DisableIcon();
                    }
                }
                _showGui = false;
            }
            else
            {
                if (!WorkshopUtils.PreLaunch())
                {
                    _showGui = true;
                }
                else
                {
                    ScreenMessages.PostScreenMessage("Recycler is in travel mode, unable to print at this time", 5, ScreenMessageStyle.UPPER_CENTER);
                }
            }
        }

        public OseModuleRecycler()
        {
            _queue = new WorkshopQueue();
            //_broker = new ResourceBroker();
            _pauseTexture = WorkshopUtils.LoadTexture("Workshop/Assets/Icons/icon_pause");
            _playTexture = WorkshopUtils.LoadTexture("Workshop/Assets/Icons/icon_play");
            _binTexture = WorkshopUtils.LoadTexture("Workshop/Assets/Icons/icon_bin");
        }

        public override string GetInfo()
        {
            StringBuilder sb = new StringBuilder("<color=#8dffec>KIS Part Recycker</color>");

            sb.Append($"\nMinimum Crew: {MinimumCrew}");
            sb.Append($"\nBase productivity factor: {ProductivityFactor:P0}");
            sb.Append($"\nUse specialist bonus: ");
            sb.Append(RUIutils.GetYesNoUIString(UseSpecializationBonus));
            if (UseSpecializationBonus)
            {
                sb.Append($"\nSpecialist skill: {ExperienceEffect}");
                sb.Append($"\nSpecialist bonus: {SpecialistEfficiencyFactor:P0} per level");

            }

            return sb.ToString();
        }
        WorkshopAnimateGeneric wag;
        WorkshopDamageController wdc;

        public override void OnStart(StartState state)
        {
            //Init the KAC Wrapper. KAC Wrapper courtey of TriggerAu
            KACWrapper.InitKACWrapper();
            if (KACWrapper.APIReady)
            {
                KACWrapper.KAC.onAlarmStateChanged += KAC_onAlarmStateChanged;
            }

            if (HighLogic.LoadedSceneIsFlight && WorkshopSettings.IsKISAvailable)
            {
                GameEvents.onVesselChange.Add(OnVesselChange);
            }
            else
            {
                Fields["RecyclerStatus"].guiActive = false;
                Events["ContextMenuOpenRecycler"].guiActive = false;
            }


            foreach (PartModule p in this.part.Modules)
            {
                if (p.moduleName == "WorkshopAnimateGeneric")
                {
                    wag = p as WorkshopAnimateGeneric;
                }
                if (p.moduleName == "WorkshopDamageController")
                {
                    wdc = p as WorkshopDamageController;
                }
            }


            if (wag != null && wag.packed)
                RecyclerStatus = "Packed";

            base.OnStart(state);
        }

        public override void OnLoad(ConfigNode node)
        {
            if (HighLogic.LoadedSceneIsFlight)
            {
                LoadModuleState(node);
            }
            base.OnLoad(node);
        }

        private void LoadModuleState(ConfigNode node)
        {
            if (node.HasValue("progress"))
                float.TryParse(node.GetValue("progress"), out progress);
            if (node.HasValue("lastUpdateTime"))
                double.TryParse(node.GetValue("lastUpdateTime"), out lastUpdateTime);

            foreach (ConfigNode cn in node.nodes)
            {
                if (cn.name == "ProcessedItem")
                {
                    _processedItem = new WorkshopItem();
                    _processedItem.Load(cn);
                }
                if (cn.name == "ProcessedBlueprint")
                {
                    _processedBlueprint = new Blueprint();
                    _processedBlueprint.Load(cn);
                }
                if (cn.name == "Queue")
                {
                    _queue.Load(cn);
                }
            }
        }

        public override void OnSave(ConfigNode node)
        {
            node.AddValue("progress", progress);
            node.AddValue("lastUpdateTime", lastUpdateTime);

            if (_processedItem != null)
            {
                var itemNode = node.AddNode("ProcessedItem");
                _processedItem.Save(itemNode);

                var blueprintNode = node.AddNode("ProcessedBlueprint");
                _processedBlueprint.Save(blueprintNode);
            }

            var queueNode = node.AddNode("Queue");
            _queue.Save(queueNode);

            base.OnSave(node);
        }

        private void UpdateProductivity()
        {
            if (_processedItem != null && UseSpecializationBonus)
                adjustedProductivity = WorkshopUtils.GetProductivityBonus(part, ExperienceEffect, SpecialistEfficiencyFactor, ProductivityFactor, WorkshopUtils.ProductivityType.recycler);
            if (_processedItem != null && wdc != null && HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().unpackedAccelCausesDamage)
                adjustedProductivity /= (float)wdc.CurDamageImpact;
        }

        public void FixedUpdate()
        {
            if (!HighLogic.LoadedSceneIsFlight)
                return;
            
            maxGeeForce = Math.Max(maxGeeForce, FlightGlobals.ActiveVessel.geeForce);

            if (_showGui)
                Events["ContextMenuOpenRecycler"].guiName = "Close Recycler";
            else
                Events["ContextMenuOpenRecycler"].guiName = "Open Recycler";
            
            if (HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().requireUnpacking || WorkshopUtils.PreLaunch())
            {
                if (wag != null)
                {
                    if (!wag.packed && !wag.Packing)
                        Events["ContextMenuOpenRecycler"].guiActive = true; // (_processedItem == null);
                    else
                        Events["ContextMenuOpenRecycler"].guiActive = false;
                }
            }
            if (wag != null)
                wag.Busy = (_processedItem != null);

            if (wag != null && wag.packed && HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().requireUnpacking)
                RecyclerStatus = "Packed";
            else
                RecyclerStatus = "Online";
                        
            try
            {
                UpdateProductivity();

                if (HighLogic.CurrentGame.Parameters.CustomParams<Workshop_Settings>().setRecycleKAC)
                    updateKACAlarm();

                ApplyPaging();

                if (lastUpdateTime == 0)
                {
                    lastUpdateTime = Planetarium.GetUniversalTime();
                    ProcessItem(TimeWarp.deltaTime);
                    return;
                }

                //Get elapsed time
                double elapsedTime = Planetarium.GetUniversalTime() - lastUpdateTime;

                //Update last update time
                lastUpdateTime = Planetarium.GetUniversalTime();

                //If our elapsed time is > the background process interval, then we'll need to do some multiple processings.
                double timeRemaining = 0;
                while (elapsedTime > 0.1) // kBackgroundProcessInterval)
                {
                    timeRemaining = ProcessItem(elapsedTime);
                    if (_processedItem == null)
                        return;
                    if (elapsedTime == timeRemaining)
                        break;
                    elapsedTime = timeRemaining;
                    //elapsedTime -= kBackgroundProcessInterval;
                }

                //Process the remaining delta time
                if (elapsedTime > 0f)
                {
                    ProcessItem(elapsedTime);
                }

            }
            catch (Exception ex)
            {
                WorkshopUtils.LogError("OseModuleWorkshop_OnUpdate", ex);
            }
        }

        private double ProcessItem(double deltaTime)
        {
            double timeRemaining = deltaTime;

            if (recyclingPaused)
            {
                RecyclerStatus = "Paused";
                return 0;
            }
            if (progress >= 99.999f) // use 99.999f to catch any floating point errors
            {
                FinishManufacturing();
                timeRemaining -= 0.01f;
            }
            if (_processedItem != null)
            {
                timeRemaining = ExecuteRecycling(deltaTime);
            }
            else
            {
                StartManufacturing();
                timeRemaining -= 0.01f;
            }

            return timeRemaining;
        }

        private void ApplyPaging()
        {
            if (_activePage != _selectedPage)
            {
                foreach (var inventory in KISWrapper.GetInventories(vessel).Where(i => i.showGui == false).ToList())
                {
                    foreach (var item in inventory.items)
                    {
                        item.Value.DisableIcon();
                    }
                }
                _activePage = _selectedPage;
            }
        }

        private void StartManufacturing()
        {
            var nextQueuedPart = _queue.Pop();
            if (nextQueuedPart != null)
            {
                _processedItem = nextQueuedPart;
                _processedBlueprint = WorkshopRecipeDatabase.ProcessPart(nextQueuedPart.Part);
                foreach (var resource in _processedBlueprint)
                {
                    resource.Units *= ConversionRate;
                }
            }
        }

        private double ExecuteRecycling(double deltaTime)
        {
            var resourceToProduce = _processedBlueprint.First(r => r.Processed < r.Units);
            var unitsToProduce = Math.Min(resourceToProduce.Units - resourceToProduce.Processed,
                                           deltaTime * adjustedProductivity);
            Log.Info("resourceToProduce: " + resourceToProduce.Name + ", unitsToProduce: " + unitsToProduce);

            notEnoughCrew = part.protoModuleCrew.Count < MinimumCrew;
            notEnoughEC = OseModuleWorkshop.AmountAvailable(this.part, UpkeepResource) < TimeWarp.deltaTime * UpkeepAmount;

            Error = "";
            if (notEnoughCrew)
            {
                RecyclerStatus = "Not enough Crew to operate";
                Error = RecyclerStatus;
            }
            else if (notEnoughEC)
            {
                RecyclerStatus = "Not enough " + UpkeepResource;
                Error = RecyclerStatus;
            }
            else
            {
                RecyclerStatus = "Recycling " + _processedItem.Part.title;
                OseModuleWorkshop.RequestResource(this.part, UpkeepResource, TimeWarp.deltaTime * UpkeepAmount);
                //_broker.RequestResource(part, UpkeepResource, UpkeepAmount, TimeWarp.deltaTime, ResourceFlowMode.ALL_VESSEL);

                resourceToProduce.Processed += unitsToProduce;
                progress = (float)(_processedBlueprint.GetProgress() * 100);
                Log.Info("progress: " + progress);
            }
            if (Error != "" && Error != LastError && HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().showPopup)
            {
                PopupDialog.SpawnPopupDialog(
                   new MultiOptionDialog(
                       "OseModuleRecyclerWarning",
                       "Warning\n" + Error + "\n",
                       this.part.partInfo.title,
                       HighLogic.UISkin,
                       CreateOptions()
                   ),
                   false,
                   HighLogic.UISkin
               );

            }
            LastError = Error;

            //Return time remaining
            //return deltaTime - unitsToProduce;


            double d = deltaTime - deltaTime * (unitsToProduce / (deltaTime * adjustedProductivity));
            Log.Info("ExecuteRecycling, returning : " + d);
            return d;
        }
        private DialogGUIBase[] CreateOptions()
        {
            List<DialogGUIBase> options = new List<DialogGUIBase>();
            if (!_showGui && FlightGlobals.ActiveVessel == this.vessel)
                options.Add(new DialogGUIButton("Open Recycler", () => onConfirm()));

            options.Add(new DialogGUIButton("Acknowledge", () => onAck()));
  

            options.Add(new DialogGUIButton("Cancel", delegate { }));
            return options.ToArray();

        }
        Action onConfirm()
        {
            ContextMenuOpenRecycler();
            return null;
        }
        Action onAck()
        {
            AcknowledgeCondition();
            return null;
        }
        private void FinishManufacturing()
        {
            ScreenMessages.PostScreenMessage("Recycling of " + _processedItem.Part.title + " finished.", 5, ScreenMessageStyle.UPPER_CENTER);
            CleanupRecycler();
        }

        private void CancelManufacturing()
        {
            if (_processedItem != null && _processedItem.Part != null)
            {
                ScreenMessages.PostScreenMessage("Recycling of " + _processedItem.Part.title + " cancelled.", 5, ScreenMessageStyle.UPPER_CENTER);
                CleanupRecycler();
            }
            else
            {
                if (_processedItem == null)
                    Log.Info("_processedItem is null");
                else
                    Log.Info("_processedItem.Part is null");
            }
            recyclingPaused = false;
        }

        private void CleanupRecycler()
        {
            // Need to turn processed  stuff over to the tanks
            foreach (var p in _processedBlueprint)
                this.part.RequestResource( p.Name, -p.Processed);

            _processedItem.DisableIcon();
            _processedItem = null;
            _processedBlueprint = null;
            progress = 0;


            deleteKACAlarm();
        }

        public override void OnInactive()
        {
            if (_showGui)
            {
                ContextMenuOpenRecycler();
            }
            base.OnInactive();
        }

        void OnVesselChange(Vessel v)
        {
            if (_showGui)
            {
                ContextMenuOpenRecycler();
            }
        }

        // ReSharper disable once UnusedMember.Local => Unity3D
        // ReSharper disable once InconsistentNaming => Unity3D
        void OnGUI()
        {
            if (_showGui)
            {
                DrawWindow();
            }
        }

        GUIStyle window;
        bool windowInitted = false;

        private void DrawWindow()
        {
            if (!HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().useAlternateSkin)
                GUI.skin = HighLogic.Skin;
            else
                GUI.color = Color.grey;

            if (!windowInitted && HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().useAlternateSkin)
            {
                windowInitted = true;
                window = new GUIStyle(HighLogic.Skin.window);
    
                window.active.background = window.normal.background;

                Texture2D tex = window.normal.background; //.CreateReadable();

                //#if DEBUG
                //                Log.Debug("WindowOpacity set to " + value);
                //                tex.SaveToDisk("unmodified_window_bkg.png");
                //#endif

                var pixels = tex.GetPixels32();

                for (int i = 0; i < pixels.Length; ++i)
                    pixels[i].a = 255;

                tex.SetPixels32(pixels); tex.Apply();
                //#if DEBUG
                //                tex.SaveToDisk("usermodified_window_bkg.png");
                //#endif
                // one of these apparently fixes the right thing
                // window.onActive.background =
                // window.onFocused.background =
                // window.onNormal.background =
                //window.onHover.background =
                window.active.background =
                window.focused.background =
                //window.hover.background =
                window.normal.background = tex;
            }

            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUI.skin.button.alignment = TextAnchor.MiddleCenter;

            if (!HighLogic.CurrentGame.Parameters.CustomParams<Workshop_MiscSettings>().useAlternateSkin)
                _windowPos = ClickThruBlocker.GUIWindow(GetInstanceID(), _windowPos, DrawWindowContents, "Recycler Menu");
            else
                _windowPos = ClickThruBlocker.GUIWindow(GetInstanceID(), _windowPos, DrawWindowContents, "Recycler Menu", window);

        }


        private void DrawWindowContents(int windowId)
        {
            WorkshopItem mouseOverItem = null;
            W_KIS_Item mouseOverItemKIS = null;

            mouseOverItemKIS = DrawInventoryItems(mouseOverItemKIS);
            mouseOverItem = DrawQueue(mouseOverItem);
            DrawMouseOverItem(mouseOverItem, mouseOverItemKIS);
            DrawRecyclingProgress();

            if (GUI.Button(new Rect(_windowPos.width - 25, 5, 20, 20), "X"))
            {
                ContextMenuOpenRecycler();
            }

            GUI.DragWindow();
        }

        W_KIS_Item DrawInventoryItems(W_KIS_Item mouseOverItem)
        {
            // AvailableItems
            const int ItemRows = 10;
            const int ItemColumns = 3;
            const int ItemsPerPage = ItemRows * ItemColumns;

            var allItems = KISWrapper.GetInventories(vessel)
                                    .SelectMany(i => i.items);
            var maxPage = allItems.Count() / ItemsPerPage;
            var availableItems = allItems
                                    .Skip(_activePage * ItemsPerPage)
                                    .Take(ItemsPerPage)
                                    .ToArray();

            for (var y = 0; y < ItemRows; y++)
            {
                for (var x = 0; x < ItemColumns; x++)
                {
                    var left = 15 + x * 55;
                    var top = 70 + y * 55;
                    var itemIndex = y * ItemColumns + x;
                    if (availableItems.Length > itemIndex)
                    {
                        var item = availableItems[itemIndex];
                        var icon = item.Value.Icon;
                        if (icon == null)
                        {
                            item.Value.EnableIcon(64);
                            icon = item.Value.Icon;
                        }
                        
                        if (GUI.Button(new Rect(left, top, 50, 50), icon.texture))
                        {
                            _queue.Add(new WorkshopItem(item.Value.availablePart));
                            item.Value.StackRemove(1);
                        }
                
                        if (item.Value.stackable)
                        {
                            GUI.Label(new Rect(left, top, 50, 50), item.Value.quantity.ToString("x#"), UI.UIStyles.lowerRightStyle);
                        }
                        if (Event.current.type == EventType.Repaint && new Rect(left, top, 50, 50).Contains(Event.current.mousePosition))
                        {
                            mouseOverItem = item.Value;
                        }
                    }
                }
            }

            if (_activePage > 0)
            {
                if (GUI.Button(new Rect(15, 645, 75, 25), "Prev"))
                {
                    _selectedPage = _activePage - 1;
                }
            }

            if (_activePage < maxPage)
            {
                if (GUI.Button(new Rect(100, 645, 75, 25), "Next"))
                {
                    _selectedPage = _activePage + 1;
                }
            }
            return mouseOverItem;
        }

        WorkshopItem DrawQueue(WorkshopItem mouseOverItem)
        {
            // Queued Items
            const int QueueRows = 4;
            const int QueueColumns = 7;
            GUI.Box(new Rect(190, 345, 440, 270), "Queue", UI.UIStyles.QueueSkin);
            for (var y = 0; y < QueueRows; y++)
            {
                for (var x = 0; x < QueueColumns; x++)
                {
                    var left = 205 + x * 60;
                    var top = 370 + y * 60;
                    var itemIndex = y * QueueColumns + x;
                    if (_queue.Count > itemIndex)
                    {
                        var item = _queue[itemIndex];
                        if (item.Icon == null)
                        {
                            item.EnableIcon(64);
                        }
                        if (GUI.Button(new Rect(left, top, 50, 50), item.Icon.texture))
                        {
                            _queue.Remove(item);
                        }
                        if (Event.current.type == EventType.Repaint && new Rect(left, top, 50, 50).Contains(Event.current.mousePosition))
                        {
                            mouseOverItem = item;
                        }
                    }
                }
            }

            return mouseOverItem;
        }

        void DrawMouseOverItem(WorkshopItem mouseOverItem, W_KIS_Item mouseOverItemKIS)
        {
            // Tooltip
            adjustedProductivity = WorkshopUtils.GetProductivityBonus(part, ExperienceEffect, SpecialistEfficiencyFactor, ProductivityFactor, WorkshopUtils.ProductivityType.recycler) ;
            if (mouseOverItem != null)
            {
                var blueprint = WorkshopRecipeDatabase.ProcessPart(mouseOverItem.Part);
                foreach (var resource in blueprint)
                {
                    resource.Units *= ConversionRate;
                }
                GUI.Box(new Rect(200, 80, 100, 100), mouseOverItem.Icon.texture);
                GUI.Box(new Rect(310, 80, 150, 100), WorkshopUtils.GetKisStats(mouseOverItem.Part), UI.UIStyles.StatsStyle);
                GUI.Box(new Rect(470, 80, 150, 100), blueprint.Print(WorkshopUtils.ProductivityType.recycler, adjustedProductivity), UI.UIStyles.StatsStyle);
                GUI.Box(new Rect(200, 190, 420, 25), mouseOverItem.Part.title, UI.UIStyles.TitleDescriptionStyle);
                GUI.Box(new Rect(200, 220, 420, 110), mouseOverItem.Part.description, UI.UIStyles.TooltipDescriptionStyle);
            }
            else if (mouseOverItemKIS != null)
            {
                var blueprint = WorkshopRecipeDatabase.ProcessPart(mouseOverItemKIS.availablePart);
                foreach (var resource in blueprint)
                {
                    resource.Units *= ConversionRate;
                }
                GUI.Box(new Rect(200, 80, 100, 100), mouseOverItemKIS.Icon.texture);
                GUI.Box(new Rect(310, 80, 150, 100), WorkshopUtils.GetKisStats(mouseOverItemKIS.availablePart), UI.UIStyles.StatsStyle);
                GUI.Box(new Rect(470, 80, 150, 100), blueprint.Print(WorkshopUtils.ProductivityType.recycler, adjustedProductivity), UI.UIStyles.StatsStyle);
                GUI.Box(new Rect(200, 190, 420, 25), mouseOverItemKIS.availablePart.title, UI.UIStyles.TitleDescriptionStyle);
                GUI.Box(new Rect(200, 220, 420, 110), mouseOverItemKIS.availablePart.description, UI.UIStyles.TooltipDescriptionStyle);
            }

        }

        void DrawRecyclingProgress()
        {
            // Currently build item
            if (_processedItem != null)
            {
                if (_processedItem.Icon == null)
                {
                    _processedItem.EnableIcon(64);
                }
                GUI.Box(new Rect(190, 620, 50, 50), _processedItem.Icon.texture);
            }
            else
            {
                GUI.Box(new Rect(190, 620, 50, 50), "");
            }

            // Progressbar
            GUI.Box(new Rect(250, 620, 260, 50), "");
            if (progress >= 1)
            {
                var color = GUI.color;
                GUI.color = new Color(0, 1, 0, 1);
                GUI.Box(new Rect(250, 620, 260 * progress / 100, 50), "");
                GUI.color = color;
            }
            string progressText = "";
            if (_processedBlueprint != null)
                progressText = string.Format("Progress: {0:n1}%, T- ", progress) + KSPUtil.PrintTime(_processedBlueprint.GetBuildTime(WorkshopUtils.ProductivityType.recycler, adjustedProductivity), 5, false);
            Log.Info("DrawRecyclingProgress, adjustedProductivity: " + adjustedProductivity);
            GUI.Label(new Rect(250, 620, 260, 50), " " + progressText);

            // Toolbar
            if (recyclingPaused)
            {
                if (GUI.Button(new Rect(520, 620, 50, 50), _playTexture))
                {
                    recyclingPaused = false;
                }
            }
            else
            {
                if (GUI.Button(new Rect(520, 620, 50, 50), _pauseTexture))
                {
                    recyclingPaused = true;
                }
            }

            if (GUI.Button(new Rect(580, 620, 50, 50), _binTexture))
            {
                CancelManufacturing();
            }
        }
    }

    
}

using KIS;
using KISAPIv1;
namespace Workshop.W_KIS
{
	using System;
	using System.Collections;
	using System.Collections.Generic;
	using System.Linq;
	//using System.Reflection;
   

	using UnityEngine;

	public class KIS_Shared
	{
#if false
        private static Type KIS_Shared_class;

		private static MethodInfo kis_GetPartVolume;
#endif

        private static KISAPIv1.PartUtilsImpl _partUtilsImpl;


        public static float GetPartVolume(AvailablePart part)
		{
            return _partUtilsImpl.GetPartVolume(part);
#if false
            if (kis_GetPartVolume == null)
				return 0;
			return (float)kis_GetPartVolume.Invoke(null, new object[] { part });
#endif
		}

		internal static void Initialize() //Assembly kisAssembly)
		{
            _partUtilsImpl = new PartUtilsImpl();
#if false
            KIS_Shared_class = kisAssembly.GetTypes().First(t => t.Name.Equals("KIS_Shared"));
			if (KIS_Shared_class != null)
				kis_GetPartVolume = KIS_Shared_class.GetMethod("GetPartVolume");
#endif
        }
	}

	public class ModuleKISItem
	{
#if false
		private static Type ModuleKISItem_class;

        private static FieldInfo kis_volumeOverride;
#endif
        //private readonly object _obj;
        KIS.ModuleKISItem _moduleKISItem;

		public float volumeOverride
		{
			get
			{
                if (_moduleKISItem == null)
                    return 0;
                return _moduleKISItem.volumeOverride;
#if false
                if (kis_volumeOverride == null)
					return 0;
				return (float)kis_volumeOverride.GetValue(_obj);
#endif
			}
		}

		public ModuleKISItem(object obj)
		{
            //_obj = obj;
            _moduleKISItem = (KIS.ModuleKISItem)obj;
		}

		internal static void Initialize() //Assembly kisAssembly)
		{
#if false
            ModuleKISItem_class = kisAssembly.GetTypes().First(t => t.Name.Equals("ModuleKISItem"));
			if (ModuleKISItem_class != null)
				kis_volumeOverride = ModuleKISItem_class.GetField("volumeOverride");
#endif
		}
	}

	public class ModuleKISInventory
	{
		public enum InventoryType { Container, Pod, Eva }
#if false
        private static Type ModuleKISInventory_class;

		private static FieldInfo kis_invType;

		private static FieldInfo kis_podSeat;

		private static FieldInfo kis_maxVolume;

        //private static FieldInfo kis_showGui;
        private static PropertyInfo p_kis_showGui;

        private static FieldInfo kis_items;

		private static PropertyInfo kis_GetContentVolume;

		private static MethodInfo kis_isFull;

		private static MethodInfo kis_AddItem;
#endif
		//private readonly object _obj;
        KIS.ModuleKISInventory _moduleKISInventory;


		public ModuleKISInventory(object obj)
		{
            //_obj = obj;
            _moduleKISInventory = (KIS.ModuleKISInventory)obj;
		}

		public InventoryType invType
		{
			get
			{
                return (InventoryType)_moduleKISInventory.invType;
#if false
                return (InventoryType)Enum.Parse(typeof(InventoryType), kis_invType.GetValue(_obj).ToString());
#endif
			}
		}

		public int podSeat
		{
			get
			{
                return _moduleKISInventory.podSeat;
#if false
                return (int)kis_podSeat.GetValue(_obj);
#endif
			}
		}

		public float maxVolume
		{
			get
			{
                return _moduleKISInventory.maxVolume;
#if false
                return (float)kis_maxVolume.GetValue(_obj);
#endif
			}
		}

		public bool showGui
		{
			get
			{
                return _moduleKISInventory.showGui;
#if false
                //return (bool)kis_showGui.GetValue(_obj);
                return (bool)p_kis_showGui.GetValue(_obj, null);
#endif
            }
        }

		public Part part
		{
			get
			{
                return _moduleKISInventory.part;
#if false
                return ((PartModule)_obj).part;
#endif
			}
		}

		public Dictionary<int, W_KIS_Item> items
        {
            get
            {
                var dict = new Dictionary<int, W_KIS_Item>();
#if false
				var inventoryItems = (IDictionary)kis_items.GetValue(_obj);
			
#else
                var inventoryItems = (IDictionary)_moduleKISInventory.items;
#endif

                foreach (DictionaryEntry entry in inventoryItems)
				{
					dict.Add((int)entry.Key, new W_KIS_Item(entry.Value));
				}

                return dict;
            }
        }

		public float GetContentVolume()
		{
            return _moduleKISInventory.totalContentsVolume;
#if false
            return (float)kis_GetContentVolume.GetValue(_obj, null);
#endif
		}

		public bool isFull()
		{
            return _moduleKISInventory.isFull();
#if false
            return (bool)kis_isFull.Invoke(_obj, null);
#endif
		}

		public W_KIS_Item AddItem(Part partPrefab)
		{
            var obj = _moduleKISInventory.AddItem(partPrefab, 1, -1);

#if false
            var obj = kis_AddItem.Invoke(_obj, new object[] { partPrefab, 1, -1 });
            _kis_item =  KIS_Item.CreateItemFromScenePart(partPrefab, 1, -1);
#endif
			return new W_KIS_Item(obj);
		}

		internal static void Initialize() //Assembly kisAssembly)
        {
#if false
            ModuleKISInventory_class = kisAssembly.GetTypes().First(t => t.Name.Equals("ModuleKISInventory"));

			kis_invType = ModuleKISInventory_class.GetField("invType");
;
            kis_podSeat = ModuleKISInventory_class.GetField("podSeat");

            kis_maxVolume = ModuleKISInventory_class.GetField("maxVolume");

            //kis_showGui = ModuleKISInventory_class.GetField("showGui");
            p_kis_showGui = ModuleKISInventory_class.GetProperty("showGui");

            kis_items = ModuleKISInventory_class.GetField("items");

            kis_AddItem = ModuleKISInventory_class.GetMethod("AddItem", new[] { typeof(Part), typeof(int), typeof(int) });

            kis_GetContentVolume = ModuleKISInventory_class.GetProperty("totalContentsVolume");

            kis_isFull = ModuleKISInventory_class.GetMethod("isFull");
#endif
        }
    }

	public class W_KIS_Item
	{
		public struct ResourceInfo
		{
#if false
            private static Type ResourceInfo_struct;

			private static FieldInfo kis_resourceName;

			private static FieldInfo kis_amount;

			private static FieldInfo kis_maxAmount;

			private readonly object _obj;
#endif
            private readonly ProtoPartResourceSnapshot _resourceInfo;

			public string resourceName
			{
				get
				{
                    return _resourceInfo.resourceName;
#if false
                    return (string)kis_resourceName.GetValue(_obj);
#endif
				}
			}

			public double amount
			{
				get
				{
                    return _resourceInfo.amount;
#if false
					return (double)kis_amount.GetValue(_obj);
#endif
				}
			}

			public double maxAmount
			{
				get
				{
                    return _resourceInfo.maxAmount;
#if false
					return (double)kis_maxAmount.GetValue(_obj);
#endif
				}
			}

			public ResourceInfo(object obj)
			{
#if false
                _obj = obj;
#endif
                _resourceInfo = (ProtoPartResourceSnapshot)obj;
			}

			public static void Initialize() // Assembly kisAssembly)
            {
#if false
                // ResourceInfo_struct = kisAssembly.GetTypes().First(t => t.Name.Equals("ResourceInfo"));

                ResourceInfo_struct = typeof(ProtoPartResourceSnapshot);

                kis_resourceName = ResourceInfo_struct.GetField("resourceName");
                kis_amount = ResourceInfo_struct.GetField("amount");
                kis_maxAmount = ResourceInfo_struct.GetField("maxAmount");
#endif
            }
        }
#if false
        private static Type KIS_Item_class;
        private static Type KIS_PartNodeUtilsImpl_class;

		private static FieldInfo kis_icon;

		private static FieldInfo kis_availablePart;

        //private static FieldInfo kis_quantity;
        private static PropertyInfo p_kis_quantity;

        private static FieldInfo kis_stackable;

		private static MethodInfo kis_GetResources;
        private static FieldInfo kis_item_partNode;

		private static MethodInfo kis_SetResource;

		private static MethodInfo kis_EnableIcon;

		private static MethodInfo kis_DisableIcon;

		private static MethodInfo kis_StackRemove;
#endif
		//private readonly object _obj;
        KIS.KIS_Item _kis_item;

		public W_KIS_Item(object obj)
		{
            //_obj = obj;
            _kis_item = (KIS.KIS_Item)obj;

        }

		public KIS_IconViewer Icon
		{
			get
			{
                if (_kis_item.icon == null)
                    return null;
                return new KIS_IconViewer(_kis_item.icon);
#if false
                var kisIcon = kis_icon.GetValue(_obj);
				if (kisIcon == null)
				{
					return null;
				}
				return new KIS_IconViewer(kisIcon);
#endif
			}
		}

		public AvailablePart availablePart
		{
			get
			{
                return _kis_item.availablePart;
#if false
                return (AvailablePart)kis_availablePart.GetValue(_obj);
#endif
			}
		}

		public int quantity
		{
            get { return _kis_item.quantity; }
#if false
            //get { return (int)kis_quantity.GetValue(_obj); }
            get { return (int)p_kis_quantity.GetValue(_obj, null); }
#endif
        }

        public bool stackable
		{
            get { return _kis_item.stackable; }
#if false
            get { return (bool) kis_stackable.GetValue(_obj); }
#endif
		}

		public ProtoPartResourceSnapshot[] GetResources()
		{
            return KISAPI.PartNodeUtils.GetResources(_kis_item.partNode);
#if false
            ConfigNode partNode =(ConfigNode) kis_item_partNode.GetValue(_obj);

            // The following is copied from KIS, file: PartNodeUtilsImpl.cs, from the function GetResources(ConfigNode partNode)
            if (partNode.HasNode("PART"))
            {
                partNode = partNode.GetNode("PART");
            }
            return partNode.GetNodes("RESOURCE")
                .Select(n => new ProtoPartResourceSnapshot(n))
                .ToArray();

         
			var list = (ProtoPartResourceSnapshot[])kis_GetResources.Invoke(_obj, new object[] { partNode } );
     
            return list;
#endif
		}

		public void SetResource(string name, int amount)
		{
            KISAPI.PartNodeUtils.UpdateResource(_kis_item.partNode, name, amount); ;
#if false
            kis_SetResource.Invoke(_obj, new object[] { name, amount });
#endif
		}

		public void EnableIcon(int resolution)
		{
            _kis_item.EnableIcon(resolution);
#if false
            kis_EnableIcon.Invoke(_obj, new object[] { resolution });
#endif
		}

		public void DisableIcon()
		{
            _kis_item.DisableIcon();
#if false
            kis_DisableIcon.Invoke(_obj, null);
#endif
		}

		public void StackRemove(int quantity)
		{
            ScreenMessages.PostScreenMessage("KIS removal of item from storage is disabled due to a bug outside of the Workshop", 10);
            //_kis_item.StackRemove(quantity);
#if false
            int result = (int)kis_StackRemove.Invoke(_obj, new object[] { quantity });
#endif
        }

		internal static void Initialize() // Assembly kisAssembly)
        {
#if false
            KIS_Item_class = kisAssembly.GetTypes().First(t => t.Name.Equals("KIS_Item"));
			kis_icon = KIS_Item_class.GetField("icon");
			kis_availablePart = KIS_Item_class.GetField("availablePart");
            //kis_quantity = KIS_Item_class.GetField("quantity");
            p_kis_quantity = KIS_Item_class.GetProperty("quantity");
            kis_stackable = KIS_Item_class.GetField("stackable");
			//kis_GetResources = KIS_Item_class.GetMethod("GetResources");
			kis_SetResource = KIS_Item_class.GetMethod("SetResource");
			kis_EnableIcon = KIS_Item_class.GetMethod("EnableIcon");
			kis_DisableIcon = KIS_Item_class.GetMethod("DisableIcon");
			kis_StackRemove = KIS_Item_class.GetMethod("StackRemove");

            kis_item_partNode = KIS_Item_class.GetField("partNode");
            KIS_PartNodeUtilsImpl_class = kisAssembly.GetTypes().First(t => t.Name.Equals("PartNodeUtilsImpl"));
     
            kis_GetResources = KIS_PartNodeUtilsImpl_class.GetMethod("GetResources");
#endif
        }
    }

	public class KIS_IconViewer
	{
#if false
        private static Type KIS_IconViewer_class;

		//private static FieldInfo kis_texture;
        private static PropertyInfo p_kis_texture;

		private static MethodInfo kis_dispose;
#endif
		//private readonly object _obj;
        KIS.KIS_IconViewer _iconViewer;

        public Texture texture
		{
			get
			{
                return _iconViewer.texture;
#if false
                return (Texture)p_kis_texture.GetValue(_obj, null);
				//return (Texture)kis_texture.GetValue(_obj);
#endif
			}
		}

		public void Dispose()
		{
            _iconViewer.Dispose();
#if false
            kis_dispose.Invoke(_obj, null);
#endif
		}

        public KIS_IconViewer(object obj)
		{
            //_obj = obj;
            _iconViewer = (KIS.KIS_IconViewer)obj;
		}

#if false
        public KIS_IconViewer(Part p, int resolution) : this(Activator.CreateInstance(KIS_IconViewer_class, new object [] { p, resolution }))
		{
		}
#else
        public KIS_IconViewer(Part p, int resolution) 
        {
            _iconViewer = new KIS.KIS_IconViewer(p, resolution);
        }
#endif

        internal static void Initialize() // Assembly kisAssembly)
        {
#if false
            KIS_IconViewer_class = kisAssembly.GetTypes().First(t => t.Name.Equals("KIS_IconViewer"));
			//kis_texture = KIS_IconViewer_class.GetField("texture");
            p_kis_texture = KIS_IconViewer_class.GetProperty("texture");

			kis_dispose = KIS_IconViewer_class.GetMethod("Dispose");
#endif
		}
	}

	public class KISWrapper
	{
		public static bool Initialize()
		{
			var kisAssembly = AssemblyLoader.loadedAssemblies.FirstOrDefault(a => a.assembly.GetName().Name.Equals("KIS", StringComparison.InvariantCultureIgnoreCase));

             if (kisAssembly == null)
			{
                    return false;
			}
            KIS_Shared.Initialize(); //  kisAssembly.assembly);
            ModuleKISInventory.Initialize(); // kisAssembly.assembly);
            ModuleKISItem.Initialize(); // kisAssembly.assembly);
            W_KIS_Item.Initialize(); // kisAssembly.assembly);
            KIS_IconViewer.Initialize(); // kisAssembly.assembly);
            W_KIS_Item.ResourceInfo.Initialize(); // kisAssembly.assembly);
            return true;
		}

		public static List<ModuleKISInventory> GetInventories(Vessel vessel)
		{
			var inventories = new List<ModuleKISInventory>();
			foreach (var part in vessel.parts)
			{
				foreach (PartModule module in part.Modules)
				{
					if (module.moduleName == "ModuleKISInventory")
					{
						inventories.Add(new ModuleKISInventory(module));
					}
				}
			}
			return inventories;
		}

		public static ModuleKISItem GetKisItem(Part part)
		{
			ModuleKISItem item = null;
			foreach (PartModule module in part.Modules)
			{
				if (module.moduleName == "ModuleKISItem")
				{
					item = new ModuleKISItem(module);
				}
			}
			return item;
		}
	}
}

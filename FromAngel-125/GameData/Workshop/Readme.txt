OSE Workshop Continued by Aelfhe1m - developed from original mod OSE Workshop by ObiVanDamme

What does it do?

The OSE - Workshop is a new Part that is meant to be used together with Kerbal Inventory System (KIS). It allows you to create parts in flight.

Current Features

    Queueing of items for production
    All Items require MaterialKits, ElectricCharge and a crew of two Kerbals to be created
    The amount of MaterialKits needed is depending on the mass of the created item => To create an item with the mass of one ton you need one ton of MaterialKits (not one unit)
	Third party mods may add or change required resources to make some parts (recipes)
    Recycling of items stored in your vessels inventory
    Processing of Ore into MaterialParts
    Pause and cancel current production

Installation

	Download and install the pre-requisites: Module Manager, Kerbal Inventory System (KIS), Community Resource Pack and FirespitterCore
	Merge the GameData folder from the OSE Workshop download with your KSP GameData folder.
	
Upgrades
	When upgrading from a previous version of OSE Workshop it is recommended that you delete the old GameData/Workshop folder before installing the new version.
	Also make sure to check that the pre-requisites are up to date.

License

OSE Workshop is licensed under a Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International license, which means

    You are permitted to use, copy and redistribute my work as-is.
    You may remix your own derivatives (new models, alternative textures, etc.) and release them under your own name.
    You may not use the material for any commercial purposes.
    You must use the same license as the original work.
    You must credit the following people when publishing your derivatives in the download and forum posts: ObiVanDamme and Enceos (OSE Workshop), KospY and Winn75 (Kerbal Attachment System).

Credits and Thanks

	Portions of this codebase include source by taniwha, used under GNU general public license.
	KIS is the original Work of KospY and Winn75 - Thank you for creating this awesome mod.
	Alshain: category icons
	Enceos: 3D models and textures 

Authors

	ObiVanDamme: mod idea and C# programming
	Aelfhe1m: maintenance, feature development and additional C# programming from Workshop version 1.2.0
	Angel-125: Caretaking and additional functionality

Changelog

1.3
- Updated for KSP 1.5.X

- Printing processes now supports "catch up" mechanics. This means that you can, for instance, start a print job, go to the Space Center, wait a few hours, and come back to find your print job nearly completed.

- Recycle processes now supports "catch up" mechanics as well.

- Fixed time estimates listed when you mouse over a part.

- Fixed integration with Extraplanetary Launchpads.

- Adjusted ModuleManager patches to make it easier to customize recipes- CRP is still very much supported!

- Reduced production rates on the OSE Workshop parts to more realistic values.

- The Workshop AI Core no longer requires crew to operate, but it requires more ElectricCharge than a regular workshop and takes twice as long to print items. It's also been moved to Field Science.

- The Chemical Workshop can now produce MaterialKits from Ore.

- Enable Recipes Option: You can now enable/disables recipes, which require more than one resource to print the part. When disabled, only the default MaterialKits resource is required (unless overriden by another mod). You can set this option from KSP's Settings screen. It is on by default.

- Require Funds Option: You can now enable/disable the Funds requirement for printing parts. You can set this option from KSP's Settings screen. It is on by default.

- Create KAC Alarms Option: You can now enable/disable integration with Kerbal Alarm Clock. When enabled, any print job that takes an hour or more will add a KAC alarm. You can set this option from KSP's Settings screen. It is on by default. 
KNOWN ISSUE: Setting a KAC alarm doesn't percisely tell you when your print jobs will be done, but it's pretty close. Work is ongoing to improve accuracy.

- Removed OseModuleCategoryAddon and associated classes.

- You can now specify production categories from within the OseModuleWorkshop config node by adding CATEGORY nodes. You'll get the full list of categories if you don't specify any CATEGORY nodes. Example config:
MODULE
{
	name = OseModuleWorkshop
	//module stuff goes here...

	CATEGORY
	{
		name = Science
		iconPath = Squad/PartList/SimpleIcons/R&D_node_icon_advsciencetech
	}
}
OSE Workshop - KIS Addon
===

This is the repository for the OSE Workshop mod for [Kerbal Space Program](http://kerbalspaceprogram.com)

OSE Workshop is the original work of ObiVanDamme and Enceos

You can find the official repo of this mod on [Github](http://github.com/ObiVanDamme/Workshop).

### Mod Description
The Ose Workshop mods adds a Workshop Part to the game, that allows the creation of Parts during missions. The Workshop requires Electric Charge and MaterialKits to build Items. 

### Required Mods
(OSE Workshop will not work without these mods)
* [KIS](http://forum.kerbalspaceprogram.com/index.php?/topic/101928-105-kerbal-inventory-system-kis-123/)
* [Module Manager](http://forum.kerbalspaceprogram.com/threads/55219)
* [CommunityResourcePack](http://forum.kerbalspaceprogram.com/index.php?/topic/83007-11)
* [Firespitter Core](http://forum.kerbalspaceprogram.com/index.php?/topic/22583-firespitter-propeller-plane-and-helicopter-parts-v71-may-5th-for-ksp-10/)

### Installation Instructions
* Download and install [Module Manager](http://forum.kerbalspaceprogram.com/threads/55219)
* Download and install [Community Resource Pack](http://forum.kerbalspaceprogram.com/index.php?/topic/83007-11)
* Download and install [Firespitter Core](http://forum.kerbalspaceprogram.com/index.php?/topic/22583-firespitter-propeller-plane-and-helicopter-parts-v71-may-5th-for-ksp-10/)
* Download and Install [KIS](http://forum.kerbalspaceprogram.com/index.php?/topic/101928-105-kerbal-inventory-system-kis-123/)
* Download the file from [GitHub](https://github.com/obivandamme/Workshop/releases)
* Unzip the file and copy the content of the GameData folder to the GameData folder in your KSP Installation

### License

OSE Workshop is licensed under a Creative Commons Attribution-NonCommercial-ShareAlike 4.0 International license, which means

* You are permitted to use, copy and redistribute my work as-is.
* You may remix your own derivatives (new models, alternative textures, etc.) and release them under your own name.
* You may not use the material for any commercial purposes.
* You must use the same license as the original work.
* You must credit the following people when publishing your derivatives in the download and forum posts: ObiVanDamme and Enceos (OSE Workshop), KospY and winn75(Kerbal Attachment System).

### Credits and Thanks!

* KAS and KIS is the original work of KospY! - Thank you for creating this awesome mods.
* Alshain: category icons
* Enceos: 3D models and textures 

=============================================================================================================================================

This ﻿mod is career mode ready. The parts and the printing process itself (cost of MaterialKits plus EC) is 
scaled so that costs of MaterialKits and other resources match the cost of the printed parts.

What does it do?

The OSE - Workshop is a new Part that is meant to be used together with Kerbal Inventory System (KIS). It
allows you to create parts in flight.

Current Features

* Queueing of items for production
* All Items require MaterialKits, ElectricCharge and a crew of two Kerbals to be created
* The amount of MaterialKits needed is depending on the mass of the created item => To create an item with 
  the mass of one ton you need one ton of MaterialKits (not one unit)
* Recycling of items stored in your vessels inventory
* Processing of Ore into MaterialParts
* Cancelation of item production
* Selection of target inventory
* Efficiency based on Crew Traits




Additions and Changes by LinuxGuruGamer

The part models have been repurposed for stand-alone games (those without EL, MKS or GC)

Regarding the stand-alone tech:
	* 3D Printing Lab, Advanced Exploration.  Can print parts
	* Recycling, Field Science (using the AICore part), Can recycle parts into what they were made from.  45% recovery
	* Ore Processing﻿, Advanced Science Tech, can process ore into MaterialKits
	* Material Extractor, Advanced Science Tech, can process ore into MaterialKits, Dirt into ExoticMinerals and RareMetal

Stock scanners and drills have been modified to adding  Dirt, ExoticMinerals and RareMetals

if EL, MKS  is installed:
	* 3D Printing Lab, Advanced Exploration, can print parts, can recycle parts
	* Workshop AI Core, Field Science, can print parts, can recycle parts, automated
	* Workshop Chemical, Advanced Science Tech, can print parts, can recycle parts, can convert ore to MaterialKits
	* Material Extractor, Advanced Science Tech, can process ore into MaterialKits, Dirt into ExoticMinerals and RareMetals

Packing and Damage
	An optional feature which add the need to have a module packed before experiencing high G's.  If not packed, then
	the part will suffer damage during acceleration.  Duct Tape will be needed to repair the damage

	Each part will have a damage value, which will increase during acceleration
	Damage increases with accel >2g, (geeForce - 2)/10 * seconds

Damage Repair
	There needs to be at least one roll of Duct Tape in the inventory of the damaged module.  You also need at
	least one Kerbal in it, having a second Kerbal will double the rate of repair.  The Duct Tape will be used
	during the repair, and if all used up, the roll will be removed from the inventory.

Damage Impact 
	Productivity will be adversly affected by damage, the impact will be  adjustedProductivity / SquareRoot(1 + damage resource) 
	Time to repair will be:  0.01/second (for 1 Kerbal)
	Duct Tape needed: 0.1/second
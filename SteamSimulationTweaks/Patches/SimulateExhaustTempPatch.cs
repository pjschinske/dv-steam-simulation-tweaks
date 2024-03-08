using DV.CabControls;
using HarmonyLib;
using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamSimulationTweaks.Patches
{
	/*[HarmonyPatch(typeof(ReciprocatingSteamEngine), nameof(ReciprocatingSteamEngine.Tick))]
	class SimulateExhaustTempPatch
	{
		static void Postfix(ReciprocatingSteamEngine __instance)
		{
			//Assuming adiabatic expansion, get the exhaust temperature based on
			//the change in cylinder volume and the steam chest temperature
			//temperature times (volume of steam ^ (adiabatic index minus 1)) is constant
			float f = __instance.steamChestTemperature.Value
				* Mathf.Pow(__instance.cylinderVolume * __instance.cutoff, ReciprocatingSteamEngine.ADIABATIC_INDEX - 1);
			__instance.exhaustTemperatureReadOut.Value = f
				/ Mathf.Pow(__instance.cylinderVolume, ReciprocatingSteamEngine.ADIABATIC_INDEX - 1);
		}
	}*/
}

using HarmonyLib;
using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamSimulationTweaks.Patches
{
	[HarmonyPatch(typeof(SteamTables), nameof(SteamTables.SteamSpecificEnthalpy))]
	internal class SteamEnthalpyCalcPatch
	{
		static void Prefix(ref float p)
		{
			if (p > 30)
			{
				p = 30;
			}
		}
	}

	[HarmonyPatch(typeof(SteamTables), nameof(SteamTables.WaterSpecificVolume))]
	internal class WaterVolumeCalcPatch
	{
		static bool Prefix(float p, ref float __result)
		{
			if (p < 30)
			{
				return true;
			}

			__result = 1.0590296977023f
				+ p * (0.00618219884772f
				+ p * (-0.00003848754877805f
				+ p * (.00000015851511484802f)));
			return false;
			//x ((1.5851511484802×10^-7 x - 0.00003848754877805) x + 0.00618219884772) + 1.0590296977023
		}
	}
}

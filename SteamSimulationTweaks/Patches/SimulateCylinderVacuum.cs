using HarmonyLib;
using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamSimulationTweaks.Patches
{

	//This was an idea to simulate a vacuum when at zero throttle, so you'd have to center
	//the reverser to get rid of a braking effect. But after doing some more research,
	//this doesn't seem to be that accurate; in real life, I think locomotives almost
	//always had some kind of device to prevent this from happening (snifters, bypass valves...)

	//TODO:
	//- re-enable
	//- fix log spam
	//- center cylinder cock pressure effect around 1 psi in ReciprocatingSteamEngine.Tick

	//[HarmonyPatch(typeof(ReciprocatingSteamEngine.CylinderData),
	//			  nameof(ReciprocatingSteamEngine.CylinderData.CalculatePistonForce))]
	internal class SimulateClosedThrottleCylinderVacuum
	{
		/*
		static bool Prefix(ReciprocatingSteamEngine.CylinderData __instance, ref float __result)
		{
			if (!Main.Settings.enableClosedThrottleCylinderVacuum)
			{
				return true;
			}
			if (__instance.sim.cylinderCockControl.Value > 0.5f)
			{

			} 
			float num = __instance.admissionPressure;
			if (__instance.positionWithinStroke > __instance.sim.cutoff)
			{
				num *= __instance.InstantaneousCylinderPressureRatio(__instance.positionWithinStroke / __instance.sim.cutoff);
			}
			__result = num * 100000f * __instance.sim.pistonArea;
			return false;
		}*/

		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			if (!Main.Settings.enableClosedThrottleCylinderVacuum)
			{
				return codes;
			}
			int i = 0, j = 0;
			//get rid of the call to Mathf.Max, and the subtraction by 1
			for (i = codes.Count - 1; i >= 0; i--)
			{
				if (codes[i].opcode != OpCodes.Call)
				{
					continue;
				}
				if (codes[i].operand is not MethodInfo)
				{
					continue;
				}
				MethodInfo method = codes[i].operand as MethodInfo;
				if (method.Name != nameof(Mathf.Max))
				{
					continue;
				}
				codes.RemoveAt(i);

				Main.Debug("After removing call to MAX:");
				foreach (var code in codes)
				{
					Main.Logger.Log(code.ToString());
				}

				for (j = i - 1; j >= 0; j--)
				{
					if (codes[j].opcode == OpCodes.Ldc_R4
						&& (float)codes[j].operand == 0.0f)
					{
						codes[j].opcode = OpCodes.Ldloc_0;
						codes[j].operand = null;
						break;
					}
					codes.RemoveAt(j);
				}
				break;
			}

			Main.Debug("After removing subtraction");
			foreach (var code in codes)
			{
				Main.Debug(code.ToString());
			}

			/*//Add back in the subtraction by 1, but in the proper place
			for (int k = j; k < codes.Count - 1; k++)
			{
				if (codes[k].opcode == OpCodes.Mul)
				{
					codes.Insert(k, new CodeInstruction(OpCodes.Ldc_R4, 1.0f));
					codes.Insert(k + 1, new CodeInstruction(OpCodes.Sub));
					break;
				}
			}
			Main.Logger.Log("Final instructions:");
			foreach (var code in codes)
			{
				Main.Logger.Log(code.ToString());
			}*/

			return codes;
		}
	}

	/*[HarmonyPatch(typeof(ReciprocatingSteamEngine),
				  nameof(ReciprocatingSteamEngine.SimulateMeanTorque))]
	internal class SimulateClosedThrottleCylinderVacuum2*/

	//[HarmonyPatch(typeof(ReciprocatingSteamEngine),
	//			  nameof(ReciprocatingSteamEngine.Tick))]
	internal class SimulateClosedThrottleCylinderVacuum3
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			if (!Main.Settings.enableClosedThrottleCylinderVacuum)
			{
				return codes;
			}

			Main.Debug("Before adding subtraction");
			foreach (var code in codes)
			{
				Main.Debug(code.ToString());
			}
			int i = 0;
			for (; i < codes.Count; i++) {
				//we want to insert a "minus 1" before the call to stfld ReciprocatingSteamEngine.admissionPressure
				//so that it's corrected everywhere in this method
				if (codes[i].opcode != OpCodes.Stfld
					|| codes[i].operand is not FieldInfo)
				{
					continue;
				}
				FieldInfo storeField = (FieldInfo)codes[i].operand;
				if (storeField.Name != nameof(ReciprocatingSteamEngine.admissionPressure))
				{
					continue;
				}

				codes.Insert(i, new CodeInstruction(OpCodes.Sub));
				codes.Insert(i, new CodeInstruction(OpCodes.Ldc_R4, 1.0f));
				break;
			}

			Main.Debug("After adding subtraction");
			foreach (var code in codes)
			{
				Main.Debug(code.ToString());
			}

			for (; i < codes.Count; i++)
			{

			}

			return codes;
		}
	}

	/*[HarmonyPatch(typeof(ReciprocatingSteamEngine.CylinderData),
				  nameof(ReciprocatingSteamEngine.CylinderData.SimulateTorque))]
	internal class SimulateClosedThrottleCylinderVacuum2
	{
		static bool Prefix(ReciprocatingSteamEngine.CylinderData __instance, float cylinderPressure, ref float __result)
		{
			if (__instance.prevIsCrankAboveHorizontal != __instance.isCrankAboveHorizontal || __instance.isInletValveOpen)
			{
				bool flag = __instance.sim.isReverserForward ^ __instance.isCrankAboveHorizontal;
				__instance.admissionPressure = (flag ? (1f - cylinderPressure) : cylinderPressure - 1);
			}
			__result = __instance.CalculatePistonForce() * __instance.crankOffset * __instance.sim.pistonStroke / 2f;
			return false;
		}
	}*/
}

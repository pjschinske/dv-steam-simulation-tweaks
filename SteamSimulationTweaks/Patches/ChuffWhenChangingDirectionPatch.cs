using HarmonyLib;
using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using static LocoSim.Implementations.ReciprocatingSteamEngine;
using static TMPro.SpriteAssetUtilities.TexturePacker_JsonArray;

namespace SteamSimulationTweaks.Patches
{
	//Steam locomotives chuff when changing directions:
	//https://www.youtube.com/watch?v=41w2Gl4WQ1s

	//This code does get the locomotives to chuff when changing direction, but sometimes it somehow
	//makes a double chuff. The bigger issue is that I can't get the steam consumption to be realistic;
	//the added chuff uses barely any steam so you can make it chuff forever by moving the reverser back
	//and forth.
	//There's gotta be a better way to do this, I just have to really take the time to figure out how the
	//valve simulation works.
	//I also don't quite know if it should always chuff at 50% reverser, or if that point changes depending
	//on where the piston is in its cycle.

	/*[HarmonyPatch(typeof(ReciprocatingSteamEngine), nameof(ReciprocatingSteamEngine.Tick))]
	static class ChuffWhenChangingDirection2Patch {
		static void Prefix(ReciprocatingSteamEngine __instance, out bool __state)
		{
			__state = __instance.isReverserForward;
		}

		//TODO: figure out how to get all steam in cylinder to exit the cylinder

		static void Postfix(ReciprocatingSteamEngine __instance, bool __state)
		{
			if (__state != __instance.isReverserForward)
			{

				//gotta fake a cutoff value so that it thinks there's some steam in the cylinders
				float old_cutoff = __instance.cutoff;
				__instance.cutoff = 1f;

				float absCrankRpm = Mathf.Abs(__instance.crankRpm.Value);
				float num3 = Mathf.InverseLerp(1f, 4f, __instance.admissionPressure);
				float cylCockControlPercentage = __instance.cylinderCockControl.Value;
				__instance.cylinderCockFlowNormalizedReadOut.Value = num3 * cylCockControlPercentage;
				float cylCockSteamPercentage = Mathf.Lerp(0f, 0.1f, cylCockControlPercentage);
				float cylinderCockSteamPressure = cylCockSteamPercentage * __instance.admissionPressure;
				bool isCylinderCracked = __instance.isCylinderCrackedReadOut.Value > 0f;
				float cylinderCrackPressure = (isCylinderCracked ? (0.5f * __instance.admissionPressure) : 0f);
				__instance.cylinderCrackFlowNormalizedReadOut.Value = (isCylinderCracked ? (1f * num3) : 0f);
				float totalPressureLoss = cylinderCockSteamPressure + cylinderCrackPressure;
				float realAdmissionPressure = Mathf.Max(0f, __instance.admissionPressure - totalPressureLoss);
				__instance.condensationExpansionRatio = __instance.CondensationExpansionRatio();
				__instance.exhaustPressureReadOut.Value = __instance.cutoff * realAdmissionPressure;
				int num9 = 0;
				for (int i = 0; i < __instance.numCylinders; i++)
				{
					__instance.cylinderData[i].SimulateCylinder(__instance.pistonRotationLead * (float)i);
					if (__instance.cylinderData[i].isInletValveOpen && __instance.admissionPressure > 0f)
					{
						num9 += 1 << i;
					}
				}
				__instance.cylindersSteamInjection.Value = num9;
				//float num10 = 0f;
				//num10 = ((absCrankRpm < 45f) ? __instance.SimulateInstantaneousTorque(realAdmissionPressure) : ((!(absCrankRpm > 60f)) ? Mathf.Lerp(SimulateInstantaneousTorque(realAdmissionPressure), SimulateMeanTorque(realAdmissionPressure), Mathf.InverseLerp(45f, 60f, absCrankRpm)) : SimulateMeanTorque(realAdmissionPressure)));
				//__instance.torqueOut.Value = Mathf.SmoothDamp(__instance.torqueOut.Value, num10, ref __instance.torqueVelocity, 0.1f, float.PositiveInfinity, delta);
				//bool flag2 = (float)num9 > 0f;

				__instance.SimulateSteamConsumption(absCrankRpm, __instance.admissionPressure, realAdmissionPressure, totalPressureLoss, true);

				__instance.cutoff = old_cutoff;

				if (__instance.chuffEventReadOut.Value == 0)
				{
					__instance.chuffEventReadOut.Value = 2;
				}
				else
				{
					__instance.chuffEventReadOut.Value = 0;
				}
			}
		}
	}*/

	//I no longer remember what I was trying to do here. I do remember that it doesn't work haha

	//[HarmonyPatch(typeof(ReciprocatingSteamEngine), nameof(ReciprocatingSteamEngine.CheckChuff))]
	static class ChuffWhenChangingDirectionPatch
	{
		/*static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator gen)
		{
			var codes = new List<CodeInstruction>(instructions);

			//I can't quite tell what this variable actually stores
			LocalBuilder num3 = gen.DeclareLocal(typeof(float));
			LocalBuilder cylinderCockControl = gen.DeclareLocal(typeof(float));
			LocalBuilder cylinderCockSteamPercent = gen.DeclareLocal(typeof(float));
			LocalBuilder cylinderCockSteamPressure = gen.DeclareLocal(typeof(float));
			LocalBuilder isCylinderCracked = gen.DeclareLocal(typeof(bool));
			LocalBuilder cylinderCrackedPressureLoss = gen.DeclareLocal(typeof(float));
			LocalBuilder cylinderPressureLoss = gen.DeclareLocal(typeof(float));
			LocalBuilder modifiedAdmissionPressure = gen.DeclareLocal(typeof(float));
			Label cylinderIsCracked = gen.DefineLabel();
			Label cylinderIsNotCracked = gen.DefineLabel();

			for (int i = codes.Count - 1; i >= 0; i--)
			{
				if (codes[i].opcode != OpCodes.Stfld)
				{
					continue;
				}
			//SimulateSteamConsumption(absRpm, pressure, admissionPressure, dumpedPressure, steamInjectedToAnyCylinder)

			//load absRpm
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.crankRpm))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Call,
					typeof(Mathf)
					.GetMethod(nameof(Mathf.Abs))));
			//load admissionPressure
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.admissionPressure))));
			//load modifiedAdmissionPressure

				// float num3 = Mathf.InverseLerp(1f, 4f, admissionPressure);
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_R4, 1));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_R4, 4));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.admissionPressure))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Call,
					typeof(Mathf)
					.GetMethod(nameof(Mathf.InverseLerp))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Stloc_S, num3));


				//float value = cylinderCockControl.Value;
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.cylinderCockControl))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt,
					typeof(PortReference)
					.GetProperty(nameof(PortReference.Value))
					.GetMethod));
				codes.Insert(i++, new CodeInstruction(OpCodes.Stloc_S, cylinderCockControl));

				//cylinderCockFlowNormalizedReadOut.Value = num3 * value;
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.cylinderCockFlowNormalizedReadOut))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, num3));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, cylinderCockControl));
				codes.Insert(i++, new CodeInstruction(OpCodes.Mul));
				codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt,
					typeof(Port)
					.GetProperty(nameof(Port.Value))
					.SetMethod));

				//float num4 = Mathf.Lerp(0f, 0.1f, value);
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_R4, 0.0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_R4, 0.1));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, cylinderCockControl));
				codes.Insert(i++, new CodeInstruction(OpCodes.Call,
					typeof(Mathf)
					.GetMethod(nameof(Mathf.Lerp))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Stloc_S, cylinderCockSteamPercent));

				//float num5 = num4 * admissionPressure;
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, cylinderCockSteamPercent));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.admissionPressure))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Mul));
				codes.Insert(i++, new CodeInstruction(OpCodes.Stloc_S, cylinderCockSteamPressure));

				//bool flag = isCylinderCrackedReadOut.Value > 0f;
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.isCylinderCrackedReadOut))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt,
					typeof(Port)
					.GetProperty(nameof(PortReference.Value))
					.GetMethod));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_R4, 0.0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Cgt));
				codes.Insert(i++, new CodeInstruction(OpCodes.Stloc_S, isCylinderCracked));

				//float num6 = (flag ? (0.5f * admissionPressure) : 0f);
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, isCylinderCracked));
				codes.Insert(i++, new CodeInstruction(OpCodes.Brtrue_S, cylinderIsCracked));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_R4, 0.0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Br_S, cylinderIsNotCracked));
				codes[i].labels.Add(cylinderIsCracked);
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_R4, 0.5));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.admissionPressure))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Mul));
				codes[i].labels.Add(cylinderIsNotCracked);
				codes.Insert(i++, new CodeInstruction(OpCodes.Stloc_S, cylinderCrackedPressureLoss));

				//cylinderCrackFlowNormalizedReadOut.Value = (flag ? (1f * num3) : 0f);

				//float num7 = num5 + num6;
				codes.Insert(i++, new CodeInstruction(OpCodes.Add));
				codes.Insert(i++, new CodeInstruction(OpCodes.Stloc_S, cylinderPressureLoss));
 new CodeInstruction(OpCodes.Ldloc_S, cylinderCockSteamPressure));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, cylinderCrackedPressureLoss));
				codes.Insert(i++,
				//float num8 = Mathf.Max(0f, admissionPressure - num7);
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_R4, 0.0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.admissionPressure))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldloc_S, cylinderPressureLoss));
				codes.Insert(i++, new CodeInstruction(OpCodes.Sub));
				codes.Insert(i++, new CodeInstruction(OpCodes.Call,
					typeof(Mathf)
					.GetMethod(nameof(Mathf.Max))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Stloc_S, modifiedAdmissionPressure));

			//call SimulateSteamConsumption

				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_1));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.admissionPressure))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt,
					typeof(ReciprocatingSteamEngine)
					.GetMethod(nameof(ReciprocatingSteamEngine.SimulateSteamConsumption))));

			//chuffEventReadOut.Value = 0
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldarg_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldfld,
					typeof(ReciprocatingSteamEngine)
					.GetField(nameof(ReciprocatingSteamEngine.chuffEventReadOut))));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_I4_0));
				codes.Insert(i++, new CodeInstruction(OpCodes.Conv_R4));
				codes.Insert(i++, new CodeInstruction(OpCodes.Callvirt,
					typeof(Port)
					.GetProperty(nameof(Port.Value))
					.SetMethod));

			//return true
				codes.Insert(i++, new CodeInstruction(OpCodes.Ldc_I4_1));
				codes.Insert(i++, new CodeInstruction(OpCodes.Ret));
				break;
			}

			return codes.AsEnumerable();
		}*/
	}
}

using HarmonyLib;
using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace SteamSimulationTweaks.Patches
{
	//[HarmonyPatch(typeof(SteamTables), nameof(SteamTables.SteamSpecificEnthalpy))]
	internal class SteamEnthalpyCalcPatch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			if (!Main.Settings.enableHighPressureSteamCalcs)
			{
				return codes;
			}

			int i = 0;
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = 2658.652363f;
					i++;
					break;
				}
			}
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = 25.7372562f;
					break;
				}
			}
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = -1.972435318f;
					break;
				}
			}
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = 0.06982953825f;
					break;
				}
			}
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = -0.000913995431f;
					break;
				}
			}

			return codes;
		}
	}

	//[HarmonyPatch(typeof(SteamTables), nameof(SteamTables.SteamSpecificVolume))]
	internal class SteamSpecificVolumePatch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			if (!Main.Settings.enableHighPressureSteamCalcs)
			{
				return codes;
			}

			int i = 0;
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = 1937.84012f;
					i++;
					break;
				}
			}
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = -1.001530101f;
					break;
				}
			}

			return codes;
		}
	}

	//[HarmonyPatch(typeof(SteamTables), nameof(SteamTables.WaterSpecificEnthalpy))]
	internal class WaterEnthalpyCalcPatch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			if (!Main.Settings.enableHighPressureSteamCalcs)
			{
				return codes;
			}

			int i = 0;
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = 415.8343707f;
					i++;
					break;
				}
			}
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = 0.2623148494f;
					break;
				}
			}

			return codes;
		}
	}

	//[HarmonyPatch(typeof(SteamTables), nameof(SteamTables.WaterSpecificVolume))]
	internal class WaterVolumeCalcPatch
	{
		static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
		{
			var codes = new List<CodeInstruction>(instructions);

			if (!Main.Settings.enableHighPressureSteamCalcs)
			{
				return codes;
			}

			int i = 0;
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = 1.076026913f;
					i++;
					break;
				}
			}
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = 0.004839953317f;
					break;
				}
			}
			for (; i < codes.Count; i++)
			{
				if (codes[i].opcode == OpCodes.Ldc_R4)
				{
					codes[i].operand = -0.00001175113383f;
					break;
				}
			}

			return codes;
		}
	}
}

using dnlib.DotNet;
using DV.Simulation.Cars;
using DV.Simulation.Controllers;
using DV.ThingTypes;
using DV.ThingTypes.TransitionHelpers;
using HarmonyLib;
using LocoSim.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamSimulationTweaks.Patches
{
	[HarmonyPatch(typeof(TrainCar), nameof(TrainCar.Awake))]
	internal class AddRealisticThrottleSimPatch
	{
		public const float S282_STEAM_CHEST_DIAMETER_M = 0.18f;
		public const float S282_STEAM_CHEST_LENGTH_M = 4.4f;//34f;
		public const float S282_STEAM_CHEST_VOLUME_M3
			= S282_STEAM_CHEST_DIAMETER_M * S282_STEAM_CHEST_DIAMETER_M / 4 * Mathf.PI * S282_STEAM_CHEST_LENGTH_M;

		public const float S060_STEAM_CHEST_DIAMETER_M = 0.15f;
		public const float S060_STEAM_CHEST_LENGTH_M = 3.5f + 1.5f; //1.5m dry pipe, 3.5m in smokebox
		public const float S060_STEAM_CHEST_VOLUME_M3
			= S060_STEAM_CHEST_DIAMETER_M * S060_STEAM_CHEST_DIAMETER_M / 4 * Mathf.PI * S060_STEAM_CHEST_LENGTH_M;

		[HarmonyAfter(new string[] { "RearrangedS282" })]
		static void Prefix(TrainCar __instance)
		{
			if (__instance is null)
			{
				return;
			}

			if (__instance.carLivery.id == "LocoS282A")
			{
				EnableOrDisableEmbers(__instance, Main.Settings.enableEmbers);
				EnableOrDisableLeakyLubricator(__instance, Main.Settings.enableLeakyLubricator);

				AddDelayedThrottleCalcDef(__instance,
					S282_STEAM_CHEST_VOLUME_M3,
					Main.Settings.s282MaxThrottleFlow,
					Main.Settings.enableFeedwaterHeaterChanges);
				AlterSafetyValvePressure(__instance, Main.Settings.s282MaxBoilerPressure);
				AddSuperheaterDef(__instance);
			}
			else if (__instance.carLivery.id == "LocoS060")
			{
				EnableOrDisableEmbers(__instance, Main.Settings.enableEmbers);
				EnableOrDisableLeakyLubricator(__instance, Main.Settings.enableLeakyLubricator);

				AddDelayedThrottleCalcDef(__instance,
					S060_STEAM_CHEST_VOLUME_M3,
					Main.Settings.s060MaxThrottleFlow,
					false);
				AlterSafetyValvePressure(__instance, Main.Settings.s060MaxBoilerPressure);
				if (Main.Settings.enableS060VolumetricEfficiencyChanges)
				{
					UseS282VolumetricEfficiency(__instance);
				}
			}
		}

		static void EnableOrDisableEmbers(TrainCar loco, bool enableEmbers)
		{
			if (enableEmbers)
			{
				//the embers are already enabled by default, no need to re-enable them.
				return;
			}

			if (loco.carLivery.id != "LocoS282A" && loco.carLivery.id != "LocoS060")
			{
				return;
			}

			SimController simController = loco.GetComponent<SimController>();
			if (simController is null)
			{
				return;
			}

			AParticlePortReader[] apprArray = simController.particlesController?.entries;
			if (apprArray is null)
			{
				return;
			}

			foreach (var appr in apprArray)
			{
				if (appr is not SteamSmokeParticlePortReader)
				{
					continue;
				}
				SteamSmokeParticlePortReader ssppr = (SteamSmokeParticlePortReader) appr;
				GameObject embersGO = ssppr.emberParticlesParent;
				foreach (var particleSystemRenderer in embersGO.GetComponentsInChildren<ParticleSystemRenderer>())
				{
					particleSystemRenderer.enabled = false;
				}
			}
		}

		static void EnableOrDisableLeakyLubricator(TrainCar loco, bool enableLeakyLubricator)
		{
			if (enableLeakyLubricator)
			{
				//lubricator is already enabled by default, no need to re-enable it.
				return;
			}

			MechanicalLubricatorDefinition lubricator = loco.transform.Find("[sim]/lubricator")?
				.GetComponent<MechanicalLubricatorDefinition>();
			if (lubricator is null)
			{
				return;
			}
			lubricator.oilLeakageRate = 0;
		}

		static void AlterSafetyValvePressure(TrainCar loco, float safetyValveOpeningPressure)
		{
			Transform sim = loco.transform.Find("[sim]");
			SimConnectionDefinition simConnectionDef = loco.GetComponent<SimController>().connectionsDefinition;
			GameObject boilerGO = sim.Find("boiler").gameObject;
			if (boilerGO == null)
			{
				Main.Logger.Error("Tried to alter safety valve on a locomotive wth no boiler");
			}
			GameObject fireboxGO = sim.Find("firebox").gameObject;
			if (fireboxGO == null)
			{
				Main.Logger.Error("Tried to alter safety valve on a locomotive wth no firebox");
			}

			var boilerDef = boilerGO.GetComponent<BoilerDefinition>();
			if (boilerDef == null)
			{
				Main.Logger.Error("Tried to alter safety valve on a locomotive wth no boiler definition");
			}
			var fireboxDef = fireboxGO.GetComponent<FireboxDefinition>();
			if (fireboxDef == null)
			{
				Main.Logger.Error("Tried to alter safety valve on a locomotive wth no firebox definition");
			}

			float safetyValveHysteresis = boilerDef.safetyValveOpeningPressure - boilerDef.safetyValveClosingPressure;
			float fireboxStartupHysteresis = boilerDef.safetyValveOpeningPressure - fireboxDef.startupMaxPressure;
			boilerDef.safetyValveOpeningPressure = safetyValveOpeningPressure + 1;
			boilerDef.safetyValveClosingPressure = safetyValveOpeningPressure + 1 - safetyValveHysteresis;
			fireboxDef.startupMaxPressure = safetyValveOpeningPressure + 1 - fireboxStartupHysteresis;

		}

		static void UseS282VolumetricEfficiency(TrainCar loco)
		{
			Transform sim = loco.transform.Find("[sim]");
			GameObject steamEngineGO = sim.Find("steamEngine").gameObject;
			var steamEngineDef = steamEngineGO.GetComponent<ReciprocatingSteamEngineDefinition>();
			if (steamEngineDef is null)
			{
				Main.Logger.Error("Cannot find the steamEngineDef");
				return;
			}
			GameObject s282EngineGO = TrainCarType.LocoSteamHeavy.ToV2().prefab
				.transform.Find("[sim]/steamEngine").gameObject;
			var s282EngineDef = s282EngineGO.GetComponent<ReciprocatingSteamEngineDefinition>();
			steamEngineDef.volumetricEfficiency = s282EngineDef.volumetricEfficiency;
		}

		static void AddDelayedThrottleCalcDef(TrainCar loco, float steamChestVolume, float maxMassFlow,
			bool hasFeedwaterHeater)
		{
			Transform sim = loco.transform.Find("[sim]");

			SimConnectionDefinition simConnectionDef = loco.GetComponent<SimController>().connectionsDefinition;
			GameObject throttleCalcGO = sim.Find("throttleCalculator").gameObject;
			var throttleCalcDef = throttleCalcGO.AddComponent<ImprovedThrottleCalculatorDefinition>();
			throttleCalcDef.ID = "throttleCalculator";
			throttleCalcDef.steamChestVolume = steamChestVolume;
			throttleCalcDef.maxMassFlow = maxMassFlow;

			//We need to replace the old reference in the SimConnectionDefinition
			for (int i = 0; i < simConnectionDef.executionOrder.Length; i++)
			{
				if (simConnectionDef.executionOrder[i].ID == "throttleCalculator")
				{
					simConnectionDef.executionOrder[i] = throttleCalcDef;
					break;
				}
			}
			throttleCalcGO.GetComponent<ConfigurableMultiplierDefinition>().enabled = false;
			
			PortReferenceConnection[] newPortReferenceConnections = {
				new("throttleCalculator.STEAM_FLOW", "steamEngine.STEAM_FLOW"),
				new("throttleCalculator.DUMP_FLOW", "steamEngine.DUMPED_FLOW"),
				new("throttleCalculator.STEAM_CHEST_TEMPERATURE", "firebox.TEMPERATURE"),
			};

			if (hasFeedwaterHeater)
			{
				GameObject feedwaterHeaterGO = new GameObject("feedwaterHeater");
				feedwaterHeaterGO.transform.parent = sim;
				var feedwaterHeater = feedwaterHeaterGO.AddComponent<FeedwaterHeaterDefinition>();
				simConnectionDef.executionOrder = simConnectionDef.executionOrder
					.Append(feedwaterHeater).ToArray();

				for (int i = 0; i < simConnectionDef.portReferenceConnections.Length; i++)
				{
					PortReferenceConnection prc = simConnectionDef.portReferenceConnections[i];
					if (prc.portReferenceId == "boiler.FEEDWATER_TEMPERATURE")
					{
						prc.portId = "feedwaterHeater.FEEDWATER_TEMPERATURE";
						break;
					}
				}

				newPortReferenceConnections = newPortReferenceConnections
					.Append(new PortReferenceConnection("feedwaterHeater.EXHAUST_PRESSURE", "steamEngine.EXHAUST_PRESSURE"))
					.ToArray();
			}

			simConnectionDef.portReferenceConnections
					= simConnectionDef.portReferenceConnections
					.Concat(newPortReferenceConnections)
					.ToArray();
		}

		static void AddSuperheaterDef(TrainCar loco)
		{
			Transform sim = loco.transform.Find("[sim]");

			SimConnectionDefinition simConnectionDef = loco.GetComponent<SimController>().connectionsDefinition;
			GameObject superheaterGO = new GameObject("superheater");
			superheaterGO.transform.parent = sim.transform;
			var superheaterDef = superheaterGO.AddComponent<SuperheaterDefinition>();
			superheaterDef.ID = "superheater";

			simConnectionDef.executionOrder = simConnectionDef.executionOrder
					.Append(superheaterDef).ToArray();

			PortReferenceConnection[] newPortReferenceConnections = {
				new("superheater.FIREBOX_TEMPERATURE", "firebox.TEMPERATURE"),
				new("superheater.FLUE_FLOW", "exhaust.AIR_FLOW"),
				new("superheater.STEAM_FLOW", "steamEngine.STEAM_FLOW"),
				new("superheater.STEAM_TEMP_IN", "boiler.TEMPERATURE"),
			};

			//replace existing steam temp connection from firebox to steamEngine
			for (int i = 0; i < simConnectionDef.portReferenceConnections.Length; i++)
			{
				PortReferenceConnection prc = simConnectionDef.portReferenceConnections[i];
				if (prc.portReferenceId == "steamEngine.STEAM_CHEST_TEMPERATURE")
				{
					prc.portId = "superheater.STEAM_TEMP_OUT";
				}
				if (prc.portReferenceId == "duplexSteamEngine.STEAM_CHEST_TEMPERATURE")
				{
					prc.portId = "superheater.STEAM_TEMP_OUT";
				}
			}

			simConnectionDef.portReferenceConnections
					= simConnectionDef.portReferenceConnections
					.Concat(newPortReferenceConnections)
					.ToArray();
		}
	}
}

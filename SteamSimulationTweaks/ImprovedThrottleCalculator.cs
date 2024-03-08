using LocoSim.Definitions;
using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamSimulationTweaks
{
	internal class ImprovedThrottleCalculator : SimComponent
	{
		public readonly PortReference boilerPressure;
		public readonly PortReference throttlePosition;
		public readonly Port steamChestPressure;
		public readonly PortReference cylSteamFlow;
		public readonly PortReference cylDumpSteamFlow;
		public readonly PortReference steamChestTemperature;

		private /*readonly*/ float steamChestVolume;
		private /*readonly*/ float maxMassFlow;

		public ImprovedThrottleCalculator(ImprovedThrottleCalculatorDefinition confMult) : base(confMult.ID)
		{
			boilerPressure = AddPortReference(confMult.boilerPressure);
			throttlePosition = AddPortReference(confMult.throttlePosition);
			steamChestPressure = AddPort(confMult.steamChestPressure);
			cylSteamFlow = AddPortReference(confMult.cylSteamFlow);
			cylDumpSteamFlow = AddPortReference(confMult.cylDumpSteamFlow);
			steamChestTemperature = AddPortReference(confMult.steamChestTemperature);
			steamChestVolume = confMult.steamChestVolume;
			maxMassFlow = confMult.maxMassFlow;
			steamChestPressure.Value = 0;
		}

		//TODO: tune locoScalar
		//- maybe think of a way to delay the throttle response. rather than just limiting the slope
		//- maybe enable back pressure?
		//- maybe use WaterPressureVessel code?
		//- Maybe use SteamTables, if WaterPressureVessel doesn't work for our purposes?

		public override void Tick(float delta)
		{
			float totalCylSteamFlow = cylSteamFlow.Value + cylDumpSteamFlow.Value;
			float maxThrottleSteamFlow = maxMassFlow * throttlePosition.Value;

			if (totalCylSteamFlow < maxThrottleSteamFlow)
			{
				//if the throttle is not restricting the air flow, we slowly
				//increase steam chest pressure all the way to boiler pressure
				float newSteamChestPressure = steamChestPressure.Value
					+ (maxThrottleSteamFlow - totalCylSteamFlow)
					/ steamChestVolume
					* delta * 60
					* 0.03f;
				steamChestPressure.Value = Mathf.Min(newSteamChestPressure, boilerPressure.Value);
			}
			else if (totalCylSteamFlow > maxThrottleSteamFlow)
			{
				//if the throttle is restricting the air flow, we decrease the
				//steam chest pressure until totalCylSteamFlow matches maxThrottleSteamFlow
				//Main.Logger.Log("Cylinder steam flow: " + totalCylSteamFlow);
				//Main.Logger.Log("Max throttle steam flow: " + maxThrottleSteamFlow);
				float newSteamChestPressure = steamChestPressure.Value
					- (totalCylSteamFlow - maxThrottleSteamFlow)
					/ steamChestVolume
					* delta * 60
					* 0.03f;
				steamChestPressure.Value = Mathf.Max(0, newSteamChestPressure);
			}
		}
	}
}

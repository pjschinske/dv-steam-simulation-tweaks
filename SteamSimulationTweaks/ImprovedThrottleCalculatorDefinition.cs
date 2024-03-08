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
	internal class ImprovedThrottleCalculatorDefinition : SimComponentDefinition
	{

		public PortReferenceDefinition boilerPressure = new PortReferenceDefinition(PortValueType.PRESSURE, "BOILER_PRESSURE");
		public PortReferenceDefinition throttlePosition = new PortReferenceDefinition(PortValueType.CONTROL, "THROTTLE");
		public PortDefinition steamChestPressure = new PortDefinition(PortType.READONLY_OUT, PortValueType.PRESSURE, "STEAM_CHEST_PRESSURE");
		public PortReferenceDefinition cylSteamFlow = new PortReferenceDefinition(PortValueType.MASS_RATE, "STEAM_FLOW");
		public PortReferenceDefinition cylDumpSteamFlow = new PortReferenceDefinition(PortValueType.MASS_RATE, "DUMP_FLOW");
		public PortReferenceDefinition steamChestTemperature = new PortReferenceDefinition(PortValueType.TEMPERATURE, "STEAM_CHEST_TEMPERATURE");
		public PortReferenceDefinition wheelSpeed = new PortReferenceDefinition(PortValueType.TEMPERATURE, "WHEEL_SPEED");

		public float steamChestVolume;
		public float maxMassFlow;
		public AnimationCurve volumetricEfficiency;

		public override SimComponent InstantiateImplementation()
		{
			ID = "throttleCalculator";
			return new ImprovedThrottleCalculator(this);
		}
	}
}

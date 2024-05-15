using LocoSim.Definitions;
using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamSimulationTweaks
{
	internal class SuperheaterDefinition : SimComponentDefinition
	{
		public PortReferenceDefinition fireboxTemp = new PortReferenceDefinition(PortValueType.TEMPERATURE, "FIREBOX_TEMPERATURE");
		public PortReferenceDefinition flueAirFlow = new PortReferenceDefinition(PortValueType.PRESSURE, "FLUE_FLOW");
		public PortReferenceDefinition steamFlow = new PortReferenceDefinition(PortValueType.MASS_RATE, "STEAM_FLOW");
		public PortReferenceDefinition steamTempIn = new PortReferenceDefinition(PortValueType.TEMPERATURE, "STEAM_TEMP_IN");
		public PortDefinition steamTempOut = new PortDefinition(PortType.READONLY_OUT, PortValueType.TEMPERATURE, "STEAM_TEMP_OUT");

		public float flueAirFlowThreshold;

		public override SimComponent InstantiateImplementation()
		{
			ID = "superheater";
			return new Superheater(this);
		}
	}
}

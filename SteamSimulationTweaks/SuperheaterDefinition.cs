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
		public PortReferenceDefinition fireboxTemp = new PortReferenceDefinition(PortValueType.PRESSURE, "FIREBOX_TEMPERATURE");
		public PortReferenceDefinition flueAirFlow = new PortReferenceDefinition(PortValueType.PRESSURE, "FLUE_FLOW");
		public PortDefinition steamChestTemp = new PortDefinition(PortType.READONLY_OUT, PortValueType.TEMPERATURE, "STEAM_CHEST_TEMPERATURE");

		public float flueAirFlowThreshold;

		public override SimComponent InstantiateImplementation()
		{
			ID = "superheater";
			return new Superheater(this);
		}
	}
}

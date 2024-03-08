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
	internal class FeedwaterHeaterDefinition : SimComponentDefinition
	{
		public PortReferenceDefinition exhaustPressure = new PortReferenceDefinition(PortValueType.PRESSURE, "EXHAUST_PRESSURE");
		public PortDefinition feedwaterTemp = new PortDefinition(PortType.READONLY_OUT, PortValueType.TEMPERATURE, "FEEDWATER_TEMPERATURE");

		public float pressureThreshold = 0.1f;
		public float idleTemp = 25;
		public float hotTemp = 110;

		public override SimComponent InstantiateImplementation()
		{
			ID = "feedwaterHeater";
			return new FeedwaterHeater(this);
		}
	}
}

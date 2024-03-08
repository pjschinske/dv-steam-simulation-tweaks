using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SteamSimulationTweaks
{
	internal class Superheater : SimComponent
	{
		public readonly PortReference fireboxTemp;
		public readonly PortReference flueAirFlow;
		//public readonly PortReference steamFlow;
		public readonly Port steamChestTemp;

		public /*readonly*/ float flueAirFlowThreshold;

		public Superheater(SuperheaterDefinition def) : base(def.ID)
		{
			fireboxTemp = AddPortReference(def.fireboxTemp);
			flueAirFlow = AddPortReference(def.flueAirFlow);
			steamChestTemp = AddPort(def.steamChestTemp, fireboxTemp.Value);
			
		}

		public override void Tick(float delta)
		{
			throw new NotImplementedException();
			/*if (flueAirFlow.Value > flueAirFlowThreshold)
			{

			}*/
		}
	}
}

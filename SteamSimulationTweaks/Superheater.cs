using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamSimulationTweaks
{
	internal class Superheater : SimComponent
	{
		public readonly PortReference fireboxTemp;
		public readonly PortReference flueAirFlow;
		public readonly PortReference steamFlow;
		public readonly PortReference steamTempIn;
		public readonly Port steamTempOut;

		public /*readonly*/ float flueAirFlowUpperThreshold = 5;
		public /*readonly*/ float flueAirFlowLowerThreshold = -0.5f;

		//superheaters usually only get up to about 700-750 degrees f (~400 C), but
		//the in-game superheater goes all the way up to the temp of the firebox.
		public readonly float efficiency = 0.5f;

		public readonly float steamFlowDerateMultiplier = 0.1f;

		public readonly float smoothingTime = 1;

		private float steamTempOutVel;

		public Superheater(SuperheaterDefinition def) : base(def.ID)
		{
			fireboxTemp = AddPortReference(def.fireboxTemp);
			flueAirFlow = AddPortReference(def.flueAirFlow);
			steamFlow = AddPortReference(def.steamFlow);
			steamTempIn = AddPortReference(def.steamTempIn);
			steamTempOut = AddPort(def.steamTempOut, fireboxTemp.Value);

		}

		public override void Tick(float delta)
		{
			float maxTempDelta = Mathf.Max(0, (fireboxTemp.Value - steamTempIn.Value) * Main.Settings.superheaterEffectiveness);

			//more air flowing through the flues means more superheat
			float flueAirFlowDerate = Mathf.InverseLerp(flueAirFlowLowerThreshold, flueAirFlowUpperThreshold, flueAirFlow.Value);

			//less steam flowing through superheater means we heat up the steam faster
			float steamFlowDerate = steamFlow.Value * steamFlowDerateMultiplier + 1;

			steamTempOut.Value = Mathf.SmoothDamp(
				steamTempOut.Value,
				maxTempDelta * flueAirFlowDerate + steamTempIn.Value,
				ref steamTempOutVel,
				smoothingTime * steamFlowDerate,
				Mathf.Infinity,
				delta);

			if (steamTempOut.Value < steamTempIn.Value)
			{
				steamTempOut.Value = steamTempIn.Value;
			}
		}
	}
}

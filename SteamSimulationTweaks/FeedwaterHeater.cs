using LocoSim.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamSimulationTweaks
{
	internal class FeedwaterHeater : SimComponent
	{
		public readonly PortReference exhaustPressure;
		public readonly Port feedwaterTemp;

		public /*readonly*/ float pressureThreshold;
		public /*readonly*/ float idleTemp;
		public /*readonly*/ float hotTemp;

		private float feedwaterTempVel;

		public FeedwaterHeater(FeedwaterHeaterDefinition def) : base(def.ID)
		{
			exhaustPressure = AddPortReference(def.exhaustPressure);
			feedwaterTemp = AddPort(def.feedwaterTemp, def.idleTemp);
			pressureThreshold = def.pressureThreshold;
			idleTemp = def.idleTemp;
			hotTemp = def.hotTemp;
		}

		public override void Tick(float delta)
		{
			if (exhaustPressure.Value > pressureThreshold)
			{
				feedwaterTemp.value = Mathf.SmoothDamp(feedwaterTemp.value, hotTemp, ref feedwaterTempVel, Main.Settings.feedwaterTempSmoothTimeUp, Mathf.Infinity, delta);
			}
			else
			{
				feedwaterTemp.value = Mathf.SmoothDamp(feedwaterTemp.value, idleTemp, ref feedwaterTempVel, Main.Settings.feedwaterTempSmoothTimeDown, Mathf.Infinity, delta);
			}
		}
	}
}

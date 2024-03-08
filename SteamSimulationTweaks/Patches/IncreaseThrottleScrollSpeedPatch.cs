using DV.CabControls;
using DV.Simulation.Controllers;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace SteamSimulationTweaks.Patches
{
	[HarmonyPatch(typeof(LeverBase), nameof(LeverBase.SetSpringTarget))]
	class IncreaseThrottleScrollSpeedPatch
	{
		static void Prefix(LeverBase __instance, ref float target)
		{
			if (__instance.hj.name != "C_Regulator"
			/*|| __instance.car.carLivery.id != "LocoS282A"*/)
			{
				return;
			}
			/*Main.Logger.Log("Old target: " + target);
			if (__instance.hj.spring.targetPosition > target)
			{
				target = __instance.hj.limits.min;
			}
			else
			{
				target = __instance.hj.limits.max;
			}
			Main.Logger.Log("New target: " + target);*/
			JointSpring spring = __instance.hj.spring;
			spring.spring = __instance.spec.scrollWheelSpring * 3;
			//Main.Logger.Log("New spring strength: " + spring.spring);
			__instance.hj.spring = spring;
		}
	}
}

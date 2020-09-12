using System;
using UnityEngine;
using Verse;

namespace MuvLuvBeta
{
	// Token: 0x0200030D RID: 781
	public class MoteCompTurretAttached : MoteDualAttached
	{
		public void Attach(TargetInfo a, Comp_TurretGun turretGun)
		{
			this.link1 = new MoteAttachLink(a);
			this.turretGun = turretGun;
		}
		private Comp_TurretGun turretGun = null;
		public override Vector3 DrawPos => turretGun?.TurretPos ?? base.DrawPos;

	}
}

using System;
using UnityEngine;
using Verse;

namespace OgsOld_CompTurret
{
	// Token: 0x0200030D RID: 781
	public class MoteOgsOld_CompTurretAttached : MoteDualAttached
	{
		public void Attach(TargetInfo a, OgsOld_CompTurretGun turretGun)
		{
			this.link1 = new MoteAttachLink(a,Vector3.zero);
			this.turretGun = turretGun;
		}
		private OgsOld_CompTurretGun turretGun = null;
		public override Vector3 DrawPos => turretGun?.TurretPos ?? base.DrawPos;

	}
}

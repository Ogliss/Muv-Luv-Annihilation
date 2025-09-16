using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace OgsOld_CompTurret
{
    // OgsOld_CompTurret.ApparelTurret
    class ApparelTurret : Apparel
    {
        private List<OgsOld_CompTurret> turrets;
        public List<OgsOld_CompTurret> Turrets
        {
            get
            {
                if (turrets.NullOrEmpty())
                {
                    if (turrets == null)
                    {
                        turrets = new List<OgsOld_CompTurret>();
                        for (int i = 0; i < this.AllComps.Count; i++)
                        {
                            OgsOld_CompTurret t = this.AllComps[i] as OgsOld_CompTurret;
                            if (t != null)
                            {
                                turrets.Add(t);
                            }
                        }
                    }
                }
                return turrets;
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (Wearer != null)
            {
                for (int i = 0; i < Turrets.Count; i++)
                {
                    OgsOld_CompTurretGun turretGun = Turrets[i] as OgsOld_CompTurretGun;
                    if (turretGun != null)
                    {
                        turretGun.CompTick();
                    }
                }
            }
        }
    }
}

using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Verse;

namespace MuvLuvAnnihilation
{
    public static class Constants
    {
        //   public static int AMSeed = 454385387;
        public static int MLASeed = 454353387;
        public static readonly string ModPrefix = "MLA_";

        //    public static Color CloakColor = new Color(0.25f, 0.25f, 0.25f, 0.0001f);
        public static int CloakNoiseTex = Shader.PropertyToID("_NoiseTex");
    }
}

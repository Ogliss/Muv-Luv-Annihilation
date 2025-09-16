using System.Linq;
using Verse;
using UnityEngine;
using HarmonyLib;
using System.Collections.Generic;

namespace MuvLuvAnnihilation.HarmonyInstance
{
    [HarmonyPatch(typeof(PawnRenderNodeWorker), "AppendDrawRequests")]
    public static class PawnRenderNodeWorker_AppendDrawRequests_Patch
    {
        public static bool Prefix(PawnRenderNode node, PawnDrawParms parms, List<PawnGraphicDrawRequest> requests)
        {
            if (parms.pawn?.apparel != null)
            {
                foreach (var apparel in parms.pawn.apparel.WornApparel)
                {
                    var extraDrawer = apparel.TryGetComp<CompExosuitDrawer>();
                    if (extraDrawer != null)
                    {
                        if (extraDrawer.hidesBody && (node is PawnRenderNode_Body 
                            || node.parent is PawnRenderNode_Body)
                            || extraDrawer.hidesHair && node is PawnRenderNode_Hair
                            || extraDrawer.hidesHead && (node is PawnRenderNode_Head
                            || node.parent is PawnRenderNode_Head))
                        {
                            requests.Add(new PawnGraphicDrawRequest(node)); // adds an empty draw request to not draw head
                            return false;
                        }

                    }
                }
            }
            return true;
        }
    }
}

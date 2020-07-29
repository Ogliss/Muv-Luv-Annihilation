
using RimWorld;
using Verse;

namespace Gun_ThorHammer//namespace for the weapons XML
{
    public class Projectile_Bullet_ThorHammer : Bullet//Projectile_LightningStrike is a class we made. it is inheriting the bullet class(eg was:"Projectile_LightningStrike")
    {
        protected override void Impact(Thing hitThing)//override is replacing how something works by default. hitThing is asigned a value.
        {
            base.Map.weatherManager.eventHandler.AddEvent(new WeatherEvent_LightningStrike(base.Map, base.Position));//taking the base map and position of the shot.

            base.Impact(hitThing);//base is the normal version before override.
        }
    }
}
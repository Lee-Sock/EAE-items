using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;



namespace testing.NPCs.Boss
{
    public class TesterBossProjectile : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("Tester's testing ball");

        }

        public override void SetDefaults()
        {
            projectile.width = 18;
            projectile.height = 42;
            projectile.aiStyle = 1;
            aiType = ProjectileID.Bullet;
            projectile.friendly = false;
            projectile.ignoreWater = true;
            projectile.penetrate = 10;
            projectile.timeLeft = 500;
            projectile.tileCollide = true;
            projectile.hostile = true;
            projectile.scale = 1.2f;
            ProjectileID.Sets.TrailCacheLength[projectile.type] = 5;
        }






        public override Color? GetAlpha(Color lightColor)
        {
            return Color.Purple;
        }

        public override void AI()
        {
            projectile.velocity.Y += projectile.ai[0];
        }

    }
}

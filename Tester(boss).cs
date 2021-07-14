using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria;
using Terraria.ID;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;




namespace testing.NPCs.Boss
{
   // [AutoloadBossHead]
    class Tester_boss_ : ModNPC
    {
        private int ai;
        private int attacktimer = 0;
        private bool fastspeed = false;

        private bool stunned;
        private int stunnedtimer;

        private int frame = 0;
        private double counting;
        
        public override void SetStaticDefaults()
        {
            DisplayName.SetDefault("tester");
            Main.npcFrameCount[npc.type] = 1;
        }
        public override void SetDefaults()
        {
            npc.width = 64;
            npc.height = 64;
            
            npc.aiStyle = -1;
            
            npc.damage = 16;
            npc.defense = 3;
            npc.lifeMax = 300;
            npc.knockBackResist = 0f;

            npc.HitSound = SoundID.NPCHit1;
            npc.DeathSound = SoundID.NPCDeath9;
            music = MusicID.Boss4;          
           
            npc.value = 1000f;

            npc.lavaImmune = true;
            npc.noGravity = true;
            npc.noTileCollide = true;

            bossBag = mod.ItemType("testerTreasureBag");
            
            
        }

        public override void ScaleExpertStats(int numPlayers, float bossLifeScale)
        {
            npc.lifeMax = (int)(npc.lifeMax * bossLifeScale);
            npc.damage = (int)(npc.damage * 1.3f);
        }

        public override void AI()
        {
            // getting player targetting
            npc.TargetClosest(true);
            Player player = Main.player[npc.target];
            Vector2 target = npc.HasPlayerTarget ? player.Center : Main.npc[npc.target].Center;

            // no spinny buisness
            npc.rotation = 0f;
            npc.netAlways = true;
            npc.TargetClosest(true);

            //boss will never have more health than its max
            if (npc.life >= npc.lifeMax)
            {
                npc.life = npc.lifeMax;
            }

            //despawning
            if (npc.target < 0 || npc.target == 255 || player.dead || !player.active)
            {
                npc.TargetClosest(false);
                npc.direction = 1;
                npc.velocity.Y = npc.velocity.Y - 0.1f;
                if (npc.timeLeft > 20)
                {
                    npc.timeLeft = 20;
                    return;
                }
            }


            //stunned
            if (stunned)
            {
                npc.velocity.X = 0;
                npc.velocity.Y = 0;
                stunnedtimer++;
                if (stunnedtimer >= 100)
                {
                    stunned = false;
                    stunnedtimer = 0;
                }

            }





            // phases
            ai++;

            //movement
            npc.ai[0] = (float)ai * 1f;
            int distance = (int)Vector2.Distance(target, npc.Center);
            if ((double)npc.ai[0] < 300)
            {
                frame = 0;
                MoveTowards(npc, target, (float)(distance > 300 ? 13f : 7f), 30f);
                npc.netUpdate = true;
            }
            else if ((double)npc.ai[0] >= 300 && (double)npc.ai[0] < 450)
            {
                stunned = true;
                frame = 1;
                npc.damage = 10;
                npc.defense = 40;
                MoveTowards(npc, target, (float)(distance > 300 ? 13f : 7f), 30f);
                npc.netUpdate = true;

            }
            else if ((double)npc.ai[0] >= 450)
            {
                stunned = false;
                frame = 2;
                npc.damage = 40;
                npc.defense = 20;
                if (!fastspeed)
                {
                    fastspeed = true;
                }
                else
                {
                    if ((double)npc.ai[0] % 50 == 0)
                    {
                        float speed = 12f;
                        Vector2 vector = new Vector2(npc.position.X + (float)npc.width * 0.5f, npc.position.Y + (float)npc.height * 0.5f);
                        float x = player.position.X + (float)(player.width / 2) - vector.X;
                        float y = player.position.Y + (float)(player.height / 2) - vector.Y;
                        float distance2 = (float)Math.Sqrt(x * x + y * y);
                        float factor = speed / distance2;
                        npc.velocity.X = x * factor;
                        npc.velocity.Y = y * factor;
                    }
                }
                npc.netUpdate = true;
            }

            //attacks
            if ((double)npc.ai[0] % (Main.expertMode ? 100 : 150) == 0 && !stunned && !fastspeed)
            {
                attacktimer++;
                if (attacktimer <= 2)
                {
                    frame = 2;
                    npc.velocity.X = 0;
                    npc.velocity.Y = 0;
                    Vector2 shootPos = npc.Center;
                    float accuracy = 5f * (npc.life / npc.lifeMax);
                    Vector2 shootVel = target - shootPos + new Vector2(Main.rand.NextFloat(-accuracy, accuracy), Main.rand.NextFloat(-accuracy, accuracy));
                    shootVel.Normalize();
                    shootVel *= 14.5f;
                    for (int i = 0; i < (Main.expertMode ? 5 : 3); i++)
                    {
                        Projectile.NewProjectile(shootPos.X + (float)(-100 * npc.direction) + (float)Main.rand.Next(-40, 41), shootPos.Y - (float)Main.rand.Next(-50, 40), shootVel.X, shootVel.Y, mod.ProjectileType("TesterBossProjectile"), npc.damage / 3, 5f);
                    }

                }
                else
                {
                    attacktimer = 0;
                }



            }

            if ((double)npc.ai[0] >= 650)
            {
                ai = 0;
                npc.alpha = 0;
                fastspeed = false;

            }

        }




        //Movetowards but custom
        private void MoveTowards(NPC npc, Vector2 playerTarget, float speed, float turnResistance) 
        {
            var move = playerTarget - npc.Center;
            float length = move.Length();
            if(length > speed) 
            {
                move *= speed / length; 
            }
            move = (npc.velocity * turnResistance + move) / (turnResistance + 1f);
            length = move.Length();
            if (length > speed)
            {
                move *= speed / length;
            }
            npc.velocity = move;
        }

        public override void BossLoot(ref string name, ref int potionType)
        {
            potionType = ItemID.HealingPotion;
        }

        public override void NPCLoot()
        {

            if (Main.expertMode)
            {
                npc.DropBossBags();
            }
            else 
            {
                Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, ItemID.LifeCrystal, Main.rand.Next(1, 3));
                if(Main.rand.Next(7) == 0) 
                {
                    Item.NewItem((int)npc.position.X, (int)npc.position.Y, npc.width, npc.height, mod.ItemType("Tester_boss_Summon"), 1);
                }                                
            }
        }

    }
}

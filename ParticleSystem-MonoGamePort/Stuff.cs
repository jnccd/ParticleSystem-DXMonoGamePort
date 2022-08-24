using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Diagnostics;
using System.IO;

namespace ParticleSystemV3
{
    public static class GameValues
    {
        public static Vector2 ScreenSize = new Vector2(1920, 1080);
        public static float GravForce = 3000;
        public static float Friction = 1.005f;
        public static bool FrictionEnabled = true;
        public static Random RDM = new Random();
        public static int ClickCooldown = 0;
        public static int GravityMode = 0;
        public static int DrawMethod = 0;
        public static bool Bloom = false;
        public static int WaterSize = 40;
        public static bool particleCollision = false;
    }

    
}

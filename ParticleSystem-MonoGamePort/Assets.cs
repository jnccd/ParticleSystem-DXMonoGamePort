using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using ParticleSystemV3;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSystemV3
{
    public static class Assets
    {
        public static Texture2D Default;
        public static Texture2D Arrow;
        public static Texture2D BigCircle;
        public static Texture2D SmallerCircle;
        public static SpriteFont Font;
        public static Effect Light;
        public static Effect Test;

        public static void Load(GraphicsDevice graphicsDevice, ContentManager Content)
        {
            Default = new Texture2D(graphicsDevice, 1, 1, false, SurfaceFormat.Color);
            Color[] Col = new Color[1];
            Col[0] = new Color(255, 255, 255, 255);
            Default.SetData<Color>(Col);

            Arrow = Content.Load<Texture2D>("Arrow");
            BigCircle = Content.Load<Texture2D>("BigCircle");
            SmallerCircle = Content.Load<Texture2D>("SmallerCircle");

            Light = Content.Load<Effect>("ParticleLight");
            Light.Parameters["WindowSize"].SetValue(GameValues.ScreenSize);

            Test = Content.Load<Effect>("Test");
            Test.Parameters["BlurWeights"].SetValue(Enumerable.Range(-7, 15).Select(x => (float)Math.Pow(0.9, x * x)).ToArray());
            Test.Parameters["i_texsize"].SetValue(new Vector2(1/GameValues.ScreenSize.X, 1/GameValues.ScreenSize.Y));

            Font = Content.Load<SpriteFont>("SpriteFont");
        }
    }
}

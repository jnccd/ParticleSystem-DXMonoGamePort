﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ParticleSystemV3;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ParticleSystemV3
{
    public static class ControlHandler
    {
        public static MouseState CurMS;
        public static MouseState LastMS;
        public static KeyboardState CurKS;
        public static KeyboardState LastKS;

        public static float Angle;
        public static float Strength;

        static int PressedFTimer = 0;

        public static void Update()
        {
            if (CurMS != null)
            {
                LastMS = CurMS;
            }

            if (CurKS != null)
            {
                LastKS = CurKS;
            }

            CurMS = Mouse.GetState();
            CurKS = Keyboard.GetState();

            if (GameValues.ClickCooldown > 0)
            {
                GameValues.ClickCooldown--;
            }
        }
        public static Vector2 GetMouseVector() { return new Vector2(CurMS.X, CurMS.Y); }
        public static void HandleControls(List<Particle> ListOfParticles, GraphicsDevice GD)
        {
            lock (ListOfParticles)
            {
                if (ControlHandler.CurKS.IsKeyDown(Keys.Insert))
                {
                    EingabenAnzeige.SetNewText("Trying to load texture...");
                    Texture2D Insertion = null;
                    try
                    {
                        string path = System.Windows.Forms.Application.StartupPath;
                        List<string> pictureFiles = Directory.GetFiles(path, "*.jpg", SearchOption.TopDirectoryOnly).ToList();
                        pictureFiles.AddRange(Directory.GetFiles(path, "*.png", SearchOption.TopDirectoryOnly).ToList());
                        pictureFiles.AddRange(Directory.GetFiles(path, "*.PNG", SearchOption.TopDirectoryOnly).ToList());
                        pictureFiles.AddRange(Directory.GetFiles(path, "*.jpeg", SearchOption.TopDirectoryOnly).ToList());
                        FileStream stream = new FileStream(pictureFiles[0], FileMode.Open);
                        Insertion = Texture2D.FromStream(GD, stream);
                        stream.Dispose();
                    }
                    catch (Exception e)
                    {
                        EingabenAnzeige.SetNewText("Couldn't load texture! Exeption: " + e.ToString());
                    }

                    if (Insertion != null)
                    {
                        Color[] Col1D = new Color[Insertion.Width * Insertion.Height];
                        Insertion.GetData(Col1D);

                        Color[,] Col2D = new Color[Insertion.Width, Insertion.Height];

                        for (int i = 0; i < Col1D.Length; i++)
                            Col2D[i % Insertion.Width, i / Insertion.Width] = Col1D[i];

                        ParticleManager.ClearParticles();

                        int HorizontalStepLength = (int)(4 * (Col2D.GetLength(0) / GameValues.ScreenSize.X));
                        int VerticalStepLength = (int)(4 * (Col2D.GetLength(1) / GameValues.ScreenSize.Y));

                        Vector2 TranslationVector = new Vector2(-Insertion.Width / 2, -Insertion.Height / 2);

                        if (HorizontalStepLength < 1)
                            HorizontalStepLength = 1;

                        if (VerticalStepLength < 1)
                            VerticalStepLength = 1;

                        int Middle = (HorizontalStepLength + VerticalStepLength) / 2;

                        for (int x = 0; x < Col2D.GetLength(0); x += Middle)
                            for (int y = 0; y < Col2D.GetLength(1); y += Middle)
                                if (!(Col2D[x, y].R == 0 && Col2D[x, y].G == 0 && Col2D[x, y].B == 0 || Col2D[x, y].A == 0))
                                    ParticleManager.AddParticle(new Particle((new Vector2(x, y) + TranslationVector) / Middle +
                                        GameValues.ScreenSize / 2, Vector2.Zero, Col2D[x, y]));

                        EingabenAnzeige.SetNewText("SUCCessfully loaded texture!");
                    }
                    else
                    {
                        EingabenAnzeige.SetNewText("Uhm... There is no texture. Try putting the image into the same foulder as the program");
                    }
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.V))
                {
                    EingabenAnzeige.SetNewText("Gave all Particles a Velocity of: " + Vector2.Zero.ToString());
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].Vel = Vector2.Zero;
                    }
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.J))
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].Pos += new Vector2(GameValues.RDM.Next(-3, 4), GameValues.RDM.Next(-3, 4));
                    }
                }

                if (ControlHandler.CurMS.LeftButton == ButtonState.Pressed && GameValues.ClickCooldown == 0)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].GetPulledBy(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), true, 2f);
                    }

                    GameValues.ClickCooldown = 7;
                }

                if (ControlHandler.CurMS.RightButton == ButtonState.Pressed && ControlHandler.LastMS.RightButton == ButtonState.Released)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].GetPulledBy(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), false, 5f);
                    }
                }

                if (ControlHandler.CurMS.MiddleButton == ButtonState.Pressed && GameValues.ClickCooldown == 0 ||
                    CurKS.IsKeyDown(Keys.Z) && GameValues.ClickCooldown == 0)
                {
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].OrbitAround(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), false);
                    }

                    GameValues.ClickCooldown = 7;
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.E))
                {
                    ParticleManager.AddParticle(new Particle(ControlHandler.CurMS.X, ControlHandler.CurMS.Y, 10, 0));
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.D))
                {
                    EingabenAnzeige.SetNewText("Deleted all Particles");
                    ListOfParticles.Clear();
                }

                if (CurKS.IsKeyDown(Keys.O))
                {
                    if (ParticleManager.ParticleVisibility < 1)
                    {
                        ParticleManager.ParticleVisibility += 0.004f;
                        EingabenAnzeige.SetNewText("Increaseing Particle visibility to " + ParticleManager.ParticleVisibility.ToString());
                    }
                }

                if (CurKS.IsKeyDown(Keys.L))
                {
                    if (ParticleManager.ParticleVisibility > 0)
                    {
                        ParticleManager.ParticleVisibility -= 0.004f;
                        EingabenAnzeige.SetNewText("Lowering Particle visibility " + ParticleManager.ParticleVisibility.ToString());
                    }
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.D0))
                {
                    EingabenAnzeige.SetNewText("Gave all Particles Position: " + (GameValues.ScreenSize / 2).ToString());
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].Pos = GameValues.ScreenSize / 2;
                    }
                }

                /*if (CurKS.IsKeyDown(Keys.PageUp) && ControlHandler.LastKS.IsKeyUp(Keys.PageUp))
                {
                    EingabenAnzeige.SetNewText("Spawned Proton");
                    lock (ListOfMarkers)
                    {
                        ListOfMarkers.Add(new Marker(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), Vector2.Zero, true));
                    }
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.PageDown) && ControlHandler.LastKS.IsKeyUp(Keys.PageDown))
                {
                    EingabenAnzeige.SetNewText("Spawned Electron");
                    lock (ListOfMarkers)
                    {
                        ListOfMarkers.Add(new Marker(new Vector2(ControlHandler.CurMS.X, ControlHandler.CurMS.Y), Vector2.Zero, false));
                    }
                }*/

                if (CurKS.IsKeyDown(Keys.R) && LastKS.IsKeyUp(Keys.R))
                {
                    EingabenAnzeige.SetNewText("Gave Particles Random Velocity");
                    for (int i = 0; i < ListOfParticles.Count; i++)
                    {
                        ListOfParticles[i].Vel = new Vector2(GameValues.RDM.Next(-5, 5), GameValues.RDM.Next(-5, 5));
                    }
                }

                if (CurKS.IsKeyDown(Keys.S))
                {
                    EingabenAnzeige.SetNewText("Particles Reset");

                    ListOfParticles.Clear();

                    for (int ix = 0; ix < GameValues.ScreenSize.X; ix += 3)
                    {
                        for (int iy = 0; iy < GameValues.ScreenSize.Y; iy += 3)
                        {
                            ParticleManager.AddParticle(new Particle(ix, iy, 0, 0, 1f, -0.01f));
                        }
                    }
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.Q) && ControlHandler.LastKS.IsKeyUp(Keys.Q))
                {
                    EingabenAnzeige.SetNewText("Spawned Rectangle");

                    for (float ix = -100; ix < 100; ix += 0.1f)
                    {
                        ParticleManager.AddParticle(new Particle(new Vector2(CurMS.X + ix, CurMS.Y - 100), Vector2.Zero));
                        ParticleManager.AddParticle(new Particle(new Vector2(CurMS.X + ix, CurMS.Y + 100), Vector2.Zero));
                    }

                    for (float iy = -100; iy < 100; iy += 0.1f)
                    {
                        ParticleManager.AddParticle(new Particle(new Vector2(CurMS.X - 100, CurMS.Y + iy), Vector2.Zero));
                        ParticleManager.AddParticle(new Particle(new Vector2(CurMS.X + 100, CurMS.Y + iy), Vector2.Zero));
                    }
                }

                if (CurKS.IsKeyDown(Keys.H) && LastKS.IsKeyUp(Keys.H))
                {
                    EingabenAnzeige.SetNewText("Spawned Circle");

                    for (double Angle = 0; Angle < Math.PI * 2; Angle += 0.001)
                    {
                        ParticleManager.AddParticle(new Particle(new Vector2((float)Math.Sin(Angle) * 100, (float)Math.Cos(Angle) * 100) + GetMouseVector(), Vector2.Zero));
                    }
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.Space) && ControlHandler.LastKS.IsKeyUp(Keys.Space))
                {
                    for (float i = 0; i < GameValues.ScreenSize.X; i += 0.1f)
                    {
                        ParticleManager.AddParticle(new Particle(new Vector2(i, CurMS.Y + i / 10), new Vector2(0.25f, 0)));
                        ParticleManager.AddParticle(new Particle(new Vector2(i, CurMS.Y + i / 10), new Vector2(0.25f, 0)));
                    }
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.P))
                {
                    for (int i = 0; i < 10; i++)
                    {
                        ParticleManager.AddParticle(new Particle(ControlHandler.CurMS.X + GameValues.RDM.Next(-10, 10), ControlHandler.CurMS.Y + GameValues.RDM.Next(-10, 10), 0, 0, 0.5f, -0.01f));
                    }
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.F))
                {
                    PressedFTimer++;
                }

                if (ControlHandler.CurKS.IsKeyUp(Keys.F) && ControlHandler.LastKS.IsKeyDown(Keys.F) && PressedFTimer < 300)
                {
                    GameValues.FrictionEnabled = !GameValues.FrictionEnabled;
                    EingabenAnzeige.SetNewText("Friction: " + GameValues.FrictionEnabled.ToString());
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.F) && ControlHandler.CurKS.IsKeyDown(Keys.OemPlus))
                {
                    GameValues.Friction += 0.0001f;
                    EingabenAnzeige.SetNewText("Friction increased to " + GameValues.Friction.ToString());
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.F) && ControlHandler.CurKS.IsKeyDown(Keys.OemMinus))
                {
                    GameValues.Friction -= 0.0001f;
                    EingabenAnzeige.SetNewText("Friction lowered to " + GameValues.Friction.ToString());
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.G) && ControlHandler.LastKS.IsKeyUp(Keys.G))
                {
                    switch (GameValues.GravityMode)
                    {
                        case 0:
                            GameValues.GravityMode = 1;
                            break;

                        case 1:
                            GameValues.GravityMode = 2;
                            break;

                        case 2:
                            GameValues.GravityMode = 0;
                            break;
                    }
                    EingabenAnzeige.SetNewText("GravityMode: " + GameValues.GravityMode.ToString());
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.B) && ControlHandler.LastKS.IsKeyUp(Keys.B))
                {
                    GameValues.Bloom = !GameValues.Bloom;
                    EingabenAnzeige.SetNewText("Bloom: " + GameValues.Bloom.ToString());
                }

                if (ControlHandler.CurKS.IsKeyDown(Keys.Y) && ControlHandler.LastKS.IsKeyUp(Keys.Y))
                {
                    switch (GameValues.DrawMethod)
                    {
                        case 0:
                            GameValues.DrawMethod = 1;
                            break;

                        case 1:
                            GameValues.DrawMethod = 0;
                            break;
                    }
                    EingabenAnzeige.SetNewText("DrawMethod: " + GameValues.DrawMethod.ToString());
                }
            }
        }
    }
}

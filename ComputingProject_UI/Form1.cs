using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using ComputingProject;
using ComputingProject.Collision;

namespace ComputingProject_UI
{
    public partial class Form1 : Form
    {
        int milliseconds = 1000/30; // 30 frames per second
        double timeStep = 24 * 3600; // One day
        double scale = 100 / Constants.AstronomicalUnit; // Scale the simulation down so that it can fit on the screen

        // Quadtree might need to be celetial object only
        QuadTree<IQuadtreeObject> screen;

        double screenWidth;
        double screenHeight;

        double screenWidthHalf;
        double screenHeightHalf;

        double objectRadius = 50;

        BackgroundWorker worker; // New thread

        TimeController timeController;

        Vector centre;

        public Form1()
        {
            InitializeComponent();
            SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            WindowState = FormWindowState.Maximized;

            timeController = new TimeController(timeStep);

            objectRadius = 10;

            // Calculate the maximum screen size
            ScreenBounds();
            // Then send that information to the object manager
            ObjectManager.SetScreenBounds(new Vector(screenWidth, screenHeight));

            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();

            // Set whether to use the debug tools 
            SetDebugTools(true, false, false, false);

            // Add the objects to the simulation
            AddObjects();
            
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Update graphics
            foreach (IQuadtreeObject co in ObjectManager.AllObjects) {
                e.Graphics.FillEllipse(co.colour, (float)((co.position.x + objectRadius) * scale + screenWidthHalf), (float)((co.position.y + objectRadius) * scale + screenHeightHalf), (float)objectRadius * 2, (float)objectRadius * 2);
            }

            if (DebugTools.DrawVelocityArrows) {
                foreach (IQuadtreeObject co in ObjectManager.AllObjects) {
                    Pen pen = new Pen(Color.Black);

                    double angle = Math.Atan2(co.velocity.y, co.velocity.x); ;
                    double length = 100;

                    Point startingPoint = new Point((int)((co.position.x + objectRadius) * scale), (int)((co.position.y + objectRadius) * scale));
                    Point endingPoint = new Point((int)((co.position.x + objectRadius + Math.Cos(angle) * Math.Sqrt(Math.Pow(co.velocity.x, 2) + (Math.Pow(co.velocity.y, 2)))) * scale), 
                                                  (int)((co.position.y + objectRadius + Math.Sin(angle) * length) * scale));

                    if (DebugTools.DebugMode) {
                        Console.WriteLine(startingPoint.ToString() + " " + endingPoint.ToString());
                    }

                    e.Graphics.DrawLine(pen, startingPoint, endingPoint);
                }
            }

            Invalidate();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker bw = (BackgroundWorker)sender;

            int frame = 0;

            while (!bw.CancellationPending) {
                Stopwatch sw = Stopwatch.StartNew();

                // Update positions of objects
                ObjectManager.Update(timeController.currentTimeStep, scale, screen, 1);

                frame += 1;

                Console.WriteLine("Frame: " + frame);

                sw.Stop();
                int msec = milliseconds - (int)sw.ElapsedMilliseconds;
                if (msec < 1)
                    msec = 1;
                System.Threading.Thread.Sleep(msec);
            }
        }

        /// <summary>
        /// Calculate the bounds of the screen and then set the main Quadtree to the size of the screen      
        /// </summary>
        void ScreenBounds() {
            screenHeight = Screen.PrimaryScreen.Bounds.Height;
            screenWidth = Screen.PrimaryScreen.Bounds.Width;
            
            screenWidthHalf = screenWidth / 2;
            screenHeightHalf = screenHeight / 2;
            
            centre = new Vector(screenWidthHalf, screenHeightHalf);
            
            screen = new QuadTree<IQuadtreeObject>(new AABB(centre, centre));
            Console.WriteLine("Screen set!");
        }

        /// <summary>
        /// What Debug Tools to use
        /// </summary>
        /// <param name="mode">Mode writes the majority of the debug information to the console</param>
        /// <param name="velocityArrows">Draws the velocity arrows on screen</param>
        /// <param name="collisionVelocities">Output the velocities after collision</param>
        /// <param name="useCollision">Use the collision system</param>
        void SetDebugTools(bool mode, bool velocityArrows, bool collisionVelocities, bool useCollision) {
            DebugTools.DebugMode = mode;
            DebugTools.DrawVelocityArrows = velocityArrows;
            DebugTools.PrintCollisionVelocities = collisionVelocities;
            DebugTools.UseCollision = useCollision;
        }

        /// <summary>
        /// Adds objects to the screen at the start
        /// </summary>
        void AddObjects() {

            Vector centre = new Vector(screenWidthHalf, screenHeightHalf);

            Console.WriteLine("Centre: " + centre.ToString());

            CircleCollider cc = new CircleCollider(new Vector(), objectRadius);

            // CelestialObject(name, mass, velocity, position, colour, collider)
            /*
            CelestialObject sun = new CelestialObject("Sun", 2E30, new Vector(0, 0), new Vector(), Brushes.Red, cc);
            CelestialObject sun1 = new CelestialObject("Sun1", 2E30, new Vector(0, 0), new Vector(Constants.AstronomicalUnit, 0), Brushes.Red, cc);
            CelestialObject sun2 = new CelestialObject("Sun2", 2E30, new Vector(0, 0), new Vector(0, Constants.AstronomicalUnit), Brushes.Red, cc);
            */
            ///*
            CelestialObject sun   = new CelestialObject("Sun", 2E30, new Vector(0, 0), centre, Brushes.Red, cc);
            CelestialObject earth = new CelestialObject("Earth", 6E24, /*new Vector(0, 29.783 * 1000)*/ new Vector(10000, 10000), new Vector(Constants.AstronomicalUnit * -1, 0), Brushes.Green, cc);
            CelestialObject venus = new CelestialObject("Venus", 4.8E24, /*new Vector(-35 * 1000, 0)*/new Vector(10000, 10000), new Vector(0,(Constants.AstronomicalUnit * 0.723)), Brushes.Blue, cc);
            //*/
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e) {

        }
    }
}

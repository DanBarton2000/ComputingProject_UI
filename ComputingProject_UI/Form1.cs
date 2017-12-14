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
        int milliseconds = 1000/30;
        double timeStep = 24 * 3600; // One day
        double scale = 250 / Constants.AstronomicalUnit;

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


            DebugTools.DebugMode = true;
            DebugTools.UseCollision = true;
            DebugTools.DrawVelocityArrows = false;
            DebugTools.PrintCollisionVelocities = false;

            AddObjects();
            
            // Add all objects at the start of the simualtion to the quadtree.
            foreach (CelestialObject obj in ObjectManager.AllObjects) {
                screen.Insert(obj);
            }
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

                    Point startingPoint = new Point((int)(co.position.x + objectRadius), (int)(co.position.y + objectRadius));
                    Point endingPoint = new Point((int)(co.position.x + objectRadius + Math.Cos(angle) * Math.Sqrt(Math.Pow(co.velocity.x, 2) + (Math.Pow(co.velocity.y, 2)))), (int)(co.position.y + objectRadius + Math.Sin(angle) * length));

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
            while (!bw.CancellationPending) {
                Stopwatch sw = Stopwatch.StartNew();

                // Update positions of objects
                ObjectManager.Update(timeController.currentTimeStep, scale, screen, -0.75);
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

        void AddObjects() {

            Vector centre = new Vector(screenWidthHalf, screenHeightHalf);

            Console.WriteLine(centre.ToString());

            CircleCollider cc = new CircleCollider(new Vector(), objectRadius);

            CelestialObject sun = new CelestialObject("Sun", 2E30, new Vector(0, 0), centre, Brushes.Red, cc);
            CelestialObject earth = new CelestialObject("Earth", 6E24, new Vector(0, 29.783 * 1000), new Vector(centre.x + Constants.AstronomicalUnit * -1, centre.y), Brushes.Green, cc);
            CelestialObject venus = new CelestialObject("Venus", 4.8E24, new Vector(-35 * 1000, 0), new Vector(centre.x, centre.y + (Constants.AstronomicalUnit * 0.723)), Brushes.Blue, cc);
        }
    }
}

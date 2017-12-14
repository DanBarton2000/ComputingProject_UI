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
        double timeStep = 24 * 3600 * 100; // One day
        double scale = 250 / Constants.AstronomicalUnit;

        double screenWidth;
        double screenHeight;

        double screenWidthHalf;
        double screenHeightHalf;

        double objectRadius;

        BackgroundWorker worker; // New thread

        TimeController timeController;

        Vector centre;
        QuadTree<IQuadtreeObject> screen;

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

            SetDebugTools();

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
                e.Graphics.FillEllipse(co.colour, (float)(co.position.x + objectRadius), (float)(co.position.y + objectRadius), (float)objectRadius * 2, (float)objectRadius * 2);
            }

            if (DebugTools.DrawVelocityArrows) {
                foreach (IQuadtreeObject co in ObjectManager.AllObjects) {
                    Pen pen = new Pen(Color.Black);

                    double angle = Math.Atan2(co.velocity.y, co.velocity.x); ;
                    double length = 100;

                    Point startingPoint = new Point((int)(co.position.x + objectRadius), (int)(co.position.y + objectRadius));
                    Point endingPoint = new Point((int)(co.position.x + objectRadius + Math.Cos(angle) * Math.Sqrt(Math.Pow(co.velocity.x, 2) + (Math.Pow(co.velocity.y, 2)))), (int)(co.position.y + objectRadius + Math.Sin(angle) * length));

                    if (DebugTools.DebugMode) {
                        Console.WriteLine("Form1 - Velocity Arrows - Starting Point: " + startingPoint.ToString() + " Ending Point: " + endingPoint.ToString());
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
                ObjectManager.Update(timeController.currentTimeStep, scale, screen, -1);
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
            //CelestialObject moon = new CelestialObject("Moon", 1.5E21, 10, 20, new Vector(1000, 700), Brushes.Red, new CircleCollider(new Vector(), objectRadius));
            //CelestialObject planet = new CelestialObject("Planet", 1E21, 40, 78, new Vector(500, 100), Brushes.Purple, new CircleCollider(new Vector(), objectRadius));
            //CelestialObject planet1 = new CelestialObject("Planet1", 1E21, 30, 50, new Vector(100, 600), Brushes.Blue, new CircleCollider(new Vector(), objectRadius));
            CelestialObject planet2 = new CelestialObject("Planet2", 1E21, -10, 0, new Vector(600, 600), Brushes.Green, new CircleCollider(new Vector(), objectRadius));
            CelestialObject planet3 = new CelestialObject("Planet3", 1E21, 10, 0, new Vector(100, 600), Brushes.Purple, new CircleCollider(new Vector(), objectRadius));
        }

        void SetDebugTools() {
            DebugTools.DebugMode = true;
            DebugTools.UseCollision = false;
            DebugTools.DrawVelocityArrows = true;
            DebugTools.PrintCollisionVelocities = true;
        }
    }
}

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
//using System.Windows.Media;
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

        double objectRadius = 100;

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

            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();

            DebugTools.DebugMode = false;

            ScreenBounds();

            ObjectManager.SetScreenBounds(new Vector(screenWidth, screenHeight));

            AddObjects();

            // Add all objects at the start of the simualtion to the quadtree.
            foreach (CelestialObject obj in ObjectManager.AllObjects) {
                screen.Insert(obj);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Forces_Click(object sender, EventArgs e)
        {

        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Update graphics
            foreach (IQuadtreeObject co in ObjectManager.AllObjects) {
                e.Graphics.FillEllipse(co.colour, (float)co.position.x, (float)co.position.y, (float)objectRadius, (float)objectRadius);
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
            CelestialObject moon = new CelestialObject("Moon", 1.5E21, 10, 20, new Vector(1000, 0), Brushes.Red, new CircleCollider(new Vector(1, 1), objectRadius));
            CelestialObject planet = new CelestialObject("Planet", 1E22, -50, 60, new Vector(500, 700), Brushes.Purple, new CircleCollider(new Vector(1, 1), objectRadius));
            CelestialObject planet1 = new CelestialObject("Planet1", 1E22, 100, 360, new Vector(100, 600), Brushes.Blue, new CircleCollider(new Vector(1, 1), objectRadius));
            CelestialObject planet2 = new CelestialObject("Planet2", 1E22, 10, 300, new Vector(100, 100), Brushes.Green, new CircleCollider(new Vector(1, 1), objectRadius));
            //CelestialObject planet3 = new CelestialObject("Planet3", 1.5E19, 0, 0, new Vector(100, 1000), Brushes.Green, new CircleCollider(new Vector(1, 1), objectRadius));
            //CelestialObject planet4 = new CelestialObject("Planet4", 1.5E19, 0, 300, new Vector(200, 70), Brushes.Green, new CircleCollider(new Vector(1, 1), objectRadius));
            //CelestialObject planet5 = new CelestialObject("Planet5", 1.5E15, 10, 300, new Vector(500, 200), Brushes.Green, new CircleCollider(new Vector(1, 1), objectRadius));
        }
    }
}

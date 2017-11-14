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
        double timeStep = 24 * 3600; // One day
        double scale = 100 / Constants.AstronomicalUnit;

        QuadTree<CelestialObject> screen;

        BackgroundWorker worker; // New thread

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            WindowState = FormWindowState.Maximized;

            DebugTools.DebugMode = false;

            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();

            SetScreenQuadtree();

            CelestialObject moon = new CelestialObject("Moon", 1.5E21, 10, 20, new Vector(1000, 700), Brushes.Red, null);
            CelestialObject planet = new CelestialObject("Planet", 1E22, 0, 0, new Vector(500, 100), Brushes.Purple, null);
            CelestialObject planet1 = new CelestialObject("Planet1", 1E21, 30, 50, new Vector(100, 600), Brushes.Blue, null);
            CelestialObject planet2 = new CelestialObject("Planet2", 1E22, 10, 165, new Vector(1200, 100), Brushes.Green, null);

            foreach (CelestialObject co in ObjectManager.AllObjects) {
                screen.Insert(co);
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
            foreach (CelestialObject co in ObjectManager.AllObjects) {
                e.Graphics.FillEllipse(co.colour, (float)co.position.x, (float)co.position.y, 100, 100);
            }
            Invalidate();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker bw = (BackgroundWorker)sender;
            while (!bw.CancellationPending) {
                Stopwatch sw = Stopwatch.StartNew();
                ObjectManager.Update(timeStep, scale);
                sw.Stop();
                int msec = milliseconds - (int)sw.ElapsedMilliseconds;
                if (msec < 1)
                    msec = 1;
                System.Threading.Thread.Sleep(msec);
            }
        }

        void SetScreenQuadtree() {
            double screenWidth = Screen.PrimaryScreen.Bounds.Width;
            double screenHeight = Screen.PrimaryScreen.Bounds.Height;

            Vector centre = new Vector(screenWidth, screenHeight);

            screen = new QuadTree<CelestialObject>(new AABB(centre, centre));
        }
    }
}

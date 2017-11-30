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

            // Calculate the maximum screen size
            ScreenBounds();
            // Then send that information to the object manager
            ObjectManager.SetScreenBounds(new Vector(screenWidth, screenHeight));

            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();


            CelestialObject moon = new CelestialObject("Moon", 1.5E21, 10, 20, new Vector(1000, 700), Brushes.Red, null);
            CelestialObject planet = new CelestialObject("Planet", 1E22, 0, 0, new Vector(500, 100), Brushes.Purple, null);
            CelestialObject planet1 = new CelestialObject("Planet1", 1E21, 30, 50, new Vector(100, 600), Brushes.Blue, null);
            CelestialObject planet2 = new CelestialObject("Planet2", 1E22, 10, 165, new Vector(1200, 100), Brushes.Green, null);
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
                    Point endingPoint = new Point((int)(co.position.x + objectRadius + Math.Cos(angle) * length), (int)(co.position.y + objectRadius + Math.Sin(angle) * length));

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
    }
}

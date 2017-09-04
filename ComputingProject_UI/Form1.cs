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

namespace ComputingProject_UI
{
    public partial class Form1 : Form
    {
        int milliseconds = 1000/30;
        double timeStep = 24 * 3600; // One day
        double scale = 250 / Constants.AstronomicalUnit;

        double fx, fy;
        BackgroundWorker worker; // New thread

        public Form1()
        {
            InitializeComponent();
            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.OptimizedDoubleBuffer, true);
            WindowState = FormWindowState.Maximized;

            worker = new BackgroundWorker();
            worker.DoWork += worker_DoWork;
            worker.WorkerSupportsCancellation = true;
            worker.RunWorkerAsync();

            CelestialObject moon = new CelestialObject("Moon", 1.5E21, 10, 20, new Vector(1000, 700), Brushes.Red);
            CelestialObject planet = new CelestialObject("Planet", 1E22, 0, 0, new Vector(500, 200), Brushes.Purple);
            CelestialObject planet1 = new CelestialObject("Planet1", 1E22, 30, 50, new Vector(100, 600), Brushes.Blue);
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void Forces_Click(object sender, EventArgs e)
        {

        }

        // Needs to be executed on another thread
        void Loop(List<CelestialObject> objects)
        {
            Dictionary<CelestialObject, double[]> forces = new Dictionary<CelestialObject, double[]>();
            foreach (CelestialObject co in objects)
            {
                fx = 0;
                fy = 0;
                foreach (CelestialObject cobj in objects)
                {
                    if (co != cobj)
                    {
                        double[] force = co.Attraction(cobj);
                        fx += force[0];
                        fy += force[1];
                    }
                }
                double[] totalForces = new double[2] { fx, fy };
                forces.Add(co, totalForces);
            }

            foreach (CelestialObject co in objects)
            {
                double[] f = forces[co];
                double massTimeStep = co.Mass * timeStep;
                co.velocity.x += f[0] / massTimeStep;
                co.velocity.y += f[1] / massTimeStep;

                co.position.x += co.velocity.x * timeStep * scale;
                co.position.y += co.velocity.y * timeStep * scale;
                Console.WriteLine("OBJ: " + co.Name + " X: " + co.position.x + " Y: " + co.position.y);
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            // Update graphics
            foreach (CelestialObject co in ObjectManager.allObjects) {
                e.Graphics.FillEllipse(co.colour, (float)co.position.x, (float)co.position.y, 100, 100);
            }
            Invalidate();
        }

        void worker_DoWork(object sender, DoWorkEventArgs e) {
            BackgroundWorker bw = (BackgroundWorker)sender;
            while (!bw.CancellationPending) {
                Stopwatch sw = Stopwatch.StartNew();
                Loop(ObjectManager.allObjects);
                sw.Stop();
                int msec = milliseconds - (int)sw.ElapsedMilliseconds;
                if (msec < 1)
                    msec = 1;
                System.Threading.Thread.Sleep(msec);
            }
        }
    }
}

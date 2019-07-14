using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;

namespace AiTest
{
    public class Brain
    {
        private readonly int dotCount;

        private Canvas canvas;
        private List<Dot> dots = new List<Dot>();

        public event EventHandler FrameHappened;
        public event EventHandler Finished;

        public int DeadDotCount => dots.Count(d => d.Dead);
        public double FrameRate { get; private set; }

        public Brain(int count, Canvas canvas)
        {
            dotCount = count;
            this.canvas = canvas;

            for (int i = 0; i < dotCount; i++)
            {
                dots.Add(new Dot(canvas));
            }
        }

        public void Start()
        {
            Task.Run(() => Loop());
        }

        public async Task Loop()
        {
            while (DeadDotCount < dotCount)
            {
                var frameStart = DateTime.Now;

                MoveDots();
                DrawDots();
                FrameHappened?.Invoke(this, EventArgs.Empty);
                await Task.Delay(1);

                var frameTime = DateTime.Now - frameStart;
                FrameRate = 1000 / frameTime.TotalMilliseconds;
            }

            Finished?.Invoke(this, EventArgs.Empty);
        }

        private void MoveDots() => dots.ForEach(d => d.Move());

        private void DrawDots() => dots.ForEach(d => d.Draw());
    }
}

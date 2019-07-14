using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace AiTest
{
    public class Dot
    {
        private const int MinSpeed = 20;
        private const int MaxSpeed = 100;
        private const double EllipseRadius = 5;

        private static readonly Brush AliveColor = Brushes.LightGreen;
        private static readonly Brush DeadColor = Brushes.Gray;
        private static readonly Random random = new Random();

        private readonly Canvas canvas;

        private DateTime lastMove = DateTime.Now;

        public Point Position { get; set; }
        public VelocityVector Velocity { get; set; }
        public float Acceleration { get; set; }
        public Ellipse Ellipse { get; set; }
        public Line Line { get; set; }

        private bool dead = false;
        public bool Dead
        {
            get => dead;
            set
            {
                dead = value;
                canvas.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Ellipse.Fill = dead ? DeadColor : AliveColor;
                    Ellipse.Stroke = dead ? DeadColor : AliveColor;
                    Line.Stroke = dead ? DeadColor : AliveColor;
                }));
            }
        }

        public Dot(Canvas canvas)
        {
            this.canvas = canvas;

            Position = new Point(random.Next(Convert.ToInt32(canvas.ActualWidth - 5)), random.Next(Convert.ToInt32(canvas.ActualHeight - 5)));

            Velocity = new VelocityVector
            {
                Speed = (random.NextDouble() * (MaxSpeed - MinSpeed)) + MinSpeed,
                Direction = random.Next(360)
            };

            Ellipse = new Ellipse
            {
                Height = EllipseRadius,
                Width = EllipseRadius,
                Stroke = AliveColor,
                Fill = AliveColor
            };

            canvas.Children.Add(Ellipse);

            Line = new Line()
            {
                Stroke = AliveColor,
                StrokeThickness = 1,
                X1 = Position.X,
                X2 = Position.X + Velocity.Speed * Math.Cos(Velocity.DirectionInRadians),
                Y1 = Position.Y,
                Y2 = Position.Y - Velocity.Speed * Math.Sin(Velocity.DirectionInRadians)
            };

            canvas.Children.Add(Line);
        }

        public void Draw()
        {
            if (!Dead)
            {
                canvas.Dispatcher.BeginInvoke(new Action(() =>
                {
                    Canvas.SetLeft(Ellipse, Position.X);
                    Canvas.SetTop(Ellipse, Position.Y);

                    Line.X1 = Position.X + (EllipseRadius / 2);
                    Line.X2 = Position.X + Velocity.Speed * Math.Cos(Velocity.DirectionInRadians);
                    Line.Y1 = Position.Y + (EllipseRadius / 2);
                    Line.Y2 = Position.Y - Velocity.Speed * Math.Sin(Velocity.DirectionInRadians);
                }));
            }
        }

        public void Move()
        {
            var time = DateTime.Now - lastMove;
            lastMove = DateTime.Now;

            var distance = Velocity.Speed * time.TotalSeconds;

            var x = distance * Math.Cos(Velocity.DirectionInRadians);
            var y = -distance * Math.Sin(Velocity.DirectionInRadians);
            Position = new Point(Position.X + x, Position.Y + y);

            if (Position.X < 0 ||
                Position.X > canvas.ActualWidth - 5 ||
                Position.Y < 0 ||
                Position.Y > canvas.ActualHeight - 5)
            {
                Dead = true;
            }
        }
    }
}

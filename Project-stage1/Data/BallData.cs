﻿using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Diagnostics;

namespace Data
{
    internal class BallData : BallDataAPI, INotifyPropertyChanged
    {
        private readonly Object lockMovement = new();
        private readonly Object lockLog = new();
        private int xValue;
        private int yValue;
        private int xDirection;
        private int yDirection;
        private int weight;
        private int radius;
        private bool moving = true;
        private Thread move;
        
        public override event PropertyChangedEventHandler? PropertyChanged;
        
        public BallData(int x, int y, int r, int w, int xDir, int yDir)
        {
            radius = r;
            xValue = x;
            yValue = y;
            xDirection = xDir;
            yDirection = yDir;
            weight = w;
            move = new(Movement) { IsBackground = true };
            Thread log = new Thread(() =>
            {
                lock (lockLog)
                {
                    this.PropertyChanged += Update;
                }
            });
            log.Start();
        }

        public override int Radius
        {
            get => radius;
        }

        public override int XValue
        {
            get => xValue;
           internal set { 
                          xValue = value; }
        }

        public override int YValue
        {
            get => yValue;
            internal set { yValue = value; }
        }

        public override int XDirection
        {
            get => xDirection;
            set {xDirection = value;
            }
        }

        public override int YDirection
        {
            get => yDirection;
            set {
                yDirection = value;
            }
        }

        public  int Weight
        {
            get => weight;
        }
        internal override void StartBall()
        {
            move.Start();
        }
        internal override void Stop()
        {
            moving = false;
        }
        private  void Movement()
        {
            Stopwatch stopwatch = new();
            while (moving)
            {
                stopwatch.Start();

                lock (lockMovement)
                {
                    XValue += XDirection;
                    YValue += YDirection;
                }
                OnPropertyChanged();

                stopwatch.Stop();

                if ((int)stopwatch.ElapsedMilliseconds < 8)
                {
                    Thread.Sleep(8 - (int)stopwatch.ElapsedMilliseconds);
                }

                stopwatch.Reset();
            }
        }
       

        private void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public void Update(Object s, PropertyChangedEventArgs e)
        {
            Logger.Instance().writeLog(new LoggerArgs(XValue,YValue,xDirection,yDirection,this.GetHashCode()));
        }
    }
}
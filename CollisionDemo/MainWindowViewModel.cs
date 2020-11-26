using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using PhysicsEngine2D.Net;

namespace CollisionDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private static readonly Random Random = new Random();
        private ObservableCollection<Ball> _balls;

        public ObservableCollection<Ball> Balls
        {
            get => _balls;
            set => SetProperty(ref _balls, value);
        }

        public MainWindowViewModel()
        {
            const float width = 1000;
            const float height = 600;
            const float minRadius = 6;
            const float maxRadius = 10;
            var balls = Enumerable.Range(0, 200).Select(i =>
            {
                var weight = GetRandom(minRadius, maxRadius);
                return new Ball
                {
                    Mass = (float)Math.Sqrt(weight),
                    Position = new Vector2(
                        GetRandom(maxRadius, width - maxRadius),
                        GetRandom(maxRadius, height - maxRadius)),
                    Radius = weight,
                    //Velocity = new Vector2(GetRandom(-100, 100), GetRandom(-100, 100)),
                    Acceleration = new Vector2(0, 100f),
                    Restitution = 0.8f,
                }.SetBound(0, 0, width, height);
            });
            Balls = new ObservableCollection<Ball>(balls);
        }

        private static float GetRandom(float a, float b)
        {
            return a + (b - a) * (float)Random.NextDouble();
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected virtual bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (EqualityComparer<T>.Default.Equals(storage, value)) return false;

            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Runtime.CompilerServices;
using PhysicsEngine2D.Net.Basic;

namespace CollisionDemo
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        private static readonly Random Random = new Random();
        private ObservableCollection<Body> _bodies;

        public ObservableCollection<Body> Bodies
        {
            get => _bodies;
            set => SetProperty(ref _bodies, value);
        }

        public MainWindowViewModel()
        {
            const float width = 1000;
            const float height = 600;
            const float minRadius = 4;
            const float maxRadius = 6;
            var bodies = Enumerable.Range(0, 2000).Select(i =>
            {
                var weight = GetRandom(minRadius, maxRadius);
                return new Body
                {
                    MassData = new MassData { Mass = (float)Math.Sqrt(weight) },
                    Shape = new Circle
                    {
                        Position = new Vector2(
                            GetRandom(maxRadius, width - maxRadius),
                            GetRandom(maxRadius, height - maxRadius)),
                        Radius = weight,
                    },
                    Material = new Material { Restitution = 0.99f },
                    Velocity = new Vector2(GetRandom(-100, 100), GetRandom(-100, 100)),
                };
            });
            Bodies = new ObservableCollection<Body>(bodies);
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;

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
            var balls = Enumerable.Range(0, 50).Select(i =>
            {
                var weight = GetRandom(5, 30);
                return new Ball
                {
                    Mass = Math.Sqrt(weight),
                    Position = new Point(GetRandom(100, 400), GetRandom(100, 400)),
                    Radius = weight,
                    Velocity = new Vector(GetRandom(-100, 100), GetRandom(-100, 100)),
                    //Gravity = new Vector(0, 9.8),
                    //Damping = 0.2,
                }.SetBound(0, 0, 500, 500);
            });
            Balls = new ObservableCollection<Ball>(balls);
        }

        private static double GetRandom(double a, double b)
        {
            return a + (b - a) * Random.NextDouble();
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
